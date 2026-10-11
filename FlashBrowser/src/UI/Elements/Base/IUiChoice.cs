using FlashBrowser.UI.Values;
using FlashBrowser.Utils;

namespace FlashBrowser.UI.Elements.Base;

public interface IUiChoice : IUiElement, IUiSetter<IUiChoice, (int index, string choice)> {
   public IUiChoice WithPrompt(IValue<string> value);

   public IUiChoice WithPrompt(string value) {
      return WithPrompt(new ConstValue<string>(value));
   }

   public IUiChoice WithChoice(IValue<string> value);

   public IUiChoice WithChoice(string value) {
      return WithChoice(new ConstValue<string>(value));
   }

   public IUiChoice WithChoices(params IValue<string>[] value);

   public IUiChoice WithSelectFgColor(Color color) {
      return WithSelectFgColor(new ConstValue<Color>(color));
   }

   public IUiChoice WithSelectBgColor(Color color) {
      return WithSelectBgColor(new ConstValue<Color>(color));
   }

   public IUiChoice WithNoSelectFgColor(Color color) {
      return WithNoSelectFgColor(new ConstValue<Color>(color));
   }

   public IUiChoice WithNoSelectBgColor(Color color) {
      return WithNoSelectBgColor(new ConstValue<Color>(color));
   }

   public IUiChoice WithSelectFgColor(IValue<Color> color);
   public IUiChoice WithSelectBgColor(IValue<Color> color);
   public IUiChoice WithNoSelectFgColor(IValue<Color> color);
   public IUiChoice WithNoSelectBgColor(IValue<Color> color);

   public IUiChoice WithChoices(params string[] value) {
      return value.Length <= 0
         ? this
         : WithChoices(value.Select(x => new ConstValue<string>(x)).ToArray<IValue<string>>());
   }
}