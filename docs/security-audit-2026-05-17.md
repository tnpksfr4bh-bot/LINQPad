# Security Audit Report

**Date:** 2026-05-17  
**Scope:** `Queries/` 전체 (35개 파일)  
**분류 기준:** Critical / High / Medium / Low

---

## 목차

1. [Critical](#critical)
2. [High](#high)
3. [Medium](#medium)
4. [Low / 참고](#low--참고)
5. [네임스페이스 목록](#네임스페이스-목록)

---

## Critical

### 1. RSA 취약 키 크기 + PKCS#1 v1.5 패딩

**파일:** `Queries/Crypto/RsaCrypto.linq`

```csharp
// 1024-bit → 최소 2048-bit 이상 필요
var (publicPem, privatePem) = RsaKeyConverter.GenerateKeyPair(1024);

// PKCS#1 v1.5 패딩 → Bleichenbacher 공격에 취약
doOaepPadding: false
```

- RSA 1024-bit는 현재 기준으로 크래킹 가능 수준으로 간주됨 (NIST 권고: 최소 2048-bit)
- `doOaepPadding: false` 는 1998년 Bleichenbacher 패딩 오라클 공격에 취약
- **수정:** `GenerateKeyPair(2048)` 이상으로 변경, `doOaepPadding: true` (OAEP) 사용

---

### 2. Apple JWT — x5c 인증서 체인 미검증

**파일:** `Queries/Iap/AppleIapService.linq`

```csharp
// x5c 값은 Base64 DER → Convert.FromBase64String() 필요 (현재 버그)
var cert = new X509Certificate2(Encoding.UTF8.GetBytes(item.ToString()));

// 유효기간·발급자·대상 검증 전부 비활성화
ValidateLifetime = false,
ValidateIssuer   = false,
ValidateAudience = false,
```

- x5c 헤더 인증서를 Apple Root CA에 대해 검증하지 않으면 임의 인증서로 서명된 JWT 위조 가능
- `Encoding.UTF8.GetBytes(item.ToString())` 는 Base64 디코딩 누락 버그 (인증서 파싱 실패 또는 오동작)
- 만료 토큰이 그대로 수락됨
- **수정:** Apple Root CA 핀닝, `Convert.FromBase64String()` 사용, `ValidateLifetime = true` 적용

---

## High

### 3. AES — 정적 IV 재사용

**파일:** `Queries/Crypto/AesCrypto.linq`

```csharp
var key = "9gBo8r5rbzOHj0S5Xmv2biYwvXm1XmRc"; // 하드코딩된 키
var iv  = "rdVsVxxOFQzODieV";                   // 고정 IV
```

- CBC 모드에서 동일한 키+IV 조합으로 여러 메시지를 암호화하면 패턴이 노출됨
- **수정:** IV는 암호화마다 `RandomNumberGenerator.Fill()` 로 새로 생성하고 암호문 앞에 첨부, 복호화 시 분리

---

### 4. MySQL SSL 비활성화

**파일:**
- `Queries/Database/SshMySqlConnection.linq`
- `Queries/Database/MySqlRepository.linq`
- `Queries/Models/AppConfigModel.linq`

```csharp
$"...SslMode=none;AllowPublicKeyRetrieval=true;"
```

- 네트워크 트래픽이 평문 전송 → 중간자 공격(MITM) 노출
- SSH 터널 경유 시에는 허용될 수 있으나, 직접 연결 시 반드시 SSL 활성화 필요
- **수정:** `SslMode=Required` (또는 `VerifyFull`), `AllowPublicKeyRetrieval=false`

---

## Medium

### 5. 모듈러 편향 (Modulo Bias)

**파일:** `Queries/Crypto/SecureStringGenerator.linq`

```csharp
sb.Append(AlphanumericChars[b % AlphanumericChars.Length]); // 문자 62개: 256 % 62 = 10
sb.Append(AppIdChars[b % AppIdChars.Length]);               // 문자 19개: 256 % 19 = 9
```

- 앞쪽 인덱스의 문자가 통계적으로 더 자주 선택되는 편향 발생
- **수정:** `RandomNumberGenerator.GetInt32(max)` 사용 (편향 없는 균등 분포)

---

### 6. 암호학적으로 안전하지 않은 ID 생성

**파일:** `Queries/Infra/DependencyInjectionSetup.linq`

```csharp
private static readonly Random _rng = new(); // System.Random (비암호화)

public static long GenerateLongId()
{
    // ...
    int a = _rng.Next(10000, 99999);
    int b = _rng.Next(100000, 999999);
    // ...
}
```

- `System.Random` 은 예측 가능한 시드 기반 PRNG
- 생성된 ID가 세션 토큰·인증 키 등 보안 컨텍스트에 사용될 경우 위험
- **수정:** `RandomNumberGenerator.GetInt32()` 로 대체

---

### 7. AWS 자격증명 하드코딩 패턴

**파일:**
- `Queries/Database/DynamoDbRepository.linq`
- `Queries/Database/AuroraDataService.linq`

```csharp
var creds = new BasicAWSCredentials("ACCESS_KEY", "SECRET_KEY");
```

- 현재는 플레이스홀더이나, 실제 키 삽입 후 git 히스토리에 영구 노출될 위험
- **수정:** IAM Role / AWS Secrets Manager / 환경변수 사용, `.gitignore` 에 자격증명 파일 등록

---

## Low / 참고

### 8. 빈 catch 블록 — 예외 무시

**파일:** `Queries/Cache/DistributedCacheManager.linq`

```csharp
try { return MessagePackSerializer.Deserialize<T>(bytes); } catch { }
```

- 역직렬화 실패가 조용히 무시됨 → 데이터 손상 감지 불가
- **수정:** 최소한 `Debug.WriteLine` 또는 로그 기록

---

### 9. MagicOnion Deadline 미설정

**파일:** `Queries/Network/MagicOnionClient.linq`

```csharp
.WithDeadline(DateTime.MaxValue) // 사실상 무제한 대기
```

- 서버 응답 지연 시 클라이언트가 영구 대기 상태에 빠질 수 있음
- **수정:** 적절한 타임아웃 값 설정 (예: `DateTime.UtcNow.AddSeconds(30)`)

---

### 10. SHA1 사용

**파일:** `Queries/Infra/BenchmarkSetup.linq`

```csharp
HashAlgorithm _sha1 = SHA1.Create();
```

- SHA1은 충돌 공격에 취약한 알고리즘
- 현재는 벤치마크 전용이므로 실제 위험도는 낮음
- 신규 코드에서 SHA1을 사용하는 패턴이 확산되지 않도록 주의

---

## 네임스페이스 목록

### 직접 구성한 커스텀 네임스페이스

| 네임스페이스 | 파일 |
|---|---|
| `Models.User` | `Queries/Models/UserModel.linq` |
| `Models.Iap` | `Queries/Models/IapModel.linq` |
| `Models.Game` | `Queries/Models/GameModel.linq` |
| `Models.App` | `Queries/Models/AppConfigModel.linq` |
| `Models.Sns` | `Queries/Models/SnsModel.linq` |
| `Models.Cms` | `Queries/Models/CmsModel.linq` |
| `Models.Service` | `Queries/Models/ServiceModel.linq` |

---

### NuGet 패키지 출처 네임스페이스

| 네임스페이스 | NuGet 패키지 |
|---|---|
| `K4os.Hash.xxHash` | `K4os.Hash.xxHash` |
| `Org.BouncyCastle.Crypto` | `BouncyCastle.Cryptography` |
| `Org.BouncyCastle.Crypto.Parameters` | `BouncyCastle.Cryptography` |
| `Org.BouncyCastle.OpenSsl` | `BouncyCastle.Cryptography` |
| `Org.BouncyCastle.Security` | `BouncyCastle.Cryptography` |
| `Jose` | `jose-jwt` |
| `Microsoft.IdentityModel.Tokens` | `System.IdentityModel.Tokens.Jwt` |
| `System.IdentityModel.Tokens.Jwt` | `System.IdentityModel.Tokens.Jwt` |
| `Newtonsoft.Json` | `Newtonsoft.Json` |
| `Newtonsoft.Json.Linq` | `Newtonsoft.Json` |
| `Google.Apis.AndroidPublisher.v3` | `Google.Apis.AndroidPublisher.v3` |
| `Google.Apis.AndroidPublisher.v3.Data` | `Google.Apis.AndroidPublisher.v3` |
| `Google.Apis.Auth.OAuth2` | `Google.Apis.AndroidPublisher.v3` |
| `Google.Apis.Services` | `Google.Apis.AndroidPublisher.v3` |
| `MessagePack` | `MessagePack` |
| `StackExchange.Redis` | `Microsoft.Extensions.Caching.StackExchangeRedis` |
| `Renci.SshNet` | `SSH.NET` |
| `Dapper` | `Dapper` |
| `Amazon.DynamoDBv2` | `AWSSDK.DynamoDBv2` |
| `Amazon.DynamoDBv2.Model` | `AWSSDK.DynamoDBv2` |
| `Amazon.RDSDataService` | `AWSSDK.RDSDataService` |
| `Amazon.RDSDataService.Model` | `AWSSDK.RDSDataService` |
| `Amazon.Runtime` | `AWSSDK.*` |
| `Grpc.Health.V1` | `Grpc.Health.V1` |
| `Grpc.Net.Client` | `Grpc.Net.Client` |
| `Grpc.Core` | `MagicOnion.Client` |
| `MagicOnion` | `MagicOnion.Client` |
| `MagicOnion.Client` | `MagicOnion.Client` |
| `MagicOnion.Serialization` | `MagicOnion.Client` |
| `CSharp.Ulid` | `ulid.net` |
| `BenchmarkDotNet.Attributes` | `BenchmarkDotNet` |
| `BenchmarkDotNet.Configs` | `BenchmarkDotNet` |
| `BenchmarkDotNet.Diagnosers` | `BenchmarkDotNet` |
| `BenchmarkDotNet.Jobs` | `BenchmarkDotNet` |
| `BenchmarkDotNet.Running` | `BenchmarkDotNet` |

---

### BCL / ASP.NET Core 네임스페이스 (프레임워크 내장)

| 네임스페이스 | 분류 |
|---|---|
| `System.Security.Cryptography` | BCL |
| `System.Security.Cryptography.X509Certificates` | BCL |
| `System.Collections.Concurrent` | BCL |
| `System.IO.Compression` | BCL |
| `System.IO.Hashing` | BCL |
| `System.Buffers.Binary` | BCL |
| `System.Globalization` | BCL |
| `System.Net` | BCL |
| `System.Net.Http` | BCL |
| `System.Threading.Tasks` | BCL |
| `System.Data` | BCL |
| `System.ComponentModel.DataAnnotations` | BCL |
| `System.ComponentModel.DataAnnotations.Schema` | BCL |
| `Microsoft.Extensions.Caching.Memory` | ASP.NET Core |
| `Microsoft.Extensions.Caching.Distributed` | ASP.NET Core |
| `Microsoft.Extensions.DependencyInjection` | ASP.NET Core |
| `Microsoft.Extensions.DependencyInjection.Extensions` | ASP.NET Core |
| `Microsoft.Extensions.Configuration` | ASP.NET Core |
| `Microsoft.Extensions.Diagnostics.HealthChecks` | ASP.NET Core |
| `Microsoft.Extensions.Hosting` | ASP.NET Core |
| `Microsoft.EntityFrameworkCore` | ASP.NET Core |
| `Microsoft.EntityFrameworkCore.Diagnostics` | ASP.NET Core |
| `Microsoft.AspNetCore.Builder` | ASP.NET Core |
| `Microsoft.AspNetCore.Diagnostics.HealthChecks` | ASP.NET Core |
| `Microsoft.AspNetCore.Http` | ASP.NET Core |
| `Microsoft.AspNetCore.Routing` | ASP.NET Core |

---

## 우선순위 요약

| 우선순위 | 항목 | 파일 |
|---|---|---|
| 🔴 1순위 | RSA OAEP 패딩 전환 + 키 크기 2048+ | `Crypto/RsaCrypto.linq` |
| 🔴 2순위 | Apple JWT x5c 체인 검증 + 파싱 버그 수정 | `Iap/AppleIapService.linq` |
| 🟠 3순위 | AES IV 동적 생성 | `Crypto/AesCrypto.linq` |
| 🟠 4순위 | MySQL SslMode 활성화 | `Database/`, `Models/AppConfigModel.linq` |
| 🟡 5순위 | 모듈러 편향 제거 | `Crypto/SecureStringGenerator.linq` |
| 🟡 6순위 | System.Random → RandomNumberGenerator 교체 | `Infra/DependencyInjectionSetup.linq` |
| 🟡 7순위 | AWS 자격증명 패턴 환경변수 전환 | `Database/DynamoDbRepository.linq` 외 |
