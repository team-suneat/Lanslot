using System.Collections.Generic;
using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat
{
    public partial class CharacterAnimator : XBehaviour, IAnimatorStateMachine
    {
        [SerializeField] protected Character _ownerCharacter;
        [SerializeField] private Animator _animator;
        public string PlayingSkillAnimationName { get; internal set; }
        public bool IsDamaging { get; internal set; }

        //

        public virtual void OnAnimatorStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        }

        public virtual void OnAnimatorStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        }

        //

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _animator = GetComponent<Animator>();
            _ownerCharacter = this.FindFirstParentComponent<Character>();
        }

        private void Awake()
        {
            _animator ??= GetComponent<Animator>();
        }

        internal void Initialize()
        {
            InitializeAnimatorParameters();
        }

        internal bool CheckPlayingSkillAnimation()
        {
            return false;
        }

        internal bool PlayDamageAnimation(DamageAssetData asset)
        {
            return false;
        }

        public virtual void PlayDeathAnimation()
        {
            _animator.UpdateAnimatorTrigger(ANIMATOR_DEATH_PARAMETER_ID, AnimatorParameters);
        }

        internal void PlaySpawnAnimation()
        {
        }

        internal void SetAttackSpeed(float attackSpeed)
        {
        }

        internal void SetDamageTriggerIndex(int targetVitalColliderIndex)
        {
        }

        internal void SetDamageTypeParamter(bool isPowerfulAttack)
        {
        }

        internal void StopAttacking()
        {
        }
    }
}