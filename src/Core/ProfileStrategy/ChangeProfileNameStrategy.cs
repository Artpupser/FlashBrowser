using FlashBrowser.Core.ProfileStrategy.Context;
using FlashBrowser.Services.Browser;
using FlashBrowser.UI.Builders;
using FlashBrowser.UI.Values;

using PupaLib.Core;

namespace FlashBrowser.Core.ProfileStrategy;

public sealed class ChangeProfileNameStrategy(
   IUiBuilder ui
) : IProfileStrategy<LaunchProfile>, IProfileStrategy<AppProfile>, IChangeProfileStrategy, IRequiredProfileStrategy {
   public Task<Option> Execute(IProfileContext<AppProfile> context, CancellationToken cancellationToken) {
      return Invoke(context, x => context.SelectedProfile!.Name = x, cancellationToken);
   }

   public string Title => "Change name";

   public Task<Option> Execute(IProfileContext<LaunchProfile> context, CancellationToken cancellationToken) {
      return Invoke(context, x => context.SelectedProfile!.Name = x, cancellationToken);
   }

   private async Task<Option> Invoke<T>(IProfileContext<T> context, Action<string> setter,
      CancellationToken cancellationToken) {
      var inputRef = new ValueRef<string>();
      while (cancellationToken.IsCancellationRequested == false) {
         ui.WithChain(chain => chain
            .WithInput<string>(input => input
               .WithRange(3, byte.MaxValue)
               .WithPrompt("Type a new name for context")
               .Ref(inputRef)));
         if (await ui.RunAndClear(cancellationToken)) {
            setter.Invoke(inputRef.Get());
            return Option.Ok();
         }

         break;
      }

      return Option.Fail();
   }
}