using FlashBrowser.Core.Pipelines.Context;
using FlashBrowser.Services.Drive;
using FlashBrowser.UI.Builders;
using FlashBrowser.UI.Styles;
using FlashBrowser.UI.Values;

using PupaLib.Core;
using PupaLib.FileIO;

namespace FlashBrowser.Core.FolderSearch;

public sealed class SearchFromDriveFolderStrategy(
   IUiBuilder ui,
   DriveService driveService,
   TypeFolderNameStrategy typeFolderNameStrategy)
   : IFolderSearchStrategy {
   public string Title => $"Search through selected drive <{driveService.UsableDrive!.Name}> with custom name";

   public async Task<Option<VirtualFolder>> Execute(IPipelineContext context, CancellationToken cancellationToken) {
      var usableDrive = context.DriveService.UsableDrive!;
      var browserFolderName = new ValueRef<string>("browser_flash");
      var verifyValue = new ValueRef<bool>();

      while (cancellationToken.IsCancellationRequested == false) {
         ui.WithChain(chain => chain
            .WithVerify(verify => verify
               .WithPrompt($"Use default name? <{browserFolderName.Get()}>")
               .Ref(verifyValue)));
         await ui.RunAndClear(cancellationToken);
         if (verifyValue.Get() == false) {
            var changedFolderNameOption = await typeFolderNameStrategy.Execute(context, cancellationToken);
            if (changedFolderNameOption.Out(out var newFolderName))
               browserFolderName.Set(newFolderName);
            else
               continue;
         }

         if (context.DriveService.FindFolderFromUsableDrive(browserFolderName.Get()).Out(out var browserFlashFolder))
            return Option<VirtualFolder>.Ok(browserFlashFolder);

         await ui.Message(
            new ConstValue<string>($"Folder '{browserFolderName.Get()}' undefined, in drive <{usableDrive.Name}>"),
            cancellationToken);
      }

      return Option<VirtualFolder>.Fail();
   }
}