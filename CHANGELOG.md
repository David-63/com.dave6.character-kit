## [0.0.20] - 2025.05.19

### Added
- Player Status 추가
- Item Inspector Stat Section 표시 추가
- StatTag Group 기반 스탯 구성 추가


### Changed
- 장비 스탯 적용 방식을 reevaluate 기반 구조로 변경
- extension 장착/해제 시 nested equipment stat 동기화 추가
- equip 판정 흐름 개선


### Removed


### Refactored
- Status UI를 section 기반 구조로 구성
- Equip 런타임 흐름 정리


### Fixed
- Extension inventory 내부 장비가 equip 상태로 잘못 판정되던 문제 수정
- Extension 제거 후 nested equipment stat이 유지되던 문제 수정
- Backpack 재장착 시 extension 내부 장비 stat이 재적용되지 않던 문제 수정


### Notes
- nested 구조 개선, status 추가
