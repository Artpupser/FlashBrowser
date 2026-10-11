using FlashBrowser.Services.OperationSystem;

using Newtonsoft.Json;

namespace FlashBrowser.Services.Browser;

[Serializable]
public sealed class AppProfile(string name) {
   [JsonProperty] public string Name { get; set; } = name;
   [JsonProperty] public string[] ExecutePaths { get; set; } = [];

   public void SetExecutePath(int index, string path) {
      if (ExecutePaths.Length <= index) {
         var paths = ExecutePaths.ToList() ?? [];
         for (var i = 0; i < index + 1; i++) paths.Add(string.Empty);
         ExecutePaths = paths.ToArray();
      }

      ExecutePaths[index] = path;
   }

   public string GetPathOrEmpty(int index, string empty = "") {
      var result = ExecutePaths.Length > index ? ExecutePaths[index] : string.Empty;
      return string.IsNullOrWhiteSpace(result) ? empty : result;
   }

   public string GetPathOrEmpty(OperatingSystemType os, string empty = "") {
      return GetPathOrEmpty((int)os, empty);
   }
}