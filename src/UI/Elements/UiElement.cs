using PupaLib.Core;

namespace FlashBrowser.UI.Elements;

public abstract class UiElement : IUiElement {
   public abstract Task<Option> Run(CancellationToken cancellationToken);
}

// public class Terminal : IUiEngine, IAnyService
// {
//     private readonly IKeyboardInputService _inputService;
//     public Terminal(IKeyboardInputService inputService)
//     {
//         _inputService = inputService;
//     }
//
//     public async Task<Option<(int index, string section)>> Choice(string[] sections, string prompt = "prompt", CancellationToken cancellationToken = default)
//     {
//         var index = 0;
//         Console.Write($"<< {prompt} >>\n");
//         var startCursor = Console.GetCursorPosition();
//         while (cancellationToken.IsCancellationRequested == false)
//         {
//             Console.SetCursorPosition(startCursor.Left, startCursor.Top);
//             for (int i = 0; i < sections.Length; i++)
//             {
//                 var item = sections[i];
//                 if (i == index)
//                 {
//                     Console.Write($"-> {item}\n");
//                     continue;
//                 }
//                 Console.Write($"   {item}\n");
//             }
//             var key = await _inputService.GetKeyboardKey();
//             if (key.KeyCode == 38 || key.KeyCode == 87) // ArrowUp 38, W 87 
//             {
//                 index = MyMath.Repeat(index - 1, 0, sections.Length - 1);
//                 continue;
//             }
//             if (key.KeyCode == 40 || key.KeyCode == 83) // ArrowDown 40, S 83
//             {
//                 index = MyMath.Repeat(index + 1, 0, sections.Length - 1);
//                 continue;
//             }
//             if (key.IsEnter()) // Enter
//             {
//                 return Option<(int, string)>.Ok((index, sections[index]));
//             }
//             if (key.IsEsc() || key.IsBackspace()) // Escape
//             {
//                 return Option<(int, string)>.Ok((-1, string.Empty));
//             }
//         }
//
//         return Option<(int, string)>.Fail();
//     }
//
//     public void Dialogue(string text, string author)
//     {
//         Console.Out.WriteLine($"<{author}> {text}");
//     }
//
//     public void Message(string text)
//     {
//         Console.Out.Write(text);
//     }
//
//     public async Task<Option> Verify(string warnText, CancellationToken cancellationToken = default)
//     {
//         Console.Out.Write($"[< {warnText} >] y/n");
//         while (cancellationToken.IsCancellationRequested == false)
//         {
//             var key = await _inputService.GetKeyboardKey();
//             if (key.KeyCode == 89) // Y 89 
//             {
//                 Console.Out.Write('\n');
//                 return Option.Ok();
//             }
//             if (key.KeyCode == 78) // N 78
//             {
//                 Console.Out.Write('\n');
//                 return Option.Fail();   
//             }
//         }
//         Console.Out.Write('\n');
//         return Option.Fail();
//     }
//
//     public async Task<Option<T>> Input<T>(string prompt = "prompt", CancellationToken cancellationToken = default)
//     {
//         var sb = new StringBuilder();
//         Console.Write($"[{prompt}] > ");
//         while (cancellationToken.IsCancellationRequested == false)
//         {
//             var cursor = Console.GetCursorPosition();
//             var keyboardKey = await _inputService.GetKeyboardKey();
//             if (keyboardKey.IsEnter()) //Enter
//             {
//                 try
//                 {
//                     var value = (T)Convert.ChangeType(sb.ToString(), typeof(T));
//                     Console.Write('\n');
//                     return Option<T>.Ok(value);
//                 }
//                 catch (FormatException)
//                 {
//                     continue;
//                 }        
//                 catch(InvalidCastException)
//                 {
//                     continue;
//                 }
//             }
//             if (keyboardKey.IsEsc()) //Escape
//             {
//                 break;
//             }
//             if(keyboardKey.IsBackspace()) //Backspace
//             {
//                 if (sb.Length < 1)
//                     continue;
//                 sb.Remove(sb.Length - 1, 1);
//                 Console.SetCursorPosition(cursor.Left - 1, cursor.Top);
//                 Console.Write(' ');
//                 Console.SetCursorPosition(cursor.Left - 1, cursor.Top);
//                 continue;
//             }
//             sb.Append(keyboardKey.Symbol);
//             Console.Write(keyboardKey.Symbol);
//         }
//         return Option<T>.Fail();
//     }
//
//     public Task Init(CancellationToken cancellationToken)
//     {
//         Console.Clear();
//         return Task.CompletedTask;
//     }
// }