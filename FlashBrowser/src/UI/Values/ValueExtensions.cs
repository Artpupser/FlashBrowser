namespace FlashBrowser.UI.Values;

public static class ValueExtensions {
   public static IValue<T> Map<F, T>(this IValue<F> value, Func<F, T> convert) {
      return new ConvertValue<F, T>(value, convert);
   }
}