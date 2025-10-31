using Sirenix.OdinInspector;
using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat
{
    /// <summary>
    /// 몬스터 캐릭터의 자동 공격을 담당하는 어빌리티입니다.
    /// 타겟이 공격 가능한 거리 내에 있으면 일정 쿨다운 후 자동으로 공격을 실행합니다.
    /// </summary>
    public class CharacterAutoAttack : CharacterAbility
    {
        public override Types Type => Types.AutoAttack;

        [FoldoutGroup("#Attack Settings")]
        [SuffixLabel("공격 쿨다운 시간")]
        [SerializeField] private float _attackCooldown = 1.0f;

        [FoldoutGroup("#Attack Settings")]
        [SuffixLabel("공격 가능한 거리")]
        [SerializeField] private float _attackRange = 2.0f;

        // 컴포넌트 캐시
        [FoldoutGroup("#Component")]
        [SerializeField] private AttackSystem _attackSystem;
        private AttackEntity _basicAttackEntity;
        private HitmarkAssetData _hitmarkAssetData;

        // 런타임 변수
        [FoldoutGroup("#For Dev", 4)][SerializeField][ReadOnly] private float _lastAttackTime;
        [FoldoutGroup("#For Dev", 4)][SerializeField][ReadOnly] private Character _currentTarget;
        [FoldoutGroup("#For Dev", 4)][SerializeField][ReadOnly] private float _distanceToTarget;
        [FoldoutGroup("#For Dev", 4)][SerializeField][ReadOnly] private bool _canAttack;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _attackSystem = GetComponentInChildren<AttackSystem>();
        }

        public override void Initialization()
        {
            base.Initialization();

            if (_attackSystem == null)
            {
                Log.Error("AttackSystem 컴포넌트를 찾을 수 없습니다.");
                return;
            }

            SetupAttackEntity();
            SetupHitmarkData();

            LogInfo("몬스터 자동 공격 시스템을 초기화합니다. 공격 범위: {0}, 쿨다운: {1}", _attackRange, _attackCooldown);
        }

        private void SetupAttackEntity()
        {
            HitmarkNames basicAttackHitmark = _attackSystem.BasicAttackHitmark;
            if (basicAttackHitmark != HitmarkNames.None)
            {
                _basicAttackEntity = _attackSystem.FindEntity(basicAttackHitmark);
                if (_basicAttackEntity == null)
                {
                    LogWarning("기본 공격용 AttackEntity를 찾을 수 없습니다. {0}", basicAttackHitmark.ToLogString());
                }
            }
        }

        private void SetupHitmarkData()
        {
            HitmarkNames basicAttackHitmark = _attackSystem.BasicAttackHitmark;
            if (basicAttackHitmark != HitmarkNames.None)
            {
                _hitmarkAssetData = ScriptableDataManager.Instance.FindHitmarkClone(basicAttackHitmark);
                if (_hitmarkAssetData.IsValid() && _hitmarkAssetData.AttackRange > 0)
                {
                    _attackRange = _hitmarkAssetData.AttackRange;
                    LogInfo("HitmarkAssetData에서 공격 범위를 설정했습니다. {0}", _attackRange);
                }
            }
        }

        public override void ProcessAbility()
        {
            if (!IsAuthorized) return;

            UpdateTarget();
            CheckAttackConditions();
        }

        /// <summary>
        /// 현재 타겟을 업데이트합니다.
        /// </summary>
        private void UpdateTarget()
        {
            _currentTarget = Owner.TargetCharacter;
        }

        /// <summary>
        /// 공격 조건을 확인하고 공격을 실행합니다.
        /// </summary>
        private void CheckAttackConditions()
        {
            // 타겟 유효성 검증
            if (!IsTargetValid(_currentTarget))
            {
                _canAttack = false;
                return;
            }

            // 타겟과의 거리 계산
            _distanceToTarget = Vector2.Distance(transform.position, _currentTarget.transform.position);

            // 공격 가능한 거리 내에 있는지 확인
            if (_distanceToTarget > _attackRange)
            {
                _canAttack = false;
                return;
            }

            // 쿨다운 체크
            if (Time.time - _lastAttackTime < _attackCooldown)
            {
                _canAttack = false;
                return;
            }

            // 모든 조건 만족 시 공격 실행
            _canAttack = true;
            TryExecuteAttack();
        }

        /// <summary>
        /// 타겟이 유효한지 확인합니다.
        /// </summary>
        private bool IsTargetValid(Character target)
        {
            if (target == null) return false;
            if (!target.IsAlive) return false;
            if (!target.gameObject.activeInHierarchy) return false;

            return true;
        }

        /// <summary>
        /// 공격을 실행합니다.
        /// </summary>
        private void TryExecuteAttack()
        {
            if (_basicAttackEntity == null)
            {
                LogWarning("기본 공격용 AttackEntity가 설정되지 않았습니다.");
                return;
            }

            _basicAttackEntity.Execute();
            _lastAttackTime = Time.time;

            LogInfo("자동 공격을 실행했습니다. 타겟: {0}, 거리: {1:F2}",
                _currentTarget.name, _distanceToTarget);
        }

        public override void ResetAbility()
        {
            base.ResetAbility();

            _lastAttackTime = 0f;
            _currentTarget = null;
            _distanceToTarget = 0f;
            _canAttack = false;

            LogInfo("자동 공격 능력을 초기화합니다.");
        }

        /// <summary>
        /// 현재 공격 가능한 상태인지 확인합니다.
        /// </summary>
        public bool CanAttack => _canAttack;

        /// <summary>
        /// 현재 타겟과의 거리를 가져옵니다.
        /// </summary>
        public float DistanceToTarget => _distanceToTarget;

        /// <summary>
        /// 공격 가능한 거리를 가져옵니다.
        /// </summary>
        public float AttackRange => _attackRange;
    }
}