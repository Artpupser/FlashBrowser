using FlashBrowser.Core.ProfileStrategy.Context;
using FlashBrowser.Extensions;
using FlashBrowser.Services.Browser;
using FlashBrowser.Services.OperationSystem;
using FlashBrowser.UI.Builders;
using FlashBrowser.UI.Styles;
using FlashBrowser.UI.Values;

using PupaLib.Core;

namespace FlashBrowser.Core.ProfileStrategy;

public sealed class ChangeApplicationPathsProfileStrategy(OperatingSystemService osService, IUiBuilder ui)
   : IProfileStrategy<AppProfile>, IChangeProfileStrategy, IRequiredProfileStrategy {
   public string Title => "Change application paths";

   public async Task<Option> Execute(IProfileContext<AppProfile> context, CancellationToken cancellationToken) {
      var profile = context.SelectedProfile!;
      var osRef = new ValueRef<(int index, string name)>();
      while (cancellationToken.IsCancellationRequested == false) {
         var osNames = osService.Select(x =>
               $"{(x == osService.CurrentOperatingSystem ? "[current] " : string.Empty)}{x.GetLowerName()}: {profile.GetPathOrEmpty((int)x)}")
            .ToArray();
         ui.WithChain(chain => chain
            .WithChoice(choice => choice
               .WithChoices(osNames)
               .WithPrompt("Select os path")
               .Ref(osRef)));
         if (await ui.RunAndClear(cancellationToken) && osRef.Get().index > -1) {
            if (osRef.Get().index != (int)osService.CurrentOperatingSystem) {
               await ui.Message(
                  $"Selected os '{(OperatingSystemType)osRef.Get().index}', now you cannot set application path to another operating system. Your os is '{osService.CurrentOperatingSystem}'",
                  cancellationToken);
               continue;
            }

            var index = osRef.Get().index;
            var pathRef = new ValueRef<string>();
            ui.WithChain(chain => chain
               .WithInput<string>(input => input
                  .WithCustomCondition(Path.Exists)
                  .WithRange()
                  .WithPrompt("Type a app path")
                  .Ref(pathRef)));

            if (await ui.RunAndClear(cancellationToken)) profile.SetExecutePath(index, pathRef.Get());
            continue;
         }

         break;
      }

      return Option.Fail();
   }
}