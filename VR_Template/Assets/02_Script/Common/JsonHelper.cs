using Newtonsoft.Json;

public static class JsonHelper
{
    public static JsonSerializerSettings Settings = null;

    public static void Init()
    {
        if (Settings == null)
            Settings = new JsonSerializerSettings();

        Settings.NullValueHandling = NullValueHandling.Ignore;
    }

    public static void UseIndent()
    {
        if (Settings == null)
            Settings = new JsonSerializerSettings();

        Settings.Formatting = Newtonsoft.Json.Formatting.Indented;
    }

    public static string ToJson(this object jsonobject)
    {
        return JsonConvert.SerializeObject(jsonobject, Settings);
    }

    public static T ToObject<T>(this string json)
    {
        return JsonConvert.DeserializeObject<T>(json, Settings);
    }

    public static T ToClone<T>(this T jsonobject)
    {
        var json = jsonobject.ToJson();
        return json.ToObject<T>();
    }
}
