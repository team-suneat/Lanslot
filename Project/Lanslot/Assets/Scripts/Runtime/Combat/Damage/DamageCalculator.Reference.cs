using TeamSuneat.Data;

namespace TeamSuneat
{
    public partial class DamageCalculator
    {
        public void RefreshReferenceValue(HitmarkAssetData damageAsset)
        {
            switch (damageAsset.LinkedDamageType)
            {
                case LinkedDamageTypes.None:
                    return;

                case LinkedDamageTypes.Attack:
                    {
                        ReferenceValue = DamageReferenceValue;
                        LogProgressReferenceValue("피해량", ReferenceValue);
                    }
                    break;

                case LinkedDamageTypes.CurrentLifeOfAttacker:
                    {
                        ReferenceValue = Attacker.MyVital.CurrentLife;
                        LogProgressReferenceValue("공격자의 현재 생명력", ReferenceValue);
                    }
                    break;

                case LinkedDamageTypes.MaxLifeOfAttacker:
                    {
                        ReferenceValue = Attacker.MyVital.MaxLife;
                        LogProgressReferenceValue("공격자의 최대 생명력", ReferenceValue);
                    }
                    break;

                case LinkedDamageTypes.CurrentShieldOfAttacker:
                    {
                        ReferenceValue = Attacker.MyVital.CurrentShield;
                        LogProgressReferenceValue("공격자의 현재 보호막", ReferenceValue);
                    }
                    break;

                case LinkedDamageTypes.MaxShieldOfAttacker:
                    {
                        ReferenceValue = Attacker.MyVital.MaxShield;
                        LogProgressReferenceValue("공격자의 최대 보호막", ReferenceValue);
                    }
                    break;

                case LinkedDamageTypes.CurrentLifeOfTarget:
                    {
                        ReferenceValue = TargetVital.CurrentLife;
                        LogProgressReferenceValue("피격자의 현재 생명력", ReferenceValue);
                    }
                    break;

                case LinkedDamageTypes.MaxLifeOfTarget:
                    {
                        ReferenceValue = TargetVital.MaxLife;
                        LogProgressReferenceValue("피격자의 최대 생명력", ReferenceValue);
                    }
                    break;

                case LinkedDamageTypes.SkillCooldownTime:
                    {
                        ReferenceValue = CooldownReferenceValue;
                        LogProgressReferenceValue("기술의 재사용 대기시간", ReferenceValue);
                    }
                    break;
            }

            if (ReferenceValue == 0)
            {
                LogErrorReferenceValue(damageAsset.LinkedDamageType, damageAsset.LinkedStateEffect, ReferenceValue);
            }
            else if (damageAsset.LinkedStateEffect != StateEffects.None)
            {
                int stack = Attacker.Buff.FindStack(damageAsset.LinkedStateEffect);
                if (stack > 0)
                {
                    ReferenceValue *= stack;
                    LogProgressReferenceValue("공격자의 상태이상 스택 적용", ReferenceValue);
                }
            }
        }

        //

        private void LogProgressReferenceValue(string description, float value)
        {
            if (Log.LevelProgress)
            {
                LogProgress("{0}을 참조값을 설정합니다. Value: {1}", description, value.ToSelectString(0));
            }
        }

        private void LogErrorReferenceValue(LinkedDamageTypes linkedDamageType, StateEffects linkedStateEffect, float value)
        {
            if (Log.LevelWarning)
            {
                LogWarning("참조값을 설정할 수 없습니다. 참조 피해 종류: {0}, 참조 상태 이상: {1}, 참조 값: {2}", linkedDamageType, linkedStateEffect, value.ToSelectString(0));
            }
        }
    }
}