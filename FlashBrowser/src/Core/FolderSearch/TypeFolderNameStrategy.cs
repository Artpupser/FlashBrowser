using FlashBrowser.Core.Pipelines.Context;
using FlashBrowser.UI.Builders;
using FlashBrowser.UI.Values;

using PupaLib.Core;

namespace FlashBrowser.Core.FolderSearch;

public sealed class TypeFolderNameStrategy(IUiBuilder ui) : IExecuteStrategy<IPipelineContext, Option<string>> {
   public string Title => "Type folder name";

   public async Task<Option<string>> Execute(IPipelineContext context, CancellationToken cancellationToken) {
      var browserFolderName = new ValueRef<string>();
      ui
         .WithChain(chain => chain
            .WithInput<string>(input => input
               .WithPrompt(Title)
               .WithCustomCondition(x =>
                  x.EndsWith('\\') == false &&
                  x.EndsWith('/') == false &&
                  x.StartsWith('\\') == false &&
                  x.StartsWith('/') == false)
               .WithRange(1, ushort.MaxValue)
               .Ref(browserFolderName)));
      var result = await ui.RunAndClear(cancellationToken);
      return result.Success ? Option<string>.Ok(browserFolderName.Get()) : Option<string>.Fail();
   }
}