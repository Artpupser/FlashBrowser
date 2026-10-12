using FlashBrowser.Core.Pipelines.Context;
using FlashBrowser.Services.Browser;
using FlashBrowser.Services.Data;
using FlashBrowser.Services.Drive;
using FlashBrowser.Services.OperationSystem;

namespace FlashBrowser.Core.Startup.Context;

public sealed class StartupContextFactory(
   DriveService driveService,
   DataService dataService,
   BrowserService browserService,
   OperatingSystemService operatingSystemService) : IStartupContextFactory {
   public IPipelineContext Create() {
      return new PipelineContext(dataService, driveService, browserService, operatingSystemService);
   }
}