## [0.0.12] - 2025.03.17

### Added

- CharacterKit 모듈 구조 재정의
- MonoBehaviour / Context / Action 계층 분리
- 각 모듈의 데이터와 로직 책임 명확화
- Base 캐릭터 구조 도입
-  Player / NPC 공용 기능을 Base 계층으로 통합
-  개별 캐릭터는 상속 기반으로 확장 가능하도록 구성
- PlayerConnector 시스템 추가
-  입력 바인딩을 PlayerController에서 분리
-  GameFlow 레이어에서 액션 연결을 담당하도록 구조 변경

### Changed
- 전체 디렉터리 구조 재편성
-  리팩토링 이전 / 이후 스크립트 분리
-  모듈 단위 기준으로 구조 재정렬
-  Handler(모듈) 구조 리팩토링
-  기존 모듈을 역할 기반으로 재구성
-  현재 Mover, Combat 모듈만 재구현 완료
- 코드 스타일 통일
-  private/protected 변수 네이밍을 _Value 형태로 변경
-  public 멤버는 PascalCase로 통일
-  프로젝트 전반에 걸쳐 네이밍 규칙 일괄 적용

### Removed
- 기존 Game State Flow 패키지 제거
-   CharacterKit 내부로 기능 병합
- ItemSystem 내 Unity UI 로직 제거
-   UI 관련 로직을 CharacterKit으로 이동
### Refactored

- 패키지 구조 재조정
-  Game Flow → CharacterKit으로 통합
-  Camera Handler → ThirdPersonCamera 패키지로 분리
-  기존 모듈 일부 제거 및 재구성 대기 상태로 전환
-  Interactor
-  Stat
-  Rig
-  Inventory & EquipHandler

### Notes
- CharacterKit의 역할을 캐릭터 공용 시스템으로 재정의하기 위한 리팩토링
- 기존 Rig / Equipment 시스템은 구조 재설계를 위해 일시 제거됨
- 현재 기본 이동 및 전투 흐름만 동작
- 변경 범위가 매우 크며, 하위 호환성을 보장하지 않음
- 이후 단계에서 제거된 모듈들을 새로운 구조 기준으로 재구현 예정