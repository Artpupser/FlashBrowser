using FlashBrowser.Core.ProfileStrategy.Context;
using FlashBrowser.Services.Browser;
using FlashBrowser.UI.Builders;
using FlashBrowser.UI.Styles;
using FlashBrowser.UI.Values;

using PupaLib.Core;

namespace FlashBrowser.Core.ProfileStrategy;

public sealed class CreateProfileStrategy(IUiBuilder ui)
   : IProfileStrategy<LaunchProfile>, IProfileStrategy<AppProfile> {
   public Task<Option> Execute(IProfileContext<AppProfile> context, CancellationToken cancellationToken) {
      return Invoke(context, x => new AppProfile(x), cancellationToken);
   }

   public string Title => "New profile";

   public Task<Option> Execute(IProfileContext<LaunchProfile> context, CancellationToken cancellationToken) {
      return Invoke(context, x => new LaunchProfile(x), cancellationToken);
   }

   public async Task<Option> Invoke<T>(IProfileContext<T> context, Func<string, T> getter,
      CancellationToken cancellationToken) {
      await ui.Message($"Create a new {(typeof(T) == typeof(AppProfile) ? "application" : string.Empty)} profile",
         cancellationToken);
      var profileNameRef = new ValueRef<string>();
      while (cancellationToken.IsCancellationRequested == false) {
         ui.WithChain(chain => chain
            .WithInput<string>(input => input
               .WithRange(3, byte.MaxValue)
               .WithPrompt("Type a profile name")
               .Ref(profileNameRef)));
         if (await ui.RunAndClear(cancellationToken))
            break;
         return Option.Fail();
      }

      context.Profiles.Add(getter(profileNameRef.Get()));
      await ui.Message($"Added a new profile with name '{profileNameRef.Get()}'", cancellationToken);
      return Option.Ok();
   }
}