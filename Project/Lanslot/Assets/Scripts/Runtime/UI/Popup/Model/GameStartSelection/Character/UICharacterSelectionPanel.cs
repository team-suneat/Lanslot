using Sirenix.OdinInspector;
using System.Collections.Generic;
using TeamSuneat.Data;
using UnityEngine;
using UnityEngine.Events;

namespace TeamSuneat.UserInterface
{
    /// <summary>
    /// 캐릭터 선택 단계를 담당하는 UI 패널
    /// </summary>
    public class UICharacterSelectionPanel : MonoBehaviour
    {
        [FoldoutGroup("#UICharacterSelectionPanel")]
        [SerializeField] private UICharacterCell[] _characterCells; // 동적 생성 셀 관리

        [FoldoutGroup("#UICharacterSelectionPanel")]
        [SerializeField] private UICharacterInfoPanel _characterInfoPanel; // 캐릭터 상세 정보 패널

        private List<PlayerCharacterData> _characterList;
        private int _currentSelectedIndex;

        private readonly UnityEvent<CharacterNames> _onCharacterSelectedEvent = new(); // 캐릭터 선택 완료 이벤트

        private void Awake()
        {
            // 셀 컴포넌트 자동 찾기
            _characterCells = GetComponentsInChildren<UICharacterCell>();
        }

        /// <summary>
        /// 패널을 초기화합니다.
        /// </summary>
        public void Setup()
        {
            _characterList = JsonDataManager.GetPlayerCharacterDataClones();
            _currentSelectedIndex = 0;

            SetupCharacterCells();
            SetupCharacterInfoPanel();
        }

        /// <summary>
        /// 캐릭터 선택 완료 이벤트를 등록합니다.
        /// </summary>
        public void RegisterCharacterSelectedEvent(UnityAction<CharacterNames> action)
        {
            _onCharacterSelectedEvent.AddListener(action);
        }

        private void SetupCharacterCells()
        {
            for (int i = 0; i < _characterCells.Length; i++)
            {
                if (_characterList.Count > i)
                {
                    PlayerCharacterData charData = _characterList[i];
                    bool isSelected = i == _currentSelectedIndex;

                    _characterCells[i].Setup(charData.Name, i, isSelected);
                    _characterCells[i].RegisterClickEvent(OnSelectCharacter);
                    _characterCells[i].SetActive(true);
                }
                else
                {
                    _characterCells[i].SetActive(false);
                }
            }
        }

        private void SetupCharacterInfoPanel()
        {
            UpdateCharacterInfoPanel(0);
        }

        private void UpdateCharacterInfoPanel(int index)
        {
            if (_characterList.IsValid(index))
            {
                PlayerCharacterData data = _characterList[index];
                _characterInfoPanel?.Bind(data);
            }
        }

        private void OnSelectCharacter(int index)
        {
            _currentSelectedIndex = index;

            UpdateCharacterInfoPanel(index);

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

        /// <summary>
        /// 현재 선택된 캐릭터를 반환합니다.
        /// </summary>
        public CharacterNames GetSelectedCharacter()
        {
            if (_characterList.IsValid(_currentSelectedIndex))
            {
                return _characterList[_currentSelectedIndex].Name;
            }
            return CharacterNames.None;
        }

        /// <summary>
        /// 캐릭터 선택을 확정하고 다음 단계로 진행합니다.
        /// </summary>
        public void ConfirmCharacterSelection()
        {
            if (_characterList.IsValid(_currentSelectedIndex))
            {
                CharacterNames selectedCharacter = _characterList[_currentSelectedIndex].Name;
                _onCharacterSelectedEvent?.Invoke(selectedCharacter);
            }
        }
    }
}
