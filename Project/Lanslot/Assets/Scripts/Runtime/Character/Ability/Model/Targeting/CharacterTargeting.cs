using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat
{
    /// <summary>
    /// 플레이어 캐릭터의 자동 타겟팅 시스템
    /// 타겟팅 거리 내에서 가장 가까운 적을 자동으로 타겟으로 설정합니다.
    /// </summary>
    public class CharacterTargeting : CharacterAbility
    {
        [FoldoutGroup("#Targeting Settings")]
        [SerializeField] private float _targetingRange = 10f;
        
        [FoldoutGroup("#Targeting Settings")]
        [SerializeField] private bool _updateEveryFrame = false;
        
        [FoldoutGroup("#Targeting Settings")]
        [SerializeField] private float _updateInterval = 0.1f;

        [FoldoutGroup("#Debug")]
        [SerializeField][ReadOnly] private Character _currentTarget;
        [FoldoutGroup("#Debug")]
        [SerializeField][ReadOnly] private float _lastUpdateTime;

        public Character CurrentTarget => _currentTarget;

        public override Types Type => Types.Targeting;

        public override CharacterConditions[] BlockingConditionStates => new CharacterConditions[]
        {
            CharacterConditions.Dead,
            CharacterConditions.Frozen,
            CharacterConditions.Stunned
        };

        public override void Initialization()
        {
            base.Initialization();

            // 플레이어 캐릭터만 타겟팅 기능 사용
            if (!Owner.IsPlayer)
            {
                LogWarning("타겟팅 시스템은 플레이어 캐릭터에서만 사용할 수 있습니다.");
                enabled = false;
                return;
            }

            LogInfo("플레이어 타겟팅 시스템을 초기화합니다. 타겟팅 거리: {0}", _targetingRange);
        }

        public override void ProcessAbility()
        {
            if (!IsAuthorized)
            {
                return;
            }

            // 갱신 주기 체크
            if (!_updateEveryFrame && Time.time - _lastUpdateTime < _updateInterval)
            {
                return;
            }

            UpdateTargeting();
            _lastUpdateTime = Time.time;
        }

        /// <summary>
        /// 타겟팅 거리 내에서 가장 가까운 적을 찾아 타겟으로 설정합니다.
        /// </summary>
        private void UpdateTargeting()
        {
            Character nearestEnemy = FindNearestEnemyInRange();

            // 현재 타겟과 새로 찾은 타겟이 다른 경우에만 업데이트
            if (_currentTarget != nearestEnemy)
            {
                if (nearestEnemy != null)
                {
                    SetTarget(nearestEnemy);
                }
                else
                {
                    ClearTarget();
                }
            }
        }

        /// <summary>
        /// 타겟팅 거리 내에서 가장 가까운 적을 찾습니다.
        /// </summary>
        /// <returns>가장 가까운 적 캐릭터, 없으면 null</returns>
        private Character FindNearestEnemyInRange()
        {
            if (CharacterManager.Instance == null || !CharacterManager.Instance.Monsters.IsValid())
            {
                return null;
            }

            Character result = null;
            float closestDistance = float.MaxValue;
            Vector3 playerPosition = Owner.transform.position;

            for (int i = 0; i < CharacterManager.Instance.MonsterCount; i++)
            {
                Character monster = CharacterManager.Instance.Monsters[i];
                
                // 살아있는 적만 체크
                if (monster == null || !monster.IsAlive)
                {
                    continue;
                }

                // 거리 계산
                Vector3 monsterPosition = monster.MyVital.GetNearestColliderPosition(playerPosition);
                float distance = Vector3.Distance(playerPosition, monsterPosition);

                // 타겟팅 거리 내에 있고, 가장 가까운 적인지 체크
                if (distance <= _targetingRange && distance < closestDistance)
                {
                    closestDistance = distance;
                    result = monster;
                }
            }

            return result;
        }

        /// <summary>
        /// 타겟을 설정합니다.
        /// </summary>
        /// <param name="target">설정할 타겟 캐릭터</param>
        private void SetTarget(Character target)
        {
            if (target == null)
            {
                return;
            }

            _currentTarget = target;
            Owner.SetTarget(target);
            
            LogInfo("타겟을 설정했습니다: {0} (거리: {1:F1})", 
                target.GetHierarchyName(), 
                Vector3.Distance(Owner.transform.position, target.transform.position));
        }

        /// <summary>
        /// 현재 타겟을 해제합니다.
        /// </summary>
        private void ClearTarget()
        {
            if (_currentTarget != null)
            {
                LogInfo("타겟을 해제했습니다: {0}", _currentTarget.GetHierarchyName());
            }

            _currentTarget = null;
            Owner.ResetTarget();
        }

        /// <summary>
        /// 타겟팅 거리를 가져옵니다.
        /// 나중에 무기 시스템에서 오버라이드할 수 있도록 virtual로 구현합니다.
        /// </summary>
        /// <returns>현재 타겟팅 거리</returns>
        protected virtual float GetTargetingRange()
        {
            return _targetingRange;
        }

        /// <summary>
        /// 타겟팅 시스템을 수동으로 리셋합니다.
        /// </summary>
        public void ResetTargeting()
        {
            ClearTarget();
            _lastUpdateTime = 0f;
        }

        #region Debug

        /// <summary>
        /// 현재 타겟팅 거리를 시각적으로 표시합니다. (디버그용)
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            if (Owner == null)
            {
                return;
            }

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(Owner.transform.position, GetTargetingRange());

            if (_currentTarget != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(Owner.transform.position, _currentTarget.transform.position);
            }
        }

        #endregion Debug
    }
}
