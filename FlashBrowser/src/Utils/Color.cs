namespace FlashBrowser.Utils;

public readonly struct Color {
   public static Color White => FromRgb(255);
   public static Color Black => FromRgb(0);


   private Color(byte r, byte g, byte b) {
      R = r;
      G = g;
      B = b;
   }

   public static Color FromRgb(byte r, byte g, byte b) {
      return new Color(r, g, b);
   }

   public static Color FromRgb((byte, byte, byte) v) {
      return new Color(v.Item1, v.Item2, v.Item3);
   }

   public static Color FromRgb(byte v) {
      return new Color(v, v, v);
   }

   public string ColorizeFgX1B(string str) {
      return $"\x1b[38;2;{R};{G};{B}m{str}\x1b[0m";
   }

   public string ColorizeBgX1B(string str) {
      return $"\x1b[48;2;{R};{G};{B}m{str}\x1b[0m";
   }

   public static string ColorizeFgBgX1B(string str, Color fg, Color bg) {
      return $"\x1b[38;2;{fg.R};{fg.G};{fg.B};48;2;{bg.R};{bg.G};{bg.B}m{str}\x1b[0m";
   }

   public byte R { get; }

   public byte G { get; }

   public byte B { get; }
}