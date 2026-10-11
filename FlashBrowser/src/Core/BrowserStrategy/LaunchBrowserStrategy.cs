using FlashBrowser.Core.Pipelines.Context;
using FlashBrowser.Services.Browser;
using FlashBrowser.UI.Builders;
using FlashBrowser.UI.Styles;

using PupaLib.Core;

namespace FlashBrowser.Core.BrowserStrategy;

public sealed class LaunchBrowserStrategy(IUiBuilder ui, BrowserService browserService) : ICoreStrategy {
   public string Title => $"Launch selected browser <{browserService.ActiveBrowser}>";
   // ui -> uiBuilder

   public async Task<Option> Execute(IPipelineContext context, CancellationToken cancellationToken = default) {
      if (context.BrowserService.Browser == null) {
         var activeBrowser = context.BrowserService.ActiveBrowser;
         try {
            await context.BrowserService.LaunchBrowser();
            await ui.Message(
               $"Launch browser <{activeBrowser}>, use '{context.BrowserService.BrowserApps[activeBrowser]}'",
               cancellationToken);
            return Option.Ok();
         } catch (Exception e) {
            await ui.Message($"Failed launch browser <{activeBrowser}> exception: {e.Message}", cancellationToken);
            return Option.Fail();
         }
      }

      return Option.Fail();
   }
}