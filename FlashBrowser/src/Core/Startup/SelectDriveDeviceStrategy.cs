using FlashBrowser.Core.Pipelines.Context;
using FlashBrowser.UI.Builders;
using FlashBrowser.UI.Styles;
using FlashBrowser.UI.Values;

using PupaLib.Core;

namespace FlashBrowser.Core.Startup;

public sealed class SelectDriveDeviceStrategy(IUiBuilder ui) : IStartupStrategy {
   public string Title => "Select drive device";

   public async Task<Option> Execute(IPipelineContext context, CancellationToken cancellationToken) {
      var driveManager = context.DriveService;
      var drives = driveManager.GetActiveEjectableDrives().ToList();
      if (drives.Any() == false) drives.AddRange(driveManager.GetActiveDrives());
      var driveNames = drives.Select(x => x.Name).ToArray();
      var driveChoiceRef = new ValueRef<(int index, string name)>();
      while (cancellationToken.IsCancellationRequested == false) {
         ui.WithChain(chain => chain
            .WithChoice(choice => choice
               .WithChoices(driveNames)
               .WithPrompt("Select drive device")
               .Ref(driveChoiceRef)));
         if ((await ui.RunAndClear(cancellationToken)).Success) {
            driveManager.UsableDrive = drives[driveChoiceRef.Get().index];
            await ui.Message(
               $"Current device is: {driveManager.UsableDrive.Name} {driveManager.UsableDrive.DriveFormat} {driveManager.UsableDrive.DriveType}",
               cancellationToken);
            break;
         }
      }

      return Option.Ok();
   }
}