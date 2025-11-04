namespace TeamSuneat.Data.Game
{
    [System.Serializable]
    public partial class VProfile
    {
        /// <summary> 할당한 아이템의 고유 번호</summary>
        public int IssuedItemSID;
        public VCharacter Character;
        public VCharacterLevel Level;
        public VCharacterStat Stat;

        public VCharacterWeapon Weapon;
        public VCharacterPotion Potion;
        public VCharacterItem Item;
        public VCurrency Currency;

        public VCharacterStage Stage;
        public VCharacterSlot Slot;
        public VStatistics Statistics;

        public void OnLoadGameData()
        {
            CreateEmptyData();

            Character.OnLoadGameData();
            Weapon.OnLoadGameData();
            Potion.OnLoadGameData();
            Item.OnLoadGameData();
            Currency.OnLoadGameData();

            Stage.OnLoadGameData();
            Slot.OnLoadGameData();
            Stat.OnLoadGameData();
            Statistics.OnLoadGameData();
        }

        public void CreateEmptyData()
        {
            Character ??= VCharacter.CreateDefault();
            Level ??= VCharacterLevel.CreateDefault();
            Weapon ??= VCharacterWeapon.CreateDefault();
            Potion ??= VCharacterPotion.CreateDefault();
            Item ??= VCharacterItem.CreateDefault();
            Currency ??= VCurrency.CreateDefault();
            Stage ??= VCharacterStage.CreateDefault();
            Slot ??= VCharacterSlot.CreateDefault();
            Stat ??= VCharacterStat.CreateDefault();
            Statistics ??= VStatistics.CreateDefault();
        }

        public void ClearIngameData()
        {
            // 사망시 레벨과 경험치를 초기화합니다.
            Level.ResetValues();

            // 인게임 무기 정보를 초기화합니다.
            Weapon.ClearIngameData();

            // 인게임 물약 정보를 초기화합니다.
            Potion.ClearIngameData();

            // 인게임 아이템 정보를 초기화합니다.
            Item.ClearIngameData();

            // 인게임 재화를 초기화합니다.
            Currency.ClearIngameCurrencies();

            // 인게임 능력치 정보를 초기화합니다.
            Stat.ClearIngameData();

            // 인게임 통계 정보를 초기화합니다.
            Statistics.ClearIngameData();
        }

        public static VProfile CreateDefault()
        {
            Log.Info(LogTags.GameData, $"새로운 게임 데이터를 생성합니다.");
            VProfile defaultProfile = new();
            defaultProfile.CreateEmptyData();

            return defaultProfile;
        }

        public int GenerateItemSID()
        {
            return ++IssuedItemSID;
        }

        internal int GetAdditionalTreasureClassCurrentDifficulty()
        {
            return 0;
        }
    }
}