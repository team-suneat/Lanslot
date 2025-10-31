using System.Collections.Generic;

namespace TeamSuneat.Data
{
    /// <summary>
    /// ScriptableDataManager의 폰트 관련 기능
    /// </summary>
    public partial class ScriptableDataManager
    {
        #region Font Get Methods

        /// <summary>
        /// 폰트 에셋을 가져옵니다.
        /// </summary>
        public FontAsset GetFontAsset(LanguageNames languageName)
        {
            int key = BitConvert.Enum32ToInt(languageName);
            return _fonts.TryGetValue(key, out var asset) ? asset : null;
        }

        #endregion Font Get Methods

        #region Font Find Methods

        /// <summary>
        /// 폰트 에셋을 찾습니다.
        /// </summary>
        public FontAsset FindFont(LanguageNames key)
        {
            return FindFont(BitConvert.Enum32ToInt(key));
        }

        public FontAsset FindFont(int tid)
        {
            if (_fonts.ContainsKey(tid))
            {
                return _fonts[tid];
            }

            return null;
        }

        #endregion Font Find Methods

        #region Font Load Methods

        /// <summary>
        /// 폰트 에셋을 동기적으로 로드합니다.
        /// </summary>
        private bool LoadFontSync(string filePath)
        {
            if (!filePath.Contains("Font_"))
            {
                return false;
            }

            FontAsset asset = ResourcesManager.LoadResource<FontAsset>(filePath);
            if (asset != null)
            {
                if (!_fonts.ContainsKey(asset.TID))
                {
                    Log.Progress("스크립터블 데이터를 읽어왔습니다. Path: {0}", filePath);
                    _fonts[asset.TID] = asset;
                }

                return true;
            }
            else
            {
                Log.Warning("스크립터블 데이터를 읽을 수 없습니다. Path: {0}", filePath);
            }

            return false;
        }

        #endregion Font Load Methods

        #region Font Refresh Methods

        /// <summary>
        /// 모든 폰트 에셋을 리프레시합니다.
        /// </summary>
        public void RefreshAllFonts()
        {
            foreach (KeyValuePair<int, FontAsset> item in _fonts) { Refresh(item.Value); }
        }

        private void Refresh(FontAsset fontAsset)
        {
            fontAsset?.Refresh();
        }

        #endregion Font Refresh Methods
    }
}
