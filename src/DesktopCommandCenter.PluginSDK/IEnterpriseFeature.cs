using Microsoft.Extensions.DependencyInjection;

namespace DesktopCommandCenter.PluginSDK;

/// <summary>
/// Interface base para qualquer funcionalidade proprietária (Enterprise) da aplicação.
/// As DLLs que implementam esta interface serão carregadas dinamicamente caso o usuário seja PRO.
/// </summary>
public interface IEnterpriseFeature
{
    /// <summary>
    /// O nome da funcionalidade (ex: "Integração IA", "Automação Premium").
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Registra os serviços dessa feature no conteiner de injeção de dependências global do App.
    /// </summary>
    void ConfigureServices(IServiceCollection services);

    /// <summary>
    /// Inicializa a funcionalidade, inserindo botões no NavigationView ou acionando rotinas de background.
    /// </summary>
    void Initialize();
}
