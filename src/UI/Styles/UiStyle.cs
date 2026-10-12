using FlashBrowser.UI.Elements;

namespace FlashBrowser.UI.Styles;

public sealed class UiStyle<T>(Action<T> elementBuilder) where T : IUiElement {
   public void Apply(T element) {
      elementBuilder?.Invoke(element);
   }
}