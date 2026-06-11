param (
    [Parameter(Mandatory=$false)]
    [string]$Version = "0.0.1"
)

$ErrorActionPreference = "Stop"

Write-Host "==================================================" -ForegroundColor Cyan
Write-Host "     DCC COMMUNITY RELEASE PACKAGER (VELOPACK)    " -ForegroundColor Cyan
Write-Host "==================================================" -ForegroundColor Cyan
Write-Host "Versão: $Version" -ForegroundColor Yellow

$PublishDir = "publish/v$Version"
$ProjectFile = "src/DesktopCommandCenter.UI/DesktopCommandCenter.UI.csproj"
$IconFile = "DCC - Logo - Modo Escuro.ico"

$PackId = "DCCCommunity"
$PackTitle = "DCC - Community"
$TargetExeName = "DCC - Community.exe"
$TargetZipName = "DCC - Community-Portable.zip"

$ReleasesDir = "Releases"
$SubReleasesDir = Join-Path $ReleasesDir "Community"

# 1. Limpeza
if (Test-Path $PublishDir) {
    Write-Host "Limpando diretório de publicação antigo em '$PublishDir'..." -ForegroundColor DarkYellow
    Remove-Item -Recurse -Force $PublishDir
}
if (Test-Path $SubReleasesDir) {
    Write-Host "Limpando diretório de Releases antigo em '$SubReleasesDir'..." -ForegroundColor DarkYellow
    Remove-Item -Recurse -Force $SubReleasesDir
}

# 2. dotnet publish
Write-Host "Iniciando compilação e publicação .NET..." -ForegroundColor Gray
$PublishCmd = "dotnet publish $ProjectFile -c ReleaseCommunity -r win-x64 --self-contained true -p:PublishSingleFile=false -p:PublishReadyToRun=true -p:WindowsPackageType=None -p:Version=$Version -o $PublishDir"
Write-Host "Executando: $PublishCmd" -ForegroundColor DarkGray
Invoke-Expression $PublishCmd

if ($LASTEXITCODE -ne 0) {
    Write-Error "A compilação do .NET falhou."
}
Write-Host "Compilação .NET finalizada com sucesso." -ForegroundColor Green

# 3. Velopack packaging
Write-Host "Iniciando empacotamento via Velopack (vpk)..." -ForegroundColor Gray
$PackCmd = "vpk pack --packId $PackId --packTitle `"$PackTitle`" --packVersion $Version --packDir $PublishDir --mainExe DesktopCommandCenter.UI.exe --icon `"$IconFile`" --outputDir `"$SubReleasesDir`""
Write-Host "Executando: $PackCmd" -ForegroundColor DarkGray
Invoke-Expression $PackCmd

if ($LASTEXITCODE -ne 0) {
    Write-Error "O empacotamento via Velopack falhou."
}

# 4. Copiando e renomeando os executáveis de saída
Write-Host "Copiando e renomeando instaladores e zips de saída..." -ForegroundColor Gray
$SetupSource = Join-Path $SubReleasesDir "${PackId}-win-Setup.exe"
$SetupDest = Join-Path $ReleasesDir $TargetExeName

$ZipSource = Join-Path $SubReleasesDir "${PackId}-win-Portable.zip"
$ZipDest = Join-Path $ReleasesDir $TargetZipName

if (Test-Path $SetupSource) {
    if (Test-Path $SetupDest) { Remove-Item -Force $SetupDest }
    Copy-Item -Path $SetupSource -Destination $SetupDest
    Write-Host "-> Instalador copiado com sucesso para: $SetupDest" -ForegroundColor Cyan
}

if (Test-Path $ZipSource) {
    if (Test-Path $ZipDest) { Remove-Item -Force $ZipDest }
    Copy-Item -Path $ZipSource -Destination $ZipDest
    Write-Host "-> Portable Zip copiado com sucesso para: $ZipDest" -ForegroundColor Cyan
}

Write-Host "==================================================" -ForegroundColor Green
Write-Host "        RELEASE COMMUNITY GERADA COM SUCESSO!     " -ForegroundColor Green
Write-Host "==================================================" -ForegroundColor Green
Write-Host "Instalador e zip portátil disponíveis em:" -ForegroundColor Gray
Write-Host "  -> $(Resolve-Path $SetupDest)" -ForegroundColor Cyan
Write-Host "==================================================" -ForegroundColor Green
