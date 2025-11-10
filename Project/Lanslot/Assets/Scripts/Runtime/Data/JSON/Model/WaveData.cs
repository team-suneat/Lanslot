namespace TeamSuneat.Data
{
    [System.Serializable]
    public class WaveData : IData<int>
    {
        public StageNames StageName;
        public int WaveNumber;
        public WaveTypes WaveType;
        public int SpawnRow;
        public int SpawnColumn;
        public int MinMonsterCount;
        public int MaxMonsterCount;
        public CharacterNames Monster1;
        public float Monster1Chance;
        public CharacterNames Monster2;
        public float Monster2Chance;
        public CharacterNames Monster3;
        public float Monster3Chance;

        public bool IsBossWave => WaveType == WaveTypes.Boss;

        public bool IsNormalWave => WaveType == WaveTypes.Normal;

        public int GetMonsterCount()
        {
            return RandomEx.Range(MinMonsterCount, MaxMonsterCount);
        }

        public CharacterNames GetRandomMonster()
        {
            // 유효한 몬스터와 확률을 수집
            System.Collections.Generic.List<(CharacterNames monster, float chance)> candidates = new();

            if (Monster1 != CharacterNames.None && Monster1Chance > 0f)
            {
                candidates.Add((Monster1, Monster1Chance));
            }
            if (Monster2 != CharacterNames.None && Monster2Chance > 0f)
            {
                candidates.Add((Monster2, Monster2Chance));
            }
            if (Monster3 != CharacterNames.None && Monster3Chance > 0f)
            {
                candidates.Add((Monster3, Monster3Chance));
            }

            if (candidates.Count == 0)
            {
                return CharacterNames.None;
            }

            // 가중치 총합 계산
            float totalWeight = 0f;
            for (int i = 0; i < candidates.Count; i++)
            {
                totalWeight += candidates[i].chance;
            }

            if (totalWeight <= 0f)
            {
                return candidates[0].monster; // 기본값
            }

            // 랜덤 선택
            float randomValue = UnityEngine.Random.Range(0f, totalWeight);
            float currentWeight = 0f;

            for (int i = 0; i < candidates.Count; i++)
            {
                currentWeight += candidates[i].chance;
                if (randomValue <= currentWeight)
                {
                    return candidates[i].monster;
                }
            }

            return candidates[candidates.Count - 1].monster;
        }

        public int GetKey()
        {
            return StageName.ToInt();
        }

        public void Refresh()
        {
        }

        public void OnLoadData()
        {
        }
    }
}