using FlashBrowser.UI.Elements;
using FlashBrowser.UI.Elements.Base;
using FlashBrowser.UI.Styles;

using PupaLib.Core;

namespace FlashBrowser.UI.Chains;

public interface IUiBuilderChain {
   public IUiBuilderChain WithMessage(Action<IUiMessage> uiBuilder, UiStyle<IUiMessage>? style = null);
   public IUiBuilderChain WithInput<T>(Action<IUiInput<T>> uiBuilder, UiStyle<IUiInput<T>>? style = null);
   public IUiBuilderChain WithVerify(Action<IUiVerify> uiBuilder, UiStyle<IUiVerify>? style = null);
   public IUiBuilderChain WithChoice(Action<IUiChoice> uiBuilder, UiStyle<IUiChoice>? style = null);
   public IUiBuilderChain WithCustom<T>(Action<T> uiBuilder, UiStyle<T>? style = null) where T : IUiElement;
   public Task<Option> Run(CancellationToken cancellationToken = default);
}