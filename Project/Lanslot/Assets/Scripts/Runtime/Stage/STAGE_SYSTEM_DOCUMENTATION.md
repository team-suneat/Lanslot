# 스테이지 시스템 개발 문서

## 개요

스테이지 시스템은 게임의 전장(Battlefield)을 생성하고 관리하며, 웨이브별 몬스터를 스폰하는 시스템입니다. 타일 기반 그리드 시스템을 사용하여 몬스터를 배치하고 관리합니다.

## 시스템 구조

### 주요 클래스

1. **StageSystem** - 스테이지 전체를 관리하는 메인 시스템
2. **BattlefieldTileGroup** - 타일 그리드와 게임 로직을 관리
3. **BattlefieldTile** - 개별 타일을 나타내는 컴포넌트
4. **StageData** - 스테이지 메타데이터
5. **WaveData** - 웨이브별 몬스터 스폰 정보

## 데이터 구조

### StageData

스테이지의 기본 정보를 담는 데이터 클래스입니다.

```csharp
public class StageData : IData<int>
{
    public StageNames Name;              // 스테이지 이름
    public string DisplayName;           // 표시 이름
    public int Width;                    // 전장 너비 (홀수만 가능, 최대 10)
    public BuildTypes[] SupportedBuildTypes; // 지원 빌드 타입
}
```

**제약사항:**
- `Width`는 반드시 홀수여야 함
- `Width`의 최대값은 10

### WaveData

웨이브별 몬스터 스폰 정보를 담는 데이터 클래스입니다.

```csharp
public class WaveData : IData<int>
{
    public StageNames StageName;         // 스테이지 이름
    public int WaveNumber;                // 웨이브 번호 (1~10)
    public WaveTypes WaveType;            // 웨이브 타입 (Normal/Boss)
    public int SpawnRow;                  // 스폰 행
    public int SpawnColumn;               // 스폰 열
    public int MinMonsterCount;           // 최소 몬스터 수
    public int MaxMonsterCount;           // 최대 몬스터 수
    public CharacterNames Monster1;       // 몬스터 1 종류
    public float Monster1Chance;          // 몬스터 1 확률
    public CharacterNames Monster2;       // 몬스터 2 종류
    public float Monster2Chance;          // 몬스터 2 확률
    public CharacterNames Monster3;       // 몬스터 3 종류
    public float Monster3Chance;          // 몬스터 3 확률
}
```

**주요 메서드:**
- `GetMonsterCount()` - MinMonsterCount와 MaxMonsterCount 사이의 랜덤 값 반환
- `GetRandomMonster()` - 가중치 기반으로 랜덤 몬스터 종류 반환

## 타일 시스템

### BattlefieldTile

개별 타일을 나타내는 컴포넌트입니다.

**속성:**
- `Row` - 타일의 행 위치 (0~9)
- `Column` - 타일의 열 위치 (0~Width-1)
- `Index` - 선형 인덱스 (Row * Width + Column)
- `IsOccupied` - 타일이 점유되어 있는지 여부
- `CurrentMonster` - 현재 타일에 배치된 몬스터

**주요 메서드:**
- `Initialize(row, column, index)` - 타일 초기화
- `Clear()` - 타일 비우기
- `SetMonster(monster)` - 몬스터 배치

### BattlefieldTileGroup

타일 그리드를 관리하고 게임 로직을 처리하는 컴포넌트입니다.

**속성:**
- `Width` - 전장 너비 (홀수, 최대 10)
- `Height` - 전장 높이 (고정 10)
- `TileSize` - 타일 크기 (기본값 0.5f)

**타일 인덱싱:**
- 좌측 최하단(row=0, column=0)이 인덱스 0부터 시작
- 우측으로 진행: 0, 1, 2, ..., Width-1
- 다음 행: Width, Width+1, ..., 2*Width-1
- 공식: `index = row * Width + column`

**타일 미리 생성:**
- 최대 100개(10행 × 10열)의 타일을 미리 생성하여 재사용
- `AutoGetComponents()`에서 자식 오브젝트의 타일들을 자동으로 수집
- `Initialize()` 호출 시 필요한 타일만 활성화

## 초기화 프로세스

### 1. StageSystem.Initialize()

스테이지 초기화의 진입점입니다.

```csharp
public void Initialize()
{
    // 1. StageData 로드
    CurrentStageData = JsonDataManager.FindStageDataClone(Name);
    
    // 2. Width 검증 (홀수 확인)
    if (CurrentStageData.Width % 2 == 0) { return; }
    
    // 3. BattlefieldTileGroup 초기화
    _battlefieldTileGroup.Initialize(CurrentStageData.Width, originPosition);
    
    // 4. 웨이브 초기 세팅
    SetupInitialWaves(Name);
}
```

