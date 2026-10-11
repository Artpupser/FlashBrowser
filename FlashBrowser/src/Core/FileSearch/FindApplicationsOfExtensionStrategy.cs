using FlashBrowser.Core.Os;

using PupaLib.FileIO;

namespace FlashBrowser.Core.FileSearch;

public sealed class FindApplicationsOfExtensionStrategy(DefaultOsProgramFolderStrategy defaultOsProgramFolderStrategy) {
   public string Title => "Find applications of extension";

   public IEnumerable<VirtualFile> Execute(string extension, CancellationToken cancellationToken) {
      if (cancellationToken.IsCancellationRequested)
         yield break;
      if (!defaultOsProgramFolderStrategy.Execute().Out(out var defaultApplicationsFolder))
         yield break;
      var dir = new DirectoryInfo(defaultApplicationsFolder.MyPath);
      var enumerateOption = new EnumerationOptions {
         MatchCasing = MatchCasing.CaseInsensitive,
         IgnoreInaccessible = true
      };
      foreach (var file in dir.EnumerateFiles($"*.{extension}", enumerateOption))
         if (file.Extension == $".{extension}" && VirtualFile.GetFile(file.FullName).Out(out var virtualFile))
            yield return virtualFile;
   }
}