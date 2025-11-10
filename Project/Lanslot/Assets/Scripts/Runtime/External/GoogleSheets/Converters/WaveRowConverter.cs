using System.Collections.Generic;
using TeamSuneat.Data;

namespace TeamSuneat
{
    public sealed class WaveRowConverter : IGoogleSheetRowConverter<WaveData>
    {
        public bool TryConvert(Dictionary<string, string> row, out WaveData model)
        {
            model = null;

            if (!row.TryGetValue("StageName", out string stageNameStr) || !GoogleSheetValueParsers.TryParseEnum<StageNames>(stageNameStr, out StageNames stageName))
            {
                Log.Warning($"필수 컬럼 StageName 누락 또는 enum 파싱 실패: {stageNameStr}");
                return false;
            }

            if (!row.TryGetValue("WaveNumber", out string waveNumberStr) || !GoogleSheetValueParsers.TryParseInt(waveNumberStr, out int waveNumber))
            {
                Log.Warning($"StageName {stageName}: WaveNumber 파싱 실패: {waveNumberStr}");
                return false;
            }

            if (!row.TryGetValue("SpawnRow", out string spawnRowStr) || !GoogleSheetValueParsers.TryParseInt(spawnRowStr, out int spawnRow))
            {
                Log.Warning($"StageName {stageName}, WaveNumber {waveNumber}: SpawnRow 파싱 실패: {spawnRowStr}");
                return false;
            }

            if (!row.TryGetValue("SpawnColumn", out string spawnColumnStr) || !GoogleSheetValueParsers.TryParseInt(spawnColumnStr, out int spawnColumn))
            {
                Log.Warning($"StageName {stageName}, WaveNumber {waveNumber}: SpawnColumn 파싱 실패: {spawnColumnStr}");
                return false;
            }

            if (!row.TryGetValue("MinMonsterCount", out string minMonsterCountStr) || !GoogleSheetValueParsers.TryParseInt(minMonsterCountStr, out int minMonsterCount))
            {
                Log.Warning($"StageName {stageName}, WaveNumber {waveNumber}: MinMonsterCount 파싱 실패: {minMonsterCountStr}");
                return false;
            }

            if (!row.TryGetValue("MaxMonsterCount", out string maxMonsterCountStr) || !GoogleSheetValueParsers.TryParseInt(maxMonsterCountStr, out int maxMonsterCount))
            {
                Log.Warning($"StageName {stageName}, WaveNumber {waveNumber}: MaxMonsterCount 파싱 실패: {maxMonsterCountStr}");
                return false;
            }

            if (!row.TryGetValue("Type", out string waveTypeStr) || !GoogleSheetValueParsers.TryParseEnum<WaveTypes>(waveTypeStr, out WaveTypes waveType))
            {
                Log.Warning($"StageName {stageName}, WaveNumber {waveNumber}: WaveType 파싱 실패: {waveTypeStr}");
                return false;
            }

            CharacterNames monster1 = CharacterNames.None;
            float monster1Chance = 0f;
            if (row.TryGetValue("Monster1", out string monster1Str))
            {
                GoogleSheetValueParsers.TryParseEnum<CharacterNames>(monster1Str, out monster1);
            }
            if (row.TryGetValue("Monster1Chance", out string monster1ChanceStr))
            {
                GoogleSheetValueParsers.TryParseFloat(monster1ChanceStr, out monster1Chance);
            }

            CharacterNames monster2 = CharacterNames.None;
            float monster2Chance = 0f;
            if (row.TryGetValue("Monster2", out string monster2Str))
            {
                GoogleSheetValueParsers.TryParseEnum<CharacterNames>(monster2Str, out monster2);
            }
            if (row.TryGetValue("Monster2Chance", out string monster2ChanceStr))
            {
                GoogleSheetValueParsers.TryParseFloat(monster2ChanceStr, out monster2Chance);
            }

            CharacterNames monster3 = CharacterNames.None;
            float monster3Chance = 0f;
            if (row.TryGetValue("Monster3", out string monster3Str))
            {
                GoogleSheetValueParsers.TryParseEnum<CharacterNames>(monster3Str, out monster3);
            }
            if (row.TryGetValue("Monster3Chance", out string monster3ChanceStr))
            {
                GoogleSheetValueParsers.TryParseFloat(monster3ChanceStr, out monster3Chance);
            }

            WaveData m = new()
            {
                StageName = stageName,
                WaveNumber = waveNumber,
                WaveType = waveType,
                SpawnRow = spawnRow,
                SpawnColumn = spawnColumn,
                MinMonsterCount = minMonsterCount,
                MaxMonsterCount = maxMonsterCount,
                Monster1 = monster1,
                Monster1Chance = monster1Chance,
                Monster2 = monster2,
                Monster2Chance = monster2Chance,
                Monster3 = monster3,
                Monster3Chance = monster3Chance,
            };

            model = m;
            return true;
        }
    }
}