### 2. BattlefieldTileGroup.Initialize()

전장 그리드를 초기화합니다.

```csharp
public void Initialize(int width, Vector3 originPosition)
{
    // 1. Width 검증
    if (!ValidateWidth(width)) { return; }
    
    // 2. 속성 설정
    SetupProperties(width, originPosition);
    
    // 3. 타일 초기화 확인
    EnsureTilesInitialized();
    
    // 4. 모든 타일 비활성화
    DeactivateAllTiles();
    
    // 5. 필요한 타일만 활성화
    ActivateTiles();
    
    // 6. 몬스터 스폰 (현재는 타일 활성화와 동일한 로직)
    SpawnMonsters();
}
```

**초기화 단계 상세:**

1. **ValidateWidth(width)**
   - Width가 홀수인지 확인
   - Width가 MAX_WIDTH(10) 이하인지 확인

2. **SetupProperties(width, originPosition)**
   - 전장 위치 설정
   - Width, _centerColumn 설정

3. **EnsureTilesInitialized()**
   - 타일이 미리 생성되었는지 확인
   - 없으면 경고 로그 출력

4. **DeactivateAllTiles()**
   - 모든 타일을 비활성화

5. **ActivateTiles()**
   - 필요한 타일만 활성화
   - 타일 위치 및 인덱스 설정

## 몬스터 생성 프로세스

### SetupInitialWaves()

웨이브별 몬스터를 생성하고 배치합니다.

```csharp
private void SetupInitialWaves(StageNames stageName)
{
    WaveData waveData = null;
    
    for (int row = 0; row < Height; row++)
    {
        // 1. 웨이브 번호 계산 (Row + 1)
        int waveNumber = GetWaveNumberFromRow(row);
        
        // 2. 웨이브 데이터 로드
        WaveData tempWaveData = JsonDataManager.GetWaveDataByNumber(stageName, waveNumber);
        if (tempWaveData != null)
        {
            waveData = tempWaveData;
        }
        
        // 3. 몬스터 수 계산
        int monsterCount = waveData.GetMonsterCount();
        if (monsterCount <= 0) continue;
        
        // 4. 열 위치 셔플 (Deck 사용)
        Deck<int> deck = new Deck<int>();
        for (int column = 0; column < Width; column++)
        {
            deck.Add(column);
        }
        deck.Shuffle();
        
        // 5. 몬스터 생성 및 배치
        for (int i = 0; i < monsterCount; i++)
        {
            int column = deck.Get(i);
            CharacterNames characterName = waveData.GetRandomMonster();
            
            BattlefieldTile tile = _battlefieldTileGroup.GetTile(row, column);
            MonsterCharacter monster = ResourcesManager.SpawnPrefab<MonsterCharacter>(
                characterName.ToString(), 
                tile.transform
            );
            
            _battlefieldTileGroup.SetTileOccupied(row, column, monster);
        }
    }
}
```

### 몬스터 생성 흐름

1. **웨이브 데이터 로드**
   - Row를 웨이브 번호로 변환 (Row 0 = Wave 1)
   - `JsonDataManager.GetWaveDataByNumber()`로 웨이브 데이터 조회

2. **몬스터 수 결정**
   - `WaveData.GetMonsterCount()`로 MinMonsterCount와 MaxMonsterCount 사이의 랜덤 값 반환

3. **스폰 위치 결정**
   - Deck을 사용하여 열 위치를 셔플
   - 몬스터 수만큼 랜덤한 열 위치 선택

4. **몬스터 종류 결정**
   - `WaveData.GetRandomMonster()`로 가중치 기반 랜덤 몬스터 선택
   - Monster1, Monster2, Monster3의 Chance 값을 가중치로 사용

5. **몬스터 스폰**
   - `ResourcesManager.SpawnPrefab<MonsterCharacter>()`로 몬스터 생성
   - 타일의 Transform을 부모로 설정
   - `SetTileOccupied()`로 타일 점유 상태 설정

### WaveData.GetRandomMonster()

가중치 기반 랜덤 몬스터 선택 알고리즘:

```csharp
public CharacterNames GetRandomMonster()
{
    // 1. 유효한 몬스터와 확률 수집
    List<(CharacterNames monster, float chance)> candidates = new();
    
    // 2. 가중치 총합 계산
    float totalWeight = candidates.Sum(c => c.chance);
    
    // 3. 랜덤 값 생성 (0 ~ totalWeight)
    float randomValue = Random.Range(0f, totalWeight);
    
    // 4. 가중치 누적하여 선택
    float currentWeight = 0f;
    for (int i = 0; i < candidates.Count; i++)
    {
        currentWeight += candidates[i].chance;
        if (randomValue <= currentWeight)
        {
            return candidates[i].monster;
        }
    }
}
```

