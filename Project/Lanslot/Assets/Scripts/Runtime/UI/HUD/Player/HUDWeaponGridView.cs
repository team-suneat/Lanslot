using System.Collections.Generic;
using TeamSuneat.Data.Game;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    public class HUDWeaponGridView : MonoBehaviour
    {
        [SerializeField]
        private HUDWeaponSlotView[] _slots;

        public void Initialize(VCharacterWeapon characterWeaponInfo)
        {
            List<VWeapon> myWeaponList = characterWeaponInfo.GetWeapons();
            for (int i = 0; i < _slots.Length; i++)
            {
                if (myWeaponList.Count > i)
                {
                    VWeapon myWeapon = myWeaponList[i];
                    Sprite weaponSprite = myWeapon.Name.LoadSprite();
                    WeaponSlotData slotData = WeaponSlotData.Equipped(weaponSprite, myWeapon.Level);

                    SetSlot(i, slotData);
                }
                else if (characterWeaponInfo.UnlockedSlotCount > i)
                {
                    SetSlot(i, WeaponSlotData.Empty());
                }
                else
                {
                    SetSlot(i, WeaponSlotData.Locked());
                }
            }
        }

        public void Clear()
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                _slots[i]?.Clear();
            }
        }

        private void SetSlot(int index, WeaponSlotData data)
        {
            if (!IsValidIndex(index))
            {
                return;
            }

            _slots[index]?.SetSlot(data);
        }

        private bool IsValidIndex(int index)
        {
            return _slots != null && index >= 0 && index < _slots.Length;
        }
    }
}