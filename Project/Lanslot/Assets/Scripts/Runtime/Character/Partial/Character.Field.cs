using Sirenix.OdinInspector;
using TeamSuneat.Feedbacks;
using UnityEngine;

namespace TeamSuneat
{
    public partial class Character
    {
        [FoldoutGroup("#Character")] public CharacterNames Name;
        [FoldoutGroup("#Character")] public string NameString;
        [FoldoutGroup("#Character")] public LayerMask TargetMask;
        [FoldoutGroup("#Character")] public bool AutoSetupOnStart;
        [FoldoutGroup("#Character")] public bool FixedTargetCharacterCamp;

        // ───────────────────────────────────────────────────────────────────────────────────────────────────────────────

        [FoldoutGroup("#Character/Component")][ChildGameObjectsOnly] public CharacterAnimator CharacterAnimator;
        [FoldoutGroup("#Character/Component")][ChildGameObjectsOnly] public CharacterRenderer CharacterRenderer;
        [FoldoutGroup("#Character/Component")][ChildGameObjectsOnly] public Animator Animator;

        [FoldoutGroup("#Character/Component")][ChildGameObjectsOnly] public AttackSystem Attack;
        [FoldoutGroup("#Character/Component")][ChildGameObjectsOnly] public BuffSystem Buff;

        [FoldoutGroup("#Character/Component")][ChildGameObjectsOnly] public PassiveSystem Passive;
        [FoldoutGroup("#Character/Component")][ChildGameObjectsOnly] public StatSystem Stat;
        [FoldoutGroup("#Character/Component")][ChildGameObjectsOnly] public Vital MyVital;

        // ───────────────────────────────────────────────────────────────────────────────────────────────────────────────

        [FoldoutGroup("#Character/Component/Point")][ChildGameObjectsOnly] public Transform CharacterPoint;
        [FoldoutGroup("#Character/Component/Point")][ChildGameObjectsOnly] public Transform[] CharacterPoints;

        [FoldoutGroup("#Character/Component/Point")][ChildGameObjectsOnly] public Transform BodyPoint;
        [FoldoutGroup("#Character/Component/Point")][ChildGameObjectsOnly] public Transform[] BodyPoints;

        [FoldoutGroup("#Character/Component/Point")][ChildGameObjectsOnly] public Transform FootPoint;
        [FoldoutGroup("#Character/Component/Point")][ChildGameObjectsOnly] public Transform[] FootPoints;

        [FoldoutGroup("#Character/Component/Point")][ChildGameObjectsOnly] public Transform HeadPoint;
        [FoldoutGroup("#Character/Component/Point")][ChildGameObjectsOnly] public Transform[] HeadPoints;

        [FoldoutGroup("#Character/Component/Point")][ChildGameObjectsOnly] public Transform ShieldPoint;
        [FoldoutGroup("#Character/Component/Point")][ChildGameObjectsOnly] public Transform WarningTextPoint;
        [FoldoutGroup("#Character/Component/Point")][ChildGameObjectsOnly] public Transform MinimapPoint;

        [FoldoutGroup("#Character/Component/Point")] public Vector2 BuffSpawnArea;
        [FoldoutGroup("#Character/Component/Point")] public bool UseCustomBuffVFXPosition;

        // ───────────────────────────────────────────────────────────────────────────────────────────────────────────────

        [FoldoutGroup("#Character/Component-Feedbacks")]
        [SuffixLabel("전투 시작 완료 피드백")]
        public GameFeedbacks OnBattleFeedbacks;

        [FoldoutGroup("#Character/Component-Feedbacks")]
        [SuffixLabel("레벨 업 피드백")]
        public GameFeedbacks OnLevelUpFeedbacks;

        // ───────────────────────────────────────────────────────────────────────────────────────────────────────────────

        [Tooltip("방어막으로부터 앞쪽 방향의 공격을 막는지 검사합니다.")]
        [FoldoutGroup("#Character/Shield")] public bool GuardForward;

        public enum GuardTypes
        {
            None,
            Forward,
            Backward,
            All,
        }

        [FoldoutGroup("#Character/Shield")] public GuardTypes GuardType;
        [FoldoutGroup("#Character/Shield")] public Collider2D ShieldCollider;
        [FoldoutGroup("#Character/Shield")] public GameObject VFXGuard;
        [FoldoutGroup("#Character/Shield")] public Vector3 VFXGuardOffset;
        [FoldoutGroup("#Character/Shield")] public Vector2 VFXGuardRandomArea;

