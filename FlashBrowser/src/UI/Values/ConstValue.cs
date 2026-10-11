namespace FlashBrowser.UI.Values;

public sealed class ConstValue<T>(T value) : IValue<T> {
   public T Get() {
      return value;
   }
}