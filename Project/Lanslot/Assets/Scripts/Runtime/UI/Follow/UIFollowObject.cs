using TeamSuneat.Setting;
using Lean.Pool;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    public class UIFollowObject : XBehaviour, IPoolable
    {
        [Title("UIFollowObject")]
        public RectTransform Rect;

        public Vector3 WorldOffset;
        public Vector3 ScreenOffset;

        [ReadOnly]
        public Transform FollowingPoint;

        public Transform ClonePoint;

        public bool IsWorldSpaceCanvas { get; set; }
        public bool UseFollowPointClone;

        [Tooltip("반드시 캔버스 내부에 있습니다.")]
        public bool IsMustBeInsideTheCanvas;

        public float HeightOffset;

        private Camera _mainCamera;
        private Vector3 _worldPosition;
        private Vector3 _screenPosition;

        private readonly float _worldSpaceByScaleValue = 0.0125f;

        private Camera MainCamera
        {
            get
            {
                if (_mainCamera == null)
                {
                    _mainCamera = Camera.main;
                }

                return _mainCamera;
            }
        }

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();
            Rect = this.FindComponent<RectTransform>("Rect");
            ClonePoint = this.FindTransform("ClonePoint");
        }

        protected void Awake()
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.zero;
        }

        public void OnSpawn()
        {
        }

        public void OnDespawn()
        {
            StopFollowing();
            IsWorldSpaceCanvas = false;
        }

        public void SetWorldOffset(Vector3 offset)
        {
            WorldOffset = offset;
        }

        private void FixedUpdate()
        {
            UpdatePosition();
        }

        public void UpdatePosition()
        {
            if (MainCamera == null || FollowingPoint == null)
            {
                return;
            }

            if (IsWorldSpaceCanvas)
            {
                _worldPosition = FollowingPoint.position + WorldOffset;
                anchoredPosition3D = _worldPosition;
            }
            else
            {
                _worldPosition = FollowingPoint.position + WorldOffset;

                _screenPosition = MainCamera.WorldToScreenPoint(_worldPosition);
                _screenPosition /= GameSetting.Instance.Video.ResolutionRate;
                _screenPosition.z = 0f;

                anchoredPosition3D = _screenPosition;
            }
        }

        public void StartFollowing(Transform point)
        {
            if (UseFollowPointClone && ClonePoint != null)
            {
                ClonePoint.SetParent(null);
                ClonePoint.position = point.position;
                FollowingPoint = ClonePoint;
            }
            else
            {
                FollowingPoint = point;
            }
        }

        public void StopFollowing()
        {
            if (ClonePoint != null)
            {
                ClonePoint.SetParent(transform);
            }

            FollowingPoint = null;
        }

        public void Setup(Transform point)
        {
            if (!IsWorldSpaceCanvas)
            {
                localScale = Vector3.one;
            }
            else
            {
                localScale = new Vector3(_worldSpaceByScaleValue, _worldSpaceByScaleValue, 1);
            }

            StartFollowing(point);
            UpdatePosition();
        }

        public void Setup(Vector3 worldPosition)
        {
            if (!IsWorldSpaceCanvas)
            {
                _worldPosition = worldPosition + WorldOffset;

                _screenPosition = MainCamera.WorldToScreenPoint(_worldPosition);
                _screenPosition *= GameSetting.Instance.Video.ResolutionRate;
                _screenPosition.z = 0f;

                anchoredPosition3D = _screenPosition;
            }
            else
            {
                _worldPosition = worldPosition + WorldOffset;
                _worldPosition.z = 0f;

                anchoredPosition3D = _worldPosition;
            }
        }
    }
}