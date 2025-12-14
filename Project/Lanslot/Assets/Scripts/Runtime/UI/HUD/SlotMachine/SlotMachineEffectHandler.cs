using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    /// <summary>
    /// 슬롯머신 아이템 결과를 받아 무기/포션 어빌리티에 위임하는 핸들러
    /// </summary>
    public class SlotMachineEffectHandler : XBehaviour
    {
        [FoldoutGroup("#Component")][SerializeField] private PlayerCharacter _playerCharacter;
        [FoldoutGroup("#Component")][SerializeField] private PlayerWeaponAbility _weaponAbility;
        [FoldoutGroup("#Component")][SerializeField] private PlayerPotionAbility _potionAbility;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _playerCharacter ??= GetComponent<PlayerCharacter>();
            _weaponAbility ??= GetComponent<PlayerWeaponAbility>();
            _potionAbility ??= GetComponent<PlayerPotionAbility>();
        }

        public void ApplySlotResult(ItemNames itemName, System.Action onCompleted = null)
        {
            if (itemName == ItemNames.None)
            {
                onCompleted?.Invoke();
                return;
            }

            EnsureReferences();

            if (_playerCharacter == null)
            {
                Log.Warning(LogTags.UI_SlotMachine, "플레이어 캐릭터를 찾을 수 없습니다.");
                onCompleted?.Invoke();
                return;
            }

            int itemId = itemName.ToInt();
            if (itemId >= 2001)
            {
                if (_potionAbility == null)
                {
                    Log.Warning(LogTags.UI_SlotMachine, "포션 어빌리티가 존재하지 않습니다.");
                    onCompleted?.Invoke();
                    return;
                }

                _potionAbility.Apply(itemName);
                onCompleted?.Invoke();
            }
            else if (itemId >= 1001)
            {
                if (_weaponAbility == null)
                {
                    Log.Warning(LogTags.UI_SlotMachine, "무기 어빌리티가 존재하지 않습니다.");
                    onCompleted?.Invoke();
                    return;
                }

                _weaponAbility.Apply(itemName, onCompleted);
            }
            else
            {
                Log.Warning(LogTags.UI_SlotMachine, "지원하지 않는 아이템입니다: {0}", itemName);
                onCompleted?.Invoke();
            }
        }

        private void EnsureReferences()
        {
            if (_playerCharacter == null)
            {
                _playerCharacter = CharacterManager.Instance != null ? CharacterManager.Instance.Player : null;
            }

            if (_weaponAbility == null && _playerCharacter != null)
            {
                _weaponAbility = _playerCharacter.FindAbility<PlayerWeaponAbility>();
            }

            if (_potionAbility == null && _playerCharacter != null)
            {
                _potionAbility = _playerCharacter.FindAbility<PlayerPotionAbility>();
            }
        }
    }
}