using FlashBrowser.UI.Elements.Base;

using Microsoft.Extensions.DependencyInjection;

namespace FlashBrowser.UI.Engines;

public interface IUiEngine {
   public IUiMessage CreateMessage(ServiceProvider serviceProvider);
   public IUiInput<T> CreateInput<T>(ServiceProvider serviceProvider);
   public IUiVerify CreateVerify(ServiceProvider serviceProvider);
   public IUiChoice CreateChoice(ServiceProvider serviceProvider);
}