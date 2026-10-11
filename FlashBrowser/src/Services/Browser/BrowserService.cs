using System.Collections.Concurrent;

using FlashBrowser.Services.Data;

using PupaLib.Core;
using PupaLib.FileIO;

using PuppeteerSharp;

namespace FlashBrowser.Services.Browser;

public interface IBrowserOptionsStrategy {
   public SupportedBrowser BrowserType { get; }
   public LaunchOptions Execute(string exePath, string profilePath);
}

public sealed class FirefoxBrowserOptionsStrategy : IBrowserOptionsStrategy {
   public SupportedBrowser BrowserType => SupportedBrowser.Firefox;

   public LaunchOptions Execute(string exePath, string profilePath) {
      return new LaunchOptions {
         Browser = BrowserType,
         Headless = false,
         HeadlessMode = HeadlessMode.False,
         ExecutablePath = exePath,
         Args = ["--start-debugger-server=9222", $"--profile=\"{profilePath}\""],
         DumpIO = true
      };
   }
}

public sealed class ChromeBrowserOptionsStrategy : IBrowserOptionsStrategy {
   public SupportedBrowser BrowserType => SupportedBrowser.Chrome;

   public LaunchOptions Execute(string exePath, string profilePath) {
      return new LaunchOptions {
         Browser = BrowserType,
         Headless = false,
         HeadlessMode = HeadlessMode.False,
         ExecutablePath = exePath,
         Args = [$"--user-data-dir=\"{profilePath}\""],
         DumpIO = false
      };
   }
}

public sealed class ChromiumBrowserOptionsStrategy : IBrowserOptionsStrategy {
   public SupportedBrowser BrowserType => SupportedBrowser.Chromium;

   public LaunchOptions Execute(string exePath, string profilePath) {
      return new LaunchOptions {
         Browser = BrowserType,
         Headless = false,
         HeadlessMode = HeadlessMode.False,
         ExecutablePath = exePath,
         Args = [$"--user-data-dir=\"{profilePath}\""],
         DumpIO = false
      };
   }
}

public sealed class BrowserLaunchOptionsFactory {
   public IBrowserOptionsStrategy Create(SupportedBrowser supportedBrowser) {
      return supportedBrowser switch {
         SupportedBrowser.Firefox => new FirefoxBrowserOptionsStrategy(),
         SupportedBrowser.Chrome => new ChromeBrowserOptionsStrategy(),
         SupportedBrowser.Chromium => new ChromiumBrowserOptionsStrategy(),
         _ => throw new ArgumentException($"{supportedBrowser} browser otpions strategy not found.")
      };
   }
}

public sealed class BrowserService {
   private readonly ConcurrentDictionary<SupportedBrowser, string> _browserApps;
   private readonly ConcurrentDictionary<SupportedBrowser, VirtualFolder> _browserFolders;
   private readonly BrowserLaunchOptionsFactory _browserLaunchOptionsFactory;
   private readonly List<IPage> _openedPages;

   public BrowserService(DataService dataService, BrowserLaunchOptionsFactory browserLaunchOptionsFactory) {
      _browserFolders = new ConcurrentDictionary<SupportedBrowser, VirtualFolder>();
      _browserApps = new ConcurrentDictionary<SupportedBrowser, string>();
      _browserLaunchOptionsFactory = browserLaunchOptionsFactory;
      _openedPages = [];
      DataService = dataService;
   }

   public IReadOnlyDictionary<SupportedBrowser, VirtualFolder> BrowserFolders => _browserFolders;
   public IReadOnlyDictionary<SupportedBrowser, string> BrowserApps => _browserApps;
   public IReadOnlyList<IPage> OpenedPages => _openedPages;
   public IBrowser? Browser { get; private set; }
   public VirtualFolder BrowserEnvironmentFolder { get; set; } = null!;
   public SupportedBrowser ActiveBrowser { get; private set; }

   public DataService DataService { get; }

   public async Task DeactivateBrowser() {
      if (Browser == null)
         return;
      await CloseAllPages();
      await Browser.CloseAsync();
      Browser.Process.Kill();
      Browser.Process.Dispose();
      await Browser.DisposeAsync();
      Browser = null;
   }

   public async Task LaunchBrowser() {
      Browser = await Puppeteer.LaunchAsync(_browserLaunchOptionsFactory
         .Create(ActiveBrowser)
         .Execute(BrowserApps[ActiveBrowser], BrowserFolders[ActiveBrowser].MyPath));
   }


   public async Task CloseAllPages(Action<IPage>? closeCallback = null) {
      if (Browser == null)
         return;

      foreach (var page in _openedPages) {
         await page.CloseAsync();
         closeCallback?.Invoke(page);
         await page.DisposeAsync();
      }

      _openedPages.Clear();
   }

   public string GetNameFromEnum(SupportedBrowser supportedBrowser) {
      return supportedBrowser.ToString().ToLower();
   }

   public void SetActiveBrowser(SupportedBrowser supportedBrowser) {
      ActiveBrowser = supportedBrowser;
   }

   public void SetFolderProfile(SupportedBrowser supportedBrowser, VirtualFolder folder) {
      _browserFolders[supportedBrowser] = folder;
   }

   public Option SetApplication(SupportedBrowser supportedBrowser, string path) {
      if (File.Exists(path)) {
         _browserApps[supportedBrowser] = path;
         return Option.Ok();
      }

      return Option.Fail();
   }

   public async Task NewPageTab(string url) {
      if (Browser == null)
         return;
      try {
         var page = await Browser.NewPageAsync(new CreatePageOptions {
            Type = CreatePageType.Tab,
            WindowBounds = new WindowBounds {
               Width = 1920,
               Height = 1090,
               WindowState = WindowState.Fullscreen
            }
         });
         _ = page.GoToAsync(url);
         _ = page.ResizeAsync(1920, 1080);
         _openedPages.Add(page);
      } catch (Exception) {
         // ignored
      }
   }

   public void ForEachSupportedBrowsers(Action<SupportedBrowser, int> callback) {
      foreach (var item in _browserApps.Keys) {
         if (OperatingSystem.IsWindows() && item == SupportedBrowser.Chromium)
            continue;
         callback(item, (int)item);
      }
   }
}