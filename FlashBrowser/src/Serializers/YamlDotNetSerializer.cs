using System.Text;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

using ISerializer = PupaLib.FileIO.Serializers.ISerializer;

namespace FlashBrowser.Serializers;

public sealed class YamlDotNetSerializer : ISerializer {
   public byte[] Serialize<T>(T content) where T : class {
      var serializer = new SerializerBuilder()
         .WithNamingConvention(CamelCaseNamingConvention.Instance)
         .Build();
      var yaml = serializer.Serialize(content);
      return Encoding.UTF8.GetBytes(yaml);
   }

   public T Deserialize<T>(byte[] bytes) where T : class {
      var deserializer = new DeserializerBuilder()
         .WithNamingConvention(CamelCaseNamingConvention.Instance)
         .Build();
      var yaml = Encoding.UTF8.GetString(bytes);
      return deserializer.Deserialize<T>(yaml);
   }
}