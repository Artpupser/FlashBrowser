namespace FlashBrowser.UI.Values;

public sealed class ConvertValue<F, T>(IValue<F> value, Func<F, T> convertFunc) : IValue<T> {
   public T Get() {
      return convertFunc.Invoke(value.Get());
   }
}