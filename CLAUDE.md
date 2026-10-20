# Nova_Lux

Unity 6000.6.2f1, 2D URP. 팀 규칙은 [README.md](README.md), 게임 규칙·수치는 [docs/02-기획서.md](docs/02-기획서.md), 용어는 [CONTEXT.md](CONTEXT.md), 되돌리기 어려운 결정은 [docs/adr/](docs/adr/).

## Unity 에디터 제어

에디터가 열려 있으면 씬·프리팹·에셋 파일(YAML)을 직접 고치지 않고 에디터를 조작한다. 먼저 `unity status`로 연결(state `ready`)을 확인한다.

1. **Unity CLI(셸 명령)가 기본이다.** `unity-cli` 스킬을 따른다. 도구 목록은 `unity list`, 호출은 `unity command <이름>`.
2. **CLI로 안 되면 UnityMCP(`mcp__UnityMCP__*`)로 같은 작업을 한 번 더 시도한다.**
3. 둘 다 안 되면 멈추고, 실패 원인(Safe Mode, 타임아웃, 연결 끊김)과 시도한 것을 사용자에게 알린다.

UnityMCP가 먼저인 작업: C# 작성 전 API 확인(`unity_reflect`, `unity_docs`), 스프라이트(`manage_sprite`), 프로파일러, ScriptableObject, 쉐이더·VFX·텍스처. CLI에 대응 도구가 없다.

## 확인 방법

- C# 수정 뒤에는 `unity recompile`로 컴파일 오류를 본 다음 진행한다.
- 화면은 `unity command screenshot`으로 확인한다. Screen Space Overlay UI는 카메라 렌더에 빠지므로, Play Mode에서 `capture_game_view`를 `source=screen`으로 찍는다.
- uGUI는 전용 도구가 없다. `set_serialized_field`, `set_component_properties`로 RectTransform 등의 값을 쓰고, 매번 스크린샷으로 확인한다.
- 에셋 이동·이름 변경은 `move_asset`, `rename_asset`으로 한다(GUID 유지).

## Agent skills

### Issue tracker

GitHub Issues (`Garv-v/Nova_Lux`, `gh` CLI). See `docs/agents/issue-tracker.md`.

### Triage labels

기본 5개 라벨 (`needs-triage`, `needs-info`, `ready-for-agent`, `ready-for-human`, `wontfix`). See `docs/agents/triage-labels.md`.

### Domain docs

single-context (루트 `CONTEXT.md` + `docs/adr/`). See `docs/agents/domain.md`.
