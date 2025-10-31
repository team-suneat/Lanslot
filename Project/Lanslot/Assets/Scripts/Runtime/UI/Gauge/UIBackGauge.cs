using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    public class UIBackGauge : XBehaviour
    {
        public Slider BackSlider;
        public Image BackValueImage;

        [ReadOnly] public float BackValue;

        [ReadOnly] public bool IsNotFollowFront;

        /// <summary> 연동된 바이탈 </summary>
        [ReadOnly] public Vital LinkedVital;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            BackSlider = this.FindComponent<Slider>("Rect/Slider (Back)");
            BackValueImage = this.FindComponent<Image>("Rect/Background Group/Background Image");
        }

        public void SetLinkedVital(Vital linkedVital, VitalResourceTypes resourceType)
        {
            LinkedVital = linkedVital;

            switch (resourceType)
            {
                case VitalResourceTypes.Life:
                    { SetBackValue(linkedVital.LifeRate); }
                    break;

                case VitalResourceTypes.Shield:
                    { SetBackValue(linkedVital.ShieldRate); }
                    break;
            }
        }

        public void ResetLinkedVital()
        {
            LinkedVital = null;
            IsNotFollowFront = false;
        }

        public void Decrease(float frontValue)
        {
            if (IsNotFollowFront)
            {
                return;
            }

            if (frontValue < BackValue)
            {
                float backValue = BackValue - (Time.deltaTime * 0.3f);

                SetBackValue(backValue);
            }
        }

        // 체력바의 백 게이지 값을 설정 (Life Back Value)

        public void SetBackValue(float backFillAmount)
        {
            BackValue = Mathf.Clamp01(backFillAmount);

            if (BackSlider != null)
            {
                BackSlider.value = BackValue;
            }

            if (BackValueImage != null)
            {
                float y = Mathf.Lerp(-86, 83, BackValue);

                BackValueImage.rectTransform.anchoredPosition3D = new Vector3(BackValueImage.rectTransform.anchoredPosition3D.x, y);
            }
        }

        public void ResetBackValue()
        {
            SetBackValue(0f);
        }
    }
}