namespace TeamSuneat.UserInterface
{
    public static class ButtonStateEffectFactory
    {
        private static SpriteChangeEffect _spriteChangeEffect;
        private static ColorChangeEffect _colorChangeEffect;
        private static NoEffect _noEffect;

        public static IButtonStateEffect Create(ButtonStateEffectType effectType)
        {
            switch (effectType)
            {
                case ButtonStateEffectType.Sprite:
                    _spriteChangeEffect ??= new SpriteChangeEffect();
                    return _spriteChangeEffect;

                case ButtonStateEffectType.Color:
                    _colorChangeEffect ??= new ColorChangeEffect();
                    return _colorChangeEffect;

                case ButtonStateEffectType.None:
                default:
                    _noEffect ??= new NoEffect();
                    return _noEffect;
            }
        }
    }
}