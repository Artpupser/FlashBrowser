using FlashBrowser.Services.Browser;

namespace FlashBrowser.Core.Factory;

public class AppProfileFactory(BrowserService browserService, IEnumerable<IProfileStrategy<AppProfile>> strategies)
   : IStrategyFactory<IProfileStrategy<AppProfile>> {
   public IEnumerable<IProfileStrategy<AppProfile>> Create() {
      return strategies;
   }
}