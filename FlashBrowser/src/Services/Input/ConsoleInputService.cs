using FlashBrowser.Services.Input.Binds;

using PupaLib.Core;

namespace FlashBrowser.Services.Input;

public sealed class ConsoleInputService(IBindPressed bindPressed) : IKeyboardInputService {
   private readonly Eventer<ConsoleKeyInfo> _keyPressed = new();

   public Task Init(CancellationToken cancellationToken) {
      Console.CancelKeyPress += (_, _) => { bindPressed.OnCancel(); };
      _ = Task.Run(() => {
         while (cancellationToken.IsCancellationRequested == false) {
            var key = Console.ReadKey(true);
            _keyPressed.Invoke(this, key);
            if ((key.Modifiers & ConsoleModifiers.Control) != 0 && key.Key == ConsoleKey.Z) bindPressed.OnCtrlZ();
         }
      }, cancellationToken);
      return Task.CompletedTask;
   }

   public async Task<KeyboardKey> GetKeyboardKey() {
      var result = await GetConsoleKey();
      return KeyboardKey.FromConsole(result);
   }

   public async Task<ConsoleKeyInfo> GetConsoleKey() {
      var tcs = new TaskCompletionSource<ConsoleKeyInfo>();
      var handler = _keyPressed.Subscribe((sender, info) => { tcs.SetResult(info); });
      var result = await tcs.Task;
      _keyPressed.Unsubscribe(handler);
      return result;
   }
}