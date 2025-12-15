using Sirenix.OdinInspector;
using System.Collections.Generic;
using TeamSuneat.Audio;
using TeamSuneat.Data.Game;
using TeamSuneat.Feedbacks;
using TeamSuneat.Setting;
using TeamSuneat.UserInterface;
using UnityEngine;

namespace TeamSuneat
{
    /// <summary> 캐릭터의 생명력을 관리하는 클래스입니다. </summary>
    public partial class Life : VitalResource
    {
        #region Field

        [Title("#Life")]
        [FoldoutGroup("#Toggle")]
        [SuffixLabel("피해를 입은 후 피해를 입지 않는 시간")]
        public float InvincibilityDurationOnDamage;

        [FoldoutGroup("#Toggle")]
        [SuffixLabel("피해를 입지 않음")]
        public bool Invulnerable = false;

        [FoldoutGroup("#Toggle")]
        [SuffixLabel("방어할 수 있음")]
        public bool Defensible = false;

        [ReadOnly]
        [FoldoutGroup("#Toggle")]
        [SuffixLabel("잠시 피해를 입지 않음")]
        public List<Component> TemporarilyInvulnerable = new();

        [ReadOnly]
        [FoldoutGroup("#Toggle")]
        [SuffixLabel("피해 후 잠시 피해를 입지 않음")]
        public bool PostDamageInvulnerable = false;

        [FoldoutGroup("#Toggle")]
        [SuffixLabel("영구 피해 면역")]
        public bool ImmuneToDamage = false;

        [FoldoutGroup("#Toggle")]
        [Tooltip("이것이 사실이면 피해 값이 Intensity 매개변수로 GameFeedbacks에 전달되어 피해값이 증가함에 따라 더 강렬한 피드백을 트리거할 수 있습니다")]
        [SuffixLabel("피해 비례 피드백")]
        public bool FeedbackIsProportionalToDamage;

        [FoldoutGroup("#Toggle")]
        [Tooltip("이것이 사실이면 피해 시 Lit 타입의 매터리얼을 사용하여 히트이펙트를 보여지게합니다.")]
        [SuffixLabel("피해시 Lit 타입 매터리얼 히트이펙트")]
        public bool DrawFlashLitOnDamage;

        [FoldoutGroup("#Death")]
        [Tooltip("이것이 사실이 아니라면 객체는 죽은 후에도 그곳에 남아있을 것입니다")]
        [SuffixLabel("사망 후 개체 삭제")]
        public bool DestroyOnDeath = true;

        [FoldoutGroup("#Death")]
        [Tooltip("캐릭터가 파괴되거나 비활성화되기까지의 시간(초)")]
        [SuffixLabel("사망 후 개체 삭제 지연 시간")]
        public float DelayBeforeDestruction = 5f;

        [FoldoutGroup("#Death")]
        [Tooltip("이것이 사실이면 캐릭터가 죽을 때 충돌이 꺼집니다")]
        [SuffixLabel("사망 후 충돌 무시")]
        public bool CollisionsOffOnDeath = false;

        [FoldoutGroup("#Death")]
        [Tooltip("이것이 사실이라면 죽음에 중력이 꺼질 것입니다")]
        [SuffixLabel("사망 후 중력 무시")]
        public bool GravityOffOnDeath = false;

        [FoldoutGroup("#Death")]
        [Tooltip("false로 설정하면 캐릭터가 사망한 위치에서 부활하고, 그렇지 않으면 초기 위치(장면이 시작될 때)로 이동합니다")]
        [SuffixLabel("초기 위치 부활")]
        public bool RespawnAtInitialLocation = false;

        #region Feedbacks

        [FoldoutGroup("#Feedback")] public GameFeedbacks HealFeedbacks;

