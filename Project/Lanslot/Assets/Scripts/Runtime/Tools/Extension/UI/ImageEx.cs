using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Image;

namespace TeamSuneat
{
    public static class ImageEx
    {
        public static bool TrySetSprite(this Image image, string spriteName)
        {
            return image.TrySetSprite(spriteName, false);
        }

        public static bool TrySetSprite(this Image image, string spriteName, bool useNativeSize)
        {
            if (image == null)
            {
                Log.Warning("이미지에 스프라이트를 적용할 수 없습니다. 이미지를 찾을 수 없습니다.");
                return false;
            }

            if (!string.IsNullOrEmpty(spriteName))
            {
                Sprite sprite = ResourcesManager.LoadResource<Sprite>(spriteName);
                if (sprite != null)
                {
                    image.sprite = sprite;
                    if (useNativeSize)
                    {
                        image.SetNativeSize();
                    }
                    return true;
                }
            }

            return false;
        }

        public static Vector3 GetAnchoredPosition3D(this Image image)
        {
            if (image != null)
            {
                RectTransform rectTransform = image.GetComponent<RectTransform>();
                return rectTransform.anchoredPosition3D;
            }

            return Vector3.zero;
        }

        public static void SetAnchoredPosition3D(this Image image, Vector3 anchoredPosition3D)
        {
            if (image != null)
            {
                RectTransform rectTransform = image.GetComponent<RectTransform>();
                rectTransform.anchoredPosition3D = anchoredPosition3D;
            }
        }

        public static void SetSprite(this Image image, Sprite sprite, bool useNativeSize = false)
        {
            if (sprite != null && image != null)
            {
                image.sprite = sprite;
                if (useNativeSize)
                {
                    image.SetNativeSize();
                }
            }
        }

        public static void ResetSprite(this Image image)
        {
            if (image != null)
            {
                image.sprite = null;
            }
        }

        public static void SetColor(this Image image, Color color)
        {
            if (image != null)
            {
                image.color = color;
            }
        }

        public static void SetColor(this Image image, Color color, float alpha)
        {
            if (image != null)
            {
                image.color = new Color(color.r, color.g, color.b, alpha);
            }
        }

        public static void SetAlpha(this Image image, float alpha)
        {
            if (image != null)
            {
                Color origin = image.color;

                image.color = new Color(origin.r, origin.g, origin.b, alpha);
            }
        }

        public static void SetFillAmount(this Image image, float fillAmount)
        {
            if (image != null)
            {
                image.fillAmount = Mathf.Clamp01(fillAmount);
            }
        }
    }
}