using FlashBrowser.UI.Elements.Base;
using FlashBrowser.UI.Elements.Terminal;

using Microsoft.Extensions.DependencyInjection;

namespace FlashBrowser.UI.Engines;

public sealed class UiTerminalEngine : IUiEngine {
   public IUiMessage CreateMessage(ServiceProvider serviceProvider) {
      return ActivatorUtilities.CreateInstance<UiTerminalMessage>(serviceProvider);
   }

   public IUiInput<T> CreateInput<T>(ServiceProvider serviceProvider) {
      return ActivatorUtilities.CreateInstance<UiTerminalInput<T>>(serviceProvider);
   }

   public IUiVerify CreateVerify(ServiceProvider serviceProvider) {
      return ActivatorUtilities.CreateInstance<UiTerminalVerify>(serviceProvider);
   }

   public IUiChoice CreateChoice(ServiceProvider serviceProvider) {
      return ActivatorUtilities.CreateInstance<UiTerminalChoice>(serviceProvider);
   }
}