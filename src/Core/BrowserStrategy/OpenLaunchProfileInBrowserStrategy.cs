using System.Diagnostics;

using FlashBrowser.Core.Factory;
using FlashBrowser.Core.Pipelines.Context;
using FlashBrowser.Core.ProfileStrategy;
using FlashBrowser.Core.ProfileStrategy.Context;
using FlashBrowser.Services.Browser;
using FlashBrowser.UI.Builders;
using FlashBrowser.UI.Styles;

using PupaLib.Core;

namespace FlashBrowser.Core.BrowserStrategy;

public sealed class OpenLaunchProfileInBrowserStrategy(
   IUiBuilder ui,
   IStrategyFactory<IProfileStrategy<LaunchProfile>> launchProfilesFactory) : ICoreStrategy {
   public string Title => "Open profile";

   public async Task<Option> Execute(IPipelineContext context, CancellationToken cancellationToken = default) {
      var strategies = launchProfilesFactory.Create();
      var selectStrategy = strategies.First(x => x is SelectProfileStrategy);
      var profileContext = new LaunchProfileContext(context.DataService.ApplicationData!.Profiles);
      while (cancellationToken.IsCancellationRequested == false && profileContext.SelectedProfile == null)
         if (await selectStrategy.Execute(profileContext, cancellationToken) == false)
            return Option.Fail();

      var selectedProfile = profileContext.SelectedProfile!;
      if (context.BrowserService.Browser != null) {
         for (var i = 0; i < selectedProfile.Links.Count; i++) {
            var link = selectedProfile.Links[i];
            try {
               await context.BrowserService.NewPageTab(link);
               await ui.Message($"Open link, '{link}'", cancellationToken);
            } catch (Exception e) {
               await ui.Message($"Failed open link, '{link}' message, {e.Message}", cancellationToken);
            }
         }

         for (var i = 0; i < selectedProfile.AppProfiles.Count; i++) {
            var appProfile = selectedProfile.AppProfiles[i];
            var executePath = appProfile.GetPathOrEmpty(context.OperatingSystemService.CurrentOperatingSystem);
            try {
               if (string.IsNullOrWhiteSpace(executePath))
                  throw new Exception(
                     $"execute path for '{context.OperatingSystemService.CurrentOperatingSystem}' operating system undefined");
               Process.Start(new ProcessStartInfo {
                  UseShellExecute = true,
                  FileName = executePath,
                  WindowStyle = ProcessWindowStyle.Maximized
               });
               await ui.Message($"Open application, '{executePath}'", cancellationToken);
            } catch (Exception e) {
               await ui.Message($"Failed open application, '{executePath}' message, {e.Message}", cancellationToken);
            }
         }

         return Option.Ok();
      }

      await ui.Message("Browser not run, launch any browser and retry", cancellationToken);
      return Option.Fail();
   }
}