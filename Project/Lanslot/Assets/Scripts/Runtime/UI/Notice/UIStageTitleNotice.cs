using System.Collections;
using TeamSuneat.Data;
using TMPro;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    public class UIStageTitleNotice : XBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _titleText;

        [SerializeField]
        [Tooltip("표시 유지 시간(초)")]
        private float _displayDuration = 2f;

        [SerializeField]
        private UICanvasGroupFader _fader;

        private Coroutine _displayRoutine;

        private void Awake()
        {
            AutoGetComponents();
            _fader ??= GetComponentInChildren<UICanvasGroupFader>(true);
        }

        public void Show(StageNames stageName)
        {
            Show(stageName.GetLocalizedString());
        }

        public void Show(string content)
        {
            if (_displayRoutine != null)
            {
                StopCoroutine(_displayRoutine);
            }

            if (_titleText != null)
            {
                _titleText.text = content;
            }

            gameObject.SetActive(true);

            if (_fader != null)
            {
                _fader.KillFade();
                _fader.SetAlpha(0f);
            }

            _displayRoutine = StartCoroutine(DisplayRoutine());
        }

        private IEnumerator DisplayRoutine()
        {
            _fader?.FadeIn();

            float waitTime = Mathf.Max(0f, _displayDuration);
            yield return new WaitForSeconds(waitTime);

            if (_fader != null)
            {
                _fader.SetCompeletedCallback(OnFadeOutComplete);
                _fader.FadeOut();
            }
            else
            {
                OnFadeOutComplete();
            }
        }

        private void OnFadeOutComplete()
        {
            _fader?.SetCompeletedCallback(null);
            gameObject.SetActive(false);

            if (_displayRoutine != null)
            {
                StopCoroutine(_displayRoutine);
                _displayRoutine = null;
            }
        }

        private void OnDisable()
        {
            if (_displayRoutine != null)
            {
                StopCoroutine(_displayRoutine);
                _displayRoutine = null;
            }

            _fader?.KillFade();
        }
    }
}

