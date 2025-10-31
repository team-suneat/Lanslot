using System.Collections.Generic;
using UnityEngine;

namespace TeamSuneat
{
    public partial class Character
    {
        // 애니메이션 (Animation)

        protected virtual void InitializeAnimatorParameters()
        {
            if (Animator != null)
            {
                AnimatorParameters = new HashSet<int>();

                Animator.AddAnimatorParameterIfExists(ANIMATOR_GROUNDED_PARAMETER_NAME, out _groundedAnimationParameter, AnimatorControllerParameterType.Bool, AnimatorParameters);
                Animator.AddAnimatorParameterIfExists(ANIMATOR_AIRBORNE_PARAMETER_NAME, out _airborneSpeedAnimationParameter, AnimatorControllerParameterType.Bool, AnimatorParameters);
                Animator.AddAnimatorParameterIfExists(ANIMATOR_SPEED_X_PARAMETER_NAME, out _xSpeedSpeedAnimationParameter, AnimatorControllerParameterType.Float, AnimatorParameters);
                Animator.AddAnimatorParameterIfExists(ANIMATOR_SPEED_Y_PARAMETER_NAME, out _ySpeedSpeedAnimationParameter, AnimatorControllerParameterType.Float, AnimatorParameters);
                Animator.AddAnimatorParameterIfExists(ANIMATOR_WORLD_SPEED_X_PARAMETER_NAME, out _worldXSpeedSpeedAnimationParameter, AnimatorControllerParameterType.Float, AnimatorParameters);
                Animator.AddAnimatorParameterIfExists(ANIMATOR_WORLD_SPEED_Y_PARAMETER_NAME, out _worldYSpeedSpeedAnimationParameter, AnimatorControllerParameterType.Float, AnimatorParameters);
                Animator.AddAnimatorParameterIfExists(ANIMATOR_COLLIDING_LEFT_PARAMETER_NAME, out _collidingLeftAnimationParameter, AnimatorControllerParameterType.Bool, AnimatorParameters);
                Animator.AddAnimatorParameterIfExists(ANIMATOR_COLLIDING_RIGHT_PARAMETER_NAME, out _collidingRightAnimationParameter, AnimatorControllerParameterType.Bool, AnimatorParameters);
                Animator.AddAnimatorParameterIfExists(ANIMATOR_COLLIDING_BELOW_PARAMETER_NAME, out _collidingBelowAnimationParameter, AnimatorControllerParameterType.Bool, AnimatorParameters);
                Animator.AddAnimatorParameterIfExists(ANIMATOR_COLLIDING_ABOVE_PARAMETER_NAME, out _collidingAboveAnimationParameter, AnimatorControllerParameterType.Bool, AnimatorParameters);
                Animator.AddAnimatorParameterIfExists(ANIMATOR_IDLE_PARAMETER_NAME, out _idleSpeedAnimationParameter, AnimatorControllerParameterType.Bool, AnimatorParameters);
                Animator.AddAnimatorParameterIfExists(ANIMATOR_ALIVE_PARAMETER_NAME, out _aliveAnimationParameter, AnimatorControllerParameterType.Bool, AnimatorParameters);
                Animator.AddAnimatorParameterIfExists(ANIMATOR_FACING_RIGHT_PARAMETER_NAME, out _facingRightAnimationParameter, AnimatorControllerParameterType.Bool, AnimatorParameters);
                Animator.AddAnimatorParameterIfExists(ANIMATOR_RANDOM_PARAMETER_NAME, out _randomAnimationParameter, AnimatorControllerParameterType.Float, AnimatorParameters);
                Animator.AddAnimatorParameterIfExists(ANIMATOR_RANDOM_CONSTANT_PARAMETER_NAME, out _randomConstantAnimationParameter, AnimatorControllerParameterType.Int, AnimatorParameters);
                Animator.AddAnimatorParameterIfExists(ANIMATOR_FLIP_PARAMETER_NAME, out _flipAnimationParameter, AnimatorControllerParameterType.Trigger, AnimatorParameters);

                // constant float 애니메이션 파라미터를 설정합니다.
                int randomConstant = RandomEx.Range(0, 1000);
                _ = Animator.UpdateAnimatorInteger(_randomConstantAnimationParameter, randomConstant, AnimatorParameters);
            }

            if (CharacterAnimator != null)
            {
                CharacterAnimator.Initialize();
            }
        }

        protected virtual void UpdateAnimators()
        {
            if (Animator != null)
            {
                _ = Animator.UpdateAnimatorBool(_aliveAnimationParameter, ConditionState.CurrentState != CharacterConditions.Dead, AnimatorParameters, PerformAnimatorSanityChecks);
                _ = Animator.UpdateAnimatorBool(_idleSpeedAnimationParameter, MovementState.CurrentState == MovementStates.Idle, AnimatorParameters, PerformAnimatorSanityChecks);

                UpdateAnimationRandomNumber();

                _ = Animator.UpdateAnimatorFloat(_randomAnimationParameter, _animatorRandomNumber, AnimatorParameters, PerformAnimatorSanityChecks);

                for (int i = 0; i < _characterAbilities.Length; i++)
                {
                    if (_characterAbilities[i].enabled && _characterAbilities[i].AbilityInitialized)
                    {
                        _characterAbilities[i].UpdateAnimator();
                    }
                }
            }
        }

        protected virtual void UpdateAnimationRandomNumber()
        {
            _animatorRandomNumber = RandomEx.Range(0f, 1f);
        }

        public void PlaySpawnAnimation()
        {
            CharacterAnimator?.PlaySpawnAnimation();
        }

        // 애니메이터 (Animator)

        public virtual void ChangeAnimator(Animator newAnimator)
        {
            Animator = newAnimator;

            if (Animator != null)
            {
                InitializeAnimatorParameters();
                if (DisableAnimatorLogs)
                {
                    Animator.logWarnings = false;
                }
            }
        }

        public virtual void AssignAnimator()
        {
            if (Animator != null)
            {
                InitializeAnimatorParameters();

                if (DisableAnimatorLogs)
                {
                    Animator.logWarnings = false;
                }
            }
        }

        public virtual void SetupAnimatorLayerWeight()
        {
        }
    }
}