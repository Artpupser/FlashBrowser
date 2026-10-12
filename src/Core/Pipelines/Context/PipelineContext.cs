using FlashBrowser.Services.Browser;
using FlashBrowser.Services.Data;
using FlashBrowser.Services.Drive;
using FlashBrowser.Services.OperationSystem;

namespace FlashBrowser.Core.Pipelines.Context;

public sealed class PipelineContext(
   DataService dataService,
   DriveService driveService,
   BrowserService browserService,
   OperatingSystemService operatingSystemService) : IPipelineContext {
   public DataService DataService { get; set; } = dataService;
   public DriveService DriveService { get; set; } = driveService;
   public BrowserService BrowserService { get; set; } = browserService;
   public OperatingSystemService OperatingSystemService { get; set; } = operatingSystemService;
}