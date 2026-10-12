using FlashBrowser.UI.Chains;

using PupaLib.Core;

namespace FlashBrowser.UI.Builders;

public interface IUiBuilder {
   public IUiBuilder WithChain(Action<IUiBuilderChain> chainBuilder);
   public Task<Option> Run(CancellationToken cancellationToken);
   public Task<Option> RunAndClear(CancellationToken cancellationToken);
   public IUiBuilder Clear();
}