namespace TeamSuneat
{
    public enum CharacterNames
    {
        None,

        IronWarden = 1001,  // 철의 수호자
        ShadowAssassin,     // 그림자 암살자
        BloodRaven,         // 핏빛 까마귀
        ThunderSeer,        // 천둥 점술사
        PainWeaver,         // 고통술사
        Forgemaster,        // 대장장이
        CombatAlchemist,    // 전투 연금술사
        DuelMaster,         // 결투의 대가
        BloodyBaron,        // 피의 남작
        GoldenGambler,      // 황금 도박꾼
        RoseKnight,         // 장미의 기사
        Gunslinger,         // 총잡이
        Trickster,          // 노름꾼

        // 적 몬스터들 (A~H타입, 일반3마리+정예3마리+보스1마리, 10단위 간격)
        // A타입 - 고블린 계열 (2000~2006)
        Goblin = 2000,        // 고블린 (일반)
        GoblinWarrior,        // 고블린 전사 (일반)
        GoblinArcher,         // 고블린 궁수 (일반)
        GoblinKnight,         // 고블린 기사 (정예)
        GoblinShaman,         // 고블린 주술사 (정예)
        GoblinAssassin,       // 고블린 암살자 (정예)
        GoblinKing,           // 고블린 왕 (보스)

        // B타입 - 오크 계열 (2010~2016)
        Orc = 2010,           // 오크 (일반)
        OrcWarrior,           // 오크 전사 (일반)
        OrcArcher,            // 오크 궁수 (일반)
        OrcChampion,          // 오크 챔피언 (정예)
        OrcShaman,            // 오크 샤먼 (정예)
        OrcBerserker,         // 오크 광전사 (정예)
        OrcWarlord,           // 오크 장군 (보스)

        // C타입 - 언데드 계열 (2020~2026)
        Skeleton = 2020,      // 해골 (일반)
        SkeletonWarrior,      // 해골 전사 (일반)
        SkeletonArcher,       // 해골 궁수 (일반)
        SkeletonKnight,       // 해골 기사 (정예)
        SkeletonMage,         // 해골 마법사 (정예)
        SkeletonCaptain,      // 해골 대장 (정예)
        SkeletonKing,         // 해골 왕 (보스)

        // D타입 - 거인 계열 (2030~2036)
        Giant = 2030,         // 거인 (일반)
        GiantWarrior,         // 거인 전사 (일반)
        GiantThrower,         // 거인 투척자 (일반)
        GiantKnight,          // 거인 기사 (정예)
        GiantMage,            // 거인 마법사 (정예)
        GiantGeneral,         // 거인 장군 (정예)
        GiantKing,            // 거인 왕 (보스)

        // E타입 - 트롤 계열 (2040~2046)
        Troll = 2040,         // 트롤 (일반)
        TrollWarrior,         // 트롤 전사 (일반)
        TrollArcher,          // 트롤 궁수 (일반)
        TrollKnight,          // 트롤 기사 (정예)
        TrollShaman,          // 트롤 샤먼 (정예)
        TrollBerserker,       // 트롤 광전사 (정예)
        TrollKing,            // 트롤 킹 (보스)

        // F타입 - 야수 계열 (2050~2056)
        Wolf = 2050,          // 늑대 (일반)
        Bear,                 // 곰 (일반)
        Boar,                 // 멧돼지 (일반)
        WolfKing,             // 늑대왕 (정예)
        BearKing,             // 곰왕 (정예)
        BoarKing,             // 멧돼지왕 (정예)
        BeastGod,             // 야수신 (보스)

        // G타입 - 식물 계열 (2060~2066)
        VineMonster = 2060,   // 덩굴괴물 (일반)
        MushroomMonster,      // 버섯괴물 (일반)
        ThornVine,            // 가시덩굴 (일반)
        AncientTree,          // 고대나무 (정예)
        PoisonMushroomKing,   // 독버섯왕 (정예)
        ThornKing,            // 가시왕 (정예)
        PlantGod,             // 식물신 (보스)

        // H타입 - 타락한 기사 계열 (2070~2076)
        FallenKnight = 2070,  // 타락한 기사 (일반)
        FallenArcher,         // 타락한 궁수 (일반)
        FallenMage,           // 타락한 마법사 (일반)
        FallenPaladin,        // 타락한 성기사 (정예)
        FallenMagicKnight,    // 타락한 마법기사 (정예)
        FallenCaptain,        // 타락한 기사장 (정예)
        FallenKing,           // 타락한 기사왕 (보스)
    }
}