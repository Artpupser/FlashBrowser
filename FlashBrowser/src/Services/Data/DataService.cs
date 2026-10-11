using FlashBrowser.Serializers;

using PupaLib.Core;
using PupaLib.FileIO;
using PupaLib.FileIO.Serializers;

namespace FlashBrowser.Services.Data;

public sealed class DataService {
   public ApplicationData? ApplicationData { get; set; }
   public VirtualFile? ApplicationDataFile { get; set; }
   public VirtualFolder? ApplicationDataFolder { get; set; }
   public ISerializer CurrentSerializer { get; } = new JsonSystemSerializer();

   public async Task Save(CancellationToken cancellationToken) {
      if (ApplicationData == null || ApplicationDataFile == null)
         return;

      await ApplicationDataFile.WriteTContentAsync(
         ApplicationData,
         new JsonNewtonsoftSerializer(),
         cancellationToken
      );
   }

   public async Task SaveDefault(CancellationToken cancellationToken) {
      if (ApplicationDataFile == null)
         return;

      await ApplicationDataFile.WriteTContentAsync(
         ApplicationData.Default(),
         CurrentSerializer,
         cancellationToken
      );
   }

   public async Task<Option> Load(CancellationToken cancellationToken) {
      if (ApplicationDataFile == null)
         return Option.Fail();

      var result = await ApplicationDataFile.ReadTContentAsync<ApplicationData>(
         CurrentSerializer,
         cancellationToken
      );

      if (result.Out(out var localConfig)) {
         ApplicationData = localConfig;
         return Option.Ok();
      }

      return Option.Fail();
      ;
   }
}