        [FoldoutGroup("#Feedback")] public GameFeedbacks DamageFeedbacks;
        [FoldoutGroup("#Feedback")] public GameFeedbacks DamagePhysicalFeedbacks;
        [FoldoutGroup("#Feedback")] public GameFeedbacks DamageMagicalFeedbacks;
        [FoldoutGroup("#Feedback")] public GameFeedbacks DamageFireFeedbacks;
        [FoldoutGroup("#Feedback")] public GameFeedbacks DamageColdFeedbacks;
        [FoldoutGroup("#Feedback")] public GameFeedbacks DamageLightningFeedbacks;
        [FoldoutGroup("#Feedback")] public GameFeedbacks DamagePoisonFeedbacks;
        [FoldoutGroup("#Feedback")] public GameFeedbacks DamageDarknessFeedbacks;
        [FoldoutGroup("#Feedback")] public GameFeedbacks DamageHolyFeedbacks;
        [FoldoutGroup("#Feedback")] public GameFeedbacks DamageBloodFeedbacks;
        [FoldoutGroup("#Feedback")] public GameFeedbacks DamageOverTimeFeedbacks;
        [FoldoutGroup("#Feedback")] public DamageTypes[] DamageOverTimeFeedbackParameters;
        [FoldoutGroup("#Feedback")] public GameFeedbacks DamageZeroFeedbacks;
        [FoldoutGroup("#Feedback")] public GameFeedbacks BlockDamageFeedbacks;

        [FoldoutGroup("#Feedback")] public GameFeedbacks DeathFeedbacks;
        [FoldoutGroup("#Feedback")] public GameFeedbacks KilledFeedbacks;
        [FoldoutGroup("#Feedback")] public GameFeedbacks SuicideFeedbacks;

        #endregion Feedbacks

        #endregion Field

        #region Parameter

        /// <summary>
        /// 생명력 타입을 반환합니다.
        /// </summary>
        public override VitalResourceTypes Type => VitalResourceTypes.Life;

        public VProfile ProfileInfo = GameApp.GetSelectedProfile();

        /// <summary>
        /// 캐릭터가 피해를 입었는지 여부를 나타냅니다.
        /// </summary>
        public bool IsDamaged { get; set; }

        #endregion Parameter

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            GetFeedbackComponents();
        }

        protected override void Awake()
        {
            base.Awake();

            if (Max == 0)
            {
                Current = 1;
                Max = 1;
            }
        }

        public override void Initialize()
        {
            LogInfo("생명력을 초기화합니다.");

            base.Initialize();

            RefreshGauge();

            IsDamaged = false;

            ClearTemporarilyInvulnerable();
        }

        public override void RefreshMaxValue(bool shouldAddExcessToCurrent = false)
        {
            if (Vital == null || Vital.Owner == null || Vital.Owner.Stat == null)
            {
                LogWarning("최대 생명력을 불러올 수 없습니다. 바이탈, 소유 캐릭터, 능력치 시스템 중 최소 하나가 없습니다.");
                return;
            }

            float statValue = Vital.Owner.Stat.FindValueOrDefault(StatNames.Health);
            int maxLifeByStat = Mathf.RoundToInt(statValue);
            if (maxLifeByStat > 0)
            {
                int previousMax = Max;
                Max = Mathf.RoundToInt(maxLifeByStat);

                LogInfo("캐릭터의 능력치에 따라 최대 생명력을 갱신합니다. {0}/{1}", Current, Max);

                if (shouldAddExcessToCurrent && Max > previousMax)
                {
                    Current += Max - previousMax;
                }
                if (Current > Max)
                {
                    Current = Max;

                    LogInfo("캐릭터의 남은 생명력이 최대 생명력보다 크다면, 최대 생명력으로 설정합니다. {0}/{1}", Current, Max);
                }
            }
        }

        public bool CheckInvulnerable()
        {
            return TemporarilyInvulnerable.Count > 0 || Invulnerable || ImmuneToDamage || PostDamageInvulnerable;
        }

        protected override void OnAddCurrentValue(int value)
        {
            base.OnAddCurrentValue(value);

            PlayHealFeedbacks(value);

            RefreshGauge();

            SendGlobalEventValueChanged();

            SendGlobalEventHeal();
        }

