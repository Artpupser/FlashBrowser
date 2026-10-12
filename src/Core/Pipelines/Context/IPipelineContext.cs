using FlashBrowser.Services.Browser;
using FlashBrowser.Services.Data;
using FlashBrowser.Services.Drive;
using FlashBrowser.Services.OperationSystem;

namespace FlashBrowser.Core.Pipelines.Context;

public interface IPipelineContext {
   public DataService DataService { get; }
   public DriveService DriveService { get; }
   public BrowserService BrowserService { get; }
   public OperatingSystemService OperatingSystemService { get; }
}