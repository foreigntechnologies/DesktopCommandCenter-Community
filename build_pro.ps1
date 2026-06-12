param (
    [Parameter(Mandatory=$false)]
    [string]$Version = "0.0.1",
    
    [Parameter(Mandatory=$false)]
    [string]$CertPath = "",

    [Parameter(Mandatory=$false)]
    [string]$CertPassword = ""
)

$ErrorActionPreference = "Stop"

# Remove the 'v' prefix if present (e.g., 'v0.0.1' becomes '0.0.1') for MSBuild compatibility
$CleanVersion = $Version -replace '^v', ''

Write-Host "==================================================" -ForegroundColor Cyan
Write-Host "        DCC PRO WIZARD PACKAGER (NSIS)           " -ForegroundColor Cyan
Write-Host "==================================================" -ForegroundColor Cyan
Write-Host "Versão: $CleanVersion" -ForegroundColor Yellow

$PublishDir = "publish/v$CleanVersion"
$ProjectFile = "src/DesktopCommandCenter.UI/DesktopCommandCenter.UI.csproj"
$ReleasesDir = "Releases"
$TargetExeName = "DCC - PRO.exe"
$TargetZipName = "DCC - PRO-Portable.zip"

# 1. Limpeza
if (Test-Path $PublishDir) {
    Write-Host "Limpando diretório de publicação antigo em '$PublishDir'..." -ForegroundColor DarkYellow
    Remove-Item -Recurse -Force $PublishDir
}

# Garante que o diretório de Releases existe
if (!(Test-Path $ReleasesDir)) {
    New-Item -ItemType Directory -Path $ReleasesDir | Out-Null
}

# 2. dotnet publish
Write-Host "Iniciando compilação e publicação .NET..." -ForegroundColor Gray
$PublishCmd = "dotnet publish $ProjectFile -c ReleasePro -r win-x64 --self-contained true -p:PublishSingleFile=false -p:PublishReadyToRun=true -p:WindowsPackageType=None -p:Version=$CleanVersion -o $PublishDir"
Write-Host "Executando: $PublishCmd" -ForegroundColor DarkGray
Invoke-Expression $PublishCmd

if ($LASTEXITCODE -ne 0) {
    Write-Error "A compilação do .NET falhou."
}
Write-Host "Compilação .NET finalizada com sucesso." -ForegroundColor Green

# 2.5 Assinatura Digital do App Base (Opcional)
if ($CertPath -ne "" -and (Test-Path $CertPath)) {
    Write-Host "Assinando o executável da aplicação com signtool..." -ForegroundColor Gray
    # Signtool do Windows SDK 10
    $SignTool = "C:\Program Files (x86)\Windows Kits\10\bin\10.0.22621.0\x64\signtool.exe"
    if (-not (Test-Path $SignTool)) {
        # Fallback para tentar achar o signtool em outra versão
        $SignToolPath = Get-ChildItem "C:\Program Files (x86)\Windows Kits\10\bin\*\x64\signtool.exe" | Select-Object -First 1
        if ($SignToolPath) { $SignTool = $SignToolPath.FullName }
    }

    if (Test-Path $SignTool) {
        $TargetApp = Join-Path $PublishDir "DesktopCommandCenter.UI.exe"
        if (Test-Path $TargetApp) {
            $SignCmd = "& `"$SignTool`" sign /f `"$CertPath`" /p `"$CertPassword`" /fd SHA256 /tr http://timestamp.digicert.com /td SHA256 `"$TargetApp`""
            Write-Host "Executando: $SignCmd" -ForegroundColor DarkGray
            Invoke-Expression $SignCmd
        }
    } else {
        Write-Host "Aviso: signtool.exe não encontrado. Instale o Windows SDK para assinar o executável." -ForegroundColor Yellow
    }
}

# 3. Empacotamento com NSIS (Gera o Instalador Executável de Fato)
Write-Host "Iniciando compilador NSIS (makensis.exe)..." -ForegroundColor Gray
$MakensisPath = "C:\Program Files (x86)\NSIS\makensis.exe"
if (!(Test-Path $MakensisPath)) {
    Write-Error "Compilador NSIS não encontrado em '$MakensisPath'. Certifique-se de que o NSIS está instalado."
}

$PackCmd = "& `"$MakensisPath`" /DVERSION=$CleanVersion installer_pro.nsi"
Write-Host "Executando: $PackCmd" -ForegroundColor DarkGray
Invoke-Expression $PackCmd

if ($LASTEXITCODE -ne 0) {
    Write-Error "O empacotamento com NSIS falhou."
}

$SetupDest = Join-Path $ReleasesDir $TargetExeName

# 3.5 Assinatura Digital do Instalador (Opcional)
if ($CertPath -ne "" -and (Test-Path $CertPath) -and (Test-Path $SignTool)) {
    Write-Host "Assinando o Instalador gerado..." -ForegroundColor Gray
    $SignSetupCmd = "& `"$SignTool`" sign /f `"$CertPath`" /p `"$CertPassword`" /fd SHA256 /tr http://timestamp.digicert.com /td SHA256 `"$SetupDest`""
    Invoke-Expression $SignSetupCmd
}

# 4. Criação do ZIP Portátil (PowerShell Nativo)
Write-Host "Criando arquivo portátil comprimido (.zip)..." -ForegroundColor Gray
$ZipDest = Join-Path $ReleasesDir $TargetZipName
if (Test-Path $ZipDest) { Remove-Item -Force $ZipDest }
Compress-Archive -Path "$PublishDir/*" -DestinationPath $ZipDest -Force
Write-Host "-> Portable Zip gerado com sucesso em: $ZipDest" -ForegroundColor Cyan

$SetupDest = Join-Path $ReleasesDir $TargetExeName

Write-Host "==================================================" -ForegroundColor Green
Write-Host "          RELEASE PRO GERADA COM SUCESSO!         " -ForegroundColor Green
Write-Host "==================================================" -ForegroundColor Green
Write-Host "Instalador e zip portátil disponíveis em:" -ForegroundColor Gray
Write-Host "  -> $(Resolve-Path $SetupDest)" -ForegroundColor Cyan
Write-Host "==================================================" -ForegroundColor Green
