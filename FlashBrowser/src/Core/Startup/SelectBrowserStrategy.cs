using FlashBrowser.Core.BrowserStrategy;
using FlashBrowser.Core.Pipelines.Context;
using FlashBrowser.UI.Builders;

using PupaLib.Core;

namespace FlashBrowser.Core.Startup;

public sealed class SelectBrowserStrategy(IUiBuilder ui) : IStartupStrategy {
   public string Title => "Select browser";

   public async Task<Option> Execute(IPipelineContext context, CancellationToken cancellationToken) {
      return await new SelectBrowserStartegy(ui).Execute(context, cancellationToken);
   }
}