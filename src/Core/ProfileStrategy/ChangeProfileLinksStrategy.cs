using FlashBrowser.Core.ProfileStrategy.Context;
using FlashBrowser.Services.Browser;
using FlashBrowser.UI.Builders;
using FlashBrowser.UI.Values;

using PupaLib.Core;

namespace FlashBrowser.Core.ProfileStrategy;

public sealed class ChangeProfileLinksStrategy(IUiBuilder ui)
   : IProfileStrategy<LaunchProfile>, IChangeProfileStrategy, IRequiredProfileStrategy {
   public string Title => "Change web links profile";

   public async Task<Option> Execute(IProfileContext<LaunchProfile> context, CancellationToken cancellationToken) {
      var webProfileRef = new ValueRef<(int index, string name)>();
      while (cancellationToken.IsCancellationRequested == false) {
         var webProfilesArr = context.SelectedProfile!.Links.ToArray();
         ui.WithChain(chain => chain
            .WithChoice(choice => choice
               .WithChoices(webProfilesArr)
               .WithChoice("New link +")
               .WithPrompt("Select a link for change")
               .Ref(webProfileRef)));

         if (await ui.RunAndClear(cancellationToken) && webProfileRef.Get().index > -1) {
            var index = webProfileRef.Get().index;
            if (index == webProfilesArr.Length) {
               var linkRef = new ValueRef<string>();
               ui.WithChain(chain => chain
                  .WithInput<string>(input => input
                     .WithCustomCondition(x => Uri.TryCreate(x, UriKind.Absolute, out _))
                     .WithRange()
                     .WithPrompt("New link")
                     .Ref(linkRef)));
               if (await ui.RunAndClear(cancellationToken)) {
                  var link = linkRef.Get();
                  context.SelectedProfile.Links = [link, .. context.SelectedProfile.Links];
               }
            } else {
               var removeVerifyRef = new ValueRef<bool>();
               ui.WithChain(chain => chain
                  .WithVerify(verify => verify
                     .WithPrompt($"Remove '{context.SelectedProfile.Links[index][..20]}...' link?")
                     .Ref(removeVerifyRef)));
               if (await ui.RunAndClear(cancellationToken) && removeVerifyRef.Get())
                  context.SelectedProfile.Links.RemoveAt(index);
               ;
            }

            continue;
         }

         break;
      }

      return Option.Fail();
   }
}