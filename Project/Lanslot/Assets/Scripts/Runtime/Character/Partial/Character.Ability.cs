namespace TeamSuneat
{
    public partial class Character
    {
        private void AutoGetAbilities()
        {
            if (_characterAbilities == null || _characterAbilities.Length == 0)
            {
                _characterAbilities = GetComponents<CharacterAbility>();
            }
        }

        private void InitializeAbilities()
        {
            LogInfo("캐릭터의 어빌리티를 초기화합니다.");

            if (_characterAbilities.IsValid())
            {
                for (int i = 0; i < _characterAbilities.Length; i++)
                {
                    _characterAbilities[i].Initialization();
                }
            }
            else
            {
                LogWarning("캐릭터의 어빌리티가 배열이 설정되어있지 않습니다.");
            }
        }

        public void ResetAbilities()
        {
            if (_characterAbilities.IsValid())
            {
                for (int i = 0; i < _characterAbilities.Length; i++)
                {
                    _characterAbilities[i].ResetAbility();
                }
            }
            else
            {
                Log.Error("캐릭터의 어빌리티가 배열이 설정되어있지 않습니다. {0}", this.GetHierarchyName());
            }
        }

        protected virtual void EarlyProcessAbilities()
        {
            if (_characterAbilities.IsValid())
            {
                for (int i = 0; i < _characterAbilities.Length; i++)
                {
                    if (_characterAbilities[i] == null) { continue; }
                    if (_characterAbilities[i].enabled)
                    {
                        if (_characterAbilities[i].AbilityInitialized)
                        {
                            _characterAbilities[i].EarlyProcessAbility();
                        }
                    }
                }
            }
        }

        protected virtual void ProcessAbilities()
        {
            if (_characterAbilities != null)
            {
                for (int i = 0; i < _characterAbilities.Length; i++)
                {
                    if (_characterAbilities[i] == null)
                    {
                        continue;
                    }

                    if (!_characterAbilities[i].enabled)
                    {
                        continue;
                    }

                    if (!_characterAbilities[i].AbilityInitialized)
                    {
                        continue;
                    }

                    _characterAbilities[i].ProcessAbility();
                }
            }

            if (MyVital != null)
            {
                MyVital.ProcessAbility();
            }
        }

        protected virtual void LateProcessAbilities()
        {
            if (_characterAbilities != null)
            {
                for (int i = 0; i < _characterAbilities.Length; i++)
                {
                    if (_characterAbilities[i] == null)
                    {
                        continue;
                    }

                    if (!_characterAbilities[i].enabled)
                    {
                        continue;
                    }

                    if (!_characterAbilities[i].AbilityInitialized)
                    {
                        continue;
                    }

                    _characterAbilities[i].LateProcessAbility();
                }
            }
        }

        protected virtual void PhysicsProcessAbilities()
        {
            if (_characterAbilities != null)
            {
                for (int i = 0; i < _characterAbilities.Length; i++)
                {
                    if (_characterAbilities[i] == null)
                    {
                        continue;
                    }

                    if (!_characterAbilities[i].enabled)
                    {
                        continue;
                    }

                    if (!_characterAbilities[i].AbilityInitialized)
                    {
                        continue;
                    }

                    _characterAbilities[i].PhysicsProcessAbility();
                }
            }
        }

        public T FindAbility<T>() where T : CharacterAbility
        {
            if (_characterAbilities.IsValid())
            {
                for (int i = 0; i < _characterAbilities.Length; i++)
                {
                    if (_characterAbilities[i] is T characterAbility)
                    {
                        return characterAbility;
                    }
                }
            }

            return null;
        }
    }
}