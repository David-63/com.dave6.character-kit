# com.dave6.character-kit
**Unity** 에서 캐릭터 컨트롤러를 구축하기 위한 내부용 프레임워크 패키지


## Requirements

- Unity Input System
- Cinemachine 3.1+
- Unity Util Package
- Timer Package
- Foundation
- Third person camera
- Item System
- Stat System package
- Object Pooling System
- Surface Reaction System

## Scope (Current)
- 캐릭터 공용 구조 기반 구성
-   Base Character logic (player/npc 확장 가능)
- 모듈 기반 캐릭터 로직
-   Mover
-   Combat
- 게임 루프 제어
-   Bootstrap + GameplayCore + Level AdditiveScene 구조
-   GameFlowController + PlayerConnector + SceneDirector에 의해 게임 상태 제어
- 모듈 아키텍처
-   Mono / Context / Action 계층 분리

## Planned (In progress / Upcomming)
- 애니메이션 시스템
- 스텟 시스템
- 로드아웃
- NPC

