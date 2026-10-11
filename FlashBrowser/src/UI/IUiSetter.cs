using FlashBrowser.UI.Values;

namespace FlashBrowser.UI;

public interface IUiSetter<out TSelf, T> where TSelf : IUiSetter<TSelf, T> {
   public TSelf Ref(in ValueRef<T> valueRef);
}