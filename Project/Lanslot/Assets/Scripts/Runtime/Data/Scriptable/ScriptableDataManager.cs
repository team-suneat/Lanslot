using System.Collections.Generic;

namespace TeamSuneat.Data
{
    /// <summary> 프로젝트에서 생성한 Scriptable Object를 관리합니다. </summary>
    public partial class ScriptableDataManager : Singleton<ScriptableDataManager>
    {
        private GameDefineAsset _gameDefine;
        private LogSettingAsset _logSetting;        

        private readonly Dictionary<int, HitmarkAsset> _hitmarkAssets = new();
        private readonly Dictionary<int, BuffAsset> _buffAssets = new();
        private readonly Dictionary<int, BuffStateEffectAsset> _stateEffectAssets = new();
        private readonly Dictionary<int, PassiveAsset> _passiveAssets = new();
        private readonly Dictionary<int, FontAsset> _fontAssets = new();
        private readonly Dictionary<int, FloatyAsset> _floatyAssets = new();
        private readonly Dictionary<int, FlickerAsset> _flickerAssets = new();
        private readonly Dictionary<int, SoundAsset> _soundAssets = new();

        public void Clear()
        {
            _logSetting = null;
            _gameDefine = null;

            _soundAssets.Clear();
            _hitmarkAssets.Clear();
            _buffAssets.Clear();
            _stateEffectAssets.Clear();
            _passiveAssets.Clear();
            _fontAssets.Clear();
            _floatyAssets.Clear();
            _flickerAssets.Clear();
        }

        public void RefreshAll()
        {
            RefreshAllBuff();
            RefreshAllPassive();
            RefreshAllHitmark();
            RefreshAllFonts();
            RefreshAllFlicker();
            RefreshAllFloaty();
            RefreshAllSounds();
        }
    }
}