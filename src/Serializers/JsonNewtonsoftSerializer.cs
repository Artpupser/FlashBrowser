using System.Text;

using Newtonsoft.Json;

using ISerializer = PupaLib.FileIO.Serializers.ISerializer;

namespace FlashBrowser.Serializers;

public sealed class JsonNewtonsoftSerializer : ISerializer {
   private static readonly JsonSerializerSettings Settings = new() {
      NullValueHandling = NullValueHandling.Ignore,
      Formatting = Formatting.None
   };

   public byte[] Serialize<T>(T content) where T : class {
      var json = JsonConvert.SerializeObject(content, Settings);
      return Encoding.UTF8.GetBytes(json);
   }

   public T Deserialize<T>(byte[] bytes) where T : class {
      var json = Encoding.UTF8.GetString(bytes);
      return JsonConvert.DeserializeObject<T>(json, Settings)!;
   }
}