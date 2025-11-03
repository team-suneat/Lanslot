using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat.Data
{
    [CreateAssetMenu(fileName = "GameDefine", menuName = "TeamSuneat/Scriptable/Define")]
    public class GameDefineAsset : ScriptableObject
    {
        public GameDefineAssetData Data;
    }

    [System.Serializable]
    public class GameDefineAssetData
    {
        [Title("스탯(Stat)")]
        [LabelText("공격 속도 최대량 ")]
        public float MAX_ATTACK_SPEED_RATE;

        [LabelText("방어력 데미지 감소 최대량 ")]
        public float MAX_DAMAGE_ARMOR_REDUCTION_RATE;

        [LabelText("저항력 데미지 감소 최대량")]
        public float MAX_DAMAGE_RESISTANCE_REDUCTION_RATE;

        [LabelText("데미지 감소 최대량")]
        public float MAX_DAMAGE_REDUCTION_RATE;

        [LabelText("크리티컬 확률 최소량")]
        public float MIN_CRITICAL_CHANCE_RATE;

        [LabelText("크리티컬 확률 최대량")]
        public float MAX_CRITICAL_CHANCE_RATE;

        [LabelText("회피 확률 최대량")]
        public float MAX_DODGE_CHANCE_RATE;

        [Title("빌드 (Build)")]
        [LabelText("에디터에서 사용하는 빌드 타입")]
        public BuildTypes EDITOR_BUILD_TYPE = BuildTypes.Editor;
    }
}