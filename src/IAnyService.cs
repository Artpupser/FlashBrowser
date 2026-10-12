namespace FlashBrowser;

public interface IAnyService {
   public Task Init(CancellationToken cancellationToken);
}