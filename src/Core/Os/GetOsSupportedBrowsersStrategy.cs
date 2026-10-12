using FlashBrowser.Services.OperationSystem;

using PupaLib.Core;

using PuppeteerSharp;

namespace FlashBrowser.Core.Os;

public sealed class GetOsSupportedBrowsersStrategy(OperatingSystemService operatingSystemService) {
   public Option<IEnumerable<SupportedBrowser>> Execute() {
      return Option<IEnumerable<SupportedBrowser>>.Ok(Get());
   }

   private IEnumerable<SupportedBrowser> Get() {
      yield return SupportedBrowser.Firefox;
      yield return SupportedBrowser.Chrome;
      if (operatingSystemService.CurrentOperatingSystem != OperatingSystemType.Windows)
         yield return SupportedBrowser.Chromium;
   }
}