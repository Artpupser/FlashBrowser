using FlashBrowser.Core;
using FlashBrowser.Core.BrowserStrategy;
using FlashBrowser.Core.Factory;
using FlashBrowser.Core.FileSearch;
using FlashBrowser.Core.FolderSearch;
using FlashBrowser.Core.Killers;
using FlashBrowser.Core.Os;
using FlashBrowser.Core.Pipelines;
using FlashBrowser.Core.ProfileStrategy;
using FlashBrowser.Core.Startup;
using FlashBrowser.Core.Startup.Context;
using FlashBrowser.Extensions;
using FlashBrowser.Services.Browser;
using FlashBrowser.Services.Data;
using FlashBrowser.Services.Drive;
using FlashBrowser.Services.Input;
using FlashBrowser.Services.Input.Binds;
using FlashBrowser.Services.OperationSystem;
using FlashBrowser.UI.Builders;
using FlashBrowser.UI.Engines;
using FlashBrowser.UI.Styles;
using FlashBrowser.UI.Values;

using Microsoft.Extensions.DependencyInjection;

namespace FlashBrowser;

public class Program : IAppLifetimeManager, IDisposable {
   private static readonly CancellationTokenSource _cts = new();
   private static ServiceProvider _provider = null!;
   private static readonly ServiceCollection _serviceCollection = [];

   public void Cancel() {
      //Message
      var keyboardInputService = _provider.GetRequiredService<IKeyboardInputService>();
      var uiEngine = _provider.GetRequiredService<IUiEngine>();
      var uiBuilder = new UiBuilder(uiEngine, keyboardInputService);
      uiBuilder.Message(
         new ConstValue<string>($"\n\n\nGraceful cancel, <thread {Thread.CurrentThread.ManagedThreadId}>"),
         CancellationToken.None).Wait();

      // Console reset
      Console.ResetColor();
      Console.CursorVisible = true;

      // Cancel token
      _cts.Cancel();
   }

   public void Dispose() {
      _cts.Dispose();
   }

   public static async Task Main(string[] args = null!) {
      await new Program().MainAsync(args);
   }

