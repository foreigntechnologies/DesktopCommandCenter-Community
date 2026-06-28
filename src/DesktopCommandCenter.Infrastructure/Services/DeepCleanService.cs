using DesktopCommandCenter.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DesktopCommandCenter.Infrastructure.Services;

public class DeepCleanService : IDeepCleanService
{
    private readonly string[] _blacklistedNames = new[]
    {
        "Windows", "Microsoft", "System", "Temp", "AppData", "Local", "Roaming", 
        "LocalLow", "Program Files", "ProgramData", "Users", "Intel", "AMD", "NVIDIA"
    };

    public Task<List<string>> CleanLeftoversAsync(string appName, string publisherName)
    {
        return Task.Run(() =>
        {
            var deletedPaths = new List<string>();

            if (string.IsNullOrWhiteSpace(appName))
                return deletedPaths;

            // Normalize and extract meaningful parts (e.g., removing versions, "Inc.", etc.)
            var safeAppName = ExtractCoreName(appName);
            var safePublisherName = ExtractCoreName(publisherName);

            // Se o nome extraído for perigoso ou muito curto (menos de 3 chars), ignorar limpeza profunda
            if (IsBlacklistedOrTooShort(safeAppName))
                return deletedPaths;

            // Busca por pastas em locais comuns
            var baseDirectories = new[]
            {
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86)
            };

            foreach (var baseDir in baseDirectories)
            {
                if (!Directory.Exists(baseDir)) continue;

                try
                {
                    // Obtém todos os diretórios do nível superior
                    var subDirs = Directory.GetDirectories(baseDir);
                    foreach (var subDir in subDirs)
                    {
                        var dirName = Path.GetFileName(subDir);
                        if (string.IsNullOrEmpty(dirName)) continue;

                        // Correspondência EXATA ou quase exata para evitar falsos positivos
                        if (dirName.Equals(safeAppName, StringComparison.OrdinalIgnoreCase) ||
                            (!string.IsNullOrEmpty(safePublisherName) && dirName.Equals(safePublisherName, StringComparison.OrdinalIgnoreCase)))
                        {
                            try
                            {
                                Directory.Delete(subDir, true);
                                deletedPaths.Add(subDir);
                            }
                            catch
                            {
                                // Pode falhar se arquivos estiverem em uso. Ignoramos e continuamos.
                            }
                        }
                    }
                }
                catch
                {
                    // Permissão negada no baseDir.
                }
            }

            return deletedPaths;
        });
    }

    private string ExtractCoreName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return string.Empty;
        
        // Remove "Inc.", "Corp.", "(R)", "TM" e versões como "v1.0"
        var clean = name.Replace("Inc.", "", StringComparison.OrdinalIgnoreCase)
                        .Replace("Inc", "", StringComparison.OrdinalIgnoreCase)
                        .Replace("Corp.", "", StringComparison.OrdinalIgnoreCase)
                        .Replace("Corp", "", StringComparison.OrdinalIgnoreCase)
                        .Replace("(R)", "", StringComparison.OrdinalIgnoreCase)
                        .Replace("TM", "", StringComparison.OrdinalIgnoreCase);

        // Se contiver versão ou traços, tenta pegar a primeira parte
        if (clean.Contains("-"))
            clean = clean.Split('-')[0];

        return clean.Trim();
    }

    private bool IsBlacklistedOrTooShort(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length < 3) return true;

        foreach (var bad in _blacklistedNames)
        {
            if (name.Equals(bad, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }
}
