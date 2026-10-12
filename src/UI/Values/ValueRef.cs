namespace FlashBrowser.UI.Values;

public sealed class ValueRef<T> : IValue<T> {
   private bool _isSet;
   private T? _value;

   public ValueRef(T value) {
      Set(value);
   }

   public ValueRef() { }

   public T Get() {
      if (!_isSet)
         throw new Exception("Value not set");
      return _value!;
   }

   public void Set(T value) {
      _value = value;
      _isSet = true;
   }

   public override string ToString() {
      return Get()?.ToString() ?? "";
   }
}