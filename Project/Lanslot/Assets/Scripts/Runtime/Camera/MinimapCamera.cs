using UnityEngine;

namespace TeamSuneat
{
    public class MinimapCamera : XBehaviour
    {
        [SerializeField] private Camera _camera;

        public Vector3 OffSet = new();
        public Vector2 ViewportAdjustment = new();
        private Collider2D _cameraBoundingCollider;
        private Transform _targetPlayer;

        private Vector3 _cameraController = new Vector3(0, 0, -10);
        private float _width;
        private float _height;
        private Vector2 _mapSize;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _camera = GetComponent<Camera>();
        }

        private void LateUpdate()
        {
            if (_camera == null) { return; }
            if (_targetPlayer == null) { return; }
            if (_cameraBoundingCollider == null) { return; }

            MoveCamera();
        }

        #region GlobalEvent

        protected override void RegisterGlobalEvent()
        {
            base.RegisterGlobalEvent();

            GlobalEvent.Register(GlobalEventType.PLAYER_CHARACTER_BATTLE_READY, OnPlayerCharacterBattleReady);
            GlobalEvent<StageNames>.Register(GlobalEventType.STAGE_SPAWNED, OnStageSpawned);
        }

        protected override void UnregisterGlobalEvent()
        {
            base.UnregisterGlobalEvent();

            GlobalEvent.Unregister(GlobalEventType.PLAYER_CHARACTER_BATTLE_READY, OnPlayerCharacterBattleReady);
            GlobalEvent<StageNames>.Unregister(GlobalEventType.STAGE_SPAWNED, OnStageSpawned);
        }

        private void OnPlayerCharacterBattleReady()
        {
            SetTargetPlayerCharacter();
        }

        private void OnStageSpawned(StageNames stageName)
        {
            SetCameraBoundingCollider();
        }

        private void SetTargetPlayerCharacter()
        {
            PlayerCharacter playerCharacter = CharacterManager.Instance.Player;
            if (playerCharacter != null)
            {
                _targetPlayer = playerCharacter.transform;
            }
        }

        private void SetCameraBoundingCollider()
        {
            _cameraBoundingCollider = FindMinimapCollider();
            if (_cameraBoundingCollider != null)
            {
                Initialize();
            }
            else
            {
                Log.Error("미니맵에서 경계용 카메라 박스 콜라이더를 찾을 수 없습니다.");
            }
        }

        #endregion GlobalEvent

        private Collider2D FindMinimapCollider()
        {
            // GameObject minimapBoundingObject = GameObject.Find("Minimap Bounding Shape 2D");
            // if (minimapBoundingObject != null)
            // {
            //     return minimapBoundingObject.GetComponent<Collider2D>();
            // }

            GameObject boundingObject = GameObject.Find("Bounding Shape 2D");
            if (boundingObject != null)
            {
                return boundingObject.GetComponent<Collider2D>();
            }

            return null;
        }

        private void Initialize()
        {
            _height = _camera.orthographicSize;
            _width = _height * (Screen.width / Screen.height);

            float sizeX = Mathf.Abs(_cameraBoundingCollider.bounds.extents.x);
            float sizeY = Mathf.Abs(_cameraBoundingCollider.bounds.extents.y);

            _mapSize = new Vector2(sizeX, sizeY);
        }

        public void MoveCamera()
        {
            if (_targetPlayer != null && _cameraBoundingCollider != null)
            {
                Vector3 targetPositionWithOffset = _targetPlayer.position + _cameraController;

                // 맵 경계 내에서 카메라 X 위치를 계산합니다.
                float adjustedWidth = _width - ViewportAdjustment.x;
                float adjustedHeight = _height - ViewportAdjustment.y;

                // 맵 경계 내에서 카메라 X 위치를 계산합니다.
                float mapLimitX = _mapSize.x - (adjustedWidth + OffSet.x);
                float clampedX = Mathf.Clamp(
                    targetPositionWithOffset.x,
                    Mathf.Max(-mapLimitX + _cameraBoundingCollider.bounds.center.x, _cameraBoundingCollider.bounds.min.x + adjustedWidth / 2),
                    Mathf.Min(mapLimitX + _cameraBoundingCollider.bounds.center.x, _cameraBoundingCollider.bounds.max.x - adjustedWidth / 2)
                );

                // 맵 경계 내에서 카메라 Y 위치를 계산합니다.
                float mapLimitY = _mapSize.y - (adjustedHeight + OffSet.y);
                float clampedY = Mathf.Clamp(
                    targetPositionWithOffset.y,
                    Mathf.Max(-mapLimitY + _cameraBoundingCollider.bounds.center.y, _cameraBoundingCollider.bounds.min.y + adjustedHeight / 2),
                    Mathf.Min(mapLimitY + _cameraBoundingCollider.bounds.center.y, _cameraBoundingCollider.bounds.max.y - adjustedHeight / 2)
                );

                // 최종 카메라 위치 계산
                Vector3 finalCameraPosition = new Vector3(clampedX, clampedY, _cameraController.z);

                // 카메라 위치가 경계 콜라이더 안에 있으면 바로 이동
                if (_cameraBoundingCollider.OverlapPoint(finalCameraPosition))
                {
                    _camera.transform.position = finalCameraPosition;
                }
                else
                {
                    // 카메라 위치가 경계 콜라이더 밖이면 가장 가까운 경계로 이동
                    Vector2 closestBoundaryPoint = _cameraBoundingCollider.ClosestPoint(finalCameraPosition);
                    _camera.transform.position = new Vector3(closestBoundaryPoint.x, closestBoundaryPoint.y, _cameraController.z);
                }
            }
        }

        public void MoveCamera2()
        {
            if (_targetPlayer != null)
            {
                Vector3 targetPositionWithOffset = _targetPlayer.position + _cameraController;

                // 맵 경계 내에서 카메라 X 위치를 계산합니다.
                float adjustedWidth = _width - ViewportAdjustment.x;
                float adjustedHeight = _height - ViewportAdjustment.y;

                // 맵 경계 내에서 카메라 X 위치를 계산합니다.
                float mapLimitX = _mapSize.x - (adjustedWidth + OffSet.x);
                float clampedX = Mathf.Clamp(
                    targetPositionWithOffset.x,
                    -mapLimitX + _cameraBoundingCollider.bounds.center.x,
                    mapLimitX + _cameraBoundingCollider.bounds.center.x
                );

                // 맵 경계 내에서 카메라 Y 위치를 계산합니다.
                float mapLimitY = _mapSize.y - (adjustedHeight + OffSet.y);
                float clampedY = Mathf.Clamp(
                    targetPositionWithOffset.y,
                    -mapLimitY + _cameraBoundingCollider.bounds.center.y,
                    mapLimitY + _cameraBoundingCollider.bounds.center.y
                );

                // 최종 카메라 위치 계산
                Vector3 finalCameraPosition = new Vector3(clampedX, clampedY, -10f);

                // 카메라 위치를 경계 콜라이더에 맞춰 이동합니다.
                Vector2 closestBoundaryPoint = _cameraBoundingCollider.ClosestPoint(finalCameraPosition);
                _camera.transform.position = new Vector3(closestBoundaryPoint.x, closestBoundaryPoint.y, -10f);
            }
        }
    }
}