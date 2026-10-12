namespace FlashBrowser.Extensions;

internal static class EnumExtensions {
   public static string GetLowerName(this Enum e) {
      return e.ToString().ToLower();
   }
}