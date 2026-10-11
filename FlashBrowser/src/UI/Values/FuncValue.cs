namespace FlashBrowser.UI.Values;

public sealed class FuncValue<T>(Func<T> func) : IValue<T> {
   public T Get() {
      return func.Invoke();
   }
}