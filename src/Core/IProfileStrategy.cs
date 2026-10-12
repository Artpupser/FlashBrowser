using FlashBrowser.Core.Pipelines.Context;
using FlashBrowser.Core.ProfileStrategy.Context;

using PupaLib.Core;
using PupaLib.FileIO;

namespace FlashBrowser.Core;

public interface IExecuteStrategy<in T, TResult> {
   public string Title { get; }
   public Task<TResult> Execute(T context, CancellationToken cancellationToken);
}

public interface IProfileStrategy<T> : IExecuteStrategy<IProfileContext<T>, Option>;

public interface IPipelineStrategy : IExecuteStrategy<IPipelineContext, Option>;

public interface ICoreStrategy : IPipelineStrategy;

public interface IStartupStrategy : IPipelineStrategy;

public interface IFolderSearchStrategy : IExecuteStrategy<IPipelineContext, Option<VirtualFolder>>;