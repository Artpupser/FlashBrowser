using FlashBrowser.Core.Pipelines.Context;
using FlashBrowser.Services.Browser;
using FlashBrowser.UI.Builders;
using FlashBrowser.UI.Styles;

using PupaLib.Core;

namespace FlashBrowser.Core.BrowserStrategy;

public sealed class UnlaunchBrowserStrategy(IUiBuilder ui, BrowserService browserService) : ICoreStrategy {
   public string Title =>
      $"Deactivate browser <{browserService.ActiveBrowser}> process {browserService.Browser?.Process.Id}";

   public async Task<Option> Execute(IPipelineContext context, CancellationToken cancellationToken = default) {
      if (context.BrowserService.Browser != null)
         try {
            await ui.Message($"Deactivate browser <{context.BrowserService.ActiveBrowser}>...", cancellationToken);
            await browserService.DeactivateBrowser();
            return Option.Ok();
         } catch (Exception e) {
            await ui.Message(
               $"Failed deactivate browser <{context.BrowserService.ActiveBrowser}> exception: {e.Message}",
               cancellationToken);
            return Option.Fail();
         }

      return Option.Fail();
   }
}