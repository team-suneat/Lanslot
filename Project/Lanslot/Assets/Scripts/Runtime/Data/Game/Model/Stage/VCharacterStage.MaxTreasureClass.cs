using TeamSuneat.Data.Game;

namespace TeamSuneat.Data
{
    public partial class VCharacterStage
    {
        /// <summary>
        /// 최대 도달 TC를 설정합니다.
        /// </summary>
        public void SetMaxReachedTreasureClass(int stageTC)
        {
            VProfile profileInfo = GameApp.GetSelectedProfile();
            int additionalTC = profileInfo.GetAdditionalTreasureClassCurrentDifficulty();
            int treasureClass = stageTC + additionalTC;
            if (MaxReachedTreasureClass < treasureClass)
            {
                MaxReachedTreasureClass = treasureClass;
                LogInfo("최대 도달 TC를 설정합니다. {0}", treasureClass);
            }
        }

        /// <summary>
        /// 최대 도달 TC를 증가시킵니다. (For Develop)
        /// </summary>
        public void AddMaxReachedTreasureClass()
        {
            int treasureClass = MaxReachedTreasureClass + 1;
            if (MaxReachedTreasureClass < treasureClass)
            {
                MaxReachedTreasureClass = treasureClass;
                LogInfo("최대 도달 TC를 설정합니다. {0}", treasureClass);
            }
        }

        /// <summary>
        /// 최대 도달 TC를 감소시킵니다. (For Develop)
        /// </summary>
        public void SubtractMaxReachedTreasureClass()
        {
            if (MaxReachedTreasureClass > 1)
            {
                int treasureClass = MaxReachedTreasureClass - 1;
                MaxReachedTreasureClass = treasureClass;
                LogInfo("최대 도달 TC를 설정합니다. {0}", treasureClass);
            }
        }
    }
}