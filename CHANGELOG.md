## [0.0.19] - 2025.05.12

### Added
- ItemInspector UI 기본 구조 추가
- GeometryChanged 기반 ItemView 배치 처리 추가
- CollectionView → LoadoutMain 컨테이너 이벤트 연결 추가


### Changed
- Loadout UI 빌드 흐름 개선 (Collection → Item 순서)
- ItemView 생성/배치 책임을 LoadoutMain으로 통합
- Visibility 기반 UI 표시 방식으로 변경


### Removed


### Refactored
- LoadoutMain 구조 및 이벤트 흐름 정리
- Collection / ItemView 갱신 구조 단순화


### Fixed
- Extension Container 제거 시 ItemView가 남는 문제 수정
- 초기 UI 계산 시 ItemView 위치가 틀어지는 문제 수정
- ContainerView missing 예외 수정


### Notes
- 동적 컨테이너 환경 대응 구조 안정화
