using PupaLib.Core;

namespace FlashBrowser.Services.Input.Binds;

public sealed class AppBinds(IAppLifetimeManager appLifetimeManager) : IBindPressed {
   public Eventer Saved { get; } = new();
   public Eventer Undo { get; } = new();
   public Eventer Rendo { get; } = new();

   public void OnCancel() {
      appLifetimeManager.Cancel();
   }

   public void OnCtrlZ() {
      Undo.Invoke(this);
   }

   public void OnSave() {
      Saved.Invoke(this);
   }

   public void OnCtrlY() {
      Rendo.Invoke(this);
   }
}