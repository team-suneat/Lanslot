using System.Collections.Generic;
using TeamSuneat.Feedbacks;
using TeamSuneat.UserInterface;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace TeamSuneat
{
    public partial class Vital : Entity
    {
        [FoldoutGroup("#Vital")]
        public bool UseIndividualCollider;

        [FoldoutGroup("#Vital")]
        public VitalColliderHandler VitalColliderHandler = new();

        //──────────────────────────────────────────────────────────────────────────────────────────────────────

        [FoldoutGroup("#Vital-Components")]
        public Character Owner;

        [FoldoutGroup("#Vital-Components")]
        public Collider2D Collider;

        [FoldoutGroup("#Vital-Components")]
        public Collider2D[] Colliders;

        [FoldoutGroup("#Vital-Components")]
        public Transform GaugePoint;

        //──────────────────────────────────────────────────────────────────────────────────────────────────────

        [FoldoutGroup("#Vital-Battle Resource")]
        public VitalResourceTypes ResourceType;

        [FoldoutGroup("#Vital-Battle Resource")]
        public Life Life;

        [FoldoutGroup("#Vital-Battle Resource")]
        public Shield Shield;

        //──────────────────────────────────────────────────────────────────────────────────────────────────────
        [FoldoutGroup("#Vital-Gauge")]
        public bool UseGauge;

        [FoldoutGroup("#Vital-Gauge")]
        public bool UseSpawnGaugeOnInit;

        [FoldoutGroup("#Vital-Gauge")]
        [ReadOnly]
        public EnemyHealthShieldView EnemyGauge
        {
            get
            {
                if (UIManager.Instance != null)
                {
                    IEnemyGaugeView view = UIManager.Instance.GaugeManager.FindEnemy(this);
                    return view as EnemyHealthShieldView;
                }

                return null;
            }
        }

        //──────────────────────────────────────────────────────────────────────────────────────────────────────

        [FoldoutGroup("#Vital-Guard")]
        public List<int> GuardColliderIndexes = new();

        [FoldoutGroup("#Vital-Guard")]
        [SuffixLabel("설정된 인덱스의 충돌체는 보호막만 피해입음")]
        public List<int> OnlyUseShieldColliderIndexes = new();

        //──────────────────────────────────────────────────────────────────────────────────────────────────────

        [FoldoutGroup("#Vital-Event")]
        public UnityEvent DieEvent; // 파괴 가능한 오브젝트에서 사용

        [FoldoutGroup("#Vital-Feedback")]
        public GameFeedbacks GuardFeedbacks;
    }
}