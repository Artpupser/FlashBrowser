using FlashBrowser.Core.BrowserStrategy;
using FlashBrowser.Services.Browser;

namespace FlashBrowser.Core.Factory;

public class BrowserCommandsFactory(BrowserService browserService, IEnumerable<ICoreStrategy> strategies)
   : IStrategyFactory<ICoreStrategy> {
   public IEnumerable<ICoreStrategy> Create() {
      return strategies
         .Where(x => !(x is ClearBrowserCookiesStrategy && browserService.OpenedPages.Count <= 0))
         .Where(x => !(x is CloseAllBrowserTabsStrategy && browserService.OpenedPages.Count <= 0))
         .Where(x => !(x is OpenLaunchProfileInBrowserStrategy && browserService.Browser == null))
         .Where(x => !(x is LaunchBrowserStrategy && browserService.Browser != null))
         .Where(x => !(x is UnlaunchBrowserStrategy && browserService.Browser == null))
         .Where(x => !(x is KillBrowserStrategy && browserService.Browser != null));
   }
}