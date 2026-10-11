using FlashBrowser.Core.Pipelines.Context;
using FlashBrowser.Services.Browser;
using FlashBrowser.UI.Builders;
using FlashBrowser.UI.Styles;
using FlashBrowser.UI.Values;

using PupaLib.Core;

namespace FlashBrowser.Core.BrowserStrategy;

public sealed class CloseAllBrowserTabsStrategy(IUiBuilder ui, BrowserService browserService) : ICoreStrategy {
   public string Title => $"Close all pages [{browserService.OpenedPages.Count}]";

   public async Task<Option> Execute(IPipelineContext context, CancellationToken cancellationToken = default) {
      while (cancellationToken.IsCancellationRequested == false) {
         var acceptRef = new ValueRef<bool>();
         ui.WithChain(chain => chain
            .WithVerify(verify => verify
               .WithPrompt("Close all pages?")
               .Ref(acceptRef)));
         if (await ui.RunAndClear(cancellationToken) && acceptRef.Get())
            await browserService.CloseAllPages(p => { ui.Message($"Page '{p.Url}' closed", cancellationToken); });
         break;
      }

      return Option.Fail();
   }
}