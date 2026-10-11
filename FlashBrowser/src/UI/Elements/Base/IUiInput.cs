using FlashBrowser.UI.Values;

namespace FlashBrowser.UI.Elements.Base;

public interface IUiInput<T> : IUiElement {
   public IUiInput<T> WithPrompt(IValue<string> value);

   public IUiInput<T> WithPrompt(string prompt) {
      return WithPrompt(new ConstValue<string>(prompt));
   }

   public IUiInput<T> WithRange(IValue<(int min, int max)> value);
   public IUiInput<T> WithSecret(IValue<char> value);

   public IUiInput<T> WithSecret(char value) {
      return WithSecret(new ConstValue<char>(value));
   }

   public IUiInput<T> WithRange(int min = 0, int max = int.MaxValue) {
      return WithRange(new ConstValue<(int min, int max)>((min, max)));
   }

   public IUiInput<T> WithCustomCondition(Func<T, bool> conditionCallback);
   public IUiInput<T> Ref(in ValueRef<T> valueRef);
}