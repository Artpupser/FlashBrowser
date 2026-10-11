namespace FlashBrowser;

public interface IApp {
   public Task Run(CancellationToken cancellationToken);
   public Task Stop();
}