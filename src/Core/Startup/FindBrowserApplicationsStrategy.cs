using FlashBrowser.Core.FileSearch;
using FlashBrowser.Core.Os;
using FlashBrowser.Core.Pipelines.Context;
using FlashBrowser.Extensions;
using FlashBrowser.Services.Browser;
using FlashBrowser.UI.Builders;
using FlashBrowser.UI.Styles;
using PupaLib.Core;
using PuppeteerSharp;
using System.Runtime.CompilerServices;

namespace FlashBrowser.Core.Startup;

public sealed class FindBrowserApplicationsStrategy(
   IUiBuilder ui,
   GetOsSupportedBrowsersStrategy getOsSupportedBrowsersStrategy,
   FindApplicationsOfNameStrategy findApplicationsOfNameStrategy) : IStartupStrategy {
   public string Title => "Find browser application";

   public async Task<Option> Execute(IPipelineContext context, CancellationToken cancellationToken) {
      await ui.Message("Search browser applications", cancellationToken);
      var currentOs = context.OperatingSystemService.CurrentOperatingSystem;
      while (cancellationToken.IsCancellationRequested == false &&
             getOsSupportedBrowsersStrategy.Execute().Out(out var supportedBrowsers)) {
         var osName = currentOs.GetLowerName();
         await ui.Message($"Set application path for '{osName}' os", cancellationToken);
         var arr = ApplySupportedBrowsers(context.BrowserService, supportedBrowsers, cancellationToken)
            .ToArray();
         if (arr.Any())
            return Option.Ok();
         break;
      }

      await ui.Message("You not have a web browser, download any supported browser correct work this application",
         cancellationToken);
      return Option.Fail();
   }

   private IEnumerable<Option> ApplySupportedBrowsers(BrowserService browserService,
      IEnumerable<SupportedBrowser> supportedBrowsers,
      [EnumeratorCancellation] CancellationToken cancellationToken) {
      foreach (var supportedBrowser in supportedBrowsers) {
         var browserName = supportedBrowser.GetLowerName();
         var collection = findApplicationsOfNameStrategy
            .Execute(browserName, cancellationToken)
            .ToArray();
         if (collection.Any()) 
            yield return ApplyApp(browserService, supportedBrowser, collection.First().MyPath);
         else
            ui.Message($"Application not found, with pattern '{browserName}'", cancellationToken);

      }
   }

   private Option ApplyApp(BrowserService browserService, SupportedBrowser browser, string path) {
      if (browserService.SetApplication(browser, path)) {
         ui.Message($"Apply new path <{path}> for browser '{browserService.GetNameFromEnum(browser)}'");
         return Option.Ok();
      }

      ui.Message($"Browser '{browserService.GetNameFromEnum(browser)}' undefined with path <{path}> ");
      return Option.Fail();
   }
}