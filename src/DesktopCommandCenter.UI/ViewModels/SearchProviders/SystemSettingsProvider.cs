using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DesktopCommandCenter.UI.ViewModels.SearchProviders;

public class SystemSettingsProvider : ISearchProvider
{
        // Principais (Windows 11)
        new() { Title = "Configurações", Description = "Início", Type = "Setting", ActionPath = "ms-settings:" },
        new() { Title = "Sistema", Description = "Configurações de Sistema", Type = "Setting", ActionPath = "ms-settings:display" },
        new() { Title = "Bluetooth e dispositivos", Description = "Configurações de Bluetooth", Type = "Setting", ActionPath = "ms-settings:bluetooth" },
        new() { Title = "Rede e Internet", Description = "Configurações de Rede", Type = "Setting", ActionPath = "ms-settings:network" },
        new() { Title = "Personalização", Description = "Configurações de Personalização", Type = "Setting", ActionPath = "ms-settings:personalization" },
        new() { Title = "Aplicativos", Description = "Configurações de Aplicativos", Type = "Setting", ActionPath = "ms-settings:appsfeatures" },
        new() { Title = "Contas", Description = "Configurações de Contas", Type = "Setting", ActionPath = "ms-settings:accounts" },
        new() { Title = "Hora e idioma", Description = "Configurações de Hora e Idioma", Type = "Setting", ActionPath = "ms-settings:dateandtime" },
        new() { Title = "Jogos", Description = "Configurações de Jogos", Type = "Setting", ActionPath = "ms-settings:gaming" },
        new() { Title = "Acessibilidade", Description = "Configurações de Acessibilidade", Type = "Setting", ActionPath = "ms-settings:easeofaccess" },
        new() { Title = "Privacidade e segurança", Description = "Configurações de Privacidade", Type = "Setting", ActionPath = "ms-settings:privacy" },
        new() { Title = "Windows Update", Description = "Configurações do Windows Update", Type = "Setting", ActionPath = "ms-settings:windowsupdate" },

        // Sistema (Sub-configurações)
        new() { Title = "Tela / Monitor", Description = "Configurações de vídeo, brilho e resolução", Type = "Setting", ActionPath = "ms-settings:display" },
        new() { Title = "Som / Áudio", Description = "Configurações de alto-falantes e microfone", Type = "Setting", ActionPath = "ms-settings:sound" },
        new() { Title = "Notificações", Description = "Ações e notificações de aplicativos", Type = "Setting", ActionPath = "ms-settings:notifications" },
        new() { Title = "Assistente de Foco", Description = "Regras para não perturbe", Type = "Setting", ActionPath = "ms-settings:quiethours" },
        new() { Title = "Bateria e Energia", Description = "Opções de energia e suspensão", Type = "Setting", ActionPath = "ms-settings:powersleep" },
        new() { Title = "Bateria", Description = "Economia de bateria e uso", Type = "Setting", ActionPath = "ms-settings:battery" },
        new() { Title = "Armazenamento", Description = "Uso de disco, limpeza e Sensor de Armazenamento", Type = "Setting", ActionPath = "ms-settings:storagesense" },
        new() { Title = "Modo Tablet", Description = "Configurações do modo tablet", Type = "Setting", ActionPath = "ms-settings:tabletmode" },
        new() { Title = "Multitarefa", Description = "Ajustar janelas e áreas de trabalho virtuais", Type = "Setting", ActionPath = "ms-settings:multitasking" },
        new() { Title = "Projetar neste PC", Description = "Configurações de projeção sem fio", Type = "Setting", ActionPath = "ms-settings:project" },
        new() { Title = "Área de Transferência", Description = "Histórico do clipboard e sincronização", Type = "Setting", ActionPath = "ms-settings:clipboard" },
        new() { Title = "Área de Trabalho Remota", Description = "Habilitar Remote Desktop", Type = "Setting", ActionPath = "ms-settings:remotedesktop" },
        new() { Title = "Sobre o Computador", Description = "Informações do sistema, processador e memória", Type = "Setting", ActionPath = "ms-settings:about" },

