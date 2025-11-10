using UnityEngine;

namespace TeamSuneat
{
    /// <summary>
    /// 전장의 타일 그리드와 게임 로직을 관리하는 컴포넌트
    /// Battlefield와 BattlefieldManager의 모든 기능을 통합한 클래스입니다.
    /// 최대 100개의 타일을 미리 생성하여 재사용합니다.
    /// </summary>
    public class BattlefieldTileGroup : XBehaviour
    {
        public const int HEIGHT = 10;
        private const int MAX_WIDTH = 10;

        [SerializeField]
        private BattlefieldTile[] _allTiles; // 미리 생성된 모든 타일 (최대 100개)
        private BattlefieldTile[] _tiles; // 현재 활성화된 타일만 참조
        private int _centerColumn;
        private bool _isInitialized;

        public int Width { get; private set; }
        public int Height => HEIGHT;

        [field: SerializeField]
        public float TileSize { get; private set; } = 0.5f;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _allTiles = GetComponentsInChildren<BattlefieldTile>(true);
        }

        /// <summary>
        /// 전장을 초기화합니다.
        /// </summary>
        public void Initialize(int width, Vector3 originPosition)
        {
            if (!ValidateWidth(width))
            {
                return;
            }

            SetupProperties(width, originPosition);
            EnsureTilesInitialized();
            DeactivateAllTiles();
            ActivateTiles();
            SpawnMonsters();

            Log.Info(LogTags.Stage, "전장 초기화 완료: ({0}x{1})", Width, HEIGHT);
        }

