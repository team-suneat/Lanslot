using System.Collections.Generic;
using TeamSuneat.Data.Game;
using UnityEngine;

namespace TeamSuneat
{
    public partial class Character
    {
        public int Level { get; protected set; } = 1;

        public SID SID => MyVital.SID;

        public bool IsPlayer => this is PlayerCharacter;

        public bool IsBoss => this is BossCharacter;

        // Target

        public virtual Transform Target => null;

        public Character TargetCharacter { get; protected set; }

        // Vital

        public float CurrentLife => MyVital.CurrentLife;

        public float MaxLife => MyVital.MaxLife;

        public bool IsAlive => MyVital != null && MyVital.IsAlive;

        //

        public bool IsBlockInput { get; set; }

        private bool _isBattleReady;

        public bool IsBattleReady
        {
            get
            {
                return _isBattleReady;
            }
            set
            {
                if (_isBattleReady != value)
                {
                    _isBattleReady = value;
                }

                if (value)
                {
                    if (IsPlayer)
                    {
                        GlobalEvent.Send(GlobalEventType.PLAYER_CHARACTER_BATTLE_READY);
                    }
                    else
                    {
                        GlobalEvent<Character>.Send(GlobalEventType.MONSTER_CHARACTER_SPAWNED, this);
                    }
                }
            }
        }

        private bool _canFlip;

        public bool CanFlip
        {
            get => _canFlip;
            set
            {
                if (_canFlip != value)
                {
                    _canFlip = value;
                    if (value)
                    {
                        LogInfo("캐릭터가 반전할 수 있습니다. {0}", value.ToBoolString());
                    }
                    else
                    {
                        LogInfo("캐릭터가 반전할 수 없습니다. {0}", value.ToBoolString());
                    }
                }
            }
        }

        public bool IgnoreCrowdControl { get; set; }

        public bool BlockDropSpawn { get; set; }

        public bool BlockDropEXP { get; set; }

        public HashSet<int> AnimatorParameters { get; set; } = new HashSet<int>();

        public Vector3 DamageDirection { get; protected set; }

        // Point를 반환합니다.

        public Transform CurrentCharacterPoint
        {
            get
            {
                if (CharacterPoint != null)
                {
                    return CharacterPoint;
                }
                else if (CharacterPoints.IsValid())
                {
                    int index = MyVital.GetNotGuardColliderIndex();
                    if (index >= 0 && CharacterPoints.Length > index)
                    {
                        return CharacterPoints[index];
                    }
                }

                return null;
            }
        }

        public Transform CurrentBodyPoint
        {
            get
            {
                if (BodyPoint != null)
                {
                    return BodyPoint;
                }
                else if (BodyPoints.IsValid())
                {
                    int index = MyVital.GetNotGuardColliderIndex();
                    if (index >= 0 && BodyPoints.Length > index)
                    {
                        return BodyPoints[index];
                    }
                }

                return null;
            }
        }

        public Transform CurrentHeadPoint
        {
            get
            {
                if (HeadPoint != null)
                {
                    return HeadPoint;
                }
                else if (HeadPoints.IsValid())
                {
                    int index = MyVital.GetNotGuardColliderIndex();
                    if (index >= 0 && HeadPoints.Length > index)
                    {
                        return HeadPoints[index];
                    }
                }

                return null;
            }
        }

        public Transform CurrentFootPoint
        {
            get
            {
                if (FootPoint != null)
                {
                    return FootPoint;
                }
                else if (FootPoints.IsValid())
                {
                    int index = MyVital.GetNotGuardColliderIndex();
                    if (index >= 0 && FootPoints.Length > index)
                    {
                        return FootPoints[index];
                    }
                }

                return null;
            }
        }

        //

        public bool IsFlying { get; set; }

        public bool IsFacingRight
        {
            get
            {
                if (CharacterModel != null)
                {
                    return CharacterModel.transform.localScale.x > 0;
                }

                return localScale.x > 0;
            }
        }

        public bool IsCrowdControl
        {
            get
            {
                if (ConditionState.CurrentState is CharacterConditions.Stunned or CharacterConditions.Frozen or CharacterConditions.Grabbed)
                {
                    return true;
                }

                return false;
            }
        }

        public VProfile ProfileInfo => GameApp.GetSelectedProfile();
    }
}