using FlashBrowser.Services.Input;
using FlashBrowser.UI.Chains;
using FlashBrowser.UI.Engines;

using Microsoft.Extensions.DependencyInjection;

using PupaLib.Core;

namespace FlashBrowser.UI.Builders;

public sealed class UiBuilder : IUiBuilder {
   private readonly Queue<IUiBuilderChain> _chains;
   private readonly ServiceProvider _serviceProvider;

   public UiBuilder(IUiEngine engine, IKeyboardInputService keyboardInputService) {
      _chains = [];
      KeyboardInputService = keyboardInputService;
      ServiceCollection serviceCollection = [];
      serviceCollection
         .AddSingleton<IKeyboardInputService>(x => keyboardInputService)
         .AddSingleton<IUiEngine>(x => engine);
      _serviceProvider = serviceCollection.BuildServiceProvider();
   }

   public IKeyboardInputService KeyboardInputService { get; }

   public IUiBuilder WithChain(Action<IUiBuilderChain> chainBuilder) {
      var chain = new UiBuilderChain(_serviceProvider);
      chainBuilder.Invoke(chain);
      _chains.Enqueue(chain);
      return this;
   }

   public async Task<Option> Run(CancellationToken cancellationToken) {
      while (_chains.Count > 0) {
         var chain = _chains.Dequeue();
         var result = await chain.Run(cancellationToken);
         if (result.Success)
            continue;
         return Option.Fail();
      }

      return Option.Ok();
   }

   public async Task<Option> RunAndClear(CancellationToken cancellationToken) {
      var result = await Run(cancellationToken);
      Clear();
      return result;
   }

   public IUiBuilder Clear() {
      _chains.Clear();
      return this;
   }
}