        public void Heal(int healValue, bool useFloatyText = true)
        {
            if (Current <= 0)
            {
                LogWarning("남은 생명력이 0에 도달했다면 회복할 수 없습니다.");
                return;
            }

            if (healValue <= 0)
            {
                LogWarning("생명력 회복량이 0과 같거나 적다면 회복할 수 없습니다.");
                return;
            }

            int calculatedHealValue = CalculateHealingValue(healValue);
            if (AddCurrentValue(calculatedHealValue))
            {
                OnHeal(calculatedHealValue, useFloatyText);
            }
        }

        private void OnHeal(int healValue, bool useFloatyText)
        {
            PlayHealFeedbacks(healValue);

            if (useFloatyText)
            {
                SpawnHealFloatyText(healValue);
            }

            RefreshGauge();

            SendGlobalEventValueChanged();

            SendGlobalEventHeal();
        }

        private int CalculateHealingValue(int baseValue)
        {
            return baseValue;
        }

        public override bool UseCurrentValue(int damageValue, DamageResult damageResult)
        {
            if (Vital != null && Vital.Owner != null && Vital.Owner.IsPlayer)
            {
                if (Current <= damageValue)
                {
                    if (GameSetting.Instance.Cheat.NotDead)
                    {
                        LogInfo("죽지 않는 치트를 사용한다면 생명력을 1 남깁니다.");
                        int value = Current - 1;
                        Current = 1;
                        OnUseCurrencyValue(value);
                        return false;
                    }
                    else if (damageResult != null && damageResult.DamageType.IsDamageOverTime())
                    {
                        LogInfo("지속 피해에는 죽지 않고 생명력을 1 남깁니다.");
                        int value = Current - 1;
                        Current = 1;
                        OnUseCurrencyValue(value);
                        return false;
                    }
                }
            }

            if (damageResult != null && Current == Max)
            {
                GlobalEvent.Send(GlobalEventType.PLAYER_CHARACTER_ATTACK_MONSTER_FULL_LIFE_TARGET);
            }

            return base.UseCurrentValue(damageValue, damageResult);
        }

        public void UseZero()
        {
            OnDamageZero?.Invoke();
            DamageZeroFeedbacks?.PlayFeedbacks();
        }

        public void OnBlockDamage(DamageResult damageResult)
        {
            OnDamageZero?.Invoke();
            DamageZeroFeedbacks?.PlayFeedbacks();

            if (damageResult.DamageType.IsInstantDamage())
            {
                BlockDamageFeedbacks?.PlayFeedbacks();
            }
        }

        public void Use(int damageValue, Character attacker, bool ignoreDeath)
        {
            if (!UseCurrentValue(damageValue))
            {
                return;
            }

            if (Current < 1 && ignoreDeath)
            {
                LogInfo("해당 피해가 캐릭터를 사망시킬 수 없는 피해로 설정되었다면 생명력을 1 남깁니다.");

                Current = 1;
            }

            if (Current < 1)
            {
                if (Vital.Owner == attacker)
                {
                    Suicide();
                }
                else
                {
                    Killed(attacker);
                }
            }
            else
            {
                SetTargetAttacker(attacker);

                EnablePostDamageInvulnerability(InvincibilityDurationOnDamage);
            }

            SpawnUseFloatyText(damageValue);

            RefreshGauge();
            PlayDamageFeedbacks(damageValue);
            SendGlobalEventValueChanged();
        }

