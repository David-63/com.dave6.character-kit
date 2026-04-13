## [0.0.14] - 2025.04.12

### Added

### Changed
- 입력 처리 구조를 Router 기반으로 분리
- UI 입력과 플레이어 입력 흐름 분리

### Removed
- PlayerConnector 제거

### Refactored
- PlayerConnector를 역할 단위 컴포넌트로 분해 (Input, Binder, System)
- GameplayHub 기반 의존성 등록 및 참조 구조 도입
- Binder 패턴으로 초기화 및 연결 책임 분리
- Loadout 관련 구성 (System, UI, Data) 연결 방식 개선

### Notes
빡빡한 초기화 구조를 기존 중압 집중형 대신에 분산 구조로 전환
이 다음으로 Interactor 를 Loadout처럼 구현할 것..!