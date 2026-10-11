namespace FlashBrowser.Core.ProfileStrategy.Context;

public interface IProfileContext<T> {
   public List<T> Profiles { get; }
   public bool IsEmpty => Profiles.Count <= 0;
   public T? SelectedProfile { get; set; }
}