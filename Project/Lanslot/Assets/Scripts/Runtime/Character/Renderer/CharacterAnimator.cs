using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat
{
    public partial class CharacterAnimator : XBehaviour, IAnimatorStateMachine
    {
        [SerializeField]
        protected Character _ownerCharacter;

        [SerializeField]
        private Animator _animator;

        public virtual void OnAnimatorStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        }

        public virtual void OnAnimatorStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stateInfo.IsName("Death"))
            {
                _ownerCharacter.Despawn();
            }
        }

        //

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _ownerCharacter = this.FindFirstParentComponent<Character>();
            _animator = GetComponent<Animator>();
        }

        private void Awake()
        {
            _ownerCharacter = this.FindFirstParentComponent<Character>();
            _animator ??= GetComponent<Animator>();
        }

        //

        internal void Initialize()
        {
            InitializeAnimatorParameters();
        }

        internal void PlaySpawnAnimation()
        {
        }

        internal bool PlayDamageAnimation(HitmarkAssetData asset)
        {
            return false;
        }

        public virtual void PlayDeathAnimation()
        {
            _animator.UpdateAnimatorTrigger(ANIMATOR_DEATH_PARAMETER_ID, AnimatorParameters);
        }

        //

        internal void SetAttackSpeed(float attackSpeed)
        {
        }

        internal void SetDamageTriggerIndex(int targetVitalColliderIndex)
        {
        }

        internal void SetDamageTypeParameter(bool isPowerfulAttack)
        {
        }

        //

        internal void StopAttacking()
        {
        }
    }
}