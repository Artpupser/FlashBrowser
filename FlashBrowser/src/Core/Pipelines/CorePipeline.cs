using FlashBrowser.Core.Factory;
using FlashBrowser.Core.Pipelines.Context;
using FlashBrowser.UI.Builders;
using FlashBrowser.UI.Styles;
using FlashBrowser.UI.Values;

using PupaLib.Core;

namespace FlashBrowser.Core.Pipelines;

public sealed class CorePipeline(
   IUiBuilder ui,
   IStrategyFactory<ICoreStrategy> commandsFactory) {
   public async Task<Option> Execute(IPipelineContext context, CancellationToken cancellationToken = default) {
      const byte maxExitAttempts = 3;
      var exitAttempts = maxExitAttempts;
      while (cancellationToken.IsCancellationRequested == false) {
         var strategiesArr = commandsFactory.Create().ToArray();
         var strategiesNames = strategiesArr.Select(x => x.Title).ToArray();
         var actionRef = new ValueRef<(int index, string name)>();
         ui.WithChain(chain => chain
            .WithChoice(choice => choice
               .WithChoices(strategiesNames)
               .WithPrompt("Select action")
               .Ref(actionRef)));
         if (await ui.RunAndClear(cancellationToken) && actionRef.Get().index > -1) {
            var strategy = strategiesArr[actionRef.Get().index];
            exitAttempts = maxExitAttempts;
            await strategy.Execute(context, cancellationToken);
         } else {
            exitAttempts -= 1;
            if (exitAttempts == 0)
               return Option.Ok();
            await ui.Message($"If you want exit, press ESC <{exitAttempts}/{maxExitAttempts}> once", cancellationToken);
         }
      }

      return Option.Fail();
   }
}