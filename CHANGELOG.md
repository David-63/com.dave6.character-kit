## [0.0.3] - 2025.12.11

### Changed

- 주요 로직을 Basic / Minimal / Normal 구성으로 분리
-   Basic: 공통적으로 쓰이며 외부에 기능을 제공하는 레이어
-   Minimal: 의존성이 적은 기초 조작 모델
-   Normal: 스탯 시스템을 적용한 모델


### Added

- StatSystem 기능 연동
-   스탯 시스템의 기반이 되는 IEntity를 상속하고 구현함

- 공중에서 관성 적용

