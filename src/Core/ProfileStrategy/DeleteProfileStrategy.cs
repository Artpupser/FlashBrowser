using FlashBrowser.Core.ProfileStrategy.Context;
using FlashBrowser.Services.Browser;
using FlashBrowser.UI.Builders;
using FlashBrowser.UI.Styles;
using FlashBrowser.UI.Values;

using PupaLib.Core;

namespace FlashBrowser.Core.ProfileStrategy;

public sealed class DeleteProfileStrategy(IUiBuilder ui)
   : IProfileStrategy<LaunchProfile>, IProfileStrategy<AppProfile>, IRequiredProfileStrategy {
   public Task<Option> Execute(IProfileContext<AppProfile> context, CancellationToken cancellationToken) {
      return Invoke(context, x => x.Name, cancellationToken);
   }

   public string Title => "Delete profile";

   public Task<Option> Execute(IProfileContext<LaunchProfile> context, CancellationToken cancellationToken) {
      return Invoke(context, x => x.Name, cancellationToken);
   }

   private async Task<Option> Invoke<T>(IProfileContext<T> context, Func<T, string> nameGetter,
      CancellationToken cancellationToken) {
      if (context.SelectedProfile == null) {
         await ui.Message("Profile not selected, retry", cancellationToken);
         return Option.Fail();
      }

      var selectedProfile = context.SelectedProfile!;
      while (cancellationToken.IsCancellationRequested == false) {
         var verifyRef = new ValueRef<bool>();
         ui.WithChain(chain => chain
            .WithVerify(verify => verify
               .WithPrompt($"Do you want delete '{nameGetter.Invoke(context.SelectedProfile)}'?")
               .Ref(verifyRef)));
         if (await ui.RunAndClear(cancellationToken) == false)
            break;

         if (verifyRef.Get()) {
            context.Profiles.Remove(selectedProfile);
            await ui.Message($"Profile with name '{nameGetter.Invoke(context.SelectedProfile)}' removed",
               cancellationToken);
            return Option.Ok();
         }
      }

      return Option.Fail();
   }
}