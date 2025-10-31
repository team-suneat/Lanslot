using TeamSuneat.Data;

namespace TeamSuneat
{
    public static partial class StringGetter
    {
        public static string GetString(this ActionNames actionName)
        {
            return JsonDataManager.FindStringClone($"Action_{actionName}");
        }
    }
}