namespace TeamSuneat
{
    public static partial class SpriteEx
    {
        public static string GetSpriteName(this CharacterNames key)
        {
            _stringBuilder.Clear();
            _stringBuilder.Append(CHARACTER_ICON_FORMAT);
            _stringBuilder.Append(key.ToLowerString());

            return _stringBuilder.ToString();
        }
    }
}