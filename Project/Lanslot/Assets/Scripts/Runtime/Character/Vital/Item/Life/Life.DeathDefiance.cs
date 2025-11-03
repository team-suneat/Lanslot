using System.Collections;
using TeamSuneat.Setting;
using TeamSuneat.UserInterface;
using UnityEngine;

namespace TeamSuneat
{
    /// <summary> 캐릭터의 생명력을 관리하는 클래스입니다. </summary>
    public partial class Life : VitalResource
    {
        private const float PLAYER_DEATH_ANIMATION_DURATION = 2f;
        private const float PLAYER_REVIVE_ANIMATION_DURATION = 2f;

        protected bool TryDeathDefiance(DamageResult damageResult)
        {
            if (Current > 0 || Vital.Owner == null || !Vital.Owner.IsPlayer)
            {
                return false;
            }

            StatModifier modifier = GetDeathDefianceHealRateModifier();
            if (modifier != null && CanTriggerDeathDefiance(modifier.Value))
            {
                Current = 0;
                RefreshGauge();

                SpawnDamageFloatyText(damageResult, Type);
                PlayDamageSFX(damageResult);
                PlayDamageFeedbacks(damageResult.DamageValue);
                ApplyHitStop();
                PlayElementDamageFeedbacks(damageResult, damageResult.DamagePosition);
                PlayDamageOverTimeFeedbacks(damageResult, damageResult.DamagePosition);

                StartXCoroutine(ProgressDeathDefiance(damageResult, modifier));
                return true;
            }

            return false;
        }

        private bool CanTriggerDeathDefiance(float healRate)
        {
            if (ProfileInfo == null)
            {
                return false;
            }

            StatSystem statSystem = Vital.Owner.Stat;
            int remaining = statSystem.FindValueOrDefaultToInt(StatNames.DeathDefianceCount) - ProfileInfo.Stat.UseDeathDefianceCount;
            return remaining > 0 && !healRate.IsZero();
        }

        public IEnumerator ProgressDeathDefiance(DamageResult damageResult, StatModifier modifier)
        {
            SetTemporarilyInvulnerable(this);
            GameSetting.Instance.Input.BlockInput();
            Vital.Owner.ChangeConditionState(CharacterConditions.Dead);
            OnRevive?.Invoke();

            yield return new WaitForSeconds(PLAYER_DEATH_ANIMATION_DURATION);

            Vital.Owner.ChangeConditionState(CharacterConditions.Normal);
            Vital.Owner.CharacterAnimator.PlaySpawnAnimation();

            DoDeathDefiance(modifier);

            yield return new WaitForSeconds(PLAYER_REVIVE_ANIMATION_DURATION);

            ResetTemporarilyInvulnerable(this);
            GameSetting.Instance.Input.UnblockInput();
            GlobalEvent<DamageResult>.Send(GlobalEventType.PLAYER_CHARACTER_DEATH_DEFIANCE, damageResult);
        }

        private void DoDeathDefiance(StatModifier modifier)
        {
            if (ProfileInfo == null)
            {
                return;
            }

            int healValue = Mathf.RoundToInt(modifier.Value * Max);
            if (AddCurrentValue(healValue))
            {
                OnHeal(healValue, true);
            }

            ProfileInfo.Stat.AddDeathDefianceCount(1);
            ProfileInfo.Stat.RegisterDeathDefianceSource(modifier.SourceName);

            string ownerCharacterName = Vital.Owner.Name.GetLocalizedString();
            UIManager.Instance.SpawnSoliloquyIngame(SoliloquyTypes.DeathDefiance, ownerCharacterName);
        }

        private StatModifier GetDeathDefianceHealRateModifier()
        {
            StatSystem statSystem = Vital.Owner.Stat;
            System.Collections.Generic.List<StatModifier> modifiers = statSystem.GetStatModifiers(StatNames.DeathDefianceHealRate);
            if (modifiers.IsValid())
            {
                for (int i = 0; i < modifiers.Count; i++)
                {
                    StatModifier modifier = modifiers[i];
                    if (!ProfileInfo.Stat.ContainsDeathDefianceSource(modifier.SourceName))
                    {
                        return modifier;
                    }
                }
            }

            return null;
        }
    }
}