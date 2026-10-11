using FlashBrowser.Core.Pipelines.Context;
using FlashBrowser.UI.Builders;
using FlashBrowser.UI.Styles;
using FlashBrowser.UI.Values;
using FlashBrowser.Utils;

using PupaLib.Core;

namespace FlashBrowser.Core.Startup;

public sealed class HelloStepStrategy(IUiBuilder ui) : IStartupStrategy {
   public string Title => "Hello";

   public async Task<Option> Execute(IPipelineContext context, CancellationToken cancellationToken) {
      var localConfig = context.DataService.ApplicationData!;
      var isNewPassword = localConfig.IsFirst;

      await ui.Message("""
                               __n__n__
                        .------`-\00/-'
                       /  ##  ## (oo)
                       / \## __   ./
                         |//YY \|/
                         |||   |||
                        
                       """, cancellationToken);
      while (cancellationToken.IsCancellationRequested == false && localConfig.IsFirst) {
         await ui.Message("Hello a new user, it's FlashBrowser app", cancellationToken);
         var newPasswordRef = new ValueRef<string>();
         var repeatNewPasswordRef = new ValueRef<string>();
         ui
            .WithChain(chain => chain
               .WithInput<string>(input => input
                  .WithPrompt("Type a new password")
                  .WithRange(8, 20)
                  .Ref(newPasswordRef))
               .WithInput<string>(input => input
                  .WithPrompt("Repeat a new password")
                  .WithRange(8, 20)
                  .Ref(repeatNewPasswordRef))
            );
         await ui.RunAndClear(cancellationToken);
         if (newPasswordRef.Get() == repeatNewPasswordRef.Get()) {
            localConfig.IsFirst = false;
            localConfig.Password = MyMath.HashSha256(newPasswordRef.Get());
            await context.DataService.Save(cancellationToken);
         }
      }

      const byte maxAttempts = 3;
      var attempts = maxAttempts;
      while (cancellationToken.IsCancellationRequested == false && localConfig.IsFirst == false &&
             isNewPassword == false) {
         await ui.Message("Hello user, it's FlashBrowser app", cancellationToken);
         var passwordRef = new ValueRef<string>();
         ui
            .WithChain(chain => chain
               .WithInput<string>(input => input
                  .WithPrompt("Type a password")
                  .WithRange(8, 20)
                  .WithSecret('*')
                  .Ref(passwordRef)));
         await ui.RunAndClear(cancellationToken);
         if (MyMath.HashSha256(passwordRef.Get()) != localConfig.Password) {
            attempts -= 1;
            if (attempts < 1) return Option.Fail();
            await ui.Message($"Wrong password, attempts {attempts}/{maxAttempts}", cancellationToken);
            continue;
         }

         break;
      }

      return Option.Ok();
   }
}