        // ───────────────────────────────────────────────────────────────────────────────────────────────────────────────

        [FoldoutGroup("#Character/Direction")]
        public SpawnFacingDirections DirectionOnStart = SpawnFacingDirections.Right;

        [FoldoutGroup("#Character/Direction")]
        [Tooltip("여기서 캐릭터가 스폰될 때 향해야 하는 방향을 강제할 수 있습니다. 기본값으로 설정하면 모델의 초기 방향과 일치합니다")]
        public SpawnFacingDirections DirectionOnSpawn = SpawnFacingDirections.Right;

        // ───────────────────────────────────────────────────────────────────────────────────────────────────────────────

        [FoldoutGroup("#Character/Animator")]
        [Tooltip("이 옵션이 true이면 애니메이션 컨트롤러에 파라미터가 존재하는지 확인하기 위해 검사합니다. " +
            "이를 false로 설정하면 애니메이터에 존재하지 않는 파라미터를 설정하려고 할 때 경고가 발생합니다. 애니메이터에 필수 파라미터가 있는지 확인하세요")]
        public bool PerformAnimatorSanityChecks = true;

        [FoldoutGroup("#Character/Animator")]
        [Tooltip("이 옵션이 true이면 애니메이터의 _animator에 관련 _animator 로그를 비활성화합니다.")]
        public bool DisableAnimatorLogs = false;

        // ───────────────────────────────────────────────────────────────────────────────────────────────────────────────

        [FoldoutGroup("#Character/Model")]
        [Tooltip("이 옵션은 일반 게임오브젝트와 달리 캐릭터가 SpriteRenderer가 캐릭터가 아닌 다른 게임오브젝트에 있는 경우 이를 자식으로 설정합니다. " +
            "그래서 캐릭터 캐릭터 전체를 회전시키고 아래로 떨어뜨립니다. 이는 3D 모델 캐릭터를 만들기 위함입니다. " +
            "이 안에 있는 스프라이트는 캐릭터가 아니고 다른 것이며, 이는 충돌을 피하기 위해 자식으로 설정된다는 의미입니다. " +
            "\n캐릭터를 회전시키는 것은 '모델'(캐릭터 본체가 아닌 시각적 오브젝트). 이렇게 하면 충돌을 피하기 위해 충돌/내부 콜라이더/스탯에서 분리(이 분리)합니다.")]
        public GameObject CharacterModel;

        [FoldoutGroup("#Character/Model")]
        [Tooltip("이 캐릭터의 카메라 타겟이 될 게임 오브젝트")]
        public GameObject CameraTarget;

        [FoldoutGroup("#Character/Model")]
        [Tooltip("카메라 타겟이 이동하는 속도")]
        public float CameraTargetSpeed = 5f;

        // ───────────────────────────────────────────────────────────────────────────────────────────────────────────────

        [FoldoutGroup("#Character/Flip")]
        public bool FlipModelOnTarget;

        [FoldoutGroup("#Character/Flip")]
        public bool FlipModelOnDirectionChange = true;

        [FoldoutGroup("#Character/Flip")]
        [EnableIf("FlipModelOnDirectionChange", true)]
        public Vector3 ModelFlipValue = new(-1, 1, 1);

        // ───────────────────────────────────────────────────────────────────────────────────────────────────────────────

        [FoldoutGroup("#Character/Rotate")]
        public bool RotateModelOnDirectionChange;

        [FoldoutGroup("#Character/Rotate")]
        public Vector3 ModelRotationValue = new(0f, 180f, 0f);

        [FoldoutGroup("#Character/Rotate")]
        public float ModelRotationSpeed = 0f;

        // ───────────────────────────────────────────────────────────────────────────────────────────────────────────────

        [FoldoutGroup("#Character/Airborne")]
        [Tooltip("캐릭터가 공중에 있을 수 있는 최대 거리")]
        public float AirborneDistance = 0.5f;

        [FoldoutGroup("#Character/Airborne")]
        [Tooltip("캐릭터가 공중에 있다고 판단하는 시간(초), 상태 변경을 방지하는 최소 시간")]
        public float AirborneMinimumTime = 0.1f;

        // ───────────────────────────────────────────────────────────────────────────────────────────────────────────────

