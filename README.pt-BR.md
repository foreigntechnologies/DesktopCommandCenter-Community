# Desktop Command Center (DCC)

<p align="center">
  <img src="src/DesktopCommandCenter.UI/Assets/StoreLogo.png" alt="Desktop Command Center Logo" width="200" />
</p>

[![Language: English](https://img.shields.io/badge/Language-English-blue.svg)](README.md)
[![Idioma: Português](https://img.shields.io/badge/Idioma-Português-green.svg)](README.pt-BR.md)
[![Idioma: Español](https://img.shields.io/badge/Idioma-Español-yellow.svg)](README.es.md)

## Visão do Produto
O **Desktop Command Center (DCC)** é uma central de produtividade em formato de barra lateral inteligente, sempre acessível e fixada na lateral da tela do Windows 11. O objetivo é eliminar a troca constante entre aplicativos, reunindo as ferramentas mais utilizadas do dia a dia em um único local.

Projetado para profissionais, desenvolvedores, criadores de conteúdo, analistas e usuários avançados.

## Filosofia: Local First + Cloud Light
- **Local First**: Todos os dados do usuário (Notas, Histórico da Área de Transferência, Configurações) permanecem estritamente na máquina local usando SQLite. Nenhum conteúdo pessoal é armazenado na nuvem.
- **Cloud Light**: A conectividade em nuvem (Firebase Auth) é utilizada exclusivamente como Provedor de Identidade (Google / GitHub / Microsoft) para vincular um ID de Usuário (UID) para validação de licença/assinatura via Stripe.

## Funcionalidades (Community / Gratuito)
- ✨ **Fluent Design System**: Construído nativamente para Windows 11, com fundo translúcido (Mica) e animações suaves.
- 🌍 **Internacionalização em Tempo Real**: Interface totalmente traduzida (Inglês, Português, Espanhol).
- 🎨 **Color Picker**: Capture cores da tela de qualquer lugar.
- 📋 **Clipboard (Smart Clipboard)**: Um serviço em background que captura silenciosamente seu histórico de cópias (Ctrl+C).
- 📝 **Notes (Notas Rápidas)**: Um bloco de notas incrivelmente rápido embutido na barra lateral.
- 🌙 **Awake**: Mantenha o seu PC ativo (previne bloqueio ou suspensão).
- 📌 **Always on Top**: Fixe janelas por cima das outras.
- 🌐 **Translator**: Tradução instantânea de textos.
- ⏱️ **Timer**: Cronômetro e temporizador embutidos.
- 🔄 **Update Center**: Central de atualizações do aplicativo.
- 🔍 **Universal Search**: Pesquisa rápida de tudo no sistema.
- ⌨️ **Command Palette**: Comandos rápidos diretamente da barra lateral.
- 💻 **FutureShell**: Um terminal robusto PTY (Pseudo Console) embutido, capaz de rodar PowerShell, CMD, Bash.

## Funcionalidades PRO (AI & Automation)
- 🤖 **ChatFT (Agente IA)**: Um agente autônomo "Local First" alimentado pelo Microsoft Semantic Kernel e Ollama. Capaz de invocar plugins C# locais. Inclui Visão e transcrição de Voz offline (Whisper).
- 💬 **Prompts de IA (Prompt Library)**: Biblioteca completa de prompts otimizados.
- ⚙️ **Automations**: Crie workflows visuais baseados em regras ou execute scripts personalizados em múltiplas linguagens.
- ☁️ **Cloud Sync**: Sincronize suas notas, atalhos, automações e IA de forma segura na nuvem.
- 👤 **Profiles**: Alterne contextos inteiros entre perfis de Trabalho, Estudo e Pessoal.
- 🧩 **Marketplace and Plugins**: (Em breve) Expanda as capacidades do sistema com novas ferramentas da comunidade.

## Arquitetura e Tecnologias
- **Framework**: .NET 9, Windows App SDK (WinUI 3)
- **Design Pattern**: Clean Architecture + MVVM
- **Banco de Dados**: Entity Framework Core + SQLite
- **Mensageria**: MediatR (Padrão CQRS)
- **Injeção de Dependências**: Microsoft.Extensions.DependencyInjection
- **Autenticação**: FirebaseAuthentication.net (OAuth)

## Como Iniciar
### Pré-requisitos
- Windows 10 (19041) ou Windows 11
- SDK do .NET 10
- Visual Studio 2022 (com workload do Windows App SDK)

### Compilar e Rodar
```bash
# Restaurar dependências
dotnet restore DesktopCommandCenter.slnx

# Compilar a solução
dotnet build DesktopCommandCenter.slnx

# Rodar o projeto de UI
dotnet run --project src/DesktopCommandCenter.UI/DesktopCommandCenter.UI.csproj
```

### Empacotamento & Releases (Velopack)
Para gerar o instalador executável de um clique (`.exe`) e a versão portátil (`.zip`) do DCC, consulte o [Guia de Empacotamento com Velopack](VELOPACK_GUIDE.md).

Você também pode utilizar os scripts automatizados diretamente no PowerShell da raiz:
```powershell
# Gerar instalador 'DCC - Community.exe'
./build_community.ps1 -Version "0.0.1"
```

### Docker & Automação Local
Este repositório possui um `docker-compose.yml` que sobe o servidor de Inteligência Artificial Local (Ollama) e disponibiliza um contêiner auxiliar para empacotar o projeto via Docker.

**1. Executar o Agente IA Local (Ollama)**
```powershell
docker-compose up -d ollama
```

**2. Compilar via Docker (Exige Contêineres do Windows)**
Certifique-se de que o seu Docker Desktop esteja na modalidade "Windows Containers" para conseguir gerar aplicações Desktop/WinUI 3.
```powershell
# Compilar a Edição Community
docker-compose run --rm build-exe 0.0.2 COMMUNITY
```

## Aviso de Segurança
Este projeto utiliza o Firebase para Gestão de Identidade. **Não comite** chaves de Service Account (`.json`) do Firebase ou arquivos de ambiente com credenciais sensíveis. O aplicativo cliente exige apenas a Web API Key pública.

## Licença
Software Proprietário. Todos os direitos reservados.


### 🤖 Modelos de IA sob Demanda
O ChatFT agora detecta automaticamente se você possui modelos instalados no Ollama. Caso seja sua primeira vez, o próprio Desktop Command Center oferecerá opções para baixar e instalar os modelos recomendados (como Phi-3, Llama 3.1 e Gemma 2) com apenas um clique e exibirá o progresso diretamente no chat, sem precisar tocar no terminal!
