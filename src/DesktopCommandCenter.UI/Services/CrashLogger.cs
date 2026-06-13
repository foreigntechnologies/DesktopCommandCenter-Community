using System;
using System.IO;
using System.Threading.Tasks;

namespace DesktopCommandCenter.UI.Services;

public static class CrashLogger
{
    private static readonly string LogDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "DCC", "Logs");

    public static void Initialize()
    {
        // Garante que a pasta de logs exista
        if (!Directory.Exists(LogDirectory))
        {
            Directory.CreateDirectory(LogDirectory);
        }

        // Exceções do AppDomain (erros genéricos não tratados)
        AppDomain.CurrentDomain.UnhandledException += (s, e) =>
        {
            LogException((Exception)e.ExceptionObject, "AppDomain");
        };

        // Exceções de Tasks Assíncronas
        TaskScheduler.UnobservedTaskException += (s, e) =>
        {
            LogException(e.Exception, "TaskScheduler");
            e.SetObserved(); // Previne que a exceção derrube o app (se possível)
        };

        // Exceções do WinUI / XAML
        Microsoft.UI.Xaml.Application.Current.UnhandledException += (s, e) =>
        {
            LogException(e.Exception, "WinUI");
            e.Handled = true; // Tenta evitar o crash (nem sempre funciona para falhas de renderização nativa)
        };
    }

    public static void LogException(Exception ex, string source)
    {
        try
        {
            string fileName = $"crash_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
            string filePath = Path.Combine(LogDirectory, fileName);

            string logContent = $"""
                ====================================================
                DESKTOP COMMAND CENTER - RELATORIO DE CRASH
                ====================================================
                Data/Hora : {DateTime.Now:dd/MM/yyyy HH:mm:ss}
                Origem    : {source}
                Versão    : {System.Reflection.Assembly.GetExecutingAssembly().GetName().Version}
                OS        : {Environment.OSVersion.VersionString}
                ====================================================
                TIPO: {ex.GetType().FullName}
                MENSAGEM: {ex.Message}
                
                STACK TRACE:
                {ex.StackTrace}
                
                INNER EXCEPTION:
                {ex.InnerException?.Message ?? "Nenhuma"}
                {ex.InnerException?.StackTrace}
                ====================================================
                Para reportar este erro, envie este arquivo para:
                suporte@foreigntechnologies.com.br
                """;

            File.WriteAllText(filePath, logContent);
        }
        catch
        {
            // Se falhar até para escrever o log, não há o que fazer.
        }
    }

    public static string GetLogsDirectory() => LogDirectory;
}
