using FlashBrowser.Core.FolderSearch;
using FlashBrowser.Core.Pipelines.Context;
using FlashBrowser.UI.Builders;
using FlashBrowser.UI.Values;

using PupaLib.Core;

namespace FlashBrowser.Core.Startup;

public sealed class SelectBrowserFolderStrategy(
   IUiBuilder ui,
   IEnumerable<IFolderSearchStrategy> folderSearchStrategies) : IStartupStrategy {
   public string Title => "Select browser folder";

   public async Task<Option> Execute(IPipelineContext context, CancellationToken cancellationToken) {
      var localConfig = context.DataService.ApplicationData!;
      var searchStrategyList = folderSearchStrategies.ToList();
      var success = false;
      while (cancellationToken.IsCancellationRequested == false) {
         var choices = searchStrategyList.Select(x => x.Title).Concat(localConfig.LastBrowserFoldersHistory).ToArray();
         var searchChoiceRef = new ValueRef<(int index, string name)>();
         ui.WithChain(chain => chain
            .WithChoice(choice => choice
               .WithChoices(choices)
               .WithPrompt("Search variant")
               .Ref(searchChoiceRef)));

         await ui.RunAndClear(cancellationToken);
         var searchIndex = searchChoiceRef.Get().index;
         var strategy = searchIndex < searchStrategyList.Count
            ? searchStrategyList[searchIndex]
            : new HistoryStrategy(localConfig.LastBrowserFoldersHistory[searchIndex - searchStrategyList.Count], ui);

         var searchOption = await strategy.Execute(context, cancellationToken);
         if (searchOption.Out(out var folder)) {
            success = true;
            context.BrowserService.BrowserEnvironmentFolder = folder;
            Remember(context);
         }

         await context.DataService.Save(cancellationToken);
         if (success)
            return Option.Ok();
      }

      return Option.Fail();
   }

   private static void Remember(IPipelineContext context) {
      var folder = context.BrowserService.BrowserEnvironmentFolder;
      var localConfig = context.DataService.ApplicationData!;
      var h = localConfig.LastBrowserFoldersHistory;
      h.Remove(folder.MyPath);
      h.Add(folder.MyPath);
      if (h.Count > 20)
         h.RemoveAt(0);
      localConfig.LastBrowserFoldersHistory = h;
   }
}