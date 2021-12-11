namespace Common.Util
{
    public static class CommonUtil
    {
        public static T Ensure<T>(T? instance = default) where T : new() => instance ?? new();
    }
}