        // Dispositivos
        new() { Title = "Bluetooth", Description = "Gerenciar dispositivos Bluetooth e outros", Type = "Setting", ActionPath = "ms-settings:bluetooth" },
        new() { Title = "Impressoras e Scanners", Description = "Adicionar ou remover dispositivos de impressão", Type = "Setting", ActionPath = "ms-settings:printers" },
        new() { Title = "Mouse", Description = "Configurações de botões e ponteiro", Type = "Setting", ActionPath = "ms-settings:mousetouchpad" },
        new() { Title = "Touchpad", Description = "Configurações do touchpad", Type = "Setting", ActionPath = "ms-settings:devices-touchpad" },
        new() { Title = "Digitação / Teclado", Description = "Configurações de digitação e autocorreção", Type = "Setting", ActionPath = "ms-settings:typing" },
        new() { Title = "Caneta e Windows Ink", Description = "Opções de stylus", Type = "Setting", ActionPath = "ms-settings:pen" },
        new() { Title = "Reprodução Automática", Description = "Configurações de AutoPlay para pendrives", Type = "Setting", ActionPath = "ms-settings:autoplay" },
        new() { Title = "USB", Description = "Configurações de notificações USB", Type = "Setting", ActionPath = "ms-settings:usb" },

        // Rede e Internet
        new() { Title = "Status da Rede", Description = "Status da conexão de internet", Type = "Setting", ActionPath = "ms-settings:network-status" },
        new() { Title = "Wi-Fi", Description = "Rede sem fio, senhas e conexões", Type = "Setting", ActionPath = "ms-settings:network-wifi" },
        new() { Title = "Ethernet / Rede Cabeada", Description = "Configurações de rede com fio", Type = "Setting", ActionPath = "ms-settings:network-ethernet" },
        new() { Title = "Conexão Discada", Description = "Dial-up", Type = "Setting", ActionPath = "ms-settings:network-dialup" },
        new() { Title = "VPN", Description = "Redes Privadas Virtuais", Type = "Setting", ActionPath = "ms-settings:network-vpn" },
        new() { Title = "Modo Avião", Description = "Desativar conexões sem fio", Type = "Setting", ActionPath = "ms-settings:network-airplanemode" },
        new() { Title = "Hotspot Móvel", Description = "Compartilhar internet", Type = "Setting", ActionPath = "ms-settings:network-mobilehotspot" },
        new() { Title = "Uso de Dados", Description = "Limite e uso de banda", Type = "Setting", ActionPath = "ms-settings:datausage" },
        new() { Title = "Proxy", Description = "Servidor Proxy de internet", Type = "Setting", ActionPath = "ms-settings:network-proxy" },

        // Personalização
        new() { Title = "Tela de Fundo / Wallpaper", Description = "Mudar imagem de fundo", Type = "Setting", ActionPath = "ms-settings:personalization-background" },
        new() { Title = "Cores / Modo Escuro", Description = "Mudar entre tema claro e escuro, cor de destaque", Type = "Setting", ActionPath = "ms-settings:colors" },
        new() { Title = "Tela de Bloqueio", Description = "Personalizar lockscreen", Type = "Setting", ActionPath = "ms-settings:lockscreen" },
        new() { Title = "Temas", Description = "Mudar cursores, sons e temas completos", Type = "Setting", ActionPath = "ms-settings:themes" },
        new() { Title = "Fontes", Description = "Instalar e visualizar tipografias", Type = "Setting", ActionPath = "ms-settings:fonts" },
        new() { Title = "Menu Iniciar", Description = "Personalizar blocos e lista de apps", Type = "Setting", ActionPath = "ms-settings:personalization-start" },
        new() { Title = "Barra de Tarefas", Description = "Ocultar, alinhar ícones", Type = "Setting", ActionPath = "ms-settings:taskbar" },

        // Aplicativos
        new() { Title = "Aplicativos Instalados", Description = "Desinstalar ou gerenciar programas", Type = "Setting", ActionPath = "ms-settings:appsfeatures" },
        new() { Title = "Aplicativos Padrão", Description = "Escolher navegador e reprodutor padrão", Type = "Setting", ActionPath = "ms-settings:defaultapps" },
        new() { Title = "Mapas Offline", Description = "Mapas salvos", Type = "Setting", ActionPath = "ms-settings:maps" },
        new() { Title = "Aplicativos de Inicialização", Description = "Programas que iniciam com o Windows", Type = "Setting", ActionPath = "ms-settings:startupapps" },

        // Contas
        new() { Title = "Suas Informações", Description = "Foto de perfil e dados da conta", Type = "Setting", ActionPath = "ms-settings:yourinfo" },
        new() { Title = "E-mail e Contas", Description = "Contas de aplicativos", Type = "Setting", ActionPath = "ms-settings:emailandaccounts" },
        new() { Title = "Opções de Entrada", Description = "Senha, PIN, Biometria, Windows Hello", Type = "Setting", ActionPath = "ms-settings:signinoptions" },
        new() { Title = "Família e Outros Usuários", Description = "Adicionar contas de convidado/família", Type = "Setting", ActionPath = "ms-settings:otherusers" },
        new() { Title = "Sincronizar Configurações", Description = "Sincronizar entre dispositivos", Type = "Setting", ActionPath = "ms-settings:sync" },

