using System;
using System.Linq;
using Sirenix.OdinInspector;

namespace TeamSuneat.Data
{
    [Serializable]
    public class AreaAssetData
    {
        [SuffixLabel("첫번째 전투 스테이지에서 정예 몬스터 생성 무시")]
        public bool IgnoreEliteInFirstStage;

        [SuffixLabel("전투 지역 여부")]
        public bool IsBattleArea;

        [MinValue(0)]
        [SuffixLabel("전투 스테이지 개수")]
        public int BattleStageCount = 2;

        [SuffixLabel("전투 슬롯")]
        [EnableIf("IsBattleArea")]
        public BattleStageSlotFlags BattleSlots = BattleStageSlotFlags.First | BattleStageSlotFlags.Second;

        [SuffixLabel("정예 몬스터 생성 수")]
        public int EliteCount;

        /// <summary>
        /// 이 지역이 전투 스테이지 슬롯을 사용하지 않는 특수 지역임을 명시합니다.
        /// </summary>
        [Title("특수 지역")]
        [LabelText("전투 스테이지 슬롯 미사용")]
        public bool IsNoBattleStageArea;

        [Title("Build Type")]
        [InfoBox("지원하는 빌드 타입을 설정합니다. 설정되지 않은 빌드에서는 해당 스테이지가 나타나지 않습니다.")]
        public BuildTypes[] SupportedBuildTypes;

        public bool IsBlock
        {
            get
            {
                if (!SupportedBuildTypes.IsValidArray())
                    return true;

#if UNITY_EDITOR
                GameDefineAssetData defineAssetData = ScriptableDataManager.Instance.GetGameDefine().Data;
                return !SupportedBuildTypes.Contains(defineAssetData.EDITOR_BUILD_TYPE);
#endif

                if (GameDefine.DEVELOPMENT_BUILD)
                {
                    return !SupportedBuildTypes.Contains(BuildTypes.Development);
                }

                return !SupportedBuildTypes.Contains(BuildTypes.Live);
            }
        }

        public bool IncludesSlot(int slotNumber)
        {
            return BattleSlots.IncludesSlot(slotNumber);
        }
    }
}