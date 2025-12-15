using DG.Tweening;
using Lean.Pool;
using Sirenix.OdinInspector;
using TeamSuneat.Data;
using TeamSuneat.Setting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    public class UIFloatyText : XBehaviour, IPoolable
    {
        public UILocalizedText Text;
        public Image TextImage;

        public AutoDespawn Despawner;
        public UIFollowObject FollowObject;

        [FoldoutGroup("Sprite-Damage")] public Sprite PhysicalCritialSprite;
        [FoldoutGroup("Sprite-Damage")] public Sprite MagicalCriticalSprite;
        [FoldoutGroup("Sprite-Damage")] public Sprite OverwhelmSprite;
        [FoldoutGroup("Sprite-Currency")] public Sprite GoldSprite;
        [FoldoutGroup("Sprite-Currency")] public Sprite MagicStoneSprite;
        [FoldoutGroup("Sprite-Currency")] public Sprite RubyKeySprite;

        private Tweener _moveTweener;
        private Tweener _scaleTweener;
        private Tweener _fadeTextTweener;
        private Tweener _fadeTextImageTweener;
        private Sequence _sequence;

        public string Content => Text.TextPro.text;

        private FloatyAsset Asset { get; set; }

        private UnityAction<UIFloatyText> DespawnCallback { get; set; }

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            Despawner = GetComponent<AutoDespawn>();
            FollowObject = GetComponent<UIFollowObject>();

            Text = GetComponentInChildren<UILocalizedText>();
        }

        public static UIFloatyMoveNames ConvertToName(DamageResult damageResult, VitalResourceTypes vitalResourceType)
        {
            if (damageResult.TargetCharacter != null)
            {
                if (damageResult.TargetCharacter.IsPlayer)
                {
                    if (vitalResourceType == VitalResourceTypes.Life)
                    {
                        return UIFloatyMoveNames.PlayerDamaged;
                    }
                    else if (vitalResourceType == VitalResourceTypes.Shield)
                    {
                        return UIFloatyMoveNames.Shield;
                    }
                }
            }

            if (damageResult.DamageType == DamageTypes.Overwhelm)
            {
                return UIFloatyMoveNames.Overwhelm;
            }

            switch (damageResult.DamageType)
            {
                case DamageTypes.Heal:
                case DamageTypes.HealOverTime:
                    {
                        return UIFloatyMoveNames.HealLife;
                    }

                case DamageTypes.Physical:
                    {
                        if (damageResult.DamageValue == float.MaxValue)
                        {
                            return UIFloatyMoveNames.Execution;
                        }
                        else if (damageResult.IsCritical)
                        {
                            return UIFloatyMoveNames.PhysicalCritical;
                        }
                        else
                        {
                            return UIFloatyMoveNames.Physical;
                        }
                    }

                case DamageTypes.Thorns:
                    {
                        return UIFloatyMoveNames.Thorns;
                    }

                case DamageTypes.DamageOverTime:
                    {
                        if (damageResult.DamageValue == float.MaxValue)
                        {
                            return UIFloatyMoveNames.Execution;
                        }
                        else if (damageResult.IsCritical)
                        {
                            return UIFloatyMoveNames.Bleeding;
                        }
                        else
                        {
                            return UIFloatyMoveNames.Bleeding;
                        }
                    }

                case DamageTypes.Execution:
                    {
                        return UIFloatyMoveNames.Execution;
                    }

                default:
                    {
                        return UIFloatyMoveNames.Content;
                    }
            }
        }

        //

        protected override void OnDisabled()
        {
            base.OnDisabled();

            StopFadeTweeners();
            StopMoveTweener();
            StopPunchScale();
        }

        public void OnSpawn()
        {
        }

        public void OnDespawn()
        {
            CallDespawnEvent();
            ResetText();
        }

        public void Despawn()
        {
            Despawner?.Despawn();
        }

        public void RegisterDespawnEvent(UnityAction<UIFloatyText> onDespawn)
        {
            DespawnCallback = onDespawn;
        }

        private void CallDespawnEvent()
        {
            if (DespawnCallback != null)
            {
                DespawnCallback.Invoke(this);
                DespawnCallback = null;
            }
        }

        //

        public void Setup(string content, UIFloatyMoveNames moveType)
        {
            Asset = ScriptableDataManager.Instance.FindFloaty(moveType);
            if (!Asset.IsValid())
            {
                Log.Warning("Float 에셋을 찾을 수 없습니다: {0}", moveType);
                return;
            }

            SetText(content);
            SetImage(moveType);

            ApplyFontColor();
            ApplyFont();

            ApplySpawnPosition();

            FadeOut();
            StartPunchScale();
        }

        #region Text

        private void ResetText()
        {
            if (Text != null)
            {
                Text.ResetText();
            }
        }

        private void SetText(string content)
        {
            if (Text != null)
            {
                Text.SetText(content);
            }
        }

        private void SetImage(UIFloatyMoveNames moveType)
        {
            if (TextImage != null)
            {
                switch (moveType)
                {
                    case UIFloatyMoveNames.PhysicalCritical:
                        {
                            TextImage.SetSprite(PhysicalCritialSprite, true);
                            TextImage.gameObject.SetActive(true);
                        }
                        break;

                    case UIFloatyMoveNames.MagicalCritical:
                        {
                            TextImage.SetSprite(MagicalCriticalSprite, true);
                            TextImage.gameObject.SetActive(true);
                        }
                        break;

                    case UIFloatyMoveNames.Overwhelm:
                        {
                            TextImage.SetSprite(OverwhelmSprite, true);
                            TextImage.gameObject.SetActive(true);
                        }
                        break;

                    case UIFloatyMoveNames.Gold:
                        {
                            TextImage.SetSprite(GoldSprite, true);
                            TextImage.gameObject.SetActive(true);
                        }
                        break;

                    case UIFloatyMoveNames.Gem:
                        {
                            TextImage.SetSprite(MagicStoneSprite, true);
                            TextImage.gameObject.SetActive(true);
                        }
                        break;

                    case UIFloatyMoveNames.RubyKey:
                        {
                            TextImage.SetSprite(RubyKeySprite, true);
                            TextImage.gameObject.SetActive(true);
                        }
                        break;

                    default:
                        {
                            TextImage.gameObject.SetActive(false);
                        }
                        break;
                }
            }
        }

        private void ApplyFontColor()
        {
            if (Text != null && Asset != null)
            {
                Text.SetTextColor(Asset.TextColor);
            }
        }

        private void ApplyFont()
        {
            if (Text != null && Asset != null)
            {
                Text.FontType = Asset.FontType;
                Text.FontTypeString = Asset.FontTypeString;
                Text.CustomFontSize = Asset.FontSize;

                Text.Refresh(GameSetting.Instance.Language.Name);
            }
        }

        #endregion Text

        #region Position

        private void ApplySpawnPosition()
        {
            if (FollowObject != null && Asset != null)
            {
                if (!Asset.SpawnArea.IsZero())
                {
                    Vector3 offset;

                    offset = RandomEx.GetVector3Value(Asset.SpawnArea);
                    offset += Asset.SpawnOffset;

                    FollowObject.SetWorldOffset(offset);
                }
            }
        }

        #endregion Position

        #region Move

        private bool _isFacingRight;

        public void SetFacingRight(Vector3 damageDirection)
        {
            if (Asset.RandomFace)
            {
                _isFacingRight = RandomEx.GetBoolValue();
            }
            else if (damageDirection.x > 0)
            {
                _isFacingRight = true;
            }
            else if (damageDirection.x < 0)
            {
                _isFacingRight = false;
            }
            else
            {
                // default
                _isFacingRight = true;
            }
        }

        public void SetFacingRight(bool isFacingRight)
        {
            if (Asset.RandomFace)
            {
                _isFacingRight = RandomEx.GetBoolValue();
            }
            else
            {
                _isFacingRight = isFacingRight;
            }
        }

        public void StartMove()
        {
            if (_moveTweener == null && _sequence == null)
            {
                ResetTextPosition();

                if (Asset == null)
                {
                    Log.Warning("UIFloatyText의 Asset을 찾을 수 없습니다.");
                    return;
                }
                else if (Asset.Type == UIFloatyMoveTypes.Velocity)
                {
                    StartVelocityMove(GetVelocity(), Asset.Duration);
                }
                else if (Asset.Type == UIFloatyMoveTypes.Jump)
                {
                    StartJumpMove();
                }
            }
        }

        private Vector2 GetVelocity()
        {
            if (_isFacingRight)
            {
                return new Vector2(Mathf.Abs(Asset.Velocity.x), Asset.Velocity.y);
            }
            else
            {
                return new Vector2(-Mathf.Abs(Asset.Velocity.x), Asset.Velocity.y);
            }
        }

        private void StartVelocityMove(Vector2 velocity, float duration)
        {
            if (velocity.IsZero())
            {
                return;
            }

            _moveTweener = Text.transform.DOLocalMove(velocity, duration);
            if (_moveTweener != null)
            {
                _moveTweener.onComplete += OnMoved;
                if (Asset.DelayTime > 0)
                {
                    _moveTweener.SetDelay(Asset.DelayTime);
                }
            }
        }

        private void StartJumpMove()
        {
            if (_isFacingRight)
            {
                _sequence = Text.transform.DOLocalJump(Asset.JumpEndValue, Asset.JumpPower, Asset.NumberOfJumps, Asset.Duration);
            }
            else
            {
                _sequence = Text.transform.DOLocalJump(Asset.JumpEndValue.FlipX(), Asset.JumpPower, Asset.NumberOfJumps, Asset.Duration);
            }

            if (_sequence != null)
            {
                _sequence.onComplete += OnMoved;
                if (Asset.DelayTime > 0)
                {
                    _sequence.SetDelay(Asset.DelayTime);
                }
            }
        }

        private void StopMoveTweener()
        {
            if (_moveTweener != null)
            {
                _moveTweener.Kill();
                _moveTweener = null;
            }
        }

        private void ResetTextPosition()
        {
            if (Text != null)
            {
                Text.transform.localPosition = Vector3.zero;
            }
        }

        private void OnMoved()
        {
            _moveTweener = null;
            _sequence = null;

            ResetTextPosition();

            Despawn();
        }

        #endregion Move

        #region Fade

        private void FadeOut()
        {
            if (!Asset.UseFadeOut)
            {
                return;
            }

            if (Text != null)
            {
                _fadeTextTweener = Text.FadeOut(Asset.FadeTargetAlpha, Asset.Duration, Asset.DelayTime);
                _fadeTextTweener.onComplete = () => { _fadeTextTweener = null; };
            }

            if (TextImage != null)
            {
                _fadeTextImageTweener = TextImage.FadeOut(Asset.FadeTargetAlpha, Asset.Duration, Asset.DelayTime);
                _fadeTextImageTweener.onComplete = () => { _fadeTextImageTweener = null; };
            }
        }

        private void StopFadeTweeners()
        {
            if (_fadeTextTweener != null)
            {
                _fadeTextTweener.Kill();
                _fadeTextTweener = null;
            }

            if (_fadeTextImageTweener != null)
            {
                _fadeTextImageTweener.Kill();
                _fadeTextImageTweener = null;
            }
        }

        #endregion Fade

        #region Scale

        private void StartPunchScale()
        {
            if (Asset.UsePunchScale)
            {
                if (Asset.PunchScaleDuration > 0)
                {
                    _scaleTweener = transform.DOPunchScale(Asset.PunchScale, Asset.PunchScaleDuration, Asset.PunchScaleVibrato);
                    _scaleTweener.onComplete += OnCompletedPunchScale;
                }
            }
        }

        private void OnCompletedPunchScale()
        {
            _scaleTweener = null;
        }

        private void StopPunchScale()
        {
            if (_scaleTweener != null)
            {
                _scaleTweener.Kill();
                _scaleTweener = null;
            }
        }

        #endregion Scale
    }
}