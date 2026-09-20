# Nova_Lux

Unity 탄막 게임 프로젝트. Universal 2D 템플릿.

## 요구 사항

- **Unity 6000.6.2f1** (다른 버전으로 열지 않는다. 열면 에셋이 재직렬화되어 불필요한 diff가 대량으로 생긴다)
- Git, Git LFS

## 처음 세팅

1. GitHub 저장소 초대를 수락한다.
2. Git LFS를 켠다 (PC당 1회).

   ```bash
   git lfs install
   ```

3. clone한다.

   ```bash
   git clone https://github.com/Garv-v/Nova_Lux.git
   ```

4. 커밋 작성자를 설정한다 (이미 되어 있으면 건너뛴다).

   ```bash
   git config --global user.name "이름"
   ```

   ```bash
   git config --global user.email "GitHub 이메일"
   ```

5. **병합 도구를 등록한다.** clone한 폴더 안에서 실행하고, 경로는 본인 PC의 Unity 설치 경로로 바꾼다. 등록하지 않으면 씬/프리팹 충돌 시 Unity 스마트 병합이 동작하지 않는다.

   ```bash
   git config --local merge.unityyamlmerge.driver "'C:/Program Files/Unity/Hub/Editor/6000.6.2f1/Editor/Data/Tools/UnityYAMLMerge.exe' merge -p %O %B %A %A"
   ```

6. Unity Hub에서 `Add > Add project from disk`로 clone한 폴더를 연다. 첫 임포트는 오래 걸리고 `Library/`가 자동으로 생긴다.
7. `Edit > Project Settings > Version Control`의 Mode가 **Visible Meta Files**인지 확인한다. `Unity Version Control`로 바뀌어 있으면 되돌린다. 이 프로젝트는 Git만 쓴다.

## 브랜치 규칙

`main` 하나와 짧게 쓰는 작업 브랜치, PR로 운영한다. `develop`/`release` 브랜치는 두지 않는다.

- **`main`**: 항상 열어서 실행되는 상태만 둔다. 직접 푸시하지 않는다.
- **작업 브랜치**: 기능 하나에 하나씩 `main`에서 만들고, 끝나면 PR로 합친 뒤 삭제한다.
- **이름**: `이름/작업내용`, 영문 소문자와 하이픈. 예: `adella/bullet-pattern`, `fiercewolf701/player-move`. 에디터/패키지 버전 변경처럼 따로 다룰 것은 `이름/chore-작업내용`.

작업 흐름:

1. `main`을 최신으로 pull한다.
2. `main` 기준으로 새 브랜치를 만든다.
3. 작게 자주 커밋한다.
4. 브랜치를 푸시하고 PR을 만든다.
5. 상대가 확인한 뒤 **Squash and merge** 한다.
6. 합친 브랜치는 삭제한다. squash 후에는 같은 브랜치를 다시 쓰지 말고 새로 만든다.

지킬 것:

- 브랜치 수명은 1~3일로 짧게 한다. 오래 갈수록 씬 충돌이 커진다.
- 작업 중 `main`이 앞서 나가면 내 브랜치에 `main`을 합쳐서 따라간다(GitHub Desktop: `Branch > Update from main`). `rebase`는 씬/프리팹에서 꼬이기 쉬워 쓰지 않는다.
- 씬, 프리팹, `ProjectSettings/` 변경은 상대가 PR에서 확인한다.
- `Packages/manifest.json`, `ProjectSettings/*`(레이어, 태그 등), 에디터 버전 변경은 겹치기 쉽다. 한 명씩 차례로 하고, 다른 작업과 섞지 말고 별도 브랜치로 빨리 합친다.
- `main` 직접 푸시 차단(브랜치 보호)은 설정하지 않았다. 약속으로 지킨다.

## 작업 규칙

- 작업 시작 전에 `main`을 pull한다.
- **씬(`.unity`)은 담당자를 정한다.** 어떤 씬을 건드리는 브랜치를 만들 때 상대에게 미리 알리고, 그 씬은 그 브랜치만 수정한다. 같은 씬을 동시에 열어 고치지 않는다. 공통 오브젝트는 프리팹으로 나눠 각자 맡는다.
- 에셋 이동/삭제/이름 변경은 **반드시 Unity 에디터 안에서** 한다. 탐색기에서 하면 `.meta`의 GUID가 깨져 참조가 끊긴다. `.meta` 파일은 에셋과 함께 항상 커밋한다.
- 자주 바뀌는 로직(탄막 패턴 등)은 씬이 아니라 C# 스크립트에 둔다.
- 탄막은 `Instantiate`/`Destroy` 대신 처음부터 **오브젝트 풀링**으로 만든다.
- `ProjectSettings/ProjectVersion.txt`나 패키지 버전이 바뀌는 변경(에디터 업그레이드 등)은 다른 변경과 섞지 않고 따로 커밋한다.

## 파일 관리

- `Library/`, `Temp/`, `Logs/`, `UserSettings/`, `.csproj`, `.sln`은 커밋하지 않는다 (`.gitignore`에 이미 제외).
- 이미지, 모델, 오디오, 영상 등 바이너리는 Git LFS로 관리된다 (`.gitattributes`). 커밋 후 `git lfs ls-files`에 나오는지 확인할 수 있다.
- GitHub의 LFS 무료 용량에는 제한이 있다. 원본 작업 파일(대용량 `.psd`, 고해상도 영상 등)은 저장소에 올리지 않는다.

## 충돌이 났을 때

씬/프리팹 충돌은 혼자 수동으로 풀지 말고 상대와 먼저 상의한다. 한쪽 변경을 버리고 다시 하는 편이 나을 때가 많다.
