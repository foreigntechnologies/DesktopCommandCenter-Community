param (
    [switch]$Elevated
)

# 1. Requisitar privilégios de Administrador (Necessário para instalar o Certificado Raiz)
if (-not $Elevated) {
    $isUserAdmin = ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
    if (-not $isUserAdmin) {
        Write-Host "Este script precisa ser executado como Administrador para instalar o certificado temporário de teste." -ForegroundColor Yellow
        Start-Process powershell.exe -ArgumentList "-NoProfile -ExecutionPolicy Bypass -File `"$PSCommandPath`" -Elevated" -Verb RunAs
        exit
    }
}

$ErrorActionPreference = "Stop"

Write-Host "==================================================" -ForegroundColor Cyan
Write-Host "   INSTALADOR DE TESTE LOCAL (Bypass da Loja)     " -ForegroundColor Cyan
Write-Host "==================================================" -ForegroundColor Cyan

$AppPackagesDir = "$PSScriptRoot\src\DesktopCommandCenter.UI\AppPackages"
$MsixPath = Get-ChildItem -Path $AppPackagesDir -Filter "*.msixbundle" -Recurse | Select-Object -First 1
if (-not $MsixPath) {
    $MsixPath = Get-ChildItem -Path $AppPackagesDir -Filter "*.msix" -Recurse | Select-Object -First 1
}

if (-not $MsixPath) {
    Write-Error "Não foi possível encontrar o arquivo .msix ou .msixbundle na pasta AppPackages."
}

Write-Host "Pacote encontrado: $($MsixPath.FullName)" -ForegroundColor Gray

# 2. Criar Certificado de Teste Temporário
$CertName = "DCC Local Test Cert"
$PublisherID = "CN=5850431E-5DBE-4D29-A9A7-3B20FEB17297"

$Cert = Get-ChildItem "Cert:\LocalMachine\Root" | Where-Object { $_.Subject -match $PublisherID -and $_.FriendlyName -eq $CertName }

if (-not $Cert) {
    Write-Host "Gerando certificado de teste temporário..." -ForegroundColor Gray
    # Cria o certificado na store LocalMachine\My
    $Cert = New-SelfSignedCertificate -Type Custom -Subject $PublisherID -KeyUsage DigitalSignature -FriendlyName $CertName -CertStoreLocation "Cert:\LocalMachine\My" -TextExtension @("2.5.29.37={text}1.3.6.1.5.5.7.3.3", "2.5.29.19={text}") -NotAfter (Get-Date).AddYears(1)
    
    # Exporta para PFX
    $PfxPath = "$PSScriptRoot\DCC_Test.pfx"
    $Pwd = ConvertTo-SecureString -String "1234" -Force -AsPlainText
    Export-PfxCertificate -Cert $Cert -FilePath $PfxPath -Password $Pwd | Out-Null
    
    # Instala o Certificado na aba Raiz Confiável
    Write-Host "Instalando certificado na raiz de confiança do seu PC..." -ForegroundColor Gray
    $store = New-Object System.Security.Cryptography.X509Certificates.X509Store "Root", "LocalMachine"
    $store.Open("ReadWrite")
    $store.Add($Cert)
    $store.Close()
} else {
    Write-Host "Certificado de teste já existe no sistema." -ForegroundColor Green
    $PfxPath = "$PSScriptRoot\DCC_Test.pfx"
    if (-not (Test-Path $PfxPath)) {
        $Pwd = ConvertTo-SecureString -String "1234" -Force -AsPlainText
        Export-PfxCertificate -Cert $Cert -FilePath $PfxPath -Password $Pwd | Out-Null
    }
}

# 3. Assinar o pacote
$SignTool = Get-ChildItem -Path "C:\Program Files (x86)\Windows Kits\10\bin" -Filter "signtool.exe" -Recurse | Where-Object { $_.FullName -match "\\x64\\" } | Select-Object -First 1 FullName
if (-not $SignTool) {
    Write-Error "SignTool não encontrado no Windows Kits!"
}

Write-Host "Assinando o pacote localmente..." -ForegroundColor Gray
$signCmd = "& `"$($SignTool.FullName)`" sign /fd SHA256 /a /f `"$PfxPath`" /p `"1234`" `"$($MsixPath.FullName)`""
Invoke-Expression $signCmd

# 4. Fechar o app se estiver aberto e Instalar o pacote!
Write-Host "Verificando se o aplicativo está em execução..." -ForegroundColor Gray
$process = Get-Process -Name "DesktopCommandCenter.UI" -ErrorAction SilentlyContinue
if ($process) {
    Write-Host "O aplicativo está aberto. Fechando para uma instalação limpa..." -ForegroundColor Yellow
    Stop-Process -Name "DesktopCommandCenter.UI" -Force -ErrorAction SilentlyContinue
    Start-Sleep -Seconds 2
}

Write-Host "Instalando o aplicativo no Windows..." -ForegroundColor Cyan
Add-AppxPackage -Path $MsixPath.FullName -ForceUpdateFromAnyVersion

Write-Host "==================================================" -ForegroundColor Green
Write-Host "      APLICATIVO INSTALADO COM SUCESSO!           " -ForegroundColor Green
Write-Host "==================================================" -ForegroundColor Green
Write-Host "Você já pode abrir o 'Desktop Command Center' pelo Menu Iniciar do Windows!" -ForegroundColor Yellow

if ($Elevated) {
    Write-Host "Pressione qualquer tecla para sair..."
    $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown") | Out-Null
}
