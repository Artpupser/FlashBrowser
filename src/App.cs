using FlashBrowser.Core.Pipelines;
using FlashBrowser.Core.Pipelines.Context;

namespace FlashBrowser;

public sealed class App(StartupPipeline startupPipeline, CorePipeline corePipeline) : IApp, IDisposable {
   private CancellationTokenSource? _linkedCts;
   private IPipelineContext? _startupContext;

   public async Task Run(CancellationToken cancellationToken) {
      _linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
      var contextOption = await startupPipeline.RunAsync(cancellationToken);
      if (contextOption.Out(out _startupContext))
         await corePipeline.Execute(_startupContext, cancellationToken);
   }

   public async Task Stop() {
      if (_linkedCts != null)
         await _linkedCts.CancelAsync();
   }

   public void Dispose() {
      _linkedCts?.Dispose();
   }
}