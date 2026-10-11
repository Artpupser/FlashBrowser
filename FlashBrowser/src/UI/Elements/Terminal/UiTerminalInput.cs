using System.Text;

using FlashBrowser.Services.Input;
using FlashBrowser.UI.Elements.Base;
using FlashBrowser.UI.Values;

using PupaLib.Core;

namespace FlashBrowser.UI.Elements.Terminal;

public sealed class UiTerminalInput<T>(
   IKeyboardInputService keyboardInputService) :
   UiElement, IUiInput<T> {
   private readonly List<Func<T, bool>> _conditionCallbacks = [];
   private IValue<string> _promptValue = new ConstValue<string>(string.Empty);
   private IValue<(int min, int max)> _rangeValue = new ConstValue<(int min, int max)>((0, int.MaxValue));
   private IValue<char>? _secretSymbolValue;
   private ValueRef<T>? _valueRef;

   public override async Task<Option> Run(CancellationToken cancellationToken) {
      while (cancellationToken.IsCancellationRequested == false) {
         var resultOption = await Invoke(cancellationToken);
         Console.CursorVisible = false;
         if (resultOption.Out(out var result) == false) return Option.Fail();

         if (_conditionCallbacks.Count == 0 || _conditionCallbacks.All(x => x.Invoke(result))) {
            _valueRef?.Set(result);
            return Option.Ok();
         }
      }

      return Option.Ok();
   }

   public IUiInput<T> Ref(in ValueRef<T> valueRef) {
      _valueRef = valueRef;
      return this;
   }

   public IUiInput<T> WithPrompt(IValue<string> value) {
      _promptValue = value;
      return this;
   }

   public IUiInput<T> WithRange(IValue<(int min, int max)> value) {
      if (value.Get().min < 0)
         throw new Exception("Min < 0");
      _rangeValue = value;
      return this;
   }

   public IUiInput<T> WithSecret(IValue<char> value) {
      _secretSymbolValue = value;
      return this;
   }

   public IUiInput<T> WithCustomCondition(Func<T, bool> conditionCallback) {
      _conditionCallbacks.Add(conditionCallback);
      return this;
   }

   private async Task<Option<T>> Invoke(CancellationToken cancellationToken) {
      var sb = new StringBuilder();
      sb.Clear();
      Console.CursorVisible = true;
      Console.Write($"[{_promptValue.Get()}] > ");
      while (cancellationToken.IsCancellationRequested == false) {
         var cursor = Console.GetCursorPosition();
         var keyboardKey = await keyboardInputService.GetKeyboardKey();
         if (keyboardKey.IsEnter()) //Enter
         {
            if (sb.Length <= _rangeValue.Get().min)
               continue;
            try {
               var result = (T)Convert.ChangeType(sb.ToString(), typeof(T));
               Console.Write('\n');
               return Option<T>.Ok(result);
            } catch {
               continue;
            }
         }

         if (keyboardKey.IsEsc()) //Escape
         {
            Console.Write('\n');
            break;
         }

         if (keyboardKey.IsBackspace()) //Backspace
         {
            if (sb.Length < 1)
               continue;
            sb.Remove(sb.Length - 1, 1);
            Console.SetCursorPosition(cursor.Left - 1, cursor.Top);
            Console.Write(' ');
            Console.SetCursorPosition(cursor.Left - 1, cursor.Top);
            continue;
         }

         if (sb.Length >= _rangeValue.Get().max) continue;
         sb.Append(keyboardKey.Symbol);
         if (_secretSymbolValue != null) {
            Console.Write(_secretSymbolValue.Get());
            continue;
         }

         Console.Write(keyboardKey.Symbol);
      }

      return Option<T>.Fail();
   }
}