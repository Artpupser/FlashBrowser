using FlashBrowser.Services.Browser;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using PupaLib.Data;

using YamlDotNet.Serialization;

namespace FlashBrowser.Services.Data;

[Serializable]
public sealed class ApplicationData : RecursionData {
   [JsonIgnore]
   [YamlIgnore]
   public bool IsFirst {
      get => DeepGet<bool>("isFirst");
      set => DeepSet("isFirst", value);
   }

   [JsonIgnore]
   [YamlIgnore]
   public string Password {
      get => DeepGet<string>("password");
      set => DeepSet<string>("password", value);
   }

   [JsonIgnore]
   [YamlIgnore]
   public bool DevMode {
      get => DeepGet<bool>("devMode");
      set => DeepSet("devMode", value);
   }

   [JsonIgnore]
   [YamlIgnore]
   public List<string> LastBrowserFoldersHistory {
      get => ((JArray)Data["history"]).ToObject<List<string>>()!;
      set => Data["history"] = JArray.FromObject(value);
   }

   [JsonIgnore]
   [YamlIgnore]
   public List<LaunchProfile> Profiles {
      get => ((JArray)Data["profiles"]).ToObject<List<LaunchProfile>>()!;
      set => Data["profiles"] = JArray.FromObject(value);
   }

   public static ApplicationData Default() {
      var obj = new ApplicationData {
         IsFirst = true,
         Password = string.Empty,
         LastBrowserFoldersHistory = [],
         DevMode = false,
         Profiles = []
      };
      return obj;
   }
}