## 타일 좌표 시스템

### 월드 좌표 계산

타일의 월드 좌표는 중심 열(_centerColumn)을 기준으로 계산됩니다.

```csharp
public Vector3 GetTileWorldPosition(int row, int column)
{
    float x = transform.position.x + ((column - _centerColumn) * TileSize);
    float y = transform.position.y + (row * TileSize);
    float z = transform.position.z;
    
    return new Vector3(x, y, z);
}
```

**예시:**
- Width = 5인 경우, _centerColumn = 2
- column 0: x = origin.x + (0-2) * 0.5 = origin.x - 1.0
- column 2: x = origin.x + (2-2) * 0.5 = origin.x (중심)
- column 4: x = origin.x + (4-2) * 0.5 = origin.x + 1.0

### 인덱스 변환

**Row/Column → Index:**
```csharp
int index = row * Width + column;
```

**Index → Row/Column:**
```csharp
int row = index / Width;
int column = index % Width;
```

## 웨이브 시스템

### Row와 WaveNumber 매핑

- Row 0 → Wave 1
- Row 1 → Wave 2
- ...
- Row 9 → Wave 10

**변환 메서드:**
```csharp
public int GetWaveNumberFromRow(int row) => row + 1;
public int GetRowFromWaveNumber(int waveNumber) => waveNumber - 1;
```

### 웨이브 타입

- `WaveTypes.Normal` - 일반 웨이브
- `WaveTypes.Boss` - 보스 웨이브

`WaveData.IsBossWave` 또는 `WaveData.IsNormalWave`로 확인 가능합니다.

## 타일 점유 관리

### 타일 점유 상태 확인

```csharp
bool isOccupied = battlefieldTileGroup.IsTileOccupied(row, column);
```

### 타일 점유 설정

```csharp
battlefieldTileGroup.SetTileOccupied(row, column, monster);
```

### 타일 비우기

```csharp
battlefieldTileGroup.ClearTile(row, column);
```

### 스폰 위치 검증

```csharp
bool isValid = battlefieldTileGroup.ValidateSpawnPosition(row, column);
// 또는
bool success = battlefieldTileGroup.TryGetSpawnPosition(row, column, out Vector3 position);
```

## 정리 및 해제

### StageSystem.CleanupStage()

스테이지를 정리합니다.

```csharp
public void CleanupStage()
{
    _battlefieldTileGroup?.Clear();
    CurrentStageData = null;
    CurrentWaveNumber = 0;
}
```

### BattlefieldTileGroup.Clear()

모든 타일을 비활성화하고 상태를 초기화합니다.

```csharp
public void Clear()
{
    ClearAllTiles();      // 모든 타일의 몬스터 제거
    DeactivateAllTiles(); // 모든 타일 비활성화
    _tiles = null;
    Width = 0;
    _centerColumn = 0;
}
```

## 주의사항

1. **Width 제약**
   - 반드시 홀수여야 함
   - 최대값은 10

2. **타일 미리 생성**
   - 타일은 미리 생성되어 있어야 함
   - `AutoGetComponents()`를 통해 자동 수집되거나, 수동으로 할당 필요

3. **웨이브 데이터**
   - 웨이브 데이터가 없으면 해당 Row는 스킵됨
   - `monsterCount <= 0`인 경우 스킵됨

4. **몬스터 스폰**
   - 몬스터는 타일의 Transform을 부모로 하여 생성됨
   - 타일 점유 상태는 자동으로 설정됨

## 예제 코드

### 스테이지 초기화

```csharp
StageSystem stageSystem = GetComponent<StageSystem>();
stageSystem.Name = StageNames.Stage1;
stageSystem.Initialize();
```

### 특정 타일의 몬스터 확인

```csharp
BattlefieldTile tile = battlefieldTileGroup.GetTile(0, 2); // Row 0, Column 2
if (tile != null && tile.IsOccupied)
{
    MonsterCharacter monster = tile.CurrentMonster;
    // 몬스터 처리
}
```

### 스폰 위치 검증 및 몬스터 배치

```csharp
if (battlefieldTileGroup.TryGetSpawnPosition(row, column, out Vector3 position))
{
    MonsterCharacter monster = ResourcesManager.SpawnPrefab<MonsterCharacter>(
        characterName.ToString(), 
        position
    );
    battlefieldTileGroup.SetTileOccupied(row, column, monster);
}
```

