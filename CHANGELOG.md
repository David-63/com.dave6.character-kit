## [0.0.18] - 2025.05.09

### Added
- Item Inspector UI 추가
- Inspector 열기/닫기 및 선택 아이템 기반 상호작용 추가
- Collection 기반 ItemView 재배치(RebuildItemPlacement) 처리 추가
- CollectionView 기반 ContainerView 생성 및 관리 구조 추가

### Changed
- Loadout UI 초기화 및 ItemView 배치 순서 개선
- Loadout open/close 입력 흐름 및 Input Action 네이밍 정리
- Runtime Binder 및 UI Hierarchy 구조 정리
- LoadoutMain의 View 생성 및 이벤트 바인딩 흐름 개선
- ItemView 배치 로직을 CollectionView 기반 탐색 구조로 변경

### Removed

### Refactored
- LoadoutMain UI 빌드 및 배치 책임 정리
- CollectionView / ItemView 생성 흐름 정리
- UI 이벤트 바인딩 및 선택 처리 구조 개선
- Inspector 관련 UI 책임 분리

### Fixed
- Loadout UI 재오픈 시 Item 위치 동기화 안정성 개선
- Extension 컨테이너 초기화 순서에 따른 ItemView 배치 문제 완화

### Notes
- UI 초기화와 Runtime 흐름이 순서 기반 구조로 정리됨
- Collection 중심 View 구조로 전환 작업 진행 중
