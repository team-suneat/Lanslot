using System.Collections.Generic;

namespace TeamSuneat.Data
{
    /// <summary> 프로젝트에서 생성한 Scriptable Object를 관리합니다. </summary>
    public partial class ScriptableDataManager : Singleton<ScriptableDataManager>
    {
        private GameDefineAsset _gameDefine;
        private LogSettingAsset _logSetting;        

        private readonly Dictionary<int, HitmarkAsset> _hitmarks = new();
        private readonly Dictionary<int, BuffAsset> _buffs = new();
        private readonly Dictionary<int, BuffStateEffectAsset> _buffStateEffects = new();
        private readonly Dictionary<int, PassiveAsset> _passives = new();
        private readonly Dictionary<int, FontAsset> _fonts = new();
        private readonly Dictionary<int, FloatyAsset> _floatys = new();
        private readonly Dictionary<int, FlickerAsset> _flickers = new();
        private readonly Dictionary<int, SoundAsset> _sounds = new();

        public void Clear()
        {
            _logSetting = null;
            _gameDefine = null;

            _sounds.Clear();
            _hitmarks.Clear();
            _buffs.Clear();
            _buffStateEffects.Clear();
            _passives.Clear();
            _fonts.Clear();
            _floatys.Clear();
            _flickers.Clear();
        }

        public void RefreshAll()
        {
            RefreshAllBuff();
            RefreshAllPassive();
            RefreshAllHitmarks();
            RefreshAllFonts();
            RefreshAllFlickers();
            RefreshAllFloatys();
            RefreshAllSounds();
        }
    }
}