using System;
using System.ComponentModel;
using System.IO;
using Microsoft.SemanticKernel;

namespace DesktopCommandCenter.Infrastructure.Plugins;

public sealed class SystemInfoPlugin
{
    [KernelFunction("get_current_time")]
    [Description("Obtém a data e hora atual do sistema local. Útil para quando o usuário perguntar que horas são ou o dia de hoje.")]
    public string GetCurrentTime()
    {
        return DateTime.Now.ToString("F");
    }

    [KernelFunction("read_text_file")]
    [Description("Lê e retorna o conteúdo em texto de um arquivo local dado o seu caminho absoluto. Útil se o usuário pedir para você ler, resumir ou explicar um arquivo específico (ex: C:\\Users\\...\\arquivo.txt).")]
    public string ReadTextFile([Description("O caminho absoluto do arquivo a ser lido")] string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                return $"O arquivo não foi encontrado no caminho: {filePath}";
            }
            
            return File.ReadAllText(filePath);
        }
        catch (Exception ex)
        {
            return $"Erro ao ler o arquivo: {ex.Message}";
        }
    }
}
