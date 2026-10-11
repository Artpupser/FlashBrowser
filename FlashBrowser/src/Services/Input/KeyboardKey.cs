namespace FlashBrowser.Services.Input;

public readonly struct KeyboardKey {
   private KeyboardKey(int keyCode, byte modCode, char symbol) {
      KeyCode = keyCode;
      ModCode = modCode;
      Symbol = symbol;
   }

   public static KeyboardKey FromConsole(in ConsoleKeyInfo info) {
      return new KeyboardKey((int)info.Key, (byte)info.Modifiers, info.KeyChar);
   }

   public static KeyboardKey FromConsole(ConsoleKey key) {
      return new KeyboardKey((int)key, (byte)ConsoleModifiers.None, '\0');
   }


   public bool IsEnter() {
      return KeyCode == 13;
   }

   public bool IsBackspace() {
      return KeyCode == 8;
   }

   public bool IsEsc() {
      return KeyCode == 27;
   }

   public int KeyCode { get; }

   public byte ModCode { get; }

   public char Symbol { get; }
}