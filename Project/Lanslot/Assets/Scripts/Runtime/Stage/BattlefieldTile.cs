using UnityEngine;

namespace TeamSuneat
{
    /// <summary>
    /// 전장의 타일 정보를 나타내는 클래스
    /// </summary>
    [System.Serializable]
    public class BattlefieldTile
    {
        public int Row;
        public int Column;
        public Vector3 WorldPosition;
        public bool IsOccupied;
        public MonsterCharacter CurrentMonster;

        public BattlefieldTile(int row, int column, Vector3 worldPosition)
        {
            Row = row;
            Column = column;
            WorldPosition = worldPosition;
            IsOccupied = false;
            CurrentMonster = null;
        }

        /// <summary>
        /// 타일을 비웁니다.
        /// </summary>
        public void Clear()
        {
            IsOccupied = false;
            CurrentMonster = null;
        }

        /// <summary>
        /// 타일에 몬스터를 배치합니다.
        /// </summary>
        public void SetMonster(MonsterCharacter monster)
        {
            CurrentMonster = monster;
            IsOccupied = monster != null;
        }
    }
}