namespace FlashBrowser.Core.ProfileStrategy.Context;

public abstract class ProfileContext<T>(List<T> profiles) {
   public List<T> Profiles { get; private set; } = profiles;
   public T? SelectedProfile { get; set; }
}