using System.Diagnostics;

namespace FlashBrowser.Core.Killers;

public interface IProcessKiller {
   public IEnumerable<KillResult> KillByPrefix(string prefix);
   public IEnumerable<KillResult> KillByContains(string name);
   public KillResult Kill(Process process);
}