using FlashBrowser.Core.Pipelines.Context;
using FlashBrowser.Core.Startup.Context;

using PupaLib.Core;

namespace FlashBrowser.Core.Pipelines;

public sealed class StartupPipeline(IStartupContextFactory startupContextFactory, IEnumerable<IStartupStrategy> steps) {
   public async Task<Option<IPipelineContext>> RunAsync(CancellationToken cancellationToken) {
      var context = startupContextFactory.Create();
      foreach (var step in steps) {
         var result = await step.Execute(context, cancellationToken);
         if (result == false)
            return Option<IPipelineContext>.Fail();
      }

      return Option<IPipelineContext>.Ok(context);
   }
}