using FlashBrowser.Core.Pipelines.Context;
using FlashBrowser.Services.Browser;
using FlashBrowser.UI.Builders;
using FlashBrowser.UI.Styles;
using FlashBrowser.UI.Values;

using PupaLib.Core;

using PuppeteerSharp;

namespace FlashBrowser.Core.BrowserStrategy;

public sealed class SelectBrowserStartegy(IUiBuilder ui) : ICoreStrategy {
   public string Title => "Select browser for work";

   public async Task<Option> Execute(IPipelineContext context, CancellationToken cancellationToken = default) {
      var loadedBrowsers = context.BrowserService.BrowserApps
         .Select(x => (e: x.Key, name: context.BrowserService.GetNameFromEnum(x.Key))).ToArray();
      var loadedBrowserNames = loadedBrowsers.Select(x => x.name).ToArray();

      if (loadedBrowsers.Length == 1) {
         SelectBrowser(context.BrowserService, loadedBrowsers.First().e);
         return Option.Ok();
      }

      var selectBrowseRef = new ValueRef<(int index, string name)>();

      while (cancellationToken.IsCancellationRequested == false) {
         ui.WithChain(chain => chain
            .WithChoice(choice => choice
               .WithChoices(loadedBrowserNames)
               .WithPrompt("Select browser for work")
               .Ref(selectBrowseRef)));
         if (await ui.RunAndClear(cancellationToken) == false)
            break;

         SelectBrowser(context.BrowserService, loadedBrowsers[selectBrowseRef.Get().index].e);
         return Option.Ok();
      }

      return Option.Fail();
   }

   private void SelectBrowser(BrowserService browserService, SupportedBrowser browser) {
      browserService.SetActiveBrowser(browser);
      ui.Message($"Active browser {browser} selected");
   }
}