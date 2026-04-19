## [0.0.16] - 2025.04.19

### Added
- Interactor 핸들러 추가
 - Interactor는 상호작용 대상을 감지하는 역할만 수행
- Interaction UI view 추가
- Interaction 로직과 UI 를 바인더로 통합함

### Changed
- ItemFactory 구현 추가 및 ItemInstance 생성 책임 담당
- GameplayHub를 통해 ItemFactory 및 ItemDatabase 접근 구조 구성
- LoadoutSystem에서 아이템 생성 로직 제거 및 Save/Load 책임으로 제한
- 아이템 생성 경로를 Factory 기반으로 통일

### Removed

### Refactored
- ItemSystem ↔ CharacterKit 간 의존성 분리
- 아이템 생성 흐름을 외부 주입 기반으로 재구성
- 생성(Create)과 복원(Load)의 책임 분리

### Notes
