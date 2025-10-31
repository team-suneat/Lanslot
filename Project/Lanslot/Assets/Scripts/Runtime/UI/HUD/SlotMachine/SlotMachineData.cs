using UnityEngine;
using System;

namespace TeamSuneat.UserInterface
{
    [CreateAssetMenu(fileName = "SlotMachineData", menuName = "Game/UI/Slot Machine Data")]
    public class SlotMachineData : ScriptableObject
    {
        [Header("슬롯 설정")]
        public int SlotCount = 6;
        public float SpinDuration = 2f;
        public float StopDelay = 0.5f;
        
        [Header("슬롯 아이템들")]
        public SlotItemData[] AvailableItems;
        
        [Header("확률 설정")]
        public SlotProbabilityData[] ProbabilityData;
    }

    [Serializable]
    public class SlotItemData
    {
        public string ItemName;
        public Sprite ItemIcon;
        public SlotResultType ResultType;
        public int Value;
        public float Weight = 1f; // 가중치 (확률 계산용)
    }

    [Serializable]
    public class SlotProbabilityData
    {
        public SlotResultType ResultType;
        public float Probability;
    }
}
