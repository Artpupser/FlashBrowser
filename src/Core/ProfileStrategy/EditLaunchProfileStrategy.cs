using FlashBrowser.Core.Factory;
using FlashBrowser.Core.Pipelines.Context;
using FlashBrowser.Core.ProfileStrategy.Context;
using FlashBrowser.Services.Browser;
using FlashBrowser.UI.Builders;
using FlashBrowser.UI.Values;

using PupaLib.Core;

namespace FlashBrowser.Core.ProfileStrategy;

public sealed class EditLaunchProfileStrategy(
   IUiBuilder ui,
   IStrategyFactory<IProfileStrategy<LaunchProfile>> launchProfilesFactory) : ICoreStrategy {
   public string Title => "Edit profiles";

   public async Task<Option> Execute(IPipelineContext context, CancellationToken cancellationToken = default) {
      var profileStrategies = launchProfilesFactory.Create().Where(x => x is not IChangeProfileStrategy).ToArray();
      var profileStrategiesNames = profileStrategies.Select(x => x.Title).ToArray();
      var profileActionRef = new ValueRef<(int index, string name)>();
      var profileContext = new LaunchProfileContext(context.DataService.ApplicationData!.Profiles);
      while (cancellationToken.IsCancellationRequested == false) {
         ui.WithChain(chain => chain
            .WithChoice(choice => choice
               .WithChoices(profileStrategiesNames)
               .WithPrompt("Select action for profile")
               .Ref(profileActionRef)));
         if (await ui.RunAndClear(cancellationToken) && profileActionRef.Get().index > -1) {
            var profileStrategy = profileStrategies[profileActionRef.Get().index];
            if (profileStrategy is IRequiredProfileStrategy)
               if (await profileStrategies.First(x => x is SelectProfileStrategy)
                      .Execute(profileContext, cancellationToken) == false)
                  continue;

            await profileStrategy.Execute(profileContext, cancellationToken);
            await ApplyAndSave(context, profileContext, cancellationToken);
            continue;
         }

         break;
      }

      return Option.Fail();
   }

   private static async Task ApplyAndSave(IPipelineContext context, LaunchProfileContext launchProfileContext,
      CancellationToken cancellationToken = default) {
      context.DataService.ApplicationData!.Profiles = launchProfileContext.Profiles;
      await context.DataService.Save(cancellationToken);
   }
}