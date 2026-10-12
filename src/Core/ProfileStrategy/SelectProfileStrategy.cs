using FlashBrowser.Core.ProfileStrategy.Context;
using FlashBrowser.Services.Browser;
using FlashBrowser.UI.Builders;
using FlashBrowser.UI.Styles;
using FlashBrowser.UI.Values;

using PupaLib.Core;

namespace FlashBrowser.Core.ProfileStrategy;

public sealed class SelectProfileStrategy(IUiBuilder ui)
   : IProfileStrategy<LaunchProfile>, IProfileStrategy<AppProfile> {
   public Task<Option> Execute(IProfileContext<AppProfile> context, CancellationToken cancellationToken) {
      return Invoke(context, x => x.Name, cancellationToken);
   }

   public string Title => "Select profile";

   public Task<Option> Execute(IProfileContext<LaunchProfile> context, CancellationToken cancellationToken) {
      return Invoke(context, x => x.Name, cancellationToken);
   }

   private async Task<Option> Invoke<T>(
      IProfileContext<T> context,
      Func<T, string> nameFunc,
      CancellationToken cancellationToken = default!) {
      if (context.IsEmpty) {
         await ui.Message("Profiles is empty, please create a new profile", cancellationToken);
         return Option.Fail();
      }

      var profilesArr = context.Profiles.ToArray();
      var profilesNamesArr = context.Profiles.Select(nameFunc).ToArray();
      var selectProfileRef = new ValueRef<(int index, string name)>();
      while (cancellationToken.IsCancellationRequested == false) {
         ui.WithChain(chain => chain
            .WithChoice(choice => choice
               .WithChoices(profilesNamesArr)
               .WithPrompt("Select profile")
               .Ref(selectProfileRef)));
         if (await ui.RunAndClear(cancellationToken) && selectProfileRef.Get().index > -1)
            break;
         return Option.Fail();
      }

      context.SelectedProfile = profilesArr[selectProfileRef.Get().index];
      return Option.Ok();
   }
}