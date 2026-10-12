namespace FlashBrowser.UI.Values;

public interface IValue<out T> {
   public T Get();
}