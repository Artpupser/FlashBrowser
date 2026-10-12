using FlashBrowser.Core.Killers;
using FlashBrowser.Core.Pipelines.Context;
using FlashBrowser.Extensions;
using FlashBrowser.Services.Browser;
using FlashBrowser.UI.Builders;
using FlashBrowser.UI.Styles;

using PupaLib.Core;

namespace FlashBrowser.Core.BrowserStrategy;

public sealed class KillBrowserStrategy(IUiBuilder ui, BrowserService browserService, IProcessKiller processKiller)
   : ICoreStrategy {
   public string Title => $"Kill all browser <{browserService.ActiveBrowser}> process";

   public async Task<Option> Execute(IPipelineContext context, CancellationToken cancellationToken = default) {
      foreach (var killResult in processKiller.KillByPrefix(browserService.ActiveBrowser.GetLowerName())) {
         if (killResult.IsSuccess) {
            await ui.Message($"Kill process '{killResult.Name}', {killResult.Id}", cancellationToken);
            continue;
         }

         await ui.Message($"Ignore killing process with name '{killResult.Name}', {killResult.Error!.Message}",
            cancellationToken);
      }

      return Option.Ok();
   }
}