        public void TakeDamage(DamageResult damageResult, Character attacker)
        {
            if (!UseCurrentValue(damageResult.DamageValueToInt, damageResult))
            {
                return;
            }

            IsDamaged = true;
            if (attacker != null && attacker.IsPlayer)
            {
                GameSetting.Instance.Statistics.AddDamage(damageResult, Time.time);
            }

            OnDamage?.Invoke(damageResult);

            SpawnDamageVFX(damageResult, damageResult.DamagePosition);
            SpawnElementalDamageVFX(damageResult, damageResult.DamagePosition);
            if (damageResult.DamageType.IsInstantDamage())
            {
                SpawnDamageBloodVFX(damageResult.DamagePosition);
            }
            OnDamageFlicker(damageResult);

            if (TryDeathDefiance(damageResult))
            {
                // 죽음을 저항합니다.
                return;
            }

            if (Current <= 0)
            {
                Current = 0;
                SendKillGlobalEvent(damageResult);

                if (Vital.Owner == attacker)
                {
                    Suicide();
                }
                else
                {
                    Killed(damageResult);
                }

                RestoreOnKill(attacker);
            }
            else
            {
                SetTargetAttacker(attacker);

                if (Vital.Owner != null)
                {
                    _ = TryPlayDamageAnimation(damageResult);
                }

                EnablePostDamageInvulnerability(InvincibilityDurationOnDamage);
                if (Vital.EnemyGauge == null)
                {
                    Vital.SpawnEnemyGauge();
                }
            }

            RefreshGauge();

            if (damageResult.DamageType.IsInstantDamage())
            {
                PlayDamageFeedbacks(damageResult.DamageValue);
            }

            SendGlobalEventValueChanged();

            ApplyHitStop();

            SpawnDamageFloatyText(damageResult, Type);
            PlayDamageSFX(damageResult);
            PlayElementDamageFeedbacks(damageResult, damageResult.DamagePosition);
            PlayDamageOverTimeFeedbacks(damageResult, damageResult.DamagePosition);
        }

        private void SetTargetAttacker(Character attacker)
        {
            if (Vital.Owner == null)
            {
                return;
            }

            if (Vital.Owner.FixedTargetCharacterCamp)
            {
                return;
            }

            Vital.Owner.SetTarget(attacker);
        }

        private bool TryPlayDamageAnimation(DamageResult damageResult)
        {
            if (Vital.Owner?.CharacterAnimator == null)
            {
                return false;
            }

            if (damageResult.Asset.NotPlayDamageAnimation)
            {
                return false;
            }

            if (damageResult.DamageType.IsDamageOverTime())
            {
                return false;
            }

            if (damageResult.DamageType == DamageTypes.Grab)
            {
                return false;
            }

            Vital.Owner.CharacterAnimator.SetDamageTriggerIndex(damageResult.TargetVitalColliderIndex);
            Vital.Owner.CharacterAnimator.SetDamageTypeParameter(damageResult.Asset.IsPowerfulAttack);

            return Vital.Owner.CharacterAnimator.PlayDamageAnimation(damageResult.Asset);
        }

        private void OnDamageFlicker(DamageResult damageResult)
        {
            if (Vital.Owner?.CharacterRenderer == null)
            {
                return;
            }

            RendererFlickerNames flickerName = damageResult.DamageType.ConvertToFlicker();
            Vital.Owner.CharacterRenderer.StartFlickerCoroutine(flickerName);
        }

        #region Floaty Text

        public void SpawnHealFloatyText(int healValue)
        {
            _ = SpawnFloatyText(healValue.ToString(), DamageTextPoint, UIFloatyMoveNames.HealLife);
        }

        private void SpawnUseFloatyText(int useValue)
        {
            _ = SpawnFloatyText(useValue.ToString(), DamageTextPoint, UIFloatyMoveNames.Physical);
        }

        private void SpawnDamageFloatyText(DamageResult damageResult, VitalResourceTypes vitalResourceType)
        {
            if (damageResult.TargetCharacter == null)
            {
                return;
            }

            string content = string.Empty;
            UIFloatyMoveNames moveType = UIFloatyMoveNames.None;

            if (damageResult.DamageValue > 0)
            {
                moveType = UIFloatyText.ConvertToName(damageResult, vitalResourceType);
                content = damageResult.DamageValueToInt.ToString();
            }

            SpawnFloatyText(content, DamageTextPoint, moveType);
        }

        #endregion Floaty Text

        private void PlayDamageSFX(DamageResult damageResult)
        {
            switch (damageResult.DamageType)
            {
                case DamageTypes.Physical:
                    _ = AudioManager.Instance.PlaySFXOneShotScaled(SoundNames.Damage_Physical, position);
                    break;

                case DamageTypes.DamageOverTime:
                    _ = AudioManager.Instance.PlaySFXOneShotScaled(SoundNames.Damage_Blood, position);
                    break;
            }
        }