        /// <summary>
        /// Width 값이 유효한지 검증합니다.
        /// </summary>
        private bool ValidateWidth(int width)
        {
            // Width 홀수 검증
            if (width % 2 == 0)
            {
                Log.Error(LogTags.Stage, "전장 Width는 홀수여야 합니다: {0}", width);
                return false;
            }

            // Width 최대값 검증
            if (width > MAX_WIDTH)
            {
                Log.Error(LogTags.Stage, "전장 Width는 최대 {0}까지 가능합니다: {1}", MAX_WIDTH, width);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 전장의 속성을 설정합니다.
        /// </summary>
        private void SetupProperties(int width, Vector3 originPosition)
        {
            transform.position = originPosition;
            Width = width;
            _centerColumn = (width - 1) / 2;
        }

        /// <summary>
        /// 타일이 미리 생성되어 있는지 확인하고, 없으면 생성합니다.
        /// </summary>
        private void EnsureTilesInitialized()
        {
            if (!_isInitialized)
            {
                if (_allTiles == null || _allTiles.Length == 0)
                {
                    Log.Warning(LogTags.Stage, "타일이 미리 생성되지 않았습니다. AutoGetComponents를 실행해주세요.");
                }

                _isInitialized = true;
            }
        }

        /// <summary>
        /// 필요한 타일만 활성화하고 초기화합니다.
        /// </summary>
        private void ActivateTiles()
        {
            int totalTiles = HEIGHT * Width;
            _tiles = new BattlefieldTile[totalTiles];

            for (int row = 0; row < HEIGHT; row++)
            {
                for (int column = 0; column < Width; column++)
                {
                    int index = GetIndex(row, column); // 현재 width 기준 인덱스
                    int allTilesIndex = (row * MAX_WIDTH) + column; // _allTiles 배열 인덱스 (MAX_WIDTH 기준)
                    Vector3 worldPosition = GetTileWorldPosition(row, column);

                    // 미리 생성된 타일 재사용
                    BattlefieldTile tile = _allTiles[allTilesIndex];
                    if (tile == null)
                    {
                        Log.Error(LogTags.Stage, "타일을 찾을 수 없습니다: AllTilesIndex={0}", allTilesIndex);
                        continue;
                    }

                    // 타일 활성화 및 위치 설정
                    tile.gameObject.SetActive(true);
                    tile.transform.position = worldPosition;
                    tile.Initialize(row, column, index);
                    _tiles[index] = tile;
                }
            }
        }

        /// <summary>
        /// 모든 타일을 비활성화합니다.
        /// </summary>
        private void DeactivateAllTiles()
        {
            if (_allTiles == null)
            {
                return;
            }

            for (int i = 0; i < _allTiles.Length; i++)
            {
                if (_allTiles[i] != null && _allTiles[i].gameObject != null)
                {
                    _allTiles[i].gameObject.SetActive(false);
                }
            }
        }

        private void SpawnMonsters()
        {
            for (int row = 0; row < HEIGHT; row++)
            {
                for (int column = 0; column < Width; column++)
                {
                    int index = GetIndex(row, column); // 현재 width 기준 인덱스
                    int allTilesIndex = (row * MAX_WIDTH) + column; // _allTiles 배열 인덱스 (MAX_WIDTH 기준)
                    Vector3 worldPosition = GetTileWorldPosition(row, column);

                    // 미리 생성된 타일 재사용
                    BattlefieldTile tile = _allTiles[allTilesIndex];
                    if (tile == null)
                    {
                        Log.Error(LogTags.Stage, "타일을 찾을 수 없습니다: AllTilesIndex={0}", allTilesIndex);
                        continue;
                    }

                    // 타일 활성화 및 위치 설정
                    tile.gameObject.SetActive(true);
                    tile.transform.position = worldPosition;
                    tile.Initialize(row, column, index);
                    _tiles[index] = tile;
                }
            }
        }

        //

        /// <summary>
        /// 타일의 월드 좌표를 계산합니다.
        /// </summary>
        public Vector3 GetTileWorldPosition(int row, int column)
        {
            float x = transform.position.x + ((column - _centerColumn) * TileSize);
            float y = transform.position.y + (row * TileSize);
            float z = transform.position.z;

            return new Vector3(x, y, z);
        }

        /// <summary>
        /// Row와 Column을 선형 인덱스로 변환합니다.
        /// 좌측 최하단(row=0, column=0)이 0부터 시작하여 우측으로, 다음 행으로 진행합니다.
        /// </summary>
        public int GetIndex(int row, int column)
        {
            return (row * Width) + column;
        }

        /// <summary>
        /// 선형 인덱스를 Row로 변환합니다.
        /// </summary>
        public int GetRow(int index)
        {
            return index / Width;
        }

        /// <summary>
        /// 선형 인덱스를 Column으로 변환합니다.
        /// </summary>
        public int GetColumn(int index)
        {
            return index % Width;
        }

        /// <summary>
        /// 선형 인덱스로 타일을 조회합니다.
        /// </summary>
        public BattlefieldTile GetTile(int index)
        {
            if (index < 0 || index >= _tiles.Length)
            {
                return null;
            }

            return _tiles[index];
        }

        /// <summary>
        /// Row와 Column으로 타일을 조회합니다.
        /// </summary>
        public BattlefieldTile GetTile(int row, int column)
        {
            if (!IsValidTile(row, column))
            {
                return null;
            }

            int index = GetIndex(row, column);
            return GetTile(index);
        }

        /// <summary>
        /// 타일 좌표가 유효한지 검증합니다.
        /// </summary>
        public bool IsValidTile(int row, int column)
        {
            if (_tiles == null)
            {
                return false;
            }

            return row >= 0 && row < HEIGHT && column >= 0 && column < Width;
        }

        /// <summary>
        /// 선형 인덱스가 유효한지 검증합니다.
        /// </summary>
        public bool IsValidIndex(int index)
        {
            if (_tiles == null)
            {
                return false;
            }

            return index >= 0 && index < _tiles.Length;
        }

        /// <summary>
        /// 타일이 점유되어 있는지 확인합니다.
        /// </summary>
        public bool IsTileOccupied(int row, int column)
        {
            BattlefieldTile tile = GetTile(row, column);
            return tile != null && tile.IsOccupied;
        }

        /// <summary>
        /// 타일을 점유 상태로 설정합니다.
        /// </summary>
        public void SetTileOccupied(int row, int column, MonsterCharacter monster)
        {
            BattlefieldTile tile = GetTile(row, column);
            if (tile != null)
            {
                tile.SetMonster(monster);
            }
        }

        /// <summary>
        /// 타일을 비웁니다.
        /// </summary>
        public void ClearTile(int row, int column)
        {
            BattlefieldTile tile = GetTile(row, column);
            tile?.Clear();
        }

        /// <summary>
        /// 스폰 위치가 유효한지 검증합니다.
        /// </summary>
        public bool ValidateSpawnPosition(int row, int column)
        {
            // 타일 유효성 확인
            if (!IsValidTile(row, column))
            {
                return false;
            }

            // 타일 점유 여부 확인
            if (IsTileOccupied(row, column))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 스폰 위치를 조회하고 검증합니다.
        /// </summary>
        public bool TryGetSpawnPosition(int row, int column, out Vector3 worldPosition)
        {
            worldPosition = Vector3.zero;

            if (!ValidateSpawnPosition(row, column))
            {
                return false;
            }

            worldPosition = GetTileWorldPosition(row, column);
            return true;
        }

        /// <summary>
        /// 모든 타일을 비웁니다.
        /// </summary>
        public void ClearAllTiles()
        {
            if (_tiles == null)
            {
                return;
            }

            for (int row = 0; row < Height; row++)
            {
                for (int column = 0; column < Width; column++)
                {
                    BattlefieldTile tile = GetTile(row, column);
                    tile?.Clear();
                }
            }
        }

        /// <summary>
        /// 모든 타일을 정리합니다. (비활성화만 수행, GameObject는 유지)
        /// </summary>
        public void Clear()
        {
            ClearAllTiles();
            DeactivateAllTiles();
            _tiles = null;
            Width = 0;
            _centerColumn = 0;
        }
    }
}