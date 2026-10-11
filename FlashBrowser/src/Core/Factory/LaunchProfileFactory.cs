using FlashBrowser.Services.Browser;

namespace FlashBrowser.Core.Factory;

public sealed class LaunchProfileFactory(
   BrowserService browserService,
   IEnumerable<IProfileStrategy<LaunchProfile>> strategies)
   : IStrategyFactory<IProfileStrategy<LaunchProfile>> {
   public IEnumerable<IProfileStrategy<LaunchProfile>> Create() {
      return strategies;
   }
}