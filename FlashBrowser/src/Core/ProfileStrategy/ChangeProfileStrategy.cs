using FlashBrowser.Core.ProfileStrategy.Context;
using FlashBrowser.Services.Browser;
using FlashBrowser.UI.Builders;
using FlashBrowser.UI.Values;

using PupaLib.Core;

namespace FlashBrowser.Core.ProfileStrategy;

public sealed class
   ChangeProfileStrategy(
      IUiBuilder ui,
      IEnumerable<IChangeProfileStrategy> strategies
   ) : IProfileStrategy<LaunchProfile>, IRequiredProfileStrategy {
   public string Title => "Change profile";

   public async Task<Option> Execute(IProfileContext<LaunchProfile> context, CancellationToken cancellationToken) {
      var changeStrategyRef = new ValueRef<(int index, string name)>();
      var strategiesArr = strategies
         .Select(x => x as IProfileStrategy<LaunchProfile>)
         .Where(x => x != null)
         .ToArray();
      var strategiesNames = strategiesArr.Select(x => x.Title).ToArray();
      while (cancellationToken.IsCancellationRequested == false) {
         ui.WithChain(chain => chain
            .WithChoice(choice => choice
               .WithChoices(strategiesNames)
               .WithPrompt("Select change profile method")
               .Ref(changeStrategyRef)));
         if (await ui.RunAndClear(cancellationToken) && changeStrategyRef.Get().index > -1) {
            var changeStrategy = strategiesArr[changeStrategyRef.Get().index]!;
            await changeStrategy.Execute(context, cancellationToken);
            continue;
         }

         break;
      }

      return Option.Fail();
   }
}