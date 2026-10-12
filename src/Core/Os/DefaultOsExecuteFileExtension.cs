using FlashBrowser.Services.OperationSystem;

namespace FlashBrowser.Core.Os;

public sealed class DefaultOsExecuteFileExtension(OperatingSystemService operatingSystemType)
{
   public string Execute()
   {
      return operatingSystemType.CurrentOperatingSystem switch
      {
         OperatingSystemType.Windows => ".exe",
         OperatingSystemType.Linux => "",
         OperatingSystemType.MacOs => "",
         _ => ""
      };
   }
}
