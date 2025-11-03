namespace TeamSuneat.Data.Game
{
    [System.Serializable]
    public partial class VProfile
    {
        /// <summary> 할당한 아이템의 고유 번호</summary>
        public int IssuedItemSID;

        public VCharacter Character;
        public VCharacterLevel Level;
        public VCharacterWeapon Weapon;
        public VCurrency Currency;
        public VCharacterStat Stat;
        public VStage Stage;
        public VStatistics Statistics;

        public CharacterNames CharacterName => Character.SelectedCharacterName;

        public void OnLoadGameData()
        {
            CreateEmptyData();

            Character.OnLoadGameData();
            Weapon.OnLoadData();
            Currency.OnLoadGameData();
            Stat.OnLoadGameData();
            Statistics.OnLoadGameData();
            Stage.OnLoadGameData();
        }

        public void CreateEmptyData()
        {
            Character ??= new VCharacter();
            Level ??= new VCharacterLevel();
            Weapon ??= new VCharacterWeapon();
            Currency ??= new();
            Stat ??= new VCharacterStat();
            Statistics ??= new();
            Stage ??= new VStage();
        }

        public void ClearIngameData()
        {
            // 사망시 레벨과 경험치를 초기화합니다.
            Level.ResetValues();

            // 전투 자원을 초기화합니다.
            Stat.ResetCurrentVitalValue();

            Log.Info(LogTags.GameData, $"[Character] {CharacterName.ToLogString()}, 플레이어 캐릭터의 인게임 데이터를 초기화합니다.");

            // 인게임 재화를 초기화합니다.
            Currency.ClearIngameCurrencies();

            // 무기 정보를 초기화합니다.
            Weapon.ClearIngameData();

            Statistics.ClearIngameData();
        }

        public static VProfile CreateDefault()
        {
            Log.Info(LogTags.GameData, $"새로운 게임 데이터를 생성합니다.");

            VProfile defaultProfile = new();
            defaultProfile.CreateEmptyData();

            // 기본 캐릭터 추가
            defaultProfile.Character.Register(CharacterNames.IronWarden);
            defaultProfile.Character.Register(CharacterNames.ShadowAssassin);
            defaultProfile.Character.Register(CharacterNames.BloodRaven);

            // 기본 캐릭터 선택
            defaultProfile.Character.Select(CharacterNames.IronWarden);

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