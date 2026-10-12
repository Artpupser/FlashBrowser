namespace FlashBrowser.Services.Input;

public interface IKeyboardInputService : IAnyService {
   public Task<KeyboardKey> GetKeyboardKey();
}