using System;
using System.Collections.Generic;

namespace TeamSuneat.Data
{
    [System.Serializable]
    public partial class VStatistics
    {
        // 플레이어의 도전 횟수
        public int ChallengeCount;

        // 플레이어의 죽음 횟수
        public int DeathCount;

        // 처치한 몬스터와 그 수
        public Dictionary<string, int> KilledMonsters = new();

        // 게임 시작 시간
        public DateTime GameStartTime;

        // 마지막 게임 저장 시간
        public DateTime LastSaveTime;

        // 가장 빠른 게임 클리어 시간 : 사용 안함
        public TimeSpan SpeedrunClearTime = TimeSpan.Zero;

        // 도전 시작 시간
        public DateTime ChallengeStartTime;

        // 난이도 단계별 스피드런 클리어 시간 (0: 모험 난이도, 1~: 악몽 난이도 단계)
        public Dictionary<int, TimeSpan> DifficultyStepClearTimes = new();

        // 현재 진행 중인 도전의 게임플레이 시간 (초)
        public float CurrentGameplayTime = 0f;

        // 정상적으로 도전 시간을 저장했는지 여부 (클리프샤이어에서만 true로 설정)
        public bool IsValidGameplayTimeSaved = false;

        public void OnLoadGameData()
        {
        }

        public void ClearIngameData()
        {
        }

        public static VStatistics CreateDefault()
        {
            return new VStatistics();
        }
    }
}