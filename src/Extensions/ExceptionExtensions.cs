namespace FlashBrowser.Extensions;

internal static class ExceptionExtensions {
   public static Exception Critical(string message) {
      return new Exception($"[CRITICAL] {message}");
   }
}