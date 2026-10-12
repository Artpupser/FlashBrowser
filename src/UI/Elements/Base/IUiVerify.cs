using FlashBrowser.Services.Input;
using FlashBrowser.UI.Values;

namespace FlashBrowser.UI.Elements.Base;

public interface IUiVerify : IUiElement, IUiSetter<IUiVerify, bool> {
   public IUiVerify WithY(IValue<string> value);
   public IUiVerify WithN(IValue<string> value);
   public IUiVerify WithYKey(IValue<KeyboardKey> key);
   public IUiVerify WithNKey(IValue<KeyboardKey> key);
   public IUiVerify WithPrompt(IValue<string> value);

   public IUiVerify WithPrompt(string value) {
      return WithPrompt(new ConstValue<string>(value));
   }

   public IUiVerify WithY(string value) {
      return WithY(new ConstValue<string>(value));
   }

   public IUiVerify WithN(string value) {
      return WithN(new ConstValue<string>(value));
   }
}