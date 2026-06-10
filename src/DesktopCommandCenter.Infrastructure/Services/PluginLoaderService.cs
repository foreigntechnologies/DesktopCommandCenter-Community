using DesktopCommandCenter.Application.Interfaces;
using DesktopCommandCenter.PluginSDK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DesktopCommandCenter.Infrastructure.Services;

public class PluginLoaderService
{
    private readonly string _pluginsDirectory;

    public PluginLoaderService()
    {
        // Define a pasta onde os plugins privados serão salvos (ex: AppData/Local/DCC/Plugins)
        string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        _pluginsDirectory = Path.Combine(appData, "DesktopCommandCenter", "Plugins");
    }

    /// <summary>
    /// Exclui todas as DLLs proprietárias caso o plano seja rebaixado para Free.
    /// </summary>
    public void DeleteEnterprisePlugins()
    {
        if (Directory.Exists(_pluginsDirectory))
        {
            try
            {
                Directory.Delete(_pluginsDirectory, true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao deletar plugins: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Carrega as DLLs privadas caso a pasta exista e instancia as funcionalidades Enterprise.
    /// </summary>
    public IEnumerable<IEnterpriseFeature> LoadEnterpriseFeatures()
    {
        var features = new List<IEnterpriseFeature>();

        if (!Directory.Exists(_pluginsDirectory))
            return features;

        string[] dllFiles = Directory.GetFiles(_pluginsDirectory, "*.dll");

        foreach (string file in dllFiles)
        {
            try
            {
                // Carrega a DLL em memória
                Assembly assembly = Assembly.LoadFrom(file);

                // Procura classes que implementam IEnterpriseFeature
                var types = assembly.GetTypes()
                    .Where(t => typeof(IEnterpriseFeature).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

                foreach (var type in types)
                {
                    if (Activator.CreateInstance(type) is IEnterpriseFeature feature)
                    {
                        features.Add(feature);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao carregar o plugin {file}: {ex.Message}");
            }
        }

        return features;
    }
}
