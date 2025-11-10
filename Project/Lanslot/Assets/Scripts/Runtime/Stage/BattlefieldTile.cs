namespace TeamSuneat
{
    /// <summary>
    /// 전장의 타일 GameObject를 나타내는 컴포넌트
    /// BattlefieldTile와 기존 BattlefieldTile 데이터 클래스를 통합한 클래스입니다.
    /// </summary>
    public class BattlefieldTile : XBehaviour
    {
        public int Row { get; private set; }
        public int Column { get; private set; }
        public int Index { get; private set; }
        public bool IsOccupied { get; private set; }
        public MonsterCharacter CurrentMonster { get; private set; }

        /// <summary>
        /// 타일을 초기화합니다.
        /// </summary>
        public void Initialize(int row, int column, int index)
        {
            Row = row;
            Column = column;
            Index = index;
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