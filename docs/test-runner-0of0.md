# PlayMode 테스트가 0/0으로 나오는 문제

Unity 6000.6.2f1, Test Framework 1.8.0. 2026-10-21 기준.

## 증상

열려 있는 에디터에서 PlayMode 테스트를 돌리면 항상 `0/0 통과`(결과 `Passed`)가 나온다.

- `unity command run_tests --mode PlayMode`, UnityMCP `run_tests`, Test Runner 창의 Run All 모두 같다.
- `list_tests`와 Test Runner 창의 트리에는 3개가 보인다.
- Run All 뒤 `TestResults.xml`은 `testcasecount="0"`이다.
- 콘솔에는 오류가 없다. `Ignoring duplicate RunFinished`는 CLI 패키지의 정상 방어 로그라 증상이 아니다.

## 확인된 것

- **배치 모드는 3/3 통과.** 테스트 코드와 프로젝트 설정은 정상이다.
- 이름을 지정한 실행이 한 번 1/1로 통과했으나 재현되지 않았다. 열린 에디터의 상태에 따라 가끔만 실행된다.

## 기각한 가설

- `ProjectSettings/EditorSettings.asset`의 `m_EnterPlayModeOptionsEnabled: 1`(도메인 리로드 끔). 0으로 바꾸고 에디터를 재시작해도 0/0이다. 원복했다.

## 우회 (PlayMode 테스트는 이 절차로 돌린다)

`unity test`는 프로젝트가 열려 있으면 거부하므로 에디터를 닫고 돌린다. 약 1분.

```bash
unity close .
unity test . --mode PlayMode --output "$TEMP/nl-results.xml"
unity open .
```

결과는 출력 XML의 `total`, `passed`, `failed` 속성으로 본다. 이전 실행의 XML이 남아 있으면 헷갈리므로 먼저 지운다.

`unity close`는 저장 없이 종료한다. 씬·프리팹을 에디터에서 편집했을 때만 먼저 `unity command save_all`을 한다. 이름 없는(저장된 적 없는) 씬이 열려 있으면 `save_all`이 "씬 저장" 창을 띄워 에디터가 멈추고 30초 뒤 시간 초과가 나므로, 먼저 `Title.unity` 같은 저장된 씬을 열어 둔다. C#만 고쳤다면 `save_all`은 필요 없다.

## 미해결

열린 에디터의 Test Runner가 실행 시점에 목록을 비우는 원인. Test Framework 1.8.0과 Unity 6000.6 호환이 의심되나 확인하지 않았다(패키지 버전 변경 시험은 안 함).
