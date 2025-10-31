using Sirenix.OdinInspector;
using System.Collections;
using TeamSuneat.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat
{
    public class GameLoadingScene : XScene
    {
        [Title("LoadingScene")]
        public FadeSceneLoader FadeSceneLoader;
        public TextMeshProUGUI LoadingTitleText;
        public TextMeshProUGUI LoadingDescText;
        public Image LoadingImage;

        [Title("LoadingScene-Index")]
        public int TipIndex;

        public int WorldViewMaxIndex;
        public int TipMaxIndex;

        #region Scene

        protected override void OnCreateScene()
        {
        }

        protected override void OnEnterScene()
        {
            // Initialize();
        }

        private void Initialize()
        {
            if (CheckShowLoadingText())
            {
                if (FadeSceneLoader.FadeInDuration > 0)
                {
                    StartCoroutine(ProcessRefreshLoadingText());
                }
                else
                {
                    RefreshLoadingText();
                }
            }
            else
            {
                DeactivateAll();
            }
        }

        private IEnumerator ProcessRefreshLoadingText()
        {
            yield return new WaitForSeconds(FadeSceneLoader.FadeInDuration);
            RefreshLoadingText();
        }

        protected override void OnExitScene()
        {
        }

        protected override void OnDestroyScene()
        {
        }

        #endregion Scene

        // Active

        private bool CheckShowLoadingText()
        {
            if (FadeSceneLoader == null)
            {
                return false;
            }

            return true;
        }

        private void DeactivateAll()
        {
            LoadingTitleText.SetActive(false);
            LoadingDescText.SetActive(false);
            LoadingImage.SetActive(false);
        }

        // Text

        private void RefreshLoadingText()
        {
            // 일반 팁
            RefreshLoadingTipText();
        }

        private void RefreshLoadingTipText()
        {
            int index = RandomEx.Range(1, TipMaxIndex + 1);
            RefreshLoadingTipText(index);
        }

        private void RefreshLoadingTipText(int index)
        {
            string titleContent = JsonDataManager.FindStringClone("Loading_Tip_Title");
            SetLoadingTitleText(titleContent);
            SetLoadingDescText(index.GetLoadingTipString());
        }

        private void RefreshLoadingWorldviewText()
        {
            int index = RandomEx.Range(1, WorldViewMaxIndex + 1);
            RefreshLoadingWorldviewText(index);
        }

        private void RefreshLoadingWorldviewText(int index)
        {
            SetLoadingTitleText(index.GetLoadingTitleString());
            SetLoadingDescText(index.GetLoadingDescString());
        }

        private void SetLoadingTitleText(string content)
        {
            if (LoadingTitleText != null)
            {
                LoadingTitleText.SetText(content);

                if (!string.IsNullOrEmpty(content))
                {
                    LoadingTitleText.SetActive(true);
                }
            }
        }

        private void SetLoadingDescText(string content)
        {
            if (LoadingDescText != null)
            {
                LoadingDescText.SetText(content);

                if (!string.IsNullOrEmpty(content))
                {
                    LoadingDescText.SetActive(true);
                }
            }
        }
    }
}