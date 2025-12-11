# com.dave6.character-kit
**Unity** 에서 캐릭터 컨트롤러를 구축하기 위한 가벼운 패키지



## Requirements

- Unity Input System
- Cinemachine 3.1+
- Timer Package
- Unity Util Package
- State Machine Package
- Stat System package



## Features

기본 캐릭터 컨트롤러 구성 요소
간단한 3인칭 카메라 타겟 설정
상태 기반 캐릭터 로직
스탯 기반 캐릭터 능력치 관리 (Stat System 연동)
샘플 프리팹 포함



## Quick Start

1. **Samples** 에 제공된 캐릭터 프리팹을 씬에 추가
2. 씬에 있는 `Main Camera` 에 `Cinemachine Brain` 컴포넌트 추가 및 `MainCamera` 태그 설정
3. 필요에 따라 `Profile` 구성



## Troubleshooting

1. 카메라가 인식되지 않음 (Camera reference is null)
메인 카메라 오브젝트에 `Main Camera` 태그가 설정되어 있지 않음, 태그 재설정

2. 카메라가 캐릭터에 붙어서 보임
플레이어의 오브젝트 layer가 default로 되어있음, 다른 레이어로 설정