using FlashBrowser.Services.OperationSystem;

namespace FlashBrowser.Extensions;

internal static class OperatingSystemTypeExtensions {
   public static bool IsWindows(this OperatingSystemType os) {
      return os == OperatingSystemType.Windows;
   }

   public static bool IsLinux(this OperatingSystemType os) {
      return os == OperatingSystemType.Linux;
   }

   public static bool IsMacOs(this OperatingSystemType os) {
      return os == OperatingSystemType.MacOs;
   }

   public static bool IsNone(this OperatingSystemType os) {
      return os == OperatingSystemType.None;
   }
}