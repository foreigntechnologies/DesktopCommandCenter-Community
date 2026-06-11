param (
    [Parameter(Mandatory=$false)]
    [string]$Version = "0.0.1"
)

$ErrorActionPreference = "Stop"

Write-Host "==================================================" -ForegroundColor Cyan
Write-Host "        DCC PRO WIZARD PACKAGER (NSIS)           " -ForegroundColor Cyan
Write-Host "==================================================" -ForegroundColor Cyan
Write-Host "Versão: $Version" -ForegroundColor Yellow

$PublishDir = "publish/v$Version"
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
$PublishCmd = "dotnet publish $ProjectFile -c ReleasePro -r win-x64 --self-contained true -p:PublishSingleFile=false -p:PublishReadyToRun=true -p:WindowsPackageType=None -p:Version=$Version -o $PublishDir"
Write-Host "Executando: $PublishCmd" -ForegroundColor DarkGray
Invoke-Expression $PublishCmd

if ($LASTEXITCODE -ne 0) {
    Write-Error "A compilação do .NET falhou."
}
Write-Host "Compilação .NET finalizada com sucesso." -ForegroundColor Green

# 3. Empacotamento com NSIS (Gera o Instalador Executável de Fato)
Write-Host "Iniciando compilador NSIS (makensis.exe)..." -ForegroundColor Gray
$MakensisPath = "C:\Program Files (x86)\NSIS\makensis.exe"
if (!(Test-Path $MakensisPath)) {
    Write-Error "Compilador NSIS não encontrado em '$MakensisPath'. Certifique-se de que o NSIS está instalado."
}

$PackCmd = "& `"$MakensisPath`" /DVERSION=$Version installer_pro.nsi"
Write-Host "Executando: $PackCmd" -ForegroundColor DarkGray
Invoke-Expression $PackCmd

if ($LASTEXITCODE -ne 0) {
    Write-Error "O empacotamento com NSIS falhou."
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
