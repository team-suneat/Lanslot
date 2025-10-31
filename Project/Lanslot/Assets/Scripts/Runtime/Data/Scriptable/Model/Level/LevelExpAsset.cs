using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TeamSuneat.Data
{
    /// <summary>
    /// 레벨별 필요한 경험치 정보를 저장하는 스크립터블 오브젝트
    /// </summary>
    [CreateAssetMenu(fileName = "LevelExp", menuName = "TeamSuneat/Scriptable/LevelExp")]
    public class LevelExpAsset : XScriptableObject
    {
        [SerializeField] private LevelExpAssetData _data;

        [Title("설명")]
        [SerializeField, Multiline(3)]
        private string _description;

        #region Properties

        public int TID => 0; // 단일 설정 에셋이므로 TID는 0
        public ExpGrowthPattern GrowthPattern => _data.GrowthPattern;
        public float GrowthMultiplier => _data.GrowthMultiplier;
        public int GrowthAmount => _data.GrowthAmount;
        public int BaseExp => _data.BaseExp;
        public int MaxLevel => _data.MaxLevel;

        #endregion Properties

        public override void Validate()
        {
            base.Validate();
            _data.Validate();
        }

        public override void Refresh()
        {
            _data.Refresh();
            base.Refresh();
        }

        public override void Rename()
        {
            Rename("LevelExp");
        }

        #region Public Methods

        /// <summary>
        /// 특정 레벨에서 다음 레벨까지 필요한 경험치를 가져옵니다.
        /// </summary>
        public int GetRequiredExpForNextLevel(int currentLevel)
        {
            return _data.GetRequiredExpForNextLevel(currentLevel);
        }

        /// <summary>
        /// 특정 레벨까지 도달하는데 필요한 총 경험치를 가져옵니다.
        /// </summary>
        public int GetTotalExpToReachLevel(int targetLevel)
        {
            return _data.GetTotalExpToReachLevel(targetLevel);
        }

        /// <summary>
        /// 현재 경험치로 도달 가능한 최대 레벨을 계산합니다.
        /// </summary>
        public int GetMaxReachableLevel(int currentExp)
        {
            return _data.GetMaxReachableLevel(currentExp);
        }

        /// <summary>
        /// 특정 레벨의 경험치 정보가 유효한지 확인합니다.
        /// </summary>
        public bool IsValidLevel(int level)
        {
            return _data.IsValidLevel(level);
        }

        #endregion Public Methods

        #region Editor Methods

#if UNITY_EDITOR

        /// <summary>
        /// 경험치 테이블 전체 미리보기를 생성합니다.
        /// </summary>
        [FoldoutGroup("#Custom Button", 6)]
        [Button("경험치 테이블 미리보기", ButtonSizes.Large)]
        private void ShowExpTable()
        {
            Log.Info("─── 경험치 테이블 ───");
            Log.Info("증가 패턴: {0}", GrowthPattern);
            Log.Info("기본 경험치: {0}", BaseExp);
            Log.Info("최대 레벨: {0}", MaxLevel);
            
            for (int level = 2; level <= Mathf.Min(10, MaxLevel); level++)
            {
                int requiredExp = _data.GetRequiredExpForLevel(level);
                int totalExp = _data.GetTotalExpToReachLevel(level);
                Log.Info("레벨 {0}: 필요 경험치 {1}, 총 경험치 {2}", level, requiredExp, totalExp);
            }
        }

#endif

        #endregion Editor Methods
    }
}
