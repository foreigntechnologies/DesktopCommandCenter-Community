# Guia de Empacotamento e Release com Velopack

Este guia orienta no processo de build, publicação e geração do instalador final (`.exe`) e pacote portátil (`.zip`) do **Desktop Command Center (DCC)** utilizando o **Velopack**.

---

## 📋 Pré-requisitos

Antes de iniciar, certifique-se de que os seguintes componentes estão instalados em sua máquina de desenvolvimento:

1. **SDK do .NET 10**: O compilador e runtime do framework utilizado no projeto.
2. **Ferramenta Velopack CLI**: A CLI global do Velopack (`vpk`) que cria os pacotes e instaladores.
   Para instalar ou atualizar a ferramenta globalmente, execute no terminal (PowerShell ou Command Prompt):
   ```powershell
   # Instalar a CLI global do Velopack (caso não possua)
   dotnet tool install -g velopack

   # Atualizar a CLI global para garantir a versão mais recente
   dotnet tool update -g velopack
   ```

---

## 🛠️ Processo Manual Passo a Passo

O processo é composto por duas etapas: compilação em pasta física e geração do instalador a partir dessa pasta.

### Passo 1: Limpeza Prévias
Se você já gerou builds anteriores, limpe as pastas para evitar lixo residual:
```powershell
Remove-Item -Recurse -Force publish/
```

### Passo 2: Publicação com `dotnet publish`
A aplicação WinUI 3 (Windows App SDK) é compilada de forma independente (self-contained) em uma pasta estruturada. Escolha a edição que deseja compilar:

#### 🟢 Edição Community
```powershell
dotnet publish src/DesktopCommandCenter.UI/DesktopCommandCenter.UI.csproj -c ReleaseCommunity -r win-x64 --self-contained true -p:PublishSingleFile=false -p:PublishReadyToRun=true -p:WindowsPackageType=None -p:Version=0.0.1 -o publish/v0.0.1
```

#### 🔵 Edição Pro
```powershell
dotnet publish src/DesktopCommandCenter.UI/DesktopCommandCenter.UI.csproj -c ReleasePro -r win-x64 --self-contained true -p:PublishSingleFile=false -p:PublishReadyToRun=true -p:WindowsPackageType=None -p:Version=0.0.1 -o publish/v0.0.1
```

> [!IMPORTANT]
> **Explicação dos parâmetros utilizados:**
> * `-c ReleaseCommunity` / `ReleasePro`: Define as constantes de compilação da edição desejada (`PRO_VERSION` ou `COMMUNITY_VERSION`).
> * `-r win-x64`: Alvos de arquitetura de 64 bits para Windows.
> * `--self-contained true`: Incorpora o runtime do .NET para que o usuário final não precise instalá-lo manualmente.
> * `-p:PublishSingleFile=false`: **Obrigatório**. WinUI 3 requer carregamento dinâmico de bibliotecas C++ nativas no bootstrapper, o que falha em arquivos únicos empacotados pelo compilador .NET padrão.
> * `-p:WindowsPackageType=None`: Indica que o app rodará fora do sandbox de pacotes MSIX do Windows (unpackaged).
> * `-p:Version=0.0.1`: Determina a versão dos assemblies gerados.
> * `-o publish/v0.0.1`: O diretório de saída onde os arquivos brutos da compilação serão salvos.

---

### Passo 3: Geração dos Instaladores com Velopack (`vpk pack`)
Uma vez gerada a pasta de publicação pelo .NET, usamos a ferramenta `vpk` para compactar a aplicação em um instalador de um clique e uma versão portátil.

```powershell
vpk pack --packId DCC --packVersion 0.0.1 --packDir publish/v0.0.1 --mainExe DesktopCommandCenter.UI.exe --icon "DCC - Logo - Modo Escuro.ico"
```

> [!TIP]
> **O que cada parâmetro faz:**
> * `--packId DCC`: Identificador único do seu aplicativo no Windows (usado nas pastas de instalação).
> * `--packVersion 0.0.1`: Versão semântica (SemVer) que aparecerá nas propriedades do arquivo e será usada para controle de atualizações.
> * `--packDir publish/v0.0.1`: A pasta que contém os arquivos gerados pelo `dotnet publish` no Passo 2.
> * `--mainExe DesktopCommandCenter.UI.exe`: Nome do executável principal do seu app que deve ser lançado ao abrir o atalho.
> * `--icon "DCC - Logo - Modo Escuro.ico"`: O arquivo `.ico` utilizado para os atalhos criados na Área de Trabalho e Menu Iniciar.

---

## 📂 Arquivos Gerados na Pasta `Releases`

Os arquivos finais serão salvos automaticamente em um diretório chamado `Releases/` na raiz do projeto:

* 📦 **`DCC-win-Setup.exe`**: O instalador de um clique para o usuário final. Ao ser executado, ele instala o app no diretório `AppData/Local/DCC`, cria atalhos no Menu Iniciar e Desktop e inicia o app instantaneamente de forma silenciosa.
* 🤐 **`DCC-win-Portable.zip`**: Versão compactada portátil da aplicação, ideal para uso rápido sem instalação.
* 📄 **`RELEASES`**: Arquivo de texto contendo os hashes MD5 e nomes dos pacotes NuGet gerados. **Essencial** caso decida hospedar atualizações automáticas via servidor HTTP/S3 ou GitHub Releases.
* 📦 **`DCC-0.0.1-full.nupkg`**: O pacote NuGet que contém os binários da versão inteira para o sistema de update do Velopack.

---

## ⚡ Automação com Script PowerShell

Para facilitar e evitar a digitação manual de comandos extensos, criamos um script automatizado chamado [`build_release.ps1`](file:///c:/Users/kogli/Desktop/Projetos/Windows/1.%20Desktop%20Command%20Center%20-%20DCC/build_release.ps1).

### Como Usar o Script:

Abra o PowerShell na raiz do projeto e execute:

```powershell
# Gerar a versão Community v0.0.1 (Padrão)
./build_release.ps1 -Version "0.0.1"

# Gerar a versão Pro v0.0.1
./build_release.ps1 -Version "0.0.1" -Config "ReleasePro"
```

O script fará a limpeza, o build completo do dotnet e invocará o Velopack automaticamente, exibindo o caminho final dos executáveis.
