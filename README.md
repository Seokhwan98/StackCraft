# StackCraft

## **프로젝트 개요**
카드 형태의 유닛, 건물, 자원 등을 배치하고 조합하여 나타나는 적을 물리쳐 스테이지를 클리어하는 솔리테어기반 전략 시뮬레이션 게임(Stacklands 모작 게임)

#### :movie_camera:[시연 영상](https://youtu.be/VLrasWr7BJQ)


## 주요 기능

### 게임 시작

- 게임 시작 버튼을 누르면 스테이지 선택 창으로 이동하며, 각 스테이지에 마우스를 올리면 간단한 설명이 하단에 출력됩니다.
- 스테이지를 선택하면 해당 스테이지로 즉시 진입합니다.

  <img src="https://github.com/user-attachments/assets/6bc8d4aa-feba-462a-887a-22f2f62324e4" width="70%" height="70%"/>
  

### <br>상호작용

- 카드를 클릭하면 해당 카드를 집을 수 있습니다.
- 카드를 들고 있는 동안, 상호작용이 가능한 카드에는 외곽선이 표시되어 시각적으로 안내됩니다.
- 외곽선이 표시된 카드 위에 들고 있는 카드를 드래그 앤 드롭하면, 두 카드가 겹쳐지며 상호작용이 시작됩니다.

  <img src="https://github.com/user-attachments/assets/a6735ef8-1fa7-488e-8f59-1b1c738e23b0" width="70%" height="70%"/>


### <br>카드생성

- 카드 위의 타이머가 완료되면, 레시피에 따라 해당 결과 카드가 생성됩니다.
- 생성된 카드는 주변에 동일한 종류의 카드가 있을 경우 자동으로 해당 스택에 합쳐집니다.

  <img src="https://github.com/user-attachments/assets/93c148f6-3862-4e3f-b09f-d7834ed440ec" width="70%" height="70%"/>


### <br>건물건축

- 화면 상단(12시 방향)에 위치한 건축 슬롯에, 해당 건물의 레시피에 해당하는 카드를 스택해두면 상호작용이 시작합니다.
- 일정시간이 지나 타이머가 완료되면, 건물 카드가 생성됩니다.
- 생성된 건물 카드를 활용해 새로운 상호작용을 진행할 수 있습니다.
  
  <img src="https://github.com/user-attachments/assets/e44a5bf0-678e-4e3d-9652-84a10279fc96" width="70%" height="70%"/>


### <br>전투

- 적 카드와 병사 카드를 겹치면, 카드 수에 비례한 전투 영역이 생성됩니다.
- 생성된 영역 안에서는 카드들이 자동으로 전투를 진행하며, 전투가 종료되면 영역이 사라지고, 살아남은 카드들은 해당 위치에 그대로 남습니다.

  <img src="https://github.com/user-attachments/assets/16e00999-cb14-4f44-8c4e-9708460e7e0e" width="70%" height="70%"/>


## <br>기술 스택

Unity, C#


## <br>팀원 소개
| 김현오 <a href="https://github.com/singateco"><img src="https://github.com/user-attachments/assets/5cf4751a-cd8d-4328-b893-d8f76379e049" width="16" height="16"/></a> | 조윤희 <a href="https://github.com/y0c0y"><img src="https://github.com/user-attachments/assets/5cf4751a-cd8d-4328-b893-d8f76379e049" width="16" height="16"/></a> | 최석환 <a href="https://github.com/Seokhwan98"><img src="https://github.com/user-attachments/assets/5cf4751a-cd8d-4328-b893-d8f76379e049" width="16" height="16"/></a> |
|:---:|:---:|:---:|
| <img src="https://github.com/user-attachments/assets/2cb02642-54cc-4e4f-8c6e-8caf826b0dfc" width="200" height="200"/> | <img src="https://github.com/user-attachments/assets/2cb02642-54cc-4e4f-8c6e-8caf826b0dfc" width="200" height="200"/> | <img src="https://github.com/user-attachments/assets/2cb02642-54cc-4e4f-8c6e-8caf826b0dfc" width="200" height="200"/> |
| 개발 | 개발 | 개발/기획 |