   public async Task MainAsync(string[] args = null!) {
      _serviceCollection
         //UI
         .AddSingleton<UiTerminalEngine>()
         .AddSingleton<IUiEngine, UiTerminalEngine>()
         .AddSingleton<IUiBuilder, UiBuilder>()
         //Input
         .AddSingleton<IBindPressed, AppBinds>()
         .AddSingleton<IKeyboardInputService, ConsoleInputService>()
         //Services
         .AddSingleton<IAnyService>(x => x.GetRequiredService<IKeyboardInputService>())

         //Browser launch options
         .AddTransient<IBrowserOptionsStrategy, FirefoxBrowserOptionsStrategy>()
         .AddTransient<IBrowserOptionsStrategy, ChromeBrowserOptionsStrategy>()
         .AddTransient<IBrowserOptionsStrategy, ChromiumBrowserOptionsStrategy>()
         .AddSingleton<BrowserLaunchOptionsFactory>()

         //Operating system
         .AddSingleton<DefaultOsProgramFolderStrategy>()
         .AddSingleton<DefaultOsExecuteFileExtension>()
         .AddSingleton<GetOsSupportedBrowsersStrategy>()
         .AddSingleton<IProcessKiller, ProcessKiller>()

         //Managers
         .AddSingleton<DriveService>()
         .AddSingleton<DataService>()
         .AddSingleton<BrowserService>()
         .AddSingleton<OperatingSystemService>()
         //Folder search strategy
         .AddSingleton<IFolderSearchStrategy, AutoSearchFromDriveStrategy>()
         .AddSingleton<IFolderSearchStrategy, SearchFromDriveFolderStrategy>()
         .AddSingleton<IFolderSearchStrategy, SearchFromApplicationFolder>()
         .AddSingleton<IFolderSearchStrategy, SearchFromCustomPath>()
         .AddSingleton<TypeFolderNameStrategy>()

         //Applications
         .AddSingleton<DefaultOsProgramFolderStrategy>()
         .AddSingleton<FindApplicationsOfNameStrategy>()

         //Startup
         .AddTransient<IStartupStrategy, DetectOperatingSystemStrategy>()
         .AddTransient<IStartupStrategy, InitKernelStrategy>()
         .AddTransient<IStartupStrategy, FindBrowserApplicationsStrategy>()
         .AddTransient<IStartupStrategy, InitApplicationEnvironment>()
         .AddTransient<IStartupStrategy, HelloStepStrategy>()
         .AddTransient<IStartupStrategy, SelectDriveDeviceStrategy>()
         .AddTransient<IStartupStrategy, SelectBrowserFolderStrategy>()
         .AddTransient<IStartupStrategy, InitBrowserEnvironmentStrategy>()
         .AddTransient<IStartupStrategy, SelectBrowserStrategy>()
         .AddSingleton<IStartupContextFactory, StartupContextFactory>()
         .AddSingleton<StartupPipeline>()

         //Profile strategy
         .AddSingleton<IProfileStrategy<LaunchProfile>, EditAppProfileStrategy>()
         .AddSingleton<IProfileStrategy<LaunchProfile>, ChangeProfileLinksStrategy>()
         .AddSingleton<IProfileStrategy<LaunchProfile>, ChangeProfileStrategy>()
         .AddSingleton<IProfileStrategy<LaunchProfile>, DeleteProfileStrategy>()
         .AddSingleton<IProfileStrategy<LaunchProfile>, CreateProfileStrategy>()
         .AddSingleton<IProfileStrategy<AppProfile>, ChangeApplicationPathsProfileStrategy>()
         .AddSingleton<IProfileStrategy<AppProfile>, CreateProfileStrategy>()
         .AddSingleton<IProfileStrategy<AppProfile>, ChangeProfileNameStrategy>()
         .AddSingleton<IProfileStrategy<AppProfile>, DeleteProfileStrategy>()
         .AddSingleton<IProfileStrategy<AppProfile>, SelectProfileStrategy>()
         .AddSingleton<IChangeProfileStrategy, ChangeProfileNameStrategy>()
         .AddSingleton<IChangeProfileStrategy, EditAppProfileStrategy>()
         .AddSingleton<IChangeProfileStrategy, ChangeProfileLinksStrategy>()
         .AddSingleton<IProfileStrategy<LaunchProfile>, SelectProfileStrategy>()
         .AddSingleton<IProfileStrategy<LaunchProfile>, ViewProfilesStrategy>()

         //Main
         .AddTransient<ICoreStrategy, OpenLaunchProfileInBrowserStrategy>()
         .AddTransient<ICoreStrategy, EditLaunchProfileStrategy>()
         .AddTransient<ICoreStrategy, SelectBrowserStartegy>()
         .AddSingleton<ICoreStrategy, ClearBrowserCookiesStrategy>()
         .AddTransient<ICoreStrategy, CloseAllBrowserTabsStrategy>()
         .AddTransient<ICoreStrategy, LaunchBrowserStrategy>()
         .AddTransient<ICoreStrategy, UnlaunchBrowserStrategy>()
         .AddTransient<ICoreStrategy, KillBrowserStrategy>()
         .AddSingleton<IStrategyFactory<IProfileStrategy<AppProfile>>, AppProfileFactory>()
         .AddSingleton<IStrategyFactory<IProfileStrategy<LaunchProfile>>, LaunchProfileFactory>()
         .AddSingleton<IStrategyFactory<ICoreStrategy>, BrowserCommandsFactory>()
         .AddSingleton<CorePipeline>()

         //App
         .AddSingleton<IAppLifetimeManager>(this)
         .AddSingleton<IApp, App>();
      _provider = _serviceCollection.BuildServiceProvider(new ServiceProviderOptions {
         ValidateScopes = true,
         ValidateOnBuild = true
      });
      await InitServices(_provider, _cts.Token);
      await ThrowIfAppIsNull(_provider).Run(_cts.Token);
   }

   private static IApp ThrowIfAppIsNull(ServiceProvider provider) {
      var app = provider.GetService<IApp>();
      if (app == null)
         throw ExceptionExtensions.Critical("IApp is null");
      return app;
   }

   private static async Task InitServices(ServiceProvider provider, CancellationToken cancellationToken = default) {
      var services = provider.GetServices<IAnyService>().ToArray();
      if (services.Any() == false)
         throw ExceptionExtensions.Critical($"Services typeof <{nameof(IAnyService)}> undefined");
      foreach (var service in services) await service.Init(cancellationToken);
   }
}