using System.Collections.Generic;
using TeamSuneat.Data.Game;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    public class HUDPotionGridView : MonoBehaviour
    {
        [SerializeField]
        private HUDPotionSlotView[] _slots;

        public void Initialize(VCharacterPotion characterPotionInfo)
        {
            List<VPotion> myPotionList = characterPotionInfo.GetPotions();
            for (int i = 0; i < _slots.Length; i++)
            {
                if (myPotionList.Count > i)
                {
                    VPotion myPotion = myPotionList[i];
                    Sprite potionSprite = myPotion.Name.LoadSprite();
                    PotionSlotData slotData = PotionSlotData.Equipped(potionSprite);

                    SetSlot(i, slotData);
                }
                else if (characterPotionInfo.UnlockedSlotCount > i)
                {
                    SetSlot(i, PotionSlotData.Empty());
                }
                else
                {
                    SetSlot(i, PotionSlotData.Locked());
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

        private void SetSlot(int index, PotionSlotData data)
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