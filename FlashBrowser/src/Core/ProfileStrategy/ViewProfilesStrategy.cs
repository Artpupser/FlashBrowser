using FlashBrowser.Core.ProfileStrategy.Context;
using FlashBrowser.Extensions;
using FlashBrowser.Services.Browser;
using FlashBrowser.Services.OperationSystem;
using FlashBrowser.UI.Builders;
using FlashBrowser.UI.Styles;

using PupaLib.Core;

namespace FlashBrowser.Core.ProfileStrategy;

public sealed class ViewProfilesStrategy(OperatingSystemService osService, IUiBuilder ui)
   : IProfileStrategy<LaunchProfile>, IProfileStrategy<AppProfile> {
   public async Task<Option> Execute(IProfileContext<AppProfile> context, CancellationToken cancellationToken) {
      if (context.IsEmpty) {
         await ui.Message("Profiles is empty, please create a new application profile", cancellationToken);
         return Option.Fail();
      }

      await ui.Message("List applications", cancellationToken);
      foreach (var profile in context.Profiles) {
         await ui.Message($"<< [{profile.Name}] >>", cancellationToken);
         foreach (var osType in osService)
            await ui.Message($"\t\t{osType.GetLowerName()}: {profile.ExecutePaths[(int)osType]}", cancellationToken);
      }

      return Option.Ok();
   }

   public string Title => "View info";

   public async Task<Option> Execute(IProfileContext<LaunchProfile> context, CancellationToken cancellationToken) {
      if (context.IsEmpty) {
         await ui.Message("Profiles is empty, please create a new profile", cancellationToken);
         return Option.Fail();
      }

      await ui.Message("List launch profiles", cancellationToken);
      foreach (var profile in context.Profiles) {
         await ui.Message($"<< [{profile.Name}] >>", cancellationToken);
         if (profile.Links.Count <= 0)
            await ui.Message("\tLinks not found", cancellationToken);
         else
            foreach (var link in profile.Links)
               await ui.Message($"\t{link}", cancellationToken);

         if (profile.AppProfiles.Count <= 0)
            await ui.Message("\tApplications not found", cancellationToken);
         else
            foreach (var appProfile in profile.AppProfiles) {
               await ui.Message($"\t{appProfile.Name} [{appProfile.ExecutePaths.Length}]", cancellationToken);
               foreach (var osType in osService)
                  await ui.Message($"\t\t{osType.GetLowerName()}: {appProfile.GetPathOrEmpty(osType, "undefined")}",
                     cancellationToken);
            }
      }

      return Option.Ok();
   }
}