        public void Killed(Character attacker)
        {
            Vital.Life.ResetTemporarilyInvulnerable(this);

            PlayDeathFeedback();
            PlayKilledFeedbacks();

            if (attacker != null)
            {
                LogInfo("캐릭터가 {0}에게 죽습니다.", attacker.Name.ToLogString());
            }
            OnKilled?.Invoke(attacker);

            if (Vital.Owner != null)
            {
                OnDeath?.Invoke(new DamageResult()
                {
                    Attacker = attacker,
                    TargetVital = Vital,
                });
            }

            PlayDeathAnimation();
            MarkDespawnToGauge();

            if (TryDespawn())
            {
                Despawn();
            }
        }

        public void Killed(DamageResult damageResult)
        {
            Vital.Life.ResetTemporarilyInvulnerable(this);

            PlayDeathFeedback();
            PlayKilledFeedbacks();

            if (damageResult != null)
            {
                if (damageResult.Attacker != null)
                {
                    LogInfo("캐릭터가 {0}에게 죽습니다.", damageResult.Attacker.Name.ToLogString());
                }
                OnKilled?.Invoke(damageResult.Attacker);
            }

            if (Vital.Owner != null)
            {
                OnDeath?.Invoke(damageResult);
            }

            PlayDeathAnimation();
            MarkDespawnToGauge();

            if (TryDespawn())
            {
                Despawn();
            }
        }

        public void Suicide()
        {
            LogInfo("캐릭터가 스스로 죽음에 이릅니다.");
            Current = 0;
            Vital.Life.ResetTemporarilyInvulnerable(this);

            PlayDeathFeedback();
            PlaySuicideFeedbacks();

            if (Vital.Owner != null)
            {
                OnDeath?.Invoke(new DamageResult()
                {
                    Attacker = Vital.Owner,
                    TargetVital = Vital
                });
            }

            PlayDeathAnimation();
            MarkDespawnToGauge();

            if (TryDespawn())
            {
                Despawn();
            }
        }

        private bool TryDespawn()
        {
            if (Vital.Owner != null)
            {
                if (Vital.Owner.IsPlayer && DestroyOnDeath)
                {
                    return true;
                }
            }

            return false;
        }

        private void PlayDeathAnimation()
        {
            if (Vital != null && Vital.Owner != null && Vital.Owner.CharacterAnimator != null)
            {
                Vital.Owner.CharacterAnimator.PlayDeathAnimation();
            }
        }

        private void MarkDespawnToGauge()
        {
            Vital?.EnemyGauge?.SetDespawnMark();
        }

        private void SendKillGlobalEvent(DamageResult damageResult)
        {
            if (Vital.Owner == null || !Vital.Owner.IsPlayer)
            {
                return;
            }

            if (damageResult == null || damageResult.Attacker == null)
            {
                return;
            }

            if (damageResult.Attacker.IsPlayer)
            {
                _ = GlobalEvent<DamageResult>.Send(GlobalEventType.PLAYER_CHARACTER_KILL_MONSTER, damageResult);
            }
        }

        private void Despawn()
        {
            if (Vital != null)
            {
                _ = CoroutineNextTimer(DelayBeforeDestruction, Vital.Despawn);
            }
        }

        private void SendGlobalEventValueChanged()
        {
            if (Vital.Owner == null)
            {
                return;
            }

            if (Vital.Owner.IsPlayer)
            {
                if (Rate >= 0.5f)
                {
                    GlobalEvent.Send(GlobalEventType.PLAYER_CHARACTER_MORE_HALF_LIFE);
                }
                else
                {
                    GlobalEvent.Send(GlobalEventType.PLAYER_CHARACTER_LESS_HALF_LIFE);
                }
            }
        }

