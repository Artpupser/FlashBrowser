using System.Text;

using FlashBrowser.Services.Input;
using FlashBrowser.UI.Elements.Base;
using FlashBrowser.UI.Values;
using FlashBrowser.Utils;

using PupaLib.Core;

namespace FlashBrowser.UI.Elements.Terminal;

public sealed class UiTerminalChoice(IKeyboardInputService keyboardInputService) : UiElement, IUiChoice {
   private readonly List<IValue<string>> _itemsValues = [];
   private readonly StringBuilder _sb = new();
   private readonly string EmptyLine = new(' ', Console.WindowWidth);
   private IValue<Color> _noSelectBg = new ConstValue<Color>(Color.Black);
   private IValue<Color> _noSelectFg = new ConstValue<Color>(Color.White);
   private IValue<string> _promptValue = new ConstValue<string>(string.Empty);
   private IValue<Color> _selectBg = new ConstValue<Color>(Color.White);
   private IValue<Color> _selectFg = new ConstValue<Color>(Color.Black);
   private ValueRef<(int, string)>? _valueRef;

   public override async Task<Option> Run(CancellationToken cancellationToken) {
      while (cancellationToken.IsCancellationRequested == false) {
         var result = await Invoke(cancellationToken);
         Console.CursorVisible = true;
         if (result.Success == false)
            break;
         _valueRef?.Set(result.Content);
         return Option.Ok();
      }

      return Option.Fail();
   }

   public IUiChoice WithPrompt(IValue<string> value) {
      _promptValue = value;
      return this;
   }

   public IUiChoice WithChoice(IValue<string> value) {
      _itemsValues.Add(value);
      return this;
   }

   public IUiChoice WithChoices(params IValue<string>[] value) {
      _itemsValues.AddRange(value);
      return this;
   }

   public IUiChoice WithSelectFgColor(IValue<Color> color) {
      _selectFg = color;
      return this;
   }

   public IUiChoice WithSelectBgColor(IValue<Color> color) {
      _selectBg = color;
      return this;
   }

   public IUiChoice WithNoSelectFgColor(IValue<Color> color) {
      _noSelectFg = color;
      return this;
   }

   public IUiChoice WithNoSelectBgColor(IValue<Color> color) {
      _noSelectBg = color;
      return this;
   }

   public IUiChoice Ref(in ValueRef<(int index, string choice)> valueRef) {
      _valueRef = valueRef;
      return this;
   }

   private void ClearLines(int amount) {
      for (var i = 0; i <= amount + 1; i++)
         _sb.AppendLine(EmptyLine);
      Console.Write(_sb.ToString());
      _sb.Clear();
   }

   private async Task<Option<(int, string)>> Invoke(CancellationToken cancellationToken) {
      var index = 0;
      ClearLines(_itemsValues.Count);
      Console.CursorVisible = false;
      var startCursor = (left: 0, top: Console.CursorTop);
      while (cancellationToken.IsCancellationRequested == false) {
         Console.CursorTop = startCursor.top - _itemsValues.Count - 1;
         _sb.AppendLine(Color.ColorizeFgBgX1B($"<< {_promptValue.Get()} >>", _noSelectFg.Get(), _noSelectBg.Get()));
         for (var i = 0; i < _itemsValues.Count; i++) {
            var item = _itemsValues[i];
            var value = item.Get();
            if (i == index) {
               _sb.AppendLine(Color.ColorizeFgBgX1B($"-> {value}", _selectFg.Get(), _selectBg.Get()));
               continue;
            }

            _sb.AppendLine(Color.ColorizeFgBgX1B($"   {value}", _noSelectFg.Get(), _noSelectBg.Get()));
         }

         Console.Write(_sb.ToString());
         _sb.Clear();
         //Console.SetCursorPosition(0, startCursor.top);
         var key = await keyboardInputService.GetKeyboardKey();
         if (IsKeyUp(key))
            index = MyMath.Repeat(index - 1, 0, _itemsValues.Count - 1);
         else if (IsKeyDown(key))
            index = MyMath.Repeat(index + 1, 0, _itemsValues.Count - 1);
         else if (key.IsEnter())
            return Option<(int, string)>.Ok((index, _itemsValues[index].Get()));
         else if (key.IsEsc() || key.IsBackspace()) return Option<(int, string)>.Ok((-1, string.Empty));
      }

      return Option<(int, string)>.Fail();
   }

   private static bool IsKeyUp(KeyboardKey key) {
      return key.KeyCode is 38 or 87 or 33; // ArrowUp 38, W 87, PageUp 33;
   }

   private static bool IsKeyDown(KeyboardKey key) {
      return key.KeyCode is 40 or 83 or 34; // ArrowDown 40, S 83, PageDown 34;
   }
}