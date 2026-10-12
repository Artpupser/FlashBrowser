using FlashBrowser.Core.Os;

using PupaLib.FileIO;

namespace FlashBrowser.Core.FileSearch;

public sealed class FindApplicationsOfNameStrategy(DefaultOsProgramFolderStrategy defaultOsProgramFolderStrategy, DefaultOsExecuteFileExtension defaultOsExecuteFileExtension) {
   public string Title => "Find applications of name";

   public IEnumerable<VirtualFile> Execute(string name, CancellationToken cancellationToken) {
      if (cancellationToken.IsCancellationRequested)
         yield break;
      if (!defaultOsProgramFolderStrategy.Execute().Out(out var defaultApplicationsFolder))
         yield break;
      var dir = new DirectoryInfo(defaultApplicationsFolder.MyPath);
      var enumerateOption = new EnumerationOptions {
         MatchCasing = MatchCasing.CaseInsensitive,
         IgnoreInaccessible = true,
         RecurseSubdirectories = true,
         MaxRecursionDepth = 3,
      };
      foreach (var file in dir.EnumerateFiles($"{name}*{defaultOsExecuteFileExtension.Execute()}", enumerateOption))
      {
         if (file.Name.Contains(name) && VirtualFile.GetFile(file.FullName).Out(out var virtualFile))
            yield return virtualFile;
      }
   }
}