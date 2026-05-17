# LINQPad Utility Library — Project Plan

## Overview
LINQPad 기반의 개인 유틸리티 라이브러리. 각 모듈을 개발·테스트한 뒤 DLL로 패키징하여
Unity, 서버사이드 .NET, 범용 dotnet 프로젝트에서 재활용하는 것이 목표.

---

## 개발 환경

| 환경 | 역할 |
|------|------|
| LINQPad (.linq) | **메인** — 개발, 테스트, 벤치마크 |
| VSCode (.cs) | **보조** — 데모/예제 용도만 (최소한으로 작성) |

### VSCode 단독 실행 (.NET 9/10 file-based)
- .NET 9+ 부터 `dotnet run file.cs` 로 프로젝트 없이 단일 파일 실행 가능
- LINQPad `.linq` 와 `.csx` 는 **서로 다른 에코시스템** (상호 변환 불가)
- `.cs` 예제 파일은 `examples/` 폴더에 위치, 데모 수준으로만 유지
- **조사 필요**: .NET 9/10 file-based app 의 정확한 제약과 LINQPad 대비 한계

---

## .NET 타겟 전략

| 타겟 | 용도 |
|------|------|
| .NET Standard 2.1 | Unity 2021+ 호환, 광범위 호환성 |
| .NET 8.0 | LTS 서버/클라우드 |
| .NET 10.0 | 최신 기능 활용 |

### Unity 호환성 관리 규칙
- .NET Standard 2.1 에서 동작하는 API → 그대로 구현
- .NET 6+ 전용 API 사용 시 → 주석 필수:
  ```csharp
  // [Unity 미지원] <이유>. 대안: <대체 방법>
  ```
- 대안이 없는 경우 → 별도 조사 후 기록

---

## 모듈 분리 계획

### Common / Client (Unity 호환 대상)
- `Models` — 데이터 모델, DTO
- `Utility` — 범용 헬퍼
- `Crypto` — 암호화, 해싱 (메서드별 Unity 호환 여부 검토)

### Server Only
- `Database` — MySQL, DynamoDB, Aurora
- `Cache` — Redis, 분산 캐시
- `Network` — HTTP, gRPC, MagicOnion
- `IAP` — Apple / Google 인앱 결제
- `Infra` — DI 설정, 플랫폼 열거형, 벤치마크

---

## DLL 패키징 로드맵

**Phase 1** — 기능별 개별 DLL
- `Crypto.dll`, `Cache.dll`, `Database.dll` 등 모듈 단위로 먼저 구성
- 각 DLL 은 타겟별(.NET Standard 2.1 / .NET 8 / .NET 10) 빌드

**Phase 2** — 단일 통합 DLL (장기 목표, 조사 필요)
- 모든 모듈을 하나의 DLL 로 병합
- 검토 도구: ILRepack, NativeAOT bundling
- 모든 타겟 런타임에서 호환되는 형태 목표

---

## 벤치마크 & 테스트 목표
- 구현 간 성능 비교 (예: Redis vs 인메모리 캐시)
- 부하 테스트로 오류 및 병목 탐지
- 기존: `Queries/Infra/BenchmarkSetup.linq` (BenchmarkDotNet 기반)
- **조사 필요**: BenchmarkDotNet 대안 또는 보완 도구

---

## 조사 항목 (Research Backlog)
- [ ] .NET 9/10 file-based app 의 기능 범위와 한계
- [ ] Unity .NET Standard 2.1 비호환 API 목록 정리
- [ ] ILRepack / NativeAOT 단일 DLL 병합 가능성
- [ ] BenchmarkDotNet 보완 또는 대체 도구
