using PupaLib.Core;

namespace FlashBrowser.UI.Elements;

public interface IUiElement {
   public Task<Option> Run(CancellationToken cancellationToken);
}