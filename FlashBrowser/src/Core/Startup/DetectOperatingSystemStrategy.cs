using FlashBrowser.Core.Pipelines.Context;
using FlashBrowser.Extensions;
using FlashBrowser.UI.Builders;
using FlashBrowser.UI.Styles;

using PupaLib.Core;

namespace FlashBrowser.Core.Startup;

public sealed class DetectOperatingSystemStrategy(IUiBuilder ui) : IStartupStrategy {
   public string Title => "Detect operation system";

   public async Task<Option> Execute(IPipelineContext context, CancellationToken cancellationToken) {
      context.OperatingSystemService.DetectCurrentOperatingSystem();
      var currentOs = context.OperatingSystemService.CurrentOperatingSystem;

      if (currentOs.IsNone()) {
         await ui.Message("Your operation system, not supported", cancellationToken);
         return Option.Fail();
      }

      await ui.Message(
         $"Your operation system is '{currentOs.GetLowerName()}'\nplatform: {Environment.OSVersion.Platform}\nversion: {Environment.OSVersion.VersionString}",
         cancellationToken);
      return Option.Ok();
   }
}