using System.Runtime.InteropServices;

namespace FlashBrowser.Utils;

internal static class Kernel32Dll {
   [DllImport("kernel32.dll", SetLastError = true)]
   public static extern bool SetConsoleMode(IntPtr hConsoleHandle, int mode);

   [DllImport("kernel32.dll", SetLastError = true)]
   public static extern bool GetConsoleMode(IntPtr handle, out int mode);

   [DllImport("kernel32.dll", SetLastError = true)]
   public static extern IntPtr GetStdHandle(int handle);

   public static void EnableANSIConsoleMode() {
      var handle = GetStdHandle(-11);
      GetConsoleMode(handle, out var mode);
      SetConsoleMode(handle, mode | 0x4);
   }
}