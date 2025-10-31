namespace TeamSuneat.Data
{
    public partial class VCharacterStage
    {
        private void AddChallengeCount()
        {
            Game.VProfile profileInfo = GameApp.GetSelectedProfile();
            if (profileInfo != null)
            {
                profileInfo.Statistics.AddChallengeCount();
                profileInfo.Statistics.RegisterChallengeStartTime();
                profileInfo.Statistics.SetGameplayTimeSaveValidity(true);
            }
        }
    }
}