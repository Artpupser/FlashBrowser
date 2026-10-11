using FlashBrowser.Core.Pipelines.Context;
using FlashBrowser.Extensions;
using FlashBrowser.UI.Builders;
using FlashBrowser.UI.Styles;
using FlashBrowser.UI.Values;

using PupaLib.Core;

namespace FlashBrowser.Core.BrowserStrategy;

public sealed class ClearBrowserCookiesStrategy(IUiBuilder ui) : ICoreStrategy {
   public string Title => "Clear cookies";

   public async Task<Option> Execute(IPipelineContext context, CancellationToken cancellationToken = default) {
      while (cancellationToken.IsCancellationRequested == false) {
         var openedPages = context.BrowserService.OpenedPages.Where(x => x.IsClosed == false).ToArray();
         string[] openedPagesNames = [.. openedPages.Select(x => new Uri(x.Url).Host), "(danger) all"];
         var siteRef = new ValueRef<(int index, string name)>();
         ui.WithChain(chain => chain
            .WithChoice(choice => choice
               .WithChoices(openedPagesNames)
               .WithPrompt("Select a site")
               .Ref(siteRef)));

         if (await ui.RunAndClear(cancellationToken) && siteRef.Get().index > -1) {
            var all = siteRef.Get().index == openedPagesNames.Length - 1;
            var verifyMessage = new ValueRef<string>(all
               ? $"Do you want clear all cookies, from browser {context.BrowserService.ActiveBrowser.GetLowerName()}"
               : $"Do you want clear cookies, host '{siteRef.Get().name}'");
            var verifyRef = new ValueRef<bool>();
            ui.WithChain(chain => chain
               .WithVerify(verify => verify
                  .WithPrompt(verifyMessage)
                  .Ref(verifyRef)));
            if (await ui.RunAndClear(cancellationToken) && verifyRef.Get()) {
               var pages = all ? openedPages : [openedPages[siteRef.Get().index]];
               foreach (var page in pages) {
                  var cookies = await page.GetCookiesAsync();
                  foreach (var cookie in cookies)
                     await ui.Message($"Find cookie:  {cookie.Domain} {cookie.Name} {cookie.Expires}",
                        cancellationToken);
                  await page.DeleteCookieAsync(cookies);
                  await ui.Message($"Removing all cookies, host '{siteRef.Get().name}'", cancellationToken);
               }
            }

            continue;
         }

         break;
      }

      return Option.Fail();
   }
}