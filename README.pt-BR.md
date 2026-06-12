# Desktop Command Center (DCC)

[![Language: English](https://img.shields.io/badge/Language-English-blue.svg)](README.md)
[![Idioma: Português](https://img.shields.io/badge/Idioma-Português-green.svg)](README.pt-BR.md)
[![Idioma: Español](https://img.shields.io/badge/Idioma-Español-yellow.svg)](README.es.md)

## Visão do Produto
O **Desktop Command Center (DCC)** é uma central de produtividade em formato de barra lateral inteligente, sempre acessível e fixada na lateral da tela do Windows 11. O objetivo é eliminar a troca constante entre aplicativos, reunindo as ferramentas mais utilizadas do dia a dia em um único local.

Projetado para profissionais, desenvolvedores, criadores de conteúdo, analistas e usuários avançados.

## Filosofia: Local First + Cloud Light
- **Local First**: Todos os dados do usuário (Notas, Histórico da Área de Transferência, Configurações) permanecem estritamente na máquina local usando SQLite. Nenhum conteúdo pessoal é armazenado na nuvem.
- **Cloud Light**: A conectividade em nuvem (Firebase Auth) é utilizada exclusivamente como Provedor de Identidade (Google / GitHub) para vincular um ID de Usuário (UID) para validação de licença/assinatura via Stripe.

## Funcionalidades
- ✨ **Fluent Design System**: Construído nativamente para Windows 11, com fundo translúcido (Mica) e animações suaves.
- 🤖 **ChatFT (Agente IA)**: Um agente autônomo "Local First" alimentado pelo Microsoft Semantic Kernel e Ollama. Capaz de invocar plugins C# locais (ler seus arquivos, checar hora do sistema) de forma 100% offline.
- 🎙️ **Voz e Visão**: Transcrição local de áudio (microfone e arquivos mp3/wav) usando Whisper.net e suporte a múltiplas modalidades (Visão) analisando imagens via Ollama.
- 📋 **Smart Clipboard**: Um serviço em background que captura silenciosamente seu histórico de cópias (Ctrl+C), permitindo buscar e reutilizar textos anteriores instantaneamente.
- 📝 **Notas Rápidas**: Um bloco de notas incrivelmente rápido embutido na barra lateral.
- 🌍 **Internacionalização em Tempo Real**: Interface totalmente traduzida (Inglês, Português, Espanhol).

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
