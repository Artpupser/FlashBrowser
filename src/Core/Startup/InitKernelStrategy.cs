using FlashBrowser.Core.Pipelines.Context;
using FlashBrowser.Services.OperationSystem;
using FlashBrowser.UI.Builders;
using FlashBrowser.UI.Styles;
using FlashBrowser.Utils;

using PupaLib.Core;

namespace FlashBrowser.Core.Startup;

public sealed class InitKernelStrategy(IUiBuilder ui) : IStartupStrategy {
   public string Title => "Initialization kernel";

   public async Task<Option> Execute(IPipelineContext context, CancellationToken cancellationToken) {
      if (context.OperatingSystemService.CurrentOperatingSystem != OperatingSystemType.Windows)
         return Option.Ok();
      try {
         await ui.Message("Kernel initialization", cancellationToken);
         Kernel32Dll.EnableANSIConsoleMode();
         await ui.Message("Enable ANSI console mode", cancellationToken);
         return Option.Ok();
      } catch (Exception e) {
         await ui.Message($"Fail kernel initialization, '{e.Message}'", cancellationToken);
         return Option.Fail();
      }

      return Option.Ok();
   }
}