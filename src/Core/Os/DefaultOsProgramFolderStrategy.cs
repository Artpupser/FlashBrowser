using FlashBrowser.Services.OperationSystem;

using PupaLib.Core;
using PupaLib.FileIO;

namespace FlashBrowser.Core.Os;

public sealed class DefaultOsProgramFolderStrategy(OperatingSystemService operatingSystemService) {
   public Option<VirtualFolder> Execute() {
      return operatingSystemService.CurrentOperatingSystem switch {
         OperatingSystemType.Windows => GetFolderOrThrow(@"C:\Program Files\"),
         OperatingSystemType.Linux => GetFolderOrThrow(@"/usr/bin/"),
         OperatingSystemType.MacOs => GetFolderOrThrow(@"/Applications/"),
         _ => Option<VirtualFolder>.Fail()
      };
   }

   private static Option<VirtualFolder> GetFolderOrThrow(string path) {
      var result = VirtualFolder.GetFolder(path);
      if (result.Success == false) {
         throw new Exception($"Base application folder '{path}' not found");
      }
      return result;
   }
}