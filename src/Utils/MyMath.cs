using System.Security.Cryptography;
using System.Text;

namespace FlashBrowser.Utils;

internal static class MyMath {
   public static int Repeat(int value, int min, int max) {
      return value < min ? max : value > max ? min : value;
   }

   public static string HashSha256(string value) {
      return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
   }
}