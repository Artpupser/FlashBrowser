using System.Diagnostics;

namespace FlashBrowser.Core.Killers;

public sealed class ProcessKiller : IProcessKiller {
   public IEnumerable<KillResult> KillByPrefix(string prefix) {
      foreach (var el in Process.GetProcesses()) {
         if (el.ProcessName.StartsWith(prefix) == false)
            continue;
         yield return Kill(el);
         el.Dispose();
      }
   }

   public IEnumerable<KillResult> KillByContains(string name) {
      foreach (var el in Process.GetProcesses()) {
         if (el.ProcessName.Contains(name) == false)
            continue;
         yield return Kill(el);
         el.Dispose();
      }
   }

   public KillResult Kill(Process process) {
      try {
         process.Kill();
         return new KillResult(process.ProcessName, process.Id);
      } catch (Exception e) {
         return new KillResult(process.ProcessName, process.Id, e);
      }
   }
}