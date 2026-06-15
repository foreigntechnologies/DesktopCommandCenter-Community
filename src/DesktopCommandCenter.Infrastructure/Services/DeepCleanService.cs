using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DesktopCommandCenter.Infrastructure.Services;

public static class DeepCleanService
{
    /// <summary>
    /// Scans well-known directories for folders matching the app name and deletes them.
    /// Also clears the user's temp directory.
    /// </summary>
    public static async Task CleanLeftoversAsync(string appName, string? publisher = null)
    {
        await Task.Run(() =>
        {
            var targetDirs = new List<string>
            {
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) // ProgramData
            };

            var keywords = new List<string>();
            if (!string.IsNullOrWhiteSpace(appName))
                keywords.Add(CleanString(appName));
            
            if (!string.IsNullOrWhiteSpace(publisher))
                keywords.Add(CleanString(publisher));

            // Prevent extremely generic keywords from deleting the whole OS
            keywords.RemoveAll(k => k.Length <= 2 || k.Equals("microsoft", StringComparison.OrdinalIgnoreCase) || k.Equals("windows", StringComparison.OrdinalIgnoreCase));

            if (keywords.Count > 0)
            {
                foreach (var baseDir in targetDirs)
                {
                    if (Directory.Exists(baseDir))
                    {
                        try
                        {
                            var subDirs = Directory.GetDirectories(baseDir);
                            foreach (var dir in subDirs)
                            {
                                var dirName = CleanString(Path.GetFileName(dir));
                                foreach (var keyword in keywords)
                                {
                                    // Match if the folder name is identical, or if it strongly contains the keyword
                                    if (dirName.Equals(keyword, StringComparison.OrdinalIgnoreCase) || 
                                        dirName.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                                    {
                                        try
                                        {
                                            Directory.Delete(dir, true);
                                        }
                                        catch { /* Access denied or in use, ignore */ }
                                        break; // move to next directory
                                    }
                                }
                            }
                        }
                        catch { /* Access denied to baseDir */ }
                    }
                }
            }

            // Always clear the user's Temp folder as a bonus for Deep Clean
            try
            {
                string tempPath = Path.GetTempPath();
                if (Directory.Exists(tempPath))
                {
                    var di = new DirectoryInfo(tempPath);
                    foreach (var file in di.GetFiles())
                    {
                        try { file.Delete(); } catch { }
                    }
                    foreach (var dir in di.GetDirectories())
                    {
                        try { dir.Delete(true); } catch { }
                    }
                }
            }
            catch { }
        });
    }

    private static string CleanString(string input)
    {
        // Remove spaces and special chars to make matching easier
        return input.Replace(" ", "").Replace(".", "").Replace("-", "").Replace("_", "");
    }
}
