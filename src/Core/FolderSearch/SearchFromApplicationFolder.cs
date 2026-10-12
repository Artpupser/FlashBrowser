using FlashBrowser.Core.Pipelines.Context;
using FlashBrowser.UI.Builders;
using FlashBrowser.UI.Styles;

using PupaLib.Core;
using PupaLib.FileIO;

namespace FlashBrowser.Core.FolderSearch;

public sealed class SearchFromApplicationFolder(IUiBuilder ui) : IFolderSearchStrategy {
   public string Title => "Auto search from application folder";

   public async Task<Option<VirtualFolder>> Execute(IPipelineContext context, CancellationToken cancellationToken) {
      if (VirtualIo.RootFolder.GetOrCreateFolderIn("browser_flash").Out(out var browserFlashFolder))
         return Option<VirtualFolder>.Ok(browserFlashFolder);

      await ui.Message($"Folder with name 'browser_flash' undefined, in {VirtualIo.RootFolder.MyPath}",
         cancellationToken);
      return Option<VirtualFolder>.Fail();
   }
}