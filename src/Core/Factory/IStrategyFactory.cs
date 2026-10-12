namespace FlashBrowser.Core.Factory;

public interface IStrategyFactory<out T> {
   public IEnumerable<T> Create();
}