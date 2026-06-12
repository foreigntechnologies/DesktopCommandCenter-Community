# Imagem base: Windows Server Core com .NET 9 SDK
# Necessário para compilar aplicações WinUI 3 nativas do Windows
FROM mcr.microsoft.com/dotnet/sdk:9.0-windowsservercore-ltsc2022

# Configurar o Shell padrão para PowerShell
SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]

# Instalar o Chocolatey (Gerenciador de pacotes do Windows)
RUN Set-ExecutionPolicy Bypass -Scope Process -Force; \
    [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072; \
    iex ((New-Object System.Net.WebClient).DownloadString('https://community.chocolatey.org/install.ps1'))

# Instalar o NSIS (Nullsoft Scriptable Install System) via Chocolatey para compilar o Instalador
RUN choco install nsis -y

# Definir o diretório de trabalho
WORKDIR /app

# Copiar o script de entrypoint
COPY builder-entrypoint.ps1 /builder-entrypoint.ps1

# Configurar o entrypoint padrão
ENTRYPOINT ["powershell", "-ExecutionPolicy", "Bypass", "-File", "/builder-entrypoint.ps1"]
