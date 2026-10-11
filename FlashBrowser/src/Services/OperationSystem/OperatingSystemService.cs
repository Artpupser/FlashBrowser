using System.Collections;

namespace FlashBrowser.Services.OperationSystem;

public sealed class OperatingSystemService : IEnumerable<OperatingSystemType> {
   public OperatingSystemType CurrentOperatingSystem { get; private set; }

   public IEnumerator<OperatingSystemType> GetEnumerator() {
      return ((IEnumerable<OperatingSystemType>)Enum.GetValues<OperatingSystemType>()
         .Where(x => x != OperatingSystemType.None)).GetEnumerator();
   }

   IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
   }

   public static bool IsOperationSystem(OperatingSystemType operatingSystemType) {
      return operatingSystemType switch {
         OperatingSystemType.Windows => OperatingSystem.IsWindows(),
         OperatingSystemType.Linux => OperatingSystem.IsLinux(),
         OperatingSystemType.MacOs => OperatingSystem.IsMacOS(),
         _ => false
      };
   }

   public void DetectCurrentOperatingSystem() {
      CurrentOperatingSystem = GetCurrentOperatingSystemType();
   }

   public OperatingSystemType GetCurrentOperatingSystemType() {
      foreach (var osType in this)
         if (IsOperationSystem(osType))
            return osType;
      return OperatingSystemType.None;
   }

   public OperatingSystemType Cast(int value) {
      return (OperatingSystemType)value;
   }

   public int Cast(OperatingSystemType value) {
      return (int)value;
   }
}