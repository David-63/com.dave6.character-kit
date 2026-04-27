## [0.0.17] - 2025.04.27

### Added
- ContainerCollection 기반 Loadout 구조 도입
- 장비 상태에 따른 Extension 컨테이너 동적 추가/제거 로직
- ContainerCollectionView UI 추가 (컨테이너 그룹 단위 표현)
- Collection 이벤트 기반 UI 동기화 (컨테이너 추가/제거 반영)

### Changed
- PlayerLoadout이 ContainerCollection 기반으로 동작하도록 변경
- LoadoutRootContext가 ContainerCollection을 관리하도록 구조 수정
- 아이템 추가/이동/제거 시 Extension 처리 로직 통합
- UI가 단일 Container → Collection 기반 구조로 전환

### Removed

### Refactored
- Loadout / Service / Context 책임 재정리
- 컨테이너 탐색 및 매핑 구조 개선 (Container → Collection)
- UI와 도메인 간 의존성 정리 및 이벤트 흐름 단순화

### Fixed
- 아이템을 자신의 하위 컨테이너로 이동할 수 있는 문제 방지 (순환 참조 차단)

### Notes
- 인벤토리 시스템이 확장 가능한 구조(ContainerCollection)로 전환됨