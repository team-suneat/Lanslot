using System;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    [Serializable]
    public class SlotItemData
    {
        public string ItemName;
        public Sprite ItemIcon;
        public SlotResultType ResultType;
        public int Value;
        public float Weight = 1f; // 가중치 (확률 계산용)
    }
}