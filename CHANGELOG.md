## [0.0.15] - 2025.04.18

### Added
- Interactor 핸들러 추가
 - Interactor는 상호작용 대상을 감지하는 역할만 수행
- Interaction UI view 추가
- Interaction 로직과 UI 를 바인더로 통합함

### Changed
- Combat 핸들러의 진행을 리펙토링하여 책임을 분리함
 - 각 모듈은 액션 실행만 담당함
 - Combat 핸들러가 ActionState의 라이프사이클 및 종료 조건을 결정하도록 변경
 - EvaluateExit(판단 포함) 대신에 IsFinised 를 사용하여 모듈 인터페이스의 규모를 축소함
- UI 입력과 플레이어 입력 흐름 분리

### Removed

### Refactored

### Notes
