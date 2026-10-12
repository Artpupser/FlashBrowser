using FlashBrowser.Services.Input;
using FlashBrowser.UI.Elements.Base;
using FlashBrowser.UI.Values;

using PupaLib.Core;

namespace FlashBrowser.UI.Elements.Terminal;

public sealed class UiTerminalVerify(
   IKeyboardInputService keyboardInputService) : UiElement, IUiVerify {
   private IValue<KeyboardKey> _keyNValue = new ConstValue<KeyboardKey>(KeyboardKey.FromConsole(ConsoleKey.N));
   private IValue<KeyboardKey> _keyYValue = new ConstValue<KeyboardKey>(KeyboardKey.FromConsole(ConsoleKey.Y));
   private IValue<string> _nValue = new ConstValue<string>("n");
   private IValue<string> _promptValue = new ConstValue<string>(string.Empty);
   private ValueRef<bool>? _valueRef;
   private IValue<string> _yValue = new ConstValue<string>("y");

   public override async Task<Option> Run(CancellationToken cancellationToken) {
      while (cancellationToken.IsCancellationRequested == false) {
         var result = await Invoke(cancellationToken);
         Console.CursorVisible = true;
         if (result.Success) {
            _valueRef?.Set(result.Content);
            return Option.Ok();
         }

         break;
      }

      return Option.Fail();
   }


   public IUiVerify WithY(IValue<string> value) {
      _yValue = value;
      return this;
   }

   public IUiVerify WithN(IValue<string> value) {
      _nValue = value;
      return this;
   }

   public IUiVerify WithPrompt(IValue<string> value) {
      _promptValue = value;
      return this;
   }

   public IUiVerify WithYKey(IValue<KeyboardKey> key) {
      _keyYValue = key;
      return this;
   }

   public IUiVerify WithNKey(IValue<KeyboardKey> key) {
      _keyNValue = key;
      return this;
   }

   public IUiVerify Ref(in ValueRef<bool> valueRef) {
      _valueRef = valueRef;
      return this;
   }

   private async Task<Option<bool>> Invoke(CancellationToken cancellationToken) {
      Console.Out.Write($"[< {_promptValue.Get()} >] {_yValue.Get()}/{_nValue.Get()}");
      Console.Out.Write('\n');
      while (cancellationToken.IsCancellationRequested == false) {
         Console.CursorVisible = false;
         var key = await keyboardInputService.GetKeyboardKey();
         if (key.KeyCode == _keyYValue.Get().KeyCode) // Y 89 
            return Option<bool>.Ok(true);
         if (key.KeyCode == _keyNValue.Get().KeyCode) // N 78
            return Option<bool>.Ok(false);
      }

      return Option<bool>.Fail();
   }
}