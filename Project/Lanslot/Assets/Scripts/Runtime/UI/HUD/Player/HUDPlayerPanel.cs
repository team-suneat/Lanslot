using TeamSuneat.Data.Game;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    public class HUDPlayerPanel : XBehaviour
    {
        [SerializeField] private HUDPortraitView _portraitView;
        [SerializeField] private HUDHealthShieldView _healthShieldView;
        [SerializeField] private HUDCurrencyView _currencyView;
        [SerializeField] private HUDWeaponGridView _weaponGridView;
        [SerializeField] private HUDPotionGridView _potionGridView;

        protected override void RegisterGlobalEvent()
        {
            base.RegisterGlobalEvent();

            GlobalEvent.Register(GlobalEventType.PLAYER_CHARACTER_BATTLE_READY, OnPlayerCharacterBattleReady);
        }

        protected override void UnregisterGlobalEvent()
        {
            base.UnregisterGlobalEvent();

            GlobalEvent.Unregister(GlobalEventType.PLAYER_CHARACTER_BATTLE_READY, OnPlayerCharacterBattleReady);
        }

        private void OnPlayerCharacterBattleReady()
        {
            VProfile profileInfo = GameApp.GetSelectedProfile();
            if (profileInfo == null)
            {
                return;
            }

            PlayerCharacter playerCharacter = CharacterManager.Instance.Player;
            if (playerCharacter == null)
            {
                return;
            }

            _healthShieldView?.Bind(playerCharacter);
            SetPortrait(playerCharacter.Name);
            SetHealth(playerCharacter.CurrentLife, playerCharacter.MaxLife);
            SetShield(playerCharacter.CurrentShield, playerCharacter.MaxShield);
            SetGold(profileInfo.Currency.GetAmount(CurrencyNames.Gold));
            SetGem(profileInfo.Currency.GetAmount(CurrencyNames.Gem));
            SetWeapons(profileInfo.Weapon);
            SetPotions(profileInfo.Potion);
        }

        public void SetPortrait(CharacterNames characterName)
        {
            if (_portraitView == null)
            {
                return;
            }

            _portraitView.SetPortrait(characterName);
            _portraitView.SetName(characterName);
        }

        public void SetHealth(int current, int max)
        {
            _healthShieldView?.SetHealth(current, max);
        }

        public void SetShield(int current, int max)
        {
            _healthShieldView?.SetShield(current, max);
        }

        public void SetGold(int amount, Sprite icon = null)
        {
            _currencyView?.SetGold(amount, icon);
        }

        public void SetGem(int amount, Sprite icon = null)
        {
            _currencyView?.SetGem(amount, icon);
        }

        public void SetWeapons(VCharacterWeapon characterWeaponInfo)
        {
            _weaponGridView?.Initialize(characterWeaponInfo);
        }

        public void SetPotions(VCharacterPotion characterPotionInfo)
        {
            _potionGridView?.Initialize(characterPotionInfo);
        }

        //

        public void Clear()
        {
            _portraitView?.Clear();
            _healthShieldView?.Clear();
            _currencyView?.Clear();
            _weaponGridView?.Clear();
            _potionGridView?.Clear();
        }
    }
}