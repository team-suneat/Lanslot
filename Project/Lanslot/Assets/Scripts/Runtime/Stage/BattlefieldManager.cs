using UnityEngine;

namespace TeamSuneat
{
    /// <summary>
    /// 전장의 타일 그리드를 관리하는 클래스
    /// </summary>
    public class BattlefieldManager
    {
        public const int HEIGHT = 10;

        private BattlefieldTile[,] _tiles;
        private Vector3 _originPosition;
        private int _centerColumn;

        public int Width { get; private set; }

        public Vector3 OriginPosition => _originPosition;

        public float TileSize { get; private set; }

        /// <summary>
        /// 전장을 초기화합니다.
        /// </summary>
        public void Initialize(int width, Vector3 originPosition, float tileSize)
        {
            // Width 홀수 검증
            if (width % 2 == 0)
            {
                Log.Error(LogTags.Stage, "전장 Width는 홀수여야 합니다: {0}", width);
                return;
            }

            Width = width;
            _originPosition = originPosition;
            TileSize = tileSize;
            _centerColumn = (width - 1) / 2;

            // 타일 배열 생성 (10xWidth)
            _tiles = new BattlefieldTile[HEIGHT, Width];

            // 각 타일 초기화
            for (int row = 0; row < HEIGHT; row++)
            {
                for (int column = 0; column < Width; column++)
                {
                    Vector3 worldPosition = GetTileWorldPosition(row, column);
                    _tiles[row, column] = new BattlefieldTile(row, column, worldPosition);
                }
            }

            Log.Info(LogTags.Stage, "전장 초기화 완료: Width={0}, Height={1}, TileSize={2}", Width, HEIGHT, TileSize);
        }

        /// <summary>
        /// 타일의 월드 좌표를 계산합니다.
        /// </summary>
        public Vector3 GetTileWorldPosition(int row, int column)
        {
            float x = _originPosition.x + ((column - _centerColumn) * TileSize);
            float y = _originPosition.y + (row * TileSize);
            float z = _originPosition.z;

            return new Vector3(x, y, z);
        }

        /// <summary>
        /// 타일을 조회합니다.
        /// </summary>
        public BattlefieldTile GetTile(int row, int column)
        {
            if (!IsValidTile(row, column))
            {
                return null;
            }

            return _tiles[row, column];
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
            tile?.SetMonster(monster);
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

            for (int row = 0; row < HEIGHT; row++)
            {
                for (int column = 0; column < Width; column++)
                {
                    _tiles[row, column]?.Clear();
                }
            }
        }

        /// <summary>
        /// 전장을 완전히 정리합니다.
        /// </summary>
        public void Clear()
        {
            ClearAllTiles();
            _tiles = null;
            Width = 0;
            _originPosition = Vector3.zero;
            TileSize = 0f;
            _centerColumn = 0;
        }
    }
}