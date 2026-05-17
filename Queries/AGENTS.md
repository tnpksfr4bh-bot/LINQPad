# Queries — 코딩 지침

## 파일 형식
- 모든 코드는 `.linq` (LINQPad 네이티브 포맷) 로 작성
- LINQPad 기능 적극 활용: `Dump()`, `LINQPad.Progress`, `Util.Cache` 등
- `.cs` 예제 파일은 `examples/` 폴더에만 위치, 최소한으로 유지

## .NET 타겟 표기
각 파일 상단 주석에 지원 타겟 명시:
```csharp
// Target: .NET Standard 2.1 | .NET 8 | .NET 10
// Target: .NET 8+ (Server only)
```

## Unity 호환성 주석
.NET Standard 2.1 미지원 API 사용 시 반드시 표기:
```csharp
// [Unity 미지원] <이유>. 대안: <대체 방법>
```

## 모듈 분류
| 폴더 | 분류 | Unity 호환 목표 |
|------|------|----------------|
| Models | Common | 필수 |
| Utility | Common | 필수 |
| Crypto | Common | 메서드별 검토 |
| Cache | Server | 해당 없음 |
| Database | Server | 해당 없음 |
| Network | Server | 해당 없음 |
| IAP | Server | 해당 없음 |
| Infra | Server | 해당 없음 |

## 벤치마크 작성
- `Infra/BenchmarkSetup.linq` 패턴 참고
- 성능 비교 시 대안 구현도 함께 작성하여 비교

## DLL 패키징 고려사항
- 외부 의존성은 최소화 (단일 DLL 병합 목표)
- Server 전용 패키지는 Common 모듈에서 참조 금지
