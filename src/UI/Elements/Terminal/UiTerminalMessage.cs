using FlashBrowser.UI.Elements.Base;
using FlashBrowser.UI.Values;
using FlashBrowser.Utils;

using PupaLib.Core;

namespace FlashBrowser.UI.Elements.Terminal;

public sealed class UiTerminalMessage : UiElement, IUiMessage {
   private IValue<string> _authorValue = new ConstValue<string>(string.Empty);
   private IValue<Color> _bgColorValue = new ConstValue<Color>(Color.Black);
   private IValue<Color> _fgColorValue = new ConstValue<Color>(Color.White);
   private IValue<string> _textValue = new ConstValue<string>(string.Empty);

   public override Task<Option> Run(CancellationToken cancellationToken) {
      var prefix = "";
      if (string.IsNullOrWhiteSpace(_authorValue.Get()) == false) prefix = $"<{_authorValue.Get()}> ";
      Console.Out.Write(
         Color.ColorizeFgBgX1B($"{prefix}{_textValue.Get()}\n", _fgColorValue.Get(), _bgColorValue.Get()));
      return Task.FromResult(Option.Ok());
   }

   public IUiMessage WithText(IValue<string> value) {
      _textValue = value;
      return this;
   }

   public IUiMessage WithAuthor(IValue<string> value) {
      _authorValue = value;
      return this;
   }

   public IUiMessage WithColorFg(IValue<Color> value) {
      _fgColorValue = value;
      return this;
   }

   public IUiMessage WithColorBg(IValue<Color> value) {
      _bgColorValue = value;
      return this;
   }
}