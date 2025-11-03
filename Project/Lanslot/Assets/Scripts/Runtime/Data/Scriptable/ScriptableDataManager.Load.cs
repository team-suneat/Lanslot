using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace TeamSuneat.Data
{
    public partial class ScriptableDataManager
    {
        public bool CheckLoaded()
        {
            if (_logSetting == default) { return false; }
            else if (_gameDefine == default) { return false; }
            else if (_levelExp == default) { return false; }
            else if (!_buffs.IsValid()) { return false; }
            else if (!_buffStateEffects.IsValid()) { return false; }
            else if (!_passives.IsValid()) { return false; }
            else if (!_hitmarks.IsValid()) { return false; }
            else if (!_fonts.IsValid()) { return false; }
            else if (!_floatys.IsValid()) { return false; }
            else if (!_flickers.IsValid()) { return false; }
            else if (!_sounds.IsValid()) { return false; }

            return true;
        }

        protected void OnLoadData()
        {
            // Core.cs의 OnLoadData() 메서드 호출
            _logSetting?.OnLoadData();

            // 사운드 OnLoadData() 메서드 호출
            foreach (KeyValuePair<int, SoundAsset> asset in _sounds)
            {
            }
        }

        public void LoadScriptableAssets()
        {
            int count = 0;
            string[] pathArray = PathManager.FindAllAssetPath();
            Clear();

            Log.Info("스크립터블 파일을 읽기 시작합니다.");

            for (int i = 0; i < pathArray.Length; i++)
            {
                string path = pathArray[i];

                if (LoadLogSettingSync(path))
                {
                    count += 1;
                }
                else if (LoadGameDefineSync(path))
                {
                    count += 1;
                }
                else if (LoadLevelExpSync(path))
                {
                    count += 1;
                }
                else if (LoadBuffSync(path))
                {
                    count += 1;
                }
                else if (LoadBuffStateEffectSync(path))
                {
                    count += 1;
                }
                else if (LoadPassiveSync(path))
                {
                    count += 1;
                }
                else if (LoadHitmarkSync(path))
                {
                    count += 1;
                }
                else if (LoadFontSync(path))
                {
                    count += 1;
                }
                else if (LoadFloatySync(path))
                {
                    count += 1;
                }
                else if (LoadFlickerSync(path))
                {
                    count += 1;
                }
                else if (LoadSoundSync(path))
                {
                    count += 1;
                }
            }

            Log.Info("파일을 읽어왔습니다. Count: {0}", count.ToString());

            OnLoadData();
        }

        public async Task LoadScriptableAssetsAsync()
        {
            Clear();
            int count = 0;

            IList<ScriptableObject> assets = await ResourcesManager.LoadResourcesByLabelAsync<UnityEngine.ScriptableObject>(AddressableLabels.Scriptable);
            for (int i = 0; i < assets.Count; i++)
            {
                ScriptableObject asset = assets[i];
                if (asset == null)
                {
                    continue;
                }

                switch (asset)
                {
                    case LogSettingAsset logSetting:
                        _logSetting = logSetting;
                        count++;
                        break;

                    case GameDefineAsset gameDefine:
                        _gameDefine = gameDefine;
                        count++;
                        break;

                    case LevelExpAsset levelExp:
                        _levelExp = levelExp;
                        count++;
                        break;

                    case BuffAsset buff:
                        if (!_buffs.ContainsKey(buff.TID))
                        {
                            _buffs[buff.TID] = buff;
                            count++;
                        }
                        break;

                    case BuffStateEffectAsset buffStateEffect:
                        if (!_buffStateEffects.ContainsKey(buffStateEffect.TID))
                        {
                            _buffStateEffects[buffStateEffect.TID] = buffStateEffect;
                            count++;
                        }
                        break;

                    case PassiveAsset passive:
                        if (!_passives.ContainsKey(passive.TID))
                        {
                            _passives[passive.TID] = passive;
                            count++;
                        }
                        break;

                    case HitmarkAsset hitmark:
                        if (!_hitmarks.ContainsKey(hitmark.TID))
                        {
                            _hitmarks[hitmark.TID] = hitmark;
                            count++;
                        }
                        break;

                    case FontAsset font:
                        if (!_fonts.ContainsKey(font.TID))
                        {
                            _fonts[font.TID] = font;
                            count++;
                        }
                        break;

                    case FloatyAsset floaty:
                        if (!_floatys.ContainsKey(floaty.TID))
                        {
                            _floatys[floaty.TID] = floaty;
                            count++;
                        }
                        break;

                    case FlickerAsset flicker:
                        if (!_flickers.ContainsKey(flicker.TID))
                        {
                            _flickers[flicker.TID] = flicker;
                            count++;
                        }
                        break;

                    case SoundAsset sound:
                        if (!_sounds.ContainsKey(sound.TID))
                        {
                            _sounds[sound.TID] = sound;
                            count++;
                        }
                        break;
                }
            }

            Log.Info("Addressable Scriptable 라벨로 파일을 읽어왔습니다. Count: {0}", count.ToString());

            OnLoadData();
        }
    }
}