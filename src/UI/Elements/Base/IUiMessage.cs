using FlashBrowser.UI.Values;
using FlashBrowser.Utils;

namespace FlashBrowser.UI.Elements.Base;

public interface IUiMessage : IUiElement {
   public IUiMessage WithText(IValue<string> value);
   public IUiMessage WithAuthor(IValue<string> value);
   public IUiMessage WithColorFg(IValue<Color> value);
   public IUiMessage WithColorBg(IValue<Color> value);

   public IUiMessage WithColorFg(Color value) {
      return WithColorFg(new ConstValue<Color>(value));
   }

   public IUiMessage WithColorBg(Color value) {
      return WithColorBg(new ConstValue<Color>(value));
   }

   public IUiMessage WithText(string value) {
      return WithText(new ConstValue<string>(value));
   }

   public IUiMessage WithAuthor(string value) {
      return WithAuthor(new ConstValue<string>(value));
   }
}