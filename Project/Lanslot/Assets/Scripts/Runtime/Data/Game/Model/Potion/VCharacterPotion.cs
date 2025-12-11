using System;
using System.Collections.Generic;

namespace TeamSuneat.Data.Game
{
    [System.Serializable]
    public class VCharacterPotion
    {
        public Dictionary<string, VPotion> Potions = new();
        public List<string> UnlockedPotions = new();
        public List<string> SlotPotionNameStrings = new();
        public int UnlockedSlotCount;

        [NonSerialized]
        private readonly Dictionary<ItemNames, VPotion> _potionMap = new();

        [NonSerialized]
        private readonly List<ItemNames> _slotPotionNames = new();

        public IReadOnlyList<ItemNames> SlotPotionNames => _slotPotionNames;

        public List<ItemNames> GetPotionNames()
        {
            return new List<ItemNames>(_slotPotionNames);
        }

        public List<VPotion> GetPotions()
        {
            List<VPotion> potions = new();
            foreach (ItemNames potionName in _slotPotionNames)
            {
                if (_potionMap.TryGetValue(potionName, out VPotion potion))
                {
                    potions.Add(potion);
                }
            }

            return potions;
        }

        public void OnLoadGameData()
        {
            _potionMap.Clear();

            ItemNames itemName = ItemNames.None;
            if (Potions.IsValid())
            {
                foreach (KeyValuePair<string, VPotion> potion in Potions)
                {
                    potion.Value.OnLoadGameData();

                    if (!EnumEx.ConvertTo(ref itemName, potion.Key))
                    {
                        Log.Error(LogTags.GameData, "포션 키를 ItemNames로 변환하지 못했습니다: {0}", potion.Key);
                        continue;
                    }

                    potion.Value.Name = itemName;
                    _potionMap[itemName] = potion.Value;
                }
            }

            _slotPotionNames.Clear();
            foreach (string slotName in SlotPotionNameStrings)
            {
                if (!EnumEx.ConvertTo(ref itemName, slotName))
                {
                    Log.Error(LogTags.GameData, "포션 슬롯 이름을 ItemNames로 변환하지 못했습니다: {0}", slotName);
                    continue;
                }

                if (_slotPotionNames.Count >= UnlockedSlotCount)
                {
                    break;
                }

                if (_potionMap.ContainsKey(itemName))
                {
                    _slotPotionNames.Add(itemName);
                }
            }

            SyncSlotPotionNameStrings();
        }

        public void ClearIngameData()
        {
            Potions.Clear();
            SlotPotionNameStrings.Clear();
            _slotPotionNames.Clear();
            _potionMap.Clear();
        }

        //

        public bool CheckUnlocked(ItemNames potionName)
        {
            return UnlockedPotions.Contains(potionName.ToString());
        }

        public void Unlock(ItemNames potionName)
        {
            string key = potionName.ToString();
            if (!UnlockedPotions.Contains(key))
            {
                UnlockedPotions.Add(key);
            }
        }

        //

        public bool HasPotion(ItemNames potionName)
        {
            return Potions.ContainsKey(potionName.ToString());
        }

        public void AddPotion(ItemNames potionName)
        {
            if (_slotPotionNames.Count >= UnlockedSlotCount)
            {
                Log.Warning(LogTags.GameData, "포션 슬롯이 가득 찼습니다. 현재/최대: {0}/{1}", _slotPotionNames.Count, UnlockedSlotCount);
                return;
            }

            string key = potionName.ToString();
            if (!_potionMap.ContainsKey(potionName))
            {
                VPotion newPotion = new(potionName);
                Potions[key] = newPotion;
                _potionMap[potionName] = newPotion;
            }

            if (!_slotPotionNames.Contains(potionName))
            {
                _slotPotionNames.Add(potionName);
                SlotPotionNameStrings.Add(key);
            }
        }

        public void RemovePotion(ItemNames potionName)
        {
            string key = potionName.ToString();
            if (_potionMap.ContainsKey(potionName))
            {
                _ = Potions.Remove(key);
                _ = _potionMap.Remove(potionName);
                _ = _slotPotionNames.Remove(potionName);
                _ = SlotPotionNameStrings.Remove(key);
            }
        }

        //

        public static VCharacterPotion CreateDefault()
        {
            return new VCharacterPotion()
            {
                UnlockedSlotCount = GameDefine.POTION_SLOT_DEFAULT_UNLOCK_COUNT,
            };
        }

        private void SyncSlotPotionNameStrings()
        {
            SlotPotionNameStrings.Clear();
            for (int i = 0; i < _slotPotionNames.Count; i++)
            {
                SlotPotionNameStrings.Add(_slotPotionNames[i].ToString());
            }
        }
    }
}