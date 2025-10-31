using Sirenix.OdinInspector;
using System.Collections.Generic;
using TeamSuneat.Data;
using TeamSuneat.Data.Game;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    /// <summary>
    /// 캐릭터 목록을 표시하고, 선택 시 상세정보 및 하단 6슬롯을 갱신하는 팝업 UI 클래스
    /// </summary>
    public class UICharacterPopup : UIPopup
    {
        [FoldoutGroup("#UICharacterPopup")]
        [SerializeField] private UICharacterCell[] _characterCells; // 동적 생성 셀 관리

        [FoldoutGroup("#UICharacterPopup")]
        [SerializeField] private UICharacterDetailPanel _detailPanel; // 패널 전체를 담는 컴포넌트

        [FoldoutGroup("#UICharacterPopup")]
        [SerializeField] private List<GameObject> _bottomSlots; // 슬롯 프리팹은 추후 별도 SlotView 등으로 교체 권장

        private List<PlayerCharacterData> _characterList;
        private int _currentSelectedIndex;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _characterCells = GetComponentsInChildren<UICharacterCell>();
        }

        public void Setup()
        {
            _characterList = JsonDataManager.GetPlayerCharacterDataClones();
            _currentSelectedIndex = 0;

            SetupCharacterCells();
            SetupDetailPanel();
        }

        private void SetupCharacterCells()
        {
            for (int i = 0; i < _characterCells.Length; i++)
            {
                _characterCells[i] = _characterCells[i];
                if (_characterList.Count > i)
                {
                    PlayerCharacterData charData = _characterList[i];
                    bool isSelected = i == _currentSelectedIndex;

                    _characterCells[i].Setup(charData.Name.GetNameString(), i, isSelected);
                    _characterCells[i].RegisterClickEvent(OnSelectCharacter);
                    _characterCells[i].SetActive(true);
                }
                else
                {
                    _characterCells[i].SetActive(false);
                }
            }
        }

        private void SetupDetailPanel()
        {
            UpdateDetailPanel(0);
            _detailPanel?.RegisterClickEvent(OnDecideCharacter);
        }

        private void UpdateDetailPanel(int index)
        {
            if (_characterList.IsValid(index))
            {
                PlayerCharacterData data = _characterList[index];
                _detailPanel?.Bind(data);
            }
        }

        private void OnSelectCharacter(int index)
        {
            _currentSelectedIndex = index;

            UpdateDetailPanel(index);

            for (int i = 0; i < _characterCells.Length; i++)
            {
                if (i == index)
                {
                    _characterCells[i].Select();
                }
                else
                {
                    _characterCells[i].Deselect();
                }
            }
        }

        private void OnDecideCharacter()
        {
            VProfile profileInfo = GameApp.GetSelectedProfile();
            CharacterNames characterName = _characterList[_currentSelectedIndex].Name;
            profileInfo.Character.Select(characterName);
        }
    }
}