using FlashBrowser.Core.Factory;
using FlashBrowser.Core.ProfileStrategy.Context;
using FlashBrowser.Services.Browser;
using FlashBrowser.UI.Builders;
using FlashBrowser.UI.Values;

using PupaLib.Core;

namespace FlashBrowser.Core.ProfileStrategy;

public sealed class EditAppProfileStrategy(
   IUiBuilder ui,
   IStrategyFactory<IProfileStrategy<AppProfile>> appProfileFactory)
   : IProfileStrategy<LaunchProfile>, IChangeProfileStrategy, IRequiredProfileStrategy {
   public string Title => "Edit applications profile";

   public async Task<Option> Execute(IProfileContext<LaunchProfile> context, CancellationToken cancellationToken) {
      var profileStrategies = appProfileFactory.Create().ToArray();
      var profileStrategiesNames = profileStrategies.Select(x => x.Title).ToArray();
      var profileStrategyRef = new ValueRef<(int index, string name)>();
      var appProfileContext = new AppProfileContext(context.SelectedProfile!.AppProfiles);
      while (cancellationToken.IsCancellationRequested == false) {
         ui.WithChain(chain => chain
            .WithChoice(choice => choice
               .WithChoices(profileStrategiesNames)
               .WithPrompt("Select action for app profile")
               .Ref(profileStrategyRef)));

         if (await ui.RunAndClear(cancellationToken) && profileStrategyRef.Get().index > -1) {
            var profileStrategy = profileStrategies[profileStrategyRef.Get().index];
            if (profileStrategy is IRequiredProfileStrategy)
               if (await profileStrategies.First(x => x is SelectProfileStrategy)
                      .Execute(appProfileContext, cancellationToken) == false)
                  continue;

            await profileStrategy.Execute(appProfileContext, cancellationToken);
            continue;
         }

         break;
      }

      return Option.Fail();
   }
}