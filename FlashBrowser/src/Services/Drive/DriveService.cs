using PupaLib.Core;
using PupaLib.FileIO;

namespace FlashBrowser.Services.Drive;

public sealed class DriveService {
   public DriveInfo? UsableDrive { get; set; }

   public Option<VirtualFolder> FindFolderFromUsableDrive(string folderName) {
      var drive = UsableDrive!;
      var driveFolderOption = VirtualFolder.GetFolder(drive.RootDirectory.FullName);
      if (driveFolderOption.Out(out var driveFolder))
         foreach (var folder in driveFolder.EnumerateFolders(SearchOption.TopDirectoryOnly))
            if (folder.Exists && folder.Name == folderName)
               return Option<VirtualFolder>.Ok(folder);

      return Option<VirtualFolder>.Fail();
   }

   public IEnumerable<DriveInfo> GetDrives() {
      return DriveInfo.GetDrives();
   }

   public IEnumerable<DriveInfo> GetActiveDrives() {
      return DriveInfo.GetDrives().Where(x => x.IsReady);
   }

   public IEnumerable<DriveInfo> GetActiveEjectableDrives() {
      return DriveInfo.GetDrives().Where(x => x.IsReady && x.DriveType == DriveType.Removable);
   }
}