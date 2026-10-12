using FlashBrowser.Core.Pipelines.Context;
using FlashBrowser.UI.Builders;
using FlashBrowser.UI.Styles;
using FlashBrowser.UI.Values;

using PupaLib.Core;
using PupaLib.FileIO;

namespace FlashBrowser.Core.FolderSearch;

public sealed class SearchFromCustomPath(IUiBuilder ui) : IFolderSearchStrategy {
   public string Title => "Search from custom path";

   public async Task<Option<VirtualFolder>> Execute(IPipelineContext context, CancellationToken cancellationToken) {
      var folderPathRef = new ValueRef<string>();

      ui
         .WithChain(chain => chain
            .WithInput<string>(input => input
               .WithPrompt("Type folder path")
               .WithRange(1, ushort.MaxValue)
               .Ref(folderPathRef)));

      if (await ui.RunAndClear(cancellationToken) == false)
         return Option<VirtualFolder>.Fail();

      if (VirtualFolder.GetFolder(folderPathRef.Get()).Out(out var browserFlashFolder))
         return Option<VirtualFolder>.Ok(browserFlashFolder);

      await ui.Message(new ConstValue<string>($"Folder with path '{folderPathRef.Get()}' undefined, in any drive"),
         cancellationToken);
      return Option<VirtualFolder>.Fail();
   }
}