using FlashBrowser.Core.Pipelines.Context;
using FlashBrowser.UI.Builders;
using FlashBrowser.UI.Styles;
using FlashBrowser.UI.Values;

using PupaLib.Core;

namespace FlashBrowser.Core.Startup;

public class InitBrowserEnvironmentStrategy(IUiBuilder ui) : IStartupStrategy {
   public string Title => "Initialization browser environment";

   public async Task<Option> Execute(IPipelineContext context, CancellationToken cancellationToken) {
      await ui.Message("Initialization or load browser environment", cancellationToken);
      while (cancellationToken.IsCancellationRequested == false) {
         var bad = false;
         context.BrowserService.ForEachSupportedBrowsers((supportedBrowser, i) => {
            var name = context.BrowserService.GetNameFromEnum(supportedBrowser);
            var folderOption = context.BrowserService.BrowserEnvironmentFolder.GetOrCreateFolderIn(name);
            if (folderOption.Out(out var folder)) {
               _ = ui.Message($"Load or create folder with name {name}, path {folder.MyPath}", cancellationToken);
               context.BrowserService.SetFolderProfile(supportedBrowser, folder);
               return;
            }

            bad = true;
            _ = ui.Message($"Bad create folder with name {name}", cancellationToken);
         });

         if (bad == false)
            break;

         var retryRef = new ValueRef<bool>();
         ui.WithChain(chain => chain
            .WithVerify(verify => verify
               .WithPrompt("You have bad attempts to create folder, retry?")
               .Ref(retryRef)));
         await ui.RunAndClear(cancellationToken);

         if (retryRef.Get())
            continue;
         break;
      }

      return Option.Ok();
   }
}