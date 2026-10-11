using FlashBrowser.Services.OperationSystem;

using PupaLib.Core;
using PupaLib.FileIO;

namespace FlashBrowser.Core.Os;

public sealed class DefaultOsProgramFolderStrategy(OperatingSystemService operatingSystemService) {
   public Option<VirtualFolder> Execute() {
      return operatingSystemService.CurrentOperatingSystem switch {
         OperatingSystemType.Windows => Option<VirtualFolder>.Ok(VirtualFolder.GetFolder(@"C:\Program Files\").Content),
         OperatingSystemType.Linux => Option<VirtualFolder>.Ok(VirtualFolder.GetFolder(@"/usr/bin/").Content),
         OperatingSystemType.MacOs => Option<VirtualFolder>.Ok(VirtualFolder.GetFolder(@"/Applications/").Content),
         _ => Option<VirtualFolder>.Fail()
      };
   }
}