using Lean.Pool;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat
{
    public class DropObject : MonoBehaviour, IPoolable
    {
        [SerializeField] protected Rigidbody2D _rigidbody;
        [SerializeField] protected Collider2D _collider;

        private bool _isInitialized;
        private bool _isActive;
        private bool _isExecuted;

        private const string PLAYER_CHARACTER_TAG = "Character";

        [Button]
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        private void AutoGetComponents()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _collider = GetComponent<Collider2D>();
        }

        public void OnSpawn()
        {
        }

        public void OnDespawn()
        {
            _isInitialized = false;
            _isActive = false;
            _isExecuted = false;
        }

        public void Despawn()
        {
            ResourcesManager.Despawn(gameObject);
        }

        public virtual void Initialize()
        {
            if (_isInitialized)
            {
                return;
            }

            _isInitialized = true;
            Log.Info(LogTags.DropObject, "{0} 초기화 완료", gameObject.name);
        }

        public virtual void Activate()
        {
            _isActive = true;
            _collider.enabled = true;

            Log.Info(LogTags.DropObject, "{0} 활성화", gameObject.name);
        }

        public virtual void Deactivate()
        {
            _isActive = false;

            Log.Info(LogTags.DropObject, "{0} 비활성화", gameObject.name);
        }

        protected virtual bool TryExecute()
        {
            if (!_isActive || !_isInitialized || _isExecuted)
            {
                return false;
            }

            return true;
        }

        public virtual void Execute()
        {
            Log.Info(LogTags.DropObject, "{0} 실행됨", gameObject.name);

            _isExecuted = true;
            _collider.enabled = false;

            // 여기에 드롭 아이템 로직 구현
            // 예: 아이템 드롭, 이펙트 재생, 사운드 재생 등
        }

        private void Start()
        {
            Initialize();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!_isActive || !_isInitialized)
            {
                return;
            }

            if (other.CompareTag(PLAYER_CHARACTER_TAG) && LayerEx.IsInMask(other.gameObject.layer, GameLayers.Mask.Player))
            {
                Execute();
            }
        }
    }
}