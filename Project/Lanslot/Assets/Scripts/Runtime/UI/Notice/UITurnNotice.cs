using System;
using TeamSuneat.Data;
using TMPro;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    public enum TurnNoticeOwner
    {
        None,
        Player,
        Monster
    }

    public class UITurnNotice : XBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _titleText;

        [SerializeField]
        [Tooltip("표시 유지 시간(초)")]
        private float _displayDuration = 1.5f;

        [SerializeField]
        private UICanvasGroupFader _fader;

        public event Action OnCompleted;

        private void Awake()
        {
            AutoGetComponents();
            _fader ??= GetComponentInChildren<UICanvasGroupFader>(true);
        }

        public void Show(TurnNoticeOwner owner)
        {
            string format = JsonDataManager.FindStringClone("Format_Turn");
            string content = JsonDataManager.FindStringClone($"Camp_{owner}");
            if (!string.IsNullOrEmpty(format) && !string.IsNullOrEmpty(content))
            {
                Show(string.Format(format, content));
            }
        }

        public void Show(string content)
        {
            if (_titleText != null)
            {
                _titleText.text = content;
            }

            gameObject.SetActive(true);

            if (_fader != null)
            {
                _fader.KillFade();
                _fader.SetCompletedCallback(OnFadeOutComplete);
                _fader.FadeInOut();
            }
            else
            {
                OnFadeOutComplete();
            }
        }

        private void OnFadeOutComplete()
        {
            _fader?.SetCompletedCallback(null);
            gameObject.SetActive(false);
            OnCompleted?.Invoke();
        }

        private void OnDisable()
        {
            _fader?.KillFade();
        }
    }
}