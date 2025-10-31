using System.Collections;
using TeamSuneat.Data.Game;
using UnityEngine;

namespace TeamSuneat
{
    public class DropEXP : DropObject
    {
        // 1레벨 : 7
        // 2레벨 : 15
        // 3레벨 : 18

        [SerializeField] private int _expAmount = 1;

        public override void Execute()
        {
            base.Execute();
            StartCoroutine(ProcessMoveToPlayer());
        }

        private IEnumerator ProcessMoveToPlayer()
        {
            var player = CharacterManager.Instance.Player;
            float distanceToPlayer = float.MaxValue;

            while (distanceToPlayer < 0.1f)
            {
                Vector3 direction = transform.position - player.position;
                distanceToPlayer = direction.magnitude;
                _rigidbody.linearVelocity = direction.normalized;
                yield return new WaitForFixedUpdate();
            }

            _rigidbody.linearVelocity = Vector2.zero;

            AddExperienceToPlayer(_expAmount);
            Deactivate();
            Despawn();
        }

        private void AddExperienceToPlayer(int expAmount)
        {
            VProfile profileInfo = GameApp.GetSelectedProfile();
            if (profileInfo != null)
            {
                profileInfo.Level.AddExperience(expAmount);
                Log.Info(LogTags.DropObject, "플레이어에게 경험치 {0} 추가됨", expAmount);
            }
        }

        public void SetExpAmount(int expAmount)
        {
            _expAmount = expAmount;
            Log.Info(LogTags.DropObject, "경험치 양이 {0}으로 설정됨", _expAmount);
        }

        public int GetExpAmount()
        {
            return _expAmount;
        }
    }
}