using System.Collections;
using UnityEngine;

namespace TeamSuneat
{
    public class BattlefieldTile : XBehaviour
    {
        public int Row { get; private set; } // 타일의 행 위치
        public int Column { get; private set; } // 타일의 열 위치
        public int Index { get; private set; } // 타일의 인덱스
        public bool IsOccupied { get; private set; } // 타일이 몬스터로 차지되었는지 여부
        public MonsterCharacter CurrentMonster { get; private set; } // 타일에 있는 몬스터

        [SerializeField]
        private SpriteRenderer _renderer;

        private Coroutine _flashCoroutine;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _renderer = GetComponent<SpriteRenderer>();
        }

        public void Initialize(int row, int column, int index)
        {
            Row = row;
            Column = column;
            Index = index;
            IsOccupied = false;
            CurrentMonster = null;

            Log.Info(LogTags.Stage, "타일 초기화: ({0}, {1}, {2})", row, column, index);
        }

        public void Clear()
        {
            IsOccupied = false;
            CurrentMonster = null;
            StopFlash();
        }

        public void SetMonster(MonsterCharacter monster)
        {
            CurrentMonster = monster;
            IsOccupied = monster != null;
        }

        public void Flash(float duration = 0.3f)
        {
            if (_renderer == null)
            {
                return;
            }

            StopFlash();
            _flashCoroutine = StartCoroutine(FlashCoroutine(duration));
        }

        private void StopFlash()
        {
            if (_flashCoroutine != null)
            {
                StopCoroutine(_flashCoroutine);
                _flashCoroutine = null;
            }

            if (_renderer != null)
            {
                _renderer.SetHitEffect(false);
            }
        }

        private IEnumerator FlashCoroutine(float duration)
        {
            if (_renderer == null)
            {
                yield break;
            }

            _renderer.SetHitEffect(true);
            yield return new WaitForSeconds(duration);
            _renderer.SetHitEffect(false);
            _flashCoroutine = null;
        }
    }
}