        // Hora e Idioma
        new() { Title = "Data e Hora", Description = "Ajustar fuso horário e relógio", Type = "Setting", ActionPath = "ms-settings:dateandtime" },
        new() { Title = "Região", Description = "País e formato de dados", Type = "Setting", ActionPath = "ms-settings:region" },
        new() { Title = "Idioma", Description = "Pacotes de tradução e teclado", Type = "Setting", ActionPath = "ms-settings:regionlanguage" },

        // Jogos
        new() { Title = "Xbox Game Bar", Description = "Atalhos e sobreposição de jogos", Type = "Setting", ActionPath = "ms-settings:gaming-gamebar" },
        new() { Title = "Capturas", Description = "Configurações de gravação de tela", Type = "Setting", ActionPath = "ms-settings:gaming-gamedvr" },
        new() { Title = "Modo de Jogo", Description = "Otimizar PC para jogos", Type = "Setting", ActionPath = "ms-settings:gaming-gamemode" },

        // Acessibilidade
        new() { Title = "Tamanho do Texto", Description = "Aumentar fonte na tela", Type = "Setting", ActionPath = "ms-settings:easeofaccess-display" },
        new() { Title = "Ponteiro do Mouse", Description = "Tamanho e cor do cursor", Type = "Setting", ActionPath = "ms-settings:easeofaccess-mousepointer" },
        new() { Title = "Lupa", Description = "Zoom na tela", Type = "Setting", ActionPath = "ms-settings:easeofaccess-magnifier" },
        new() { Title = "Filtros de Cor", Description = "Daltonismo e alto contraste", Type = "Setting", ActionPath = "ms-settings:easeofaccess-colorfilter" },
        new() { Title = "Narrador", Description = "Leitor de tela", Type = "Setting", ActionPath = "ms-settings:easeofaccess-narrator" },

        // Privacidade
        new() { Title = "Privacidade Geral", Description = "Permissões básicas do Windows", Type = "Setting", ActionPath = "ms-settings:privacy-general" },
        new() { Title = "Pesquisa e Permissões", Description = "Histórico de busca e Cortana", Type = "Setting", ActionPath = "ms-settings:cortana" },
        new() { Title = "Permissões: Localização", Description = "GPS e acessos", Type = "Setting", ActionPath = "ms-settings:privacy-location" },
        new() { Title = "Permissões: Câmera", Description = "Quais apps usam a webcam", Type = "Setting", ActionPath = "ms-settings:privacy-webcam" },
        new() { Title = "Permissões: Microfone", Description = "Quais apps ouvem seu mic", Type = "Setting", ActionPath = "ms-settings:privacy-microphone" },
        new() { Title = "Aplicativos em Segundo Plano", Description = "Quais rodam ocultos", Type = "Setting", ActionPath = "ms-settings:privacy-backgroundapps" },

        // Atualização e Segurança
        new() { Title = "Windows Update", Description = "Atualizar o sistema operacional", Type = "Setting", ActionPath = "ms-settings:windowsupdate" },
        new() { Title = "Segurança do Windows", Description = "Antivírus, Defender, Firewall", Type = "Setting", ActionPath = "ms-settings:windowsdefender" },
        new() { Title = "Backup", Description = "Backup de arquivos e OneDrive", Type = "Setting", ActionPath = "ms-settings:backup" },
        new() { Title = "Solução de Problemas", Description = "Troubleshooting de erros", Type = "Setting", ActionPath = "ms-settings:troubleshoot" },
        new() { Title = "Recuperação / Formatar", Description = "Restaurar o PC ou inicialização avançada", Type = "Setting", ActionPath = "ms-settings:recovery" },
        new() { Title = "Ativação do Windows", Description = "Chave do produto", Type = "Setting", ActionPath = "ms-settings:activation" },
        new() { Title = "Localizar meu Dispositivo", Description = "Rastrear PC perdido", Type = "Setting", ActionPath = "ms-settings:findmydevice" },
        new() { Title = "Para Desenvolvedores", Description = "Sideload e opções dev", Type = "Setting", ActionPath = "ms-settings:developers" }
    };

    public Task<IEnumerable<SearchResultItem>> SearchAsync(string query, CancellationToken token)
    {
        var lowerQuery = query.ToLowerInvariant();
        var results = _allSettings.Where(s => s.Title.ToLowerInvariant().Contains(lowerQuery) || s.Description.ToLowerInvariant().Contains(lowerQuery));
        return Task.FromResult(results);
    }
}
