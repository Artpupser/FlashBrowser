using System.Text;

namespace FlashBrowser.UI.Values;

public sealed class ConcatValue(params IValue<object>[] values) : IValue<string> {
   public string Get() {
      var sb = new StringBuilder();
      foreach (var f in values) sb.Append(f.Get());

      return sb.ToString();
   }
}