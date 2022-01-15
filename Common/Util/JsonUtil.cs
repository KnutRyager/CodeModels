using Newtonsoft.Json;

namespace Common.Util;

public static class JsonUtil
{
    public static string ToJson(object? obj) => JsonConvert.SerializeObject(obj);
    public static T FromJson<T>(string json) => JsonConvert.DeserializeObject<T>(json)!;
}
