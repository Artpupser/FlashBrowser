namespace FlashBrowser.Core.Killers;

public record KillResult(string Name, int Id, Exception? Error = null) {
   public bool IsSuccess => Error != null;
}