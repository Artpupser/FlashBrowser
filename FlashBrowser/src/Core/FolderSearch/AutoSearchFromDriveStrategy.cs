using FlashBrowser.Core.Pipelines.Context;
using FlashBrowser.Services.Drive;
using FlashBrowser.UI.Builders;
using FlashBrowser.UI.Styles;
using FlashBrowser.UI.Values;

using PupaLib.Core;
using PupaLib.FileIO;

namespace FlashBrowser.Core.FolderSearch;

public sealed class AutoSearchFromDriveStrategy(
   DriveService driveService,
   IUiBuilder ui) : IFolderSearchStrategy {
   public string Title => $"Search through selected drive <{driveService.UsableDrive!.Name}>";

   public async Task<Option<VirtualFolder>> Execute(IPipelineContext context, CancellationToken cancellationToken) {
      var usableDrive = context.DriveService.UsableDrive!;
      var browserFolderName = new ConstValue<string>("browser_flash");
      if (context.DriveService.FindFolderFromUsableDrive(browserFolderName.Get()).Out(out var browserFlashFolder))
         return Option<VirtualFolder>.Ok(browserFlashFolder);
      await ui.Message($"Folder '{browserFolderName.Get()}' undefined, in drive <{usableDrive.Name}>",
         cancellationToken);
      return Option<VirtualFolder>.Fail();
   }
}