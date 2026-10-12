using FlashBrowser.Core.Pipelines.Context;
using FlashBrowser.Services.Data;
using FlashBrowser.UI.Builders;
using FlashBrowser.UI.Styles;

using PupaLib.Core;
using PupaLib.FileIO;

namespace FlashBrowser.Core.Startup;

public sealed class InitApplicationEnvironment(IUiBuilder ui) : IStartupStrategy {
   private const string ApplicationEnvironmentFileName = "local.json";
   public string Title => "Initialization application environment";

   public async Task<Option> Execute(IPipelineContext context, CancellationToken cancellationToken) {
      if (VirtualIo.RootFolder.GetOrCreateFolderIn("data").Out(out var localConfigFolder)) {
         context.DataService.ApplicationDataFolder = localConfigFolder;
         await ui.Message($"Load or create folder '{localConfigFolder.MyPath}'",
            cancellationToken);
         if (localConfigFolder.GetOrCreateFileIn(ApplicationEnvironmentFileName).Out(out var localConfigFile)) {
            context.DataService.ApplicationDataFile = localConfigFile;
            while (cancellationToken.IsCancellationRequested == false)
               try {
                  if (localConfigFile.SizeInBytes <= 0) {
                     await ui.Message($"Create and load default config '{nameof(ApplicationData)}'",
                        cancellationToken);
                     await context.DataService.SaveDefault(cancellationToken);
                  }

                  await ui.Message(
                     $"Try read and parse config '{nameof(ApplicationData)}' use serializer <{context.DataService.CurrentSerializer.GetType().Name}>",
                     cancellationToken);
                  var result = await context.DataService.Load(cancellationToken);
                  if (result == false)
                     throw new Exception("Bad read file");
                  break;
               } catch (Exception e) {
                  await ui.Message($"Bad read config '{nameof(ApplicationData)}' {e.Message} retry...",
                     cancellationToken);
               }
         }
      }

      return Option.Ok();
   }
}