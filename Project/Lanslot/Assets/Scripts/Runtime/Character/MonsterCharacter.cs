using Sirenix.OdinInspector;
using TeamSuneat.Data;
using TMPro;
using UnityEngine;

namespace TeamSuneat
{
    public class MonsterCharacter : Character
    {
        [FoldoutGroup("#Character/Component/Monster")]
        [ChildGameObjectsOnly]
        [SerializeField] private DropObjectSpawner _dropObjectSpawner;

        public override Transform Target => null;

        public override LogTags LogTag => LogTags.Monster;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _dropObjectSpawner = GetComponentInChildren<DropObjectSpawner>();
        }

        protected override void OnStart()
        {
            base.OnStart();

            if (AutoSetupOnStart)
            {
                Initialize();
            }
        }

        public override void Initialize()
        {
            SetupLevel();

            base.Initialize();

            PlaySpawnAnimation();
            CharacterManager.Instance.Register(this);
        }

        //

        public override void ResetTarget()
        {
            base.ResetTarget();
        }

        public override void SetTarget(Vital targetVital)
        {
            if (targetVital == null)
            {
                return;
            }

            if (targetVital.Owner == null)
            {
                return;
            }

            TargetCharacter = targetVital.Owner;
        }

        public override void SetTarget(Character targetCharacter)
        {
            TargetCharacter = targetCharacter;
        }

        //
        public override void AddCharacterStats()
        {
            MonsterCharacterData data = JsonDataManager.FindMonsterCharacterDataClone(Name);
            if (data != null)
            {
                Stat.AddWithSourceInfo(StatNames.Health, data.Health, this, NameString, "CharacterBase");
                Stat.AddWithSourceInfo(StatNames.Damage, data.Damage, this, NameString, "CharacterBase");
            }
        }

        //

        protected override void OnDeath(DamageResult damageResult)
        {
            base.OnDeath(damageResult);

            _dropObjectSpawner?.SpawnDropEXP(position);
            
            CharacterAnimator?.PlayDeathAnimation();
        }
    }
}