        public void SendGlobalEventHeal()
        {
            if (Vital.Owner == null)
            {
                return;
            }

            if (Vital.Owner.IsPlayer)
            {
                _ = GlobalEvent<int, int>.Send(GlobalEventType.PLAYER_CHARACTER_HEAL, Current, Max);
            }
            else
            {
                _ = GlobalEvent<int, int>.Send(GlobalEventType.MONSTER_CHARACTER_HEAL, Current, Max);
            }
        }

        private void RestoreOnKill(Character attacker)
        {
            if (attacker != null && attacker.MyVital != null)
            {
            }
        }

        #region Feedback

        private void GetFeedbackComponents()
        {
            Transform parentTransform = GetParentTransform();
            if (parentTransform == null)
            {
                return;
            }

            Transform feedbackParent = GetFeedbackParentTransform(parentTransform);
            if (feedbackParent == null)
            {
                return;
            }

            HealFeedbacks = feedbackParent.FindComponent<GameFeedbacks>("Heal");
            DamageFeedbacks = feedbackParent.FindComponent<GameFeedbacks>("Damage");
            DamagePhysicalFeedbacks = feedbackParent.FindComponent<GameFeedbacks>("Damage(Physical)");
            DamageMagicalFeedbacks = feedbackParent.FindComponent<GameFeedbacks>("Damage(Magical)");
            DamageFireFeedbacks = feedbackParent.FindComponent<GameFeedbacks>("Damage(Fire)");
            DamageColdFeedbacks = feedbackParent.FindComponent<GameFeedbacks>("Damage(Cold)");
            DamageLightningFeedbacks = feedbackParent.FindComponent<GameFeedbacks>("Damage(Lightning)");
            DamagePoisonFeedbacks = feedbackParent.FindComponent<GameFeedbacks>("Damage(Poison)");
            DamageDarknessFeedbacks = feedbackParent.FindComponent<GameFeedbacks>("Damage(Darkness)");
            DamageHolyFeedbacks = feedbackParent.FindComponent<GameFeedbacks>("Damage(Holy)");
            DamageBloodFeedbacks = feedbackParent.FindComponent<GameFeedbacks>("Damage(Blood)");
            DamageZeroFeedbacks = feedbackParent.FindComponent<GameFeedbacks>("Damage(Zero)");
            BlockDamageFeedbacks = feedbackParent.FindComponent<GameFeedbacks>("BlockDamage");
            DeathFeedbacks = feedbackParent.FindComponent<GameFeedbacks>("Death");
            KilledFeedbacks = feedbackParent.FindComponent<GameFeedbacks>("Killed");
            SuicideFeedbacks = feedbackParent.FindComponent<GameFeedbacks>("Suicide");
        }

        private Transform GetParentTransform()
        {
            return Vital.Owner != null ? Vital.Owner.transform : null;
        }

        private Transform GetFeedbackParentTransform(Transform parentTransform)
        {
            Transform feedbackParent = parentTransform.FindTransform("#Feedbacks");
            if (feedbackParent == null)
            {
                feedbackParent = parentTransform.FindTransform("Model/#Feedbacks");
            }

            return feedbackParent;
        }

        private void PlayHealFeedbacks(float damageValue)
        {
            if (FeedbackIsProportionalToDamage)
            {
                HealFeedbacks?.PlayFeedbacks(position, 0, damageValue);
            }
            else
            {
                HealFeedbacks?.PlayFeedbacks(position, 0);
            }
        }

        private void PlayDamageFeedbacks(float damageValue)
        {
            if (FeedbackIsProportionalToDamage)
            {
                DamageFeedbacks?.PlayFeedbacks(position, 0, damageValue);
            }
            else
            {
                DamageFeedbacks?.PlayFeedbacks(position, 0);
            }
        }

        private void PlayDeathFeedback()
        {
            DeathFeedbacks?.PlayFeedbacks();
        }

        private void PlayKilledFeedbacks()
        {
            KilledFeedbacks?.PlayFeedbacks();
        }

        private void PlaySuicideFeedbacks()
        {
            SuicideFeedbacks?.PlayFeedbacks();
        }

