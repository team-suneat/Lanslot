namespace TeamSuneat
{
    public enum ControllerTypes
    { Player, AI }

    public enum FacingDirections
    { Left, Right }

    public enum SpawnFacingDirections
    { Left, Right }

    public enum CharacterConditions
    {
        Normal,

        /// <summary> 제어된 움직임 </summary>
        ControlledMovement,

        /// <summary> 얼음 상태 </summary>
        Frozen,

        /// <summary> 일시 정지 </summary>
        Paused,

        /// <summary> 사망 </summary>
        Dead,

        /// <summary> 기절 </summary>
        Stunned,

        /// <summary> 붙잡혔음 </summary>
        Grabbed,

        /// <summary> 속박 </summary>
        Snared,
    }

    public enum MovementStates
    {
        Null,

        /// <summary> 대기 </summary>
        Idle,

        /// <summary> 걷다 </summary>
        Walking,

        /// <summary> 달리다 </summary>
        Running,

        /// <summary> 돌진하다 </summary>
        Dashing,

        /// <summary> 쥐고있다 </summary>
        Gripping,

        /// <summary> 밀다 </summary>
        Pushing,

        /// <summary> 경로를 따르다 </summary>
        FollowingPath,

        /// <summary> 공격하다 </summary>
        Attack,

        /// <summary> 포션을 소모하다 </summary>
        ConsumingPotion,

        /// <summary> 넉백 </summary>
        Knockback,
    }
}