using TeamSuneat.Feedbacks;
using UnityEngine;

namespace TeamSuneat
{
    public partial class Vital : Entity
    {
        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            Owner = this.FindFirstParentComponent<Character>();
            Collider = GetComponent<Collider2D>();
            Colliders = this.GetComponentsInOnlyChildren<Collider2D>();

            Life = GetComponent<Life>();
            Shield = GetComponent<Shield>();

            GaugePoint = this.FindTransform("Point-Gauge");
            if (GaugePoint == null)
            {
                GaugePoint = transform;
            }

            Transform parentTransform = GetParentTransform();
            if (parentTransform == null)
            {
                return;
            }

            Transform feedbackParent = GetFeedbackParentTransform(parentTransform);
            if (feedbackParent == null)
            {
                return;
            }

            GuardFeedbacks = feedbackParent.FindComponent<GameFeedbacks>("Guard");
        }

        private Transform GetParentTransform()
        {
            return Owner != null ? Owner.transform : null;
        }

        private Transform GetFeedbackParentTransform(Transform parentTransform)
        {
            Transform feedbackParent = parentTransform.FindTransform("#Feedbacks");
            if (feedbackParent == null)
            {
                feedbackParent = parentTransform.FindTransform("Model/#Feedbacks");
            }

            return feedbackParent;
        }

        public override void AutoAddComponents()
        {
            base.AutoAddComponents();

            if (Collider == null)
            {
                if (!UseIndividualCollider)
                {
                    Collider = gameObject.AddComponent<Collider2D>();
                }
            }
        }

        public override void AutoNaming()
        {
            if (Owner != null)
            {
                SetGameObjectName($"Vital({Owner.Name})");
            }
        }

#if UNITY_EDITOR

        private void OnValidate()
        {
            if (Colliders.IsValid())
            {
                for (int i = 0; i < Colliders.Length; i++)
                {
                    if (Colliders[i].offset != Vector2.zero)
                    {
                        Log.Warning(LogTags.Develop, "바이탈 충돌체의 오프셋이 설정되어있습니다. {0}", this.GetHierarchyPath());
                    }
                }
            }
            else if (Collider != null)
            {
                if (Collider.offset != Vector2.zero)
                {
                    Log.Warning(LogTags.Develop, "바이탈 충돌체의 오프셋이 설정되어있습니다. {0}", this.GetHierarchyPath());
                }

                if (Collider.gameObject != gameObject)
                {
                    Log.Warning(LogTags.Develop, "바이탈 충돌체가 다른 곳에 설정되어있습니다. {0}", this.GetHierarchyPath());
                }
            }
        }

#endif
    }
}