        private void PlayElementDamageFeedbacks(DamageResult damageResult, Vector3 damagePosition)
        {
            switch (damageResult.DamageType)
            {
                case DamageTypes.Physical:
                    DamagePhysicalFeedbacks?.PlayFeedbacks(damagePosition, 0);
                    break;

                case DamageTypes.Thorns:
                    DamagePhysicalFeedbacks?.PlayFeedbacks(damagePosition, 0);
                    break;

                case DamageTypes.DamageOverTime:
                    DamageBloodFeedbacks?.PlayFeedbacks(damagePosition, 0);
                    break;
            }
        }

        private void PlayDamageOverTimeFeedbacks(DamageResult damageResult, Vector3 damagePosition)
        {
            if (damageResult.DamageType.IsDamageOverTime())
            {
                int parameter = GetDamageOverTimeFeedbackParameter(damageResult.DamageType);
                if (FeedbackIsProportionalToDamage)
                {
                    DamageOverTimeFeedbacks?.PlayFeedbacks(position, parameter, damageResult.DamageValue);
                }
                else
                {
                    DamageOverTimeFeedbacks?.PlayFeedbacks(position, parameter);
                }
            }
        }

        private int GetDamageOverTimeFeedbackParameter(DamageTypes damageType)
        {
            for (int i = 0; i < DamageOverTimeFeedbackParameters.Length; i++)
            {
                if (DamageOverTimeFeedbackParameters[i] == damageType)
                {
                    return i;
                }
            }

            return 999;
        }

        #endregion Feedback

        #region Gauge

        protected void RefreshGauge()
        {
            Vital.RefreshLifeGauge();

            SendGlobalEventOfRefresh();
        }

        private void SendGlobalEventOfRefresh()
        {
            if (Vital.Owner == null)
            {
                return;
            }

            if (Vital.Owner.IsPlayer)
            {
                _ = GlobalEvent<int, int>.Send(GlobalEventType.PLAYER_CHARACTER_REFRESH_LIFE, Current, Max);
            }
            else
            {
                _ = GlobalEvent<int, int>.Send(GlobalEventType.MONSTER_CHARACTER_REFRESH_LIFE, Current, Max);
            }
        }

        #endregion Gauge

        #region Effect

        private void SpawnElementalDamageVFX(DamageResult damageResult, Vector3 damagePosition)
        {
            switch (damageResult.DamageType)
            {
                case DamageTypes.DamageOverTime:
                    _ = VFXManager.Spawn("fx_damage_blood", damagePosition, true);
                    break;
            }
        }

        private void SpawnDamageBloodVFX(Vector3 damagePosition)
        {
            if (Vital == null || Vital.Owner == null)
            {
                return;
            }

            if (Vital.Owner.IsPlayer)
            {
                _ = VFXManager.Spawn("fx_player_damage_blood", damagePosition, true);
            }
        }

        private void SpawnDamageVFX(DamageResult damageResult, Vector3 damagePosition)
        {
            if (!damageResult.DamageType.IsInstantDamage())
            {
                return;
            }

            if (Vital == null)
            {
                Log.Error("피격 이펙트를 생성할 수 없습니다. 바이탈 또는 캐릭터가 설정되지 않았습니다. {0}", this.GetHierarchyPath());
                return;
            }

            switch (damageResult.InstantVFXDamageType)
            {
                case VFXInstantDamageType.Sharp:
                    if (Vital.Owner != null && Vital.Owner.IsPlayer)
                    {
                        _ = VFXManager.Spawn("fx_player_damage_sharp", damagePosition, true);
                    }
                    else
                    {
                        _ = VFXManager.Spawn("fx_monster_damage_sharp", damagePosition, true);
                    }
                    break;

                case VFXInstantDamageType.Blunt:
                    if (Vital.Owner != null && Vital.Owner.IsPlayer)
                    {
                        _ = VFXManager.Spawn("fx_player_damage_blunt", damagePosition, true);
                    }
                    else
                    {
                        _ = VFXManager.Spawn("fx_monster_damage_blunt", damagePosition, true);
                    }
                    break;
            }
        }

        #endregion Effect
    }
}