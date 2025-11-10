using Sirenix.OdinInspector;
using System.Collections.Generic;
using TeamSuneat.Data;
using TeamSuneat.Data.Game;
using UnityEngine;
using UnityEngine.Events;

namespace TeamSuneat.UserInterface
{
    /// <summary>
    /// 무기 선택 단계를 담당하는 UI 패널 (최대 2종 선택)
    /// </summary>
    public class UIWeaponSelectionPanel : MonoBehaviour
    {
        [FoldoutGroup("#UIWeaponSelectionPanel")]
        [SerializeField] private UIWeaponCell[] _weaponCells; // 동적 생성 셀 관리

        [FoldoutGroup("#UIWeaponSelectionPanel")]
        [SerializeField] private UIWeaponInfoPanel _weaponInfoPanel; // 무기 상세 정보 패널

        private const int MAX_SELECTED_WEAPONS = 3;
        private List<WeaponData> _weaponList;
        private readonly List<int> _selectedWeaponIndices = new(); // 선택된 무기 인덱스 리스트 (최대 2개)
        private readonly UnityEvent<List<ItemNames>> _onWeaponsSelectedEvent = new(); // 무기 선택 완료 이벤트
        private int _decidedWeaponIndex = -1; // 결정된 무기 인덱스 (선택한 캐릭터의 기본 무기)

        private void Awake()
        {
            // 셀 컴포넌트 자동 찾기
            _weaponCells = GetComponentsInChildren<UIWeaponCell>();
            _weaponInfoPanel ??= GetComponentInChildren<UIWeaponInfoPanel>();

            if (_weaponCells.IsValid())
            {
                for (int i = 0; i < _weaponCells.Length; i++)
                {
                    _weaponCells[i].RegisterClickEvent(OnSelectWeapon);
                }
            }
        }

        public void Setup()
        {
            _weaponList = JsonDataManager.GetWeaponDataClones();
            _selectedWeaponIndices.Clear();
            _decidedWeaponIndex = -1;

            // 기본 무기 설정을 먼저 수행 (셀 설정 전에 결정된 무기 인덱스 결정)
            FindDefaultWeaponIndex();
            SetupWeaponCells();
            SetupWeaponInfoPanel();
        }

        public void RegisterWeaponsSelectedEvent(UnityAction<List<ItemNames>> action)
        {
            _onWeaponsSelectedEvent.AddListener(action);
        }

        private void SetupWeaponCells()
        {
            VProfile profileInfo = GameApp.GetSelectedProfile();

            for (int i = 0; i < _weaponCells.Length; i++)
            {
                if (_weaponList.Count > i)
                {
                    WeaponData weaponData = _weaponList[i];
                    bool isSelected = _selectedWeaponIndices.Contains(i);
                    bool isDecided = i == _decidedWeaponIndex;

                    // 소지하지 않은 무기는 잠금 처리
                    bool isLocked = !(profileInfo?.Weapon.CheckUnlocked(weaponData.Name) ?? false);

                    _weaponCells[i].Setup(weaponData, i, isSelected, isDecided, isLocked);
                    _weaponCells[i].SetActive(true);
                }
                else
                {
                    _weaponCells[i].SetActive(false);
                }
            }
        }

        /// <summary>
        /// 선택된 캐릭터의 기본 무기 인덱스를 찾습니다.
        /// </summary>
        private void FindDefaultWeaponIndex()
        {
            VProfile profileInfo = GameApp.GetSelectedProfile();
            if (profileInfo == null)
            {
                return;
            }

            CharacterNames selectedCharacter = profileInfo.Character.SelectedCharacterName;
            if (selectedCharacter == CharacterNames.None)
            {
                return;
            }

            // 선택된 캐릭터 데이터 조회
            PlayerCharacterData characterData = JsonDataManager.FindPlayerCharacterDataClone(selectedCharacter);
            if (characterData == null || characterData.Weapon == ItemNames.None)
            {
                return;
            }

            // 기본 무기 인덱스 찾기
            ItemNames defaultWeapon = characterData.Weapon;
            for (int i = 0; i < _weaponList.Count; i++)
            {
                if (_weaponList[i].Name == defaultWeapon)
                {
                    _decidedWeaponIndex = i;
                    _selectedWeaponIndices.Add(i);

                    // 기본 무기 등록
                    profileInfo.Weapon.AddWeapon(defaultWeapon);
                    break;
                }
            }
        }

        private void SetupWeaponInfoPanel()
        {
            // 결정된 기본 무기 정보 표시 (있는 경우)
            if (_decidedWeaponIndex >= 0 && _weaponList.IsValid(_decidedWeaponIndex))
            {
                UpdateWeaponInfoPanel(_decidedWeaponIndex);
            }
            // 그렇지 않으면 첫 번째 무기 정보 표시
            else if (_weaponList.Count > 0)
            {
                UpdateWeaponInfoPanel(0);
            }
        }

        private void UpdateWeaponInfoPanel(int index)
        {
            if (_weaponList.IsValid(index))
            {
                WeaponData weaponData = _weaponList[index];
                _weaponInfoPanel?.Bind(weaponData);
            }
        }

        private void OnSelectWeapon(int index)
        {
            if (!_weaponList.IsValid(index))
            {
                return;
            }

            // 무기 정보 패널 업데이트 (선택한 무기 정보 표시)
            UpdateWeaponInfoPanel(index);

            // 결정된 무기는 클릭해도 무시 (선택 해제 불가)
            if (index == _decidedWeaponIndex)
            {
                return;
            }

            // 잠금된 무기는 클릭해도 무시 (선택 불가)
            if (_weaponCells[index].IsLocked)
            {
                return;
            }

            // 이미 선택된 무기인 경우 선택 해제
            if (_selectedWeaponIndices.Contains(index))
            {
                _ = _selectedWeaponIndices.Remove(index);
                _weaponCells[index].Deselect();
            }
            else
            {
                // 최대 선택 개수 초과 시 결정되지 않은 무기 중 가장 먼저 선택된 무기 해제
                if (_selectedWeaponIndices.Count >= MAX_SELECTED_WEAPONS)
                {
                    // 결정되지 않은 무기 중 가장 먼저 선택된 것을 찾아서 해제
                    for (int i = 0; i < _selectedWeaponIndices.Count; i++)
                    {
                        int selectedIndex = _selectedWeaponIndices[i];
                        if (selectedIndex != _decidedWeaponIndex)
                        {
                            _selectedWeaponIndices.RemoveAt(i);
                            _weaponCells[selectedIndex].Deselect();
                            break;
                        }
                    }
                }

                // 새 무기 선택
                _selectedWeaponIndices.Add(index);
                _weaponCells[index].Select();
            }
        }

        public List<ItemNames> GetSelectedWeapons()
        {
            List<ItemNames> result = new();
            for (int i = 0; i < _selectedWeaponIndices.Count; i++)
            {
                int index = _selectedWeaponIndices[i];
                if (_weaponList.IsValid(index))
                {
                    result.Add(_weaponList[index].Name);
                }
            }
            return result;
        }

        public bool IsSelectionComplete()
        {
            return _selectedWeaponIndices.Count == MAX_SELECTED_WEAPONS;
        }

        public void ConfirmWeaponSelection()
        {
            List<ItemNames> selectedWeapons = GetSelectedWeapons();
            if (selectedWeapons.Count == MAX_SELECTED_WEAPONS)
            {
                _onWeaponsSelectedEvent?.Invoke(selectedWeapons);
            }
        }

        public void Show()
        { gameObject.SetActive(true); }

        public void Hide()
        { gameObject.SetActive(false); }
    }
}