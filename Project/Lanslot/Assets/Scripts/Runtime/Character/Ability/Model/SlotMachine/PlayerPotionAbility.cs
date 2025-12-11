using Sirenix.OdinInspector;
using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat
{
    /// <summary>
    /// 슬롯머신 포션 효과 적용 어빌리티
    /// </summary>
    public class PlayerPotionAbility : CharacterAbility
    {
        public override void Initialization()
        {
            base.Initialization();
            EnsureReferences();
        }

        public void Apply(ItemNames itemName)
        {
            if (itemName == ItemNames.None)
            {
                return;
            }

            EnsureReferences();

            if (Owner == null)
            {
                Log.Warning(LogTags.UI_SlotMachine, "플레이어 캐릭터를 찾을 수 없습니다.");
                return;
            }

            PotionData potionData = JsonDataManager.FindPotionDataClone(itemName);
            if (potionData == null || potionData.Name == ItemNames.None)
            {
                Log.Warning(LogTags.UI_SlotMachine, "포션 데이터를 찾을 수 없습니다: {0}", itemName);
                return;
            }

            if (Owner.Stat == null)
            {
                Log.Warning(LogTags.UI_SlotMachine, "플레이어 스탯 시스템이 초기화되지 않았습니다.");
                return;
            }

            Owner.Stat.AddWithSourceInfo(potionData.StatName, potionData.StatValue, Owner, itemName.ToString(), "SlotMachinePotion");
            Log.Info(LogTags.UI_SlotMachine, "포션 효과 적용: {0} {1}", potionData.StatName, potionData.StatValue);
        }

        private void EnsureReferences()
        {
            Owner ??= (base.Owner as PlayerCharacter);
        }
    }
}

