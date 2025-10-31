#if !DISABLESTEAMWORKS
using Steamworks;
#endif

#if EOS_SUPPORTED && !EOS_DISABLE
using PlayEveryWare.EpicOnlineServices;
using Epic.OnlineServices;
using Epic.OnlineServices.Achievements;
#endif

using UnityEngine;

namespace TeamSuneat.Data.Game
{
    /// <summary>
    /// 기본 업적 시스템 클래스
    /// 특정 이벤트에 제한적인 기능들을 제거하고 핵심 업적 기능만 제공
    /// Steam과 EOS 플랫폼 지원은 유지
    /// </summary>
    public partial class VProfile
    {
        /// <summary>
        /// 업적 해금 (기본 기능)
        /// </summary>
        /// <param name="achievementName">해금할 업적 이름</param>
        public void UnlockAchievement(AchievementNames achievementName)
        {
            if (GameDefine.EDITOR_OR_DEVELOPMENT_BUILD)
            {
                Debug.Log($"[Steamworks.NET] 에디터 또는 개발용 빌드일 때, 업적의 해금이 무시됩니다: {achievementName}");
                return;
            }

            UnlockAchievementForSteam(achievementName);
            UnlockAchievementForEpic(achievementName);
        }

        /// <summary>
        /// 업적 잠금 (개발용)
        /// </summary>
        /// <param name="ID">잠글 업적 ID</param>
        public void LockAchievement(string ID)
        {
            if (!GameDefine.EDITOR_OR_DEVELOPMENT_BUILD)
            {
                Debug.Log($"[Steamworks.NET] 에디터 또는 개발용 빌드가 아닐 때, 업적을 다시 잠글 수 없습니다: {ID}");
                return;
            }

            LockAchievementForSteam(ID);
        }

        //────────────────────────────────────────────────────────────────────────────────────────────────────────────────

        private void UnlockAchievementForSteam(AchievementNames achievementName)
        {
#if !DISABLESTEAMWORKS
            if (!SteamManager.Initialized || !SteamManager.SteamStatsReady)
            {
                Debug.Log($"Steam 초기화되지 않았거나 Stats 준비 중: {achievementName}");
                return;
            }

            string achieveID = achievementName.ToString();
            if (!SteamUserStats.GetAchievement(achieveID, out bool achieved))
            {
                Debug.Log($"[Steamworks.NET] 업적 상태 확인 실패 (GetAchievement false): {achieveID}");
                return;
            }

            if (!achieved)
            {
                if (!SteamUserStats.SetAchievement(achieveID))
                {
                    Debug.Log($"[Steamworks.NET] 업적 설정 실패: {achieveID}");
                    return;
                }

                if (!SteamUserStats.StoreStats())
                {
                    Debug.Log($"[Steamworks.NET] 업적 저장(StoreStats) 실패: {achieveID}");
                }
                else
                {
                    Debug.Log($"[Steamworks.NET] 업적 해금 성공: {achieveID}");
                }
            }
#endif
        }

        private void LockAchievementForSteam(string ID)
        {
#if !DISABLESTEAMWORKS
            if (SteamUserStats.GetAchievement(ID, out bool isUnlock))
            {
                SteamUserStats.ClearAchievement(ID);
                if (SteamUserStats.StoreStats())
                {
                    Debug.Log($"[Steamworks.NET] 도전과제를 잠금합니다. {ID}");
                }
                else
                {
                    Debug.LogWarning($"[Steamworks.NET] 도전과제의 잠금 시도가 실패했습니다. {ID}");
                }
            }
            else
            {
                Debug.LogWarning($"[Steamworks.NET] 이미 도전과제가 잠금되었습니다. {ID}");
            }
#endif
        }

        //────────────────────────────────────────────────────────────────────────────────────────────────────────────────

        private void UnlockAchievementForEpic(AchievementNames achievementName)
        {
#if EOS_SUPPORTED && !EOS_DISABLE
            string achievementId = achievementName.ToString();
            var eosManager = EOSManager.Instance;
            ProductUserId productUserId = eosManager.GetProductUserId();

            if (productUserId == null)
            {
                Debug.LogWarning($"[EOS] 유저 로그인 상태가 아닙니다. 업적 해금 실패: {achievementId}");
                return;
            }

            UnlockAchievementsOptions unlockOptions = new()
            {
                UserId = productUserId,
                AchievementIds = new Utf8String[] { achievementId }
            };
            var platformInterface = eosManager.GetEOSPlatformInterface();
            AchievementsInterface achievementsInterface = platformInterface.GetAchievementsInterface();
            achievementsInterface.UnlockAchievements(ref unlockOptions, null,
                (ref OnUnlockAchievementsCompleteCallbackInfo result) =>
                {
                    if (result.ResultCode == Result.Success)
                    {
                        Debug.Log($"[EOS] 업적 해금 성공: {achievementId}");
                    }
                    else
                    {
                        Debug.LogWarning($"[EOS] 업적 해금 실패: {achievementId}, {result.ResultCode}");
                    }
                });
#endif
        }

        //────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    }
}