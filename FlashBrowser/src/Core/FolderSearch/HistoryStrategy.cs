using FlashBrowser.Core.Pipelines.Context;
using FlashBrowser.UI.Builders;
using FlashBrowser.UI.Styles;
using FlashBrowser.UI.Values;

using PupaLib.Core;
using PupaLib.FileIO;

namespace FlashBrowser.Core.FolderSearch;

public sealed class HistoryStrategy(string path, IUiBuilder ui) : IFolderSearchStrategy {
   public string Title => "History";

   public async Task<Option<VirtualFolder>> Execute(IPipelineContext context, CancellationToken cancellationToken) {
      var localConfig = context.DataService.ApplicationData!;
      var history = localConfig.LastBrowserFoldersHistory;
      if (VirtualFolder.GetFolder(path).Out(out var browserFolder)) return Option<VirtualFolder>.Ok(browserFolder);

      await ui.Message($"Folder with path '{path}' undefined, in any drive", cancellationToken);
      var acceptRef = new ValueRef<bool>();
      ui
         .WithChain(chain => chain
            .WithVerify(verify => verify
               .WithPrompt($"Remove path from history? '{path}'")
               .Ref(acceptRef)));
      await ui.RunAndClear(cancellationToken);
      if (acceptRef.Get()) {
         history.Remove(browserFolder.MyPath);
         localConfig.LastBrowserFoldersHistory = history;
      }

      return Option<VirtualFolder>.Fail();
   }
}