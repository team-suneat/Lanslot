using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TeamSuneat.Data
{
    [Serializable]
    public class StageAssetData : ScriptableData<int>
    {
        [SuffixLabel("에셋 변경 모드")]
        public bool IsChangingAsset;

        [EnableIf("IsChangingAsset")]
        [SuffixLabel("스테이지 이름")]
        public StageNames Name;

        [EnableIf("IsChangingAsset")]
        [SuffixLabel("지역 이름")]
        public AreaNames Area;

        [SuffixLabel("스테이지 순서")]
        [GUIColor("GetIntColor")]
        public int Order;

        [SuffixLabel("스테이지 TC")]
        [GUIColor("GetIntColor")]
        public int TreasureClass;

        [Title("웨이브 설정")]
        [InfoBox("이 스테이지에서 진행될 웨이브들을 설정합니다. 시간 순서대로 자동으로 전환됩니다.")]
        [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "WaveName")]
        public List<WaveData> Waves = new List<WaveData>();

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

        public int TID => BitConvert.Enum32ToInt(Name);

        public int WaveCount => Waves?.Count ?? 0;

        public bool HasValidWaves => Waves != null && Waves.Count > 0;

        public int LastWaveIndex => WaveCount > 0 ? WaveCount - 1 : -1;

        public WaveData GetWave(int index)
        {
            if (index < 0 || index >= WaveCount)
                return null;

            return Waves[index];
        }

        public override int GetKey()
        {
            return TID;
        }

        public override void Refresh()
        {
            base.Refresh();

            AutoSettingWaves();
            IsChangingAsset = false;
        }

        public override void OnLoadData()
        {
            // 모든 웨이브 데이터 검증
            if (Waves != null)
            {
                for (int i = 0; i < Waves.Count; i++)
                {
                    Waves[i]?.Validate();
                }
            }
        }

        public void AutoSettingWaves()
        {
            if (Waves == null) return;

            for (int i = 0; i < Waves.Count; i++)
            {
                Waves[i]?.Refresh();
            }
        }
    }
}