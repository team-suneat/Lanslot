namespace TeamSuneat
{
    public static partial class SpriteEx
    {
        public static string GetSpriteName(this PassiveNames key)
        {
            _stringBuilder.Clear();
            _stringBuilder.Append(PASSIVE_ICON_FORMAT);
            _stringBuilder.Append(key.ToLowerString());

            return _stringBuilder.ToString();
        }
    }
}