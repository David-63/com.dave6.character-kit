## [0.0.3] - 2025.12.11

### Added

- AnimatorHandler 추가
-  현재 Locomotion 애니메이션은 FreeLook 만 구현
-  ChangeAnimation 함수는 1회성 애니메이션 호출용 함수

- CombatHandler 추가
-  MeleeState의 공격 로직을 전부 위임, 
-  State는 진입조건과 CombatHandler에게 공격 요청만 진행