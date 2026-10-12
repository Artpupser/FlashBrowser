using Newtonsoft.Json;

namespace FlashBrowser.Services.Browser;

[Serializable]
public sealed class LaunchProfile(string name) {
   [JsonProperty] public string Name { get; set; } = name;
   [JsonProperty] public List<string> Links { get; set; } = [];
   [JsonProperty] public List<AppProfile> AppProfiles { get; set; } = [];
}