using TeamSuneat.Data;

namespace TeamSuneat
{
    public partial class DamageCalculator
    {
        #region 능력치 검색 (Stat Find)

        /// <summary>
        /// 공격자가 해당 능력치를 가지고 있는지 확인합니다.
        /// </summary>
        /// <param name="statName">확인할 능력치 이름</param>
        /// <returns>능력치를 가지고 있으면 true</returns>
        private bool ContainsAttackerStatValue(StatNames statName)
        {
            if (Attacker != null)
            {
                if (Attacker.Stat.ContainsKey(statName))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 공격자의 능력치 값을 검색합니다.
        /// </summary>
        /// <param name="statName">검색할 능력치 이름</param>
        /// <returns>능력치 값 (없으면 기본값)</returns>
        private float FindAttackerStatValue(StatNames statName)
        {
            if (Attacker != null)
            {
                return Attacker.Stat.FindValueOrDefault(statName);
            }
            else
            {
                StatData statData = JsonDataManager.FindStatDataClone(statName);
                if (statData.IsValid())
                {
                    return statData.DefaultValue;
                }
            }

            return 0f;
        }

        /// <summary>
        /// 공격자의 계산된 능력치 값을 검색합니다.
        /// </summary>
        /// <param name="statName">검색할 능력치 이름</param>
        /// <returns>계산된 능력치 값 (없으면 기본값)</returns>
        private float FindAttackerCalculateStatValue(StatNames statName)
        {
            if (Attacker != null)
            {
                return Attacker.Stat.FindValueOrDefault(statName);
            }
            else
            {
                StatData statData = JsonDataManager.FindStatDataClone(statName);
                if (statData.IsValid())
                {
                    return statData.DefaultValue;
                }
            }

            return 0f;
        }

        /// <summary>
        /// 피격자의 능력치 값을 검색합니다.
        /// </summary>
        /// <param name="statName">검색할 능력치 이름</param>
        /// <returns>능력치 값 (없으면 기본값)</returns>
        private float FindTargetStatValue(StatNames statName)
        {
            if (TargetCharacter != null)
            {
                return TargetCharacter.Stat.FindValueOrDefault(statName);
            }
            else
            {
                StatData statData = JsonDataManager.FindStatDataClone(statName);
                if (statData.IsValid())
                {
                    return statData.DefaultValue;
                }
            }

            return 0f;
        }

        /// <summary>
        /// 피격자의 계산된 능력치 값을 검색합니다.
        /// </summary>
        /// <param name="statName">검색할 능력치 이름</param>
        /// <returns>계산된 능력치 값 (없으면 기본값)</returns>
        private float FindTargetCalculateStatValue(StatNames statName)
        {
            if (TargetCharacter != null)
            {
                return TargetCharacter.Stat.FindValueOrDefault(statName);
            }
            else
            {
                StatData statData = JsonDataManager.FindStatDataClone(statName);
                if (statData.IsValid())
                {
                    return statData.DefaultValue;
                }
            }

            return 0f;
        }

        #endregion 능력치 검색 (Stat Find)

        #region 상태이상 검색 (State Effect Find)

        /// <summary>
        /// 피격자가 해당 상태이상을 가지고 있는지 확인합니다.
        /// </summary>
        /// <param name="stateEffect">확인할 상태이상</param>
        /// <returns>상태이상을 가지고 있으면 true</returns>
        private bool ContainsTargetCharacterStateEffect(StateEffects stateEffect)
        {
            return TargetCharacter != null && TargetCharacter.Buff.ContainsStateEffect(stateEffect);
        }

        /// <summary>
        /// 피격자가 두 상태이상 중 하나라도 가지고 있는지 확인합니다.
        /// </summary>
        /// <param name="stateEffect1">확인할 상태이상 1</param>
        /// <param name="stateEffect2">확인할 상태이상 2</param>
        /// <returns>상태이상을 가지고 있으면 true</returns>
        private bool ContainsTargetCharacterStateEffects(StateEffects stateEffect1, StateEffects stateEffect2)
        {
            if (TargetCharacter != null)
            {
                if (TargetCharacter.Buff.ContainsStateEffect(stateEffect1))
                {
                    return true;
                }
                if (TargetCharacter.Buff.ContainsStateEffect(stateEffect2))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 공격자의 상태이상 스택 수를 검색합니다.
        /// </summary>
        /// <param name="stateEffect">검색할 상태이상</param>
        /// <returns>상태이상 스택 수</returns>
        private int FindAttackerStateEffectStack(StateEffects stateEffect)
        {
            if (Attacker != null)
            {
                int stack = Attacker.Buff.FindStack(stateEffect);
                return stack;
            }

            return 0;
        }

        /// <summary>
        /// 피격자의 상태이상 스택 수를 검색합니다.
        /// </summary>
        /// <param name="stateEffect">검색할 상태이상</param>
        /// <returns>상태이상 스택 수</returns>
        private int FindTargetStateEffectStack(StateEffects stateEffect)
        {
            if (TargetCharacter != null)
            {
                int stack = TargetCharacter.Buff.FindStack(stateEffect);
                return stack;
            }

            return 0;
        }

        #endregion 상태이상 검색 (State Effect Find)

        #region 능력치 헬퍼 (Stat Helper)

        /// <summary>
        /// 공격자의 능력치를 검색하고 0이 아닐 때만 결과에 추가하고 로그를 남깁니다.
        /// </summary>
        /// <param name="statName">검색할 능력치 이름</param>
        /// <param name="result">결과에 추가할 변수</param>
        /// <param name="logMethod">로그 메서드 (선택적)</param>
        /// <returns>능력치 값이 0이 아닌 경우 true</returns>
        private bool TryAddAttackerStatValue(StatNames statName, ref float result, System.Action<float> logMethod = null)
        {
            float statValue = FindAttackerStatValue(statName);
            if (!statValue.IsZero())
            {
                result += statValue;
                logMethod?.Invoke(statValue);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 공격자의 능력치를 검색하고 0이 아닐 때만 결과에 추가하고 데미지 증폭 로그를 남깁니다.
        /// </summary>
        private bool TryAddAttackerStatValueWithAmplificationLog(StatNames statName, ref float result)
        {
            float statValue = FindAttackerStatValue(statName);
            if (!statValue.IsZero())
            {
                result += statValue;
                LogDamageAmplificationByStat(statName, statValue);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 공격자의 능력치를 검색하고 0보다 클 때만 결과에 추가하고 로그를 남깁니다.
        /// </summary>
        /// <param name="statName">검색할 능력치 이름</param>
        /// <param name="result">결과에 추가할 변수</param>
        /// <param name="logMethod">로그 메서드 (선택적)</param>
        /// <returns>능력치 값이 0보다 큰 경우 true</returns>
        private bool TryAddAttackerStatValueIfPositive(StatNames statName, ref float result, System.Action<float> logMethod = null)
        {
            float statValue = FindAttackerStatValue(statName);
            if (statValue > 0)
            {
                result += statValue;
                logMethod?.Invoke(statValue);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 피격자의 능력치를 검색하고 0이 아닐 때만 결과에 추가하고 로그를 남깁니다.
        /// </summary>
        /// <param name="statName">검색할 능력치 이름</param>
        /// <param name="result">결과에 추가할 변수</param>
        /// <param name="logMethod">로그 메서드 (선택적)</param>
        /// <returns>능력치 값이 0이 아닌 경우 true</returns>
        private bool TryAddTargetStatValue(StatNames statName, ref float result, System.Action<float> logMethod = null)
        {
            float statValue = FindTargetStatValue(statName);
            if (!statValue.IsZero())
            {
                result += statValue;
                logMethod?.Invoke(statValue);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 공격자의 계산된 능력치를 검색하고 0이 아닐 때만 결과에 추가하고 로그를 남깁니다.
        /// </summary>
        /// <param name="statName">검색할 능력치 이름</param>
        /// <param name="result">결과에 추가할 변수</param>
        /// <param name="logMethod">로그 메서드 (선택적)</param>
        /// <returns>능력치 값이 0이 아닌 경우 true</returns>
        private bool TryAddAttackerCalculateStatValue(StatNames statName, ref float result, System.Action<float> logMethod = null)
        {
            float statValue = FindAttackerCalculateStatValue(statName);
            if (!statValue.IsZero())
            {
                result += statValue;
                logMethod?.Invoke(statValue);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 피격자의 계산된 능력치 값을 검색하고 결과에 추가합니다.
        /// </summary>
        /// <param name="statName">검색할 능력치 이름</param>
        /// <param name="result">결과에 추가할 변수</param>
        /// <param name="logMethod">로그 메서드 (선택적)</param>
        /// <returns>값이 0이 아닌 경우 true</returns>
        private bool TryAddTargetCalculateStatValue(StatNames statName, ref float result, System.Action<float> logMethod = null)
        {
            float statValue = FindTargetCalculateStatValue(statName);
            if (!statValue.IsZero())
            {
                result += statValue;
                logMethod?.Invoke(statValue);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 능력치 값을 검색하고 조건에 따라 계산된 값을 결과에 추가합니다.
        /// </summary>
        /// <param name="statName">검색할 능력치 이름</param>
        /// <param name="result">결과에 추가할 변수</param>
        /// <param name="calculation">계산 함수</param>
        /// <param name="logMethod">로그 메서드 (선택적)</param>
        /// <returns>계산된 값이 0이 아닌 경우 true</returns>
        private bool TryAddCalculatedStatValue(StatNames statName, ref float result, System.Func<float, float> calculation, System.Action<float> logMethod = null)
        {
            float statValue = FindAttackerStatValue(statName);
            float calculatedValue = calculation(statValue);
            if (!calculatedValue.IsZero())
            {
                result += calculatedValue;
                logMethod?.Invoke(calculatedValue);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 조건부로 능력치 값을 검색하고 결과에 추가합니다.
        /// </summary>
        /// <param name="condition">조건 함수</param>
        /// <param name="statName">검색할 능력치 이름</param>
        /// <param name="result">결과에 추가할 변수</param>
        /// <param name="logMethod">로그 메서드 (선택적)</param>
        /// <returns>조건을 만족하고 값이 추가된 경우 true</returns>
        private bool TryAddStatValueIfCondition(bool condition, StatNames statName, ref float result, System.Action<float> logMethod = null)
        {
            if (!condition)
            {
                return false;
            }

            return TryAddAttackerStatValue(statName, ref result, logMethod);
        }

        /// <summary>
        /// 상태이상 조건부로 능력치 값을 검색하고 결과에 추가합니다.
        /// </summary>
        /// <param name="stateEffect">확인할 상태이상</param>
        /// <param name="statName">검색할 능력치 이름</param>
        /// <param name="result">결과에 추가할 변수</param>
        /// <param name="logMethod">로그 메서드 (선택적)</param>
        /// <returns>상태이상이 있고 값이 추가된 경우 true</returns>
        private bool TryAddStatValueIfStateEffect(StateEffects stateEffect, StatNames statName, ref float result, System.Action<float> logMethod = null)
        {
            return TryAddStatValueIfCondition(ContainsTargetCharacterStateEffect(stateEffect), statName, ref result, logMethod);
        }

        /// <summary>
        /// 상태이상 조건에 따른 능력치를 검색하고 0이 아닐 때만 결과에 추가하고 상태이상 데미지 로그를 남깁니다.
        /// </summary>
        private bool TryAddStatValueIfStateEffectWithLog(StateEffects stateEffect, StatNames statName, ref float result)
        {
            if (ContainsTargetCharacterStateEffect(stateEffect))
            {
                float statValue = FindAttackerStatValue(statName);
                if (!statValue.IsZero())
                {
                    result += statValue;
                    LogDamageToStateEffectEnemies(stateEffect, statValue);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 여러 상태이상 중 하나라도 있으면 능력치 값을 검색하고 결과에 추가합니다.
        /// </summary>
        /// <param name="stateEffects">확인할 상태이상들</param>
        /// <param name="statName">검색할 능력치 이름</param>
        /// <param name="result">결과에 추가할 변수</param>
        /// <param name="logMethod">로그 메서드 (선택적)</param>
        /// <returns>상태이상이 있고 값이 추가된 경우 true</returns>
        private bool TryAddStatValueIfAnyStateEffect(StateEffects[] stateEffects, StatNames statName, ref float result, System.Action<float> logMethod = null)
        {
            bool hasAnyStateEffect = false;
            foreach (StateEffects stateEffect in stateEffects)
            {
                if (ContainsTargetCharacterStateEffect(stateEffect))
                {
                    hasAnyStateEffect = true;
                    break;
                }
            }
            return TryAddStatValueIfCondition(hasAnyStateEffect, statName, ref result, logMethod);
        }

        /// <summary>
        /// 공격자의 능력치를 검색하고 특정 값이 아닐 때만 결과에 추가하고 로그를 남깁니다.
        /// </summary>
        /// <param name="statName">검색할 능력치 이름</param>
        /// <param name="result">결과에 추가할 변수</param>
        /// <param name="notEqualValue">비교할 값</param>
        /// <param name="logMethod">로그 메서드 (선택적)</param>
        /// <returns>능력치 값이 비교값과 다른 경우 true</returns>
        private bool TryAddAttackerStatValueIfNotEqual(StatNames statName, ref float result, float notEqualValue, System.Action<float> logMethod = null)
        {
            float statValue = FindAttackerStatValue(statName);
            if (!statValue.Compare(notEqualValue))
            {
                result *= statValue;
                logMethod?.Invoke(statValue);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 피격자의 능력치를 검색하고 특정 값이 아닐 때만 결과에 추가하고 로그를 남깁니다.
        /// </summary>
        /// <param name="statName">검색할 능력치 이름</param>
        /// <param name="result">결과에 추가할 변수</param>
        /// <param name="notEqualValue">비교할 값</param>
        /// <param name="logMethod">로그 메서드 (선택적)</param>
        /// <returns>능력치 값이 비교값과 다른 경우 true</returns>
        private bool TryAddTargetStatValueIfNotEqual(StatNames statName, ref float result, float notEqualValue, System.Action<float> logMethod = null)
        {
            float statValue = FindTargetStatValue(statName);
            if (!statValue.Compare(notEqualValue))
            {
                result *= statValue;
                logMethod?.Invoke(statValue);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 피격자의 계산된 능력치를 검색하고 특정 값이 아닐 때만 결과에 추가하고 로그를 남깁니다.
        /// </summary>
        /// <param name="statName">검색할 능력치 이름</param>
        /// <param name="result">결과에 추가할 변수</param>
        /// <param name="notEqualValue">비교할 값</param>
        /// <param name="logMethod">로그 메서드 (선택적)</param>
        /// <returns>능력치 값이 비교값과 다른 경우 true</returns>
        private bool TryAddTargetCalculateStatValueIfNotEqual(StatNames statName, ref float result, float notEqualValue, System.Action<float> logMethod = null)
        {
            float statValue = FindTargetCalculateStatValue(statName);
            if (!statValue.Compare(notEqualValue))
            {
                result *= statValue;
                logMethod?.Invoke(statValue);
                return true;
            }
            return false;
        }

        #endregion 능력치 헬퍼 (Stat Helper)
    }
}