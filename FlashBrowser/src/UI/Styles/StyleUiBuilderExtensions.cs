using FlashBrowser.UI.Builders;
using FlashBrowser.UI.Values;

namespace FlashBrowser.UI.Styles;

public static class StyleUiBuilderExtensions {
   public static async Task Message(
      this IUiBuilder ui,
      IValue<string> value,
      CancellationToken cancellationToken = default) {
      ui
         .WithChain(chain => chain
            .WithMessage(message => message
               .WithText(value)));
      await ui.RunAndClear(cancellationToken);
   }

   public static Task Message(
      this IUiBuilder ui,
      string value,
      CancellationToken cancellationToken = default) {
      return ui.Message(new ConstValue<string>(value), cancellationToken);
   }
}