        [FoldoutGroup("#Character/Events")]
        [Tooltip("이 옵션이 true이면 캐릭터의 상태 변경 시 이벤트를 보내거나 받을 수 있는 이벤트를 활성화합니다")]
        public bool SendStateChangeEvents = true;

        [FoldoutGroup("#Character/Events")]
        [Tooltip("이 옵션이 true이면 상태 업데이트 이벤트에 추가 정보가 추가되고 이벤트를 활성화합니다")]
        public bool SendStateUpdateEvents = true;

        // ───────────────────────────────────────────────────────────────────────────────────────────────────────────────

        [FoldoutGroup("#Character/State Machine")]
        public StateMachine<MovementStates> MovementState;

        [FoldoutGroup("#Character/State Machine")]
        public StateMachine<CharacterConditions> ConditionState;

        // ───────────────────────────────────────────────────────────────────────────────────────────────────────────────

        [FoldoutGroup("#Character/Collider")]
        [LabelText("사망 시 충돌체 크기 변경")]
        public bool IsChangeColliderSizeOnDeath;

        [FoldoutGroup("#Character/Collider")]
        [EnableIf("IsChangeColliderSizeOnDeath")]
        public Vector2 ColliderSizeOnDeath;

        // ───────────────────────────────────────────────────────────────────────────────────────────────────────────────

        protected Vector3 _raycastOrigin;
        protected RaycastHit2D _raycastHit2D;

        #region Animator Parameters

        protected const string ANIMATOR_GROUNDED_PARAMETER_NAME = "Grounded";
        protected const string ANIMATOR_AIRBORNE_PARAMETER_NAME = "Airborne";
        protected const string ANIMATOR_SPEED_X_PARAMETER_NAME = "xSpeed";
        protected const string ANIMATOR_SPEED_Y_PARAMETER_NAME = "ySpeed";
        protected const string ANIMATOR_WORLD_SPEED_X_PARAMETER_NAME = "WorldXSpeed";
        protected const string ANIMATOR_WORLD_SPEED_Y_PARAMETER_NAME = "WorldYSpeed";
        protected const string ANIMATOR_COLLIDING_LEFT_PARAMETER_NAME = "CollidingLeft";
        protected const string ANIMATOR_COLLIDING_RIGHT_PARAMETER_NAME = "CollidingRight";
        protected const string ANIMATOR_COLLIDING_BELOW_PARAMETER_NAME = "CollidingBelow";
        protected const string ANIMATOR_COLLIDING_ABOVE_PARAMETER_NAME = "CollidingAbove";

        protected const string ANIMATOR_IDLE_PARAMETER_NAME = "Idle";
        protected const string ANIMATOR_ALIVE_PARAMETER_NAME = "Alive";
        protected const string ANIMATOR_FACING_RIGHT_PARAMETER_NAME = "FacingRight";
        protected const string ANIMATOR_RANDOM_PARAMETER_NAME = "Random";
        protected const string ANIMATOR_RANDOM_CONSTANT_PARAMETER_NAME = "RandomConstant";
        protected const string ANIMATOR_FLIP_PARAMETER_NAME = "Flip";

        protected int _groundedAnimationParameter;
        protected int _airborneSpeedAnimationParameter;
        protected int _xSpeedSpeedAnimationParameter;
        protected int _ySpeedSpeedAnimationParameter;
        protected int _worldXSpeedSpeedAnimationParameter;
        protected int _worldYSpeedSpeedAnimationParameter;
        protected int _collidingLeftAnimationParameter;
        protected int _collidingRightAnimationParameter;
        protected int _collidingBelowAnimationParameter;
        protected int _collidingAboveAnimationParameter;
        protected int _idleSpeedAnimationParameter;
        protected int _aliveAnimationParameter;
        protected int _facingRightAnimationParameter;
        protected int _randomAnimationParameter;
        protected int _randomConstantAnimationParameter;
        protected int _flipAnimationParameter;

        #endregion Animator Parameters

        protected Color _initialColor;
        protected float _originalGravity;

        protected Vector3 _targetModelRotation;
        protected Vector3 _cameraOffset = Vector3.zero;

        protected float _animatorRandomNumber;
        protected CharacterConditions _conditionStateBeforeFreeze;

        [FoldoutGroup("#Character/Component")]
        [SerializeField] protected CharacterAbility[] _characterAbilities;
    }
}