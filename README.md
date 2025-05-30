# 🎨 Colorfly

Colorfly는 **‘색’**을 테마로 한 퍼즐 기반 상호작용 게임입니다.

플레이어는 나비 캐릭터를 조작하여 다양한 오브젝트들과 색을 조합하고, 이를 통해 목표 지점에 도달하는 것이 목표입니다.

<br/>

## 목차
[1.개요](#개요)

[2.게임 설명](#게임-설명)

[3.맵 에디터](#맵-에디터)

<br/>

## 개요

![Image Sequence_001_0000](https://github.com/user-attachments/assets/8c076eff-a801-462e-b1b8-25af56e034ee)

이름: COLORFLY

장르: 소코반, 퍼즐 게임

기간: 2024.08 ~ 진행 중

도구: Unity, C#, Git

인원: 6인

<br/>

## 게임 설명
<img src="https://github.com/user-attachments/assets/bf25f842-51bc-482c-980b-06808302f174" width="45%" height="45%">
<img src="https://github.com/user-attachments/assets/81a4d79a-e606-4651-bee8-4a11de9b636d" width="45%" height="45%">

### 🧩 게임 소개

Colorfly는 단순한 이동 퍼즐이 아닌, 색의 조합과 상태 변화, 오브젝트 간 상호작용에 기반한 창의적인 색 퍼즐 게임입니다.

각 맵은 다양한 오브젝트들로 구성되어 있으며, 이들을 이해하고 활용해 정답에 도달해야 합니다.
<br/><br/>


### 🎮 주요 시스템
**플레이어 조작**

+ 방향키로 상하좌우 인접한 칸으로 이동할 수 있습니다. 

+ Z 키를 이용해 이전 턴으로 되돌릴 수 있습니다.

<br/>


**색 변화 메커니즘:**

+ 물감: 오브젝트와 결합해 색을 바꿀 수 있습니다.

+ 브러쉬: 색을 혼합할 수 있습니다.

+ 스펀지: 색을 제거할 수 있습니다.

+ 팔레트: 색을 저장해두고 필요할 때 꺼내 쓸 수 있습니다.

<br/>

**상태 변화 및 물리적 상호작용:**

+ 아크릴: 오브젝트를 코팅 상태로 만들어 밀 수 있게 합니다.

+ 나무 망치: 코팅 상태를 제거합니다.

+ 브러쉬: 플레이어와 상호작용시 색 혼합 준비 상태로 바뀝니다.

+ 트레일 브러쉬: 지나가는 길에 자신과 같은 색의 색타일을 배치합니다.

+ 색모래: 일정 턴수 동안 색을 바꿉니다.

<br/>

**조건부 지형 요소:**

+ 색타일: 같은 색일 때만 위로 올라갈 수 있습니다.

+ 필터: 칸과 칸 사이에 존재하며 같은 색일 때만 통과할 수 있습니다.

<br/>

**기타 오브젝트**
+ 자물쇠: 정해진 3개의 스탬프를 순서대로 밟으면 해제되는 벽입니다.

+ 스탬프: 같은 색일때 밟을 시 상호작용합니다.

<br/>

### 🗺️ 맵 예시

<img src="https://github.com/user-attachments/assets/28d285a4-185d-4cc3-a7a2-d2fa9384e104" width="45%" height="45%">
<img src="https://github.com/user-attachments/assets/34c7f20c-5ee4-4406-a1ba-6ce7901f97f7" width="45%" height="45%">
<img src="https://github.com/user-attachments/assets/fcb0a4f2-3fa8-4388-b7d0-d1b1543147a9" width="45%" height="45%">
<img src="https://github.com/user-attachments/assets/64d9adf9-b31a-4333-90ff-075158f751a5" width="45%" height="45%">

<br/><br/>

## 맵 에디터
![Image Sequence_017_0000](https://github.com/user-attachments/assets/4789b529-d6bf-4f0f-8433-20273c16f159)

만들어진 맵을 플레이하는 것 뿐만 아니라 유저가 직접 맵을 제작하고 플레이할 수도 있습니다.

### 주요 시스템

**기본 기능**

+ 설정 버튼을 통해 맵 크기를 변경할 수 있습니다.

+ Ctrl + S 로 저장할 수 있습니다.

+ Ctrl + Z / Ctrl + Y 로 실행 취소 (Undo) 와 다시 실행 (Redo) 이 가능합니다.

+ T 로 현재 맵을 테스트 할 수 있습니다.

<br/>

**에디터 모드**
+ 펜: 오브젝트를 배치할 수 있습니다.

+ 지우개: 오브젝트를 제거합니다.

+ 선택: 오브젝트를 선택해 세부 옵션을 설정합니다.



