using Sirenix.OdinInspector;
using System.Collections.Generic;
using TeamSuneat.Data.Game;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    /// <summary>
    /// 게임 시작 시 캐릭터와 무기를 선택하는 팝업 UI 클래스
    /// </summary>
    public class UIGameStartSelectionPopup : UIPopup
    {
        public override UIPopupNames Name => UIPopupNames.GameStartSelection;

        /// <summary>
        /// 선택 단계 타입
        /// </summary>
        private enum SelectionStep
        {
            Character,  // 캐릭터 선택 단계
            Weapon,     // 무기 선택 단계
        }

        [FoldoutGroup("#UIGameStartSelectionPopup")]
        [SerializeField] private UICharacterSelectionPanel _characterSelectionPanel; // 캐릭터 선택 패널

        [FoldoutGroup("#UIGameStartSelectionPopup")]
        [SerializeField] private UIWeaponSelectionPanel _weaponSelectionPanel; // 무기 선택 패널

        [FoldoutGroup("#UIGameStartSelectionPopup")]
        [SerializeField] private UIPointerEventButton _backButton; // 뒤로가기 버튼 (무기 선택 → 캐릭터 선택)

        [FoldoutGroup("#UIGameStartSelectionPopup")]
        [SerializeField] private UIPointerEventButton _nextButton; // 다음 단계 버튼 (캐릭터 선택 → 무기 선택)

        [FoldoutGroup("#UIGameStartSelectionPopup")]
        [SerializeField] private UIPointerEventButton _completeButton; // 완료 버튼 (무기 선택 완료)

        private SelectionStep _currentStep = SelectionStep.Character;
        private CharacterNames _selectedCharacter = CharacterNames.None;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _characterSelectionPanel ??= GetComponentInChildren<UICharacterSelectionPanel>();
            _weaponSelectionPanel ??= GetComponentInChildren<UIWeaponSelectionPanel>();
        }

        public void Setup()
        {
            _currentStep = SelectionStep.Character;
            _selectedCharacter = CharacterNames.None;

            RegisterCharacterSelectedEvent();
            RegisterWeaponsSelectedEvent();
            SetupButtonEvents();
            ShowCharacterSelection();
        }

        private void SetupButtonEvents()
        {
            _nextButton?.RegisterClickEvent(OnNextButtonClick);
            _backButton?.RegisterClickEvent(OnBackButtonClick);
            _completeButton?.RegisterClickEvent(OnCompleteButtonClick);
        }

        private void RegisterCharacterSelectedEvent()
        {
            _characterSelectionPanel?.RegisterCharacterSelectedEvent(OnCharacterSelected);
            _characterSelectionPanel?.RegisterCharacterSelectionChangedEvent(OnCharacterSelectionChanged);
        }

        private void RegisterWeaponsSelectedEvent()
        {
            _weaponSelectionPanel?.RegisterWeaponsSelectedEvent(OnWeaponsSelected);
        }

        private void ShowCharacterSelection()
        {
            _currentStep = SelectionStep.Character;

            _characterSelectionPanel?.SetActive(true);
            _characterSelectionPanel?.Setup();
            _weaponSelectionPanel?.SetActive(false);

            _nextButton?.SetActive(true);
            _backButton?.SetActive(false);
            _completeButton?.SetActive(false);

            // 초기 선택된 캐릭터에 따라 버튼 상태 설정
            RefreshNextButtonState();
        }

        private void ShowWeaponSelection()
        {
            _currentStep = SelectionStep.Weapon;

            _characterSelectionPanel?.SetActive(false);
            _weaponSelectionPanel?.SetActive(true);
            _weaponSelectionPanel?.Setup();

            _nextButton?.SetActive(false);
            _backButton?.SetActive(true);
            _completeButton?.SetActive(true);
        }

        private void OnNextButtonClick()
        {
            if (_currentStep == SelectionStep.Character)
            {
                OnDecideCharacterSelection();
            }
        }

        private void OnBackButtonClick()
        {
            if (_currentStep == SelectionStep.Weapon)
            {
                ShowCharacterSelection();
            }
        }

        private void OnCompleteButtonClick()
        {
            if (_currentStep == SelectionStep.Weapon)
            {
                OnDecideWeaponSelection();
            }
        }

        private void OnCharacterSelected(CharacterNames characterName)
        {
            _selectedCharacter = characterName;
        }

        private void OnCharacterSelectionChanged(CharacterNames characterName)
        {
            _selectedCharacter = characterName;
            RefreshNextButtonState();
        }

        private void RefreshNextButtonState()
        {
            if (_nextButton == null || _currentStep != SelectionStep.Character)
            {
                return;
            }

            CharacterNames selectedCharacter = _characterSelectionPanel?.GetSelectedCharacter() ?? CharacterNames.None;
            
            if (selectedCharacter == CharacterNames.None)
            {
                _nextButton.Lock();
                return;
            }

            // 선택된 캐릭터가 해금되었는지 확인
            VProfile profileInfo = GameApp.GetSelectedProfile();
            bool isUnlocked = profileInfo?.Character.Contains(selectedCharacter) ?? false;

            if (isUnlocked)
            {
                _nextButton.Unlock();
            }
            else
            {
                _nextButton.Lock();
            }
        }

        private void OnDecideCharacterSelection()
        {
            CharacterNames selectedCharacter = _characterSelectionPanel?.GetSelectedCharacter() ?? CharacterNames.None;

            if (selectedCharacter == CharacterNames.None)
            {
                return;
            }

            // 캐릭터 저장
            VProfile profileInfo = GameApp.GetSelectedProfile();
            profileInfo?.Character.Select(selectedCharacter);

            _selectedCharacter = selectedCharacter;

            // 무기 선택 단계로 전환
            ShowWeaponSelection();
        }

        private void OnWeaponsSelected(List<WeaponNames> selectedWeapons)
        {
            // 이벤트만 수신, 실제 저장은 OnDecideWeaponSelection에서 수행
        }

        private void OnDecideWeaponSelection()
        {
            if (_weaponSelectionPanel != null)
            {
                if (_weaponSelectionPanel.IsSelectionComplete())
                {
                    CloseWithSuccess();
                }
            }
        }
    }
}