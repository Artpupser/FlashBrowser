using FlashBrowser.Core.Pipelines.Context;

namespace FlashBrowser.Core.Startup.Context;

public interface IStartupContextFactory {
   public IPipelineContext Create();
}