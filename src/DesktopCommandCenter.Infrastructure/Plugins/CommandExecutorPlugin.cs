using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using Microsoft.SemanticKernel;

namespace DesktopCommandCenter.Infrastructure.Plugins;

public sealed class CommandExecutorPlugin
{
    [KernelFunction("run_powershell_command")]
    [Description("Executa um comando no PowerShell do Windows localmente e retorna o texto da saída (stdout e stderr). Use isso para verificar instalações (ex: ng version, node -v), rodar builds ou realizar tarefas que o usuário pedir no sistema dele. ATENÇÃO: Nunca execute comandos destrutivos (como rm -rf, del) sem antes avisar o usuário.")]
    public string RunPowerShellCommand([Description("O comando em PowerShell a ser executado.")] string command)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"\n[IA executando comando em background: {command}]\n");
        Console.ResetColor();

        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-NoProfile -NonInteractive -Command \"{command.Replace("\"", "\\\"")}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8
            };

            using var process = Process.Start(psi);
            if (process == null)
            {
                return "Falha ao iniciar o processo do PowerShell.";
            }

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (!string.IsNullOrWhiteSpace(error))
            {
                if (!string.IsNullOrWhiteSpace(output))
                {
                    return $"Output:\n{output}\n\nError:\n{error}";
                }
                return $"Error:\n{error}";
            }

            if (string.IsNullOrWhiteSpace(output))
            {
                return "O comando foi executado com sucesso, mas não retornou nenhuma saída (output vazio).";
            }

            return output.Trim();
        }
        catch (Exception ex)
        {
            return $"Exceção ao tentar rodar o comando: {ex.Message}";
        }
    }
}
