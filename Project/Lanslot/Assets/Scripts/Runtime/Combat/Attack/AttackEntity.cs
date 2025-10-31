using TeamSuneat.Data;

namespace TeamSuneat
{
    public partial class AttackEntity : XBehaviour
    {
        public Character Owner;
        public HitmarkNames Name;
        public string NameString;

        protected DamageCalculator _damageInfo = new DamageCalculator();
        protected HitmarkAssetData AssetData;

        public Vital TargetVital { get; private set; }

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            Owner = this.FindFirstParentComponent<Character>();
        }

        private void OnValidate()
        {
            Validate();
        }

        protected virtual void Validate()
        {
            EnumEx.ConvertTo(ref Name, NameString);
        }

        public override void AutoSetting()
        {
            base.AutoSetting();

            NameString = Name.ToString();
        }

        public override void AutoNaming()
        {
            SetGameObjectName(NameString);
        }

        public virtual void Initialize()
        {
            LogInfo("공격 독립체를 초기화합니다.");

            InitializeFeedbacks();
            LoadAndValidateAssetData();
            SetupDamageCalculator();
        }

        private void SetupDamageCalculator()
        {
            _damageInfo.HitmarkAssetData = AssetData;
            _damageInfo.SetAttacker(Owner);
            _damageInfo.AttackEntity = this;
        }

        private void LoadAndValidateAssetData()
        {
            if (!ValidateHitmarkName())
            {
                return;
            }

            LoadHitmarkAsset();
            ValidateAssetData();
        }

        private bool ValidateHitmarkName()
        {
            if (Name == HitmarkNames.None)
            {
                Log.Error("공격 독립체의 히트마크 이름이 설정되지 않았습니다. {0}", this.GetHierarchyPath());
                return false;
            }
            return true;
        }

        private void LoadHitmarkAsset()
        {
            AssetData = ScriptableDataManager.Instance.FindHitmarkClone(Name);
        }

        private void ValidateAssetData()
        {
            if (!AssetData.IsValid())
            {
                Log.Error("공격 독립체의 히트마크 에셋이 설정되지 않았습니다. {0}, {1}", Name.ToLogString(), this.GetHierarchyPath());
                return;
            }

            if (!AssetData.Damage.IsValid())
            {
                Log.Error("공격 독립체의 데미지 에셋이 설정되지 않았습니다. {0}, {1}", Name.ToLogString(), this.GetHierarchyPath());
            }
        }

        public virtual void OnBattleReady()
        {
        }

        public virtual void OnOwnerDeath()
        {
        }

        public virtual void Activate()
        {
            LogInfo("공격을 활성화합니다.");
        }

        public virtual void Deactivate()
        {
            LogInfo("공격을 비활성화합니다.");
        }

        public virtual void Execute()
        {
            LogInfo("공격을 실행합니다.");
        }

        protected void OnExecute(bool isAttackSuccessed)
        {
        }

        public virtual void SetOwner(Character caster)
        {
            if (caster != null)
            {
                Owner = caster;
            }
        }

        public virtual void SetTarget(Vital targetVital)
        {
            TargetVital = targetVital;
        }

        public virtual Vital GetTargetVital()
        {
            return TargetVital;
        }

        public virtual Vital GetTargetVital(int index)
        {
            return TargetVital;
        }
    }
}