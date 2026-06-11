param (
    [Parameter(Mandatory=$true)]
    [string]$Version,

    [Parameter(Mandatory=$false)]
    [ValidateSet("ReleaseCommunity", "ReleasePro")]
    [string]$Config = "ReleaseCommunity"
)

$ErrorActionPreference = "Stop"

Write-Host "==================================================" -ForegroundColor Cyan
Write-Host " DCC RELEASE PACKAGER (VELOPACK) " -ForegroundColor Cyan
Write-Host "==================================================" -ForegroundColor Cyan
Write-Host "Versão: $Version" -ForegroundColor Yellow
Write-Host "Configuração: $Config" -ForegroundColor Yellow

$PublishDir = "publish/v$Version"
$ProjectFile = "src/DesktopCommandCenter.UI/DesktopCommandCenter.UI.csproj"
$IconFile = "DCC - Logo - Modo Escuro.ico"

# 1. Limpeza
if (Test-Path $PublishDir) {
    Write-Host "Limpando diretório de publicação antigo em '$PublishDir'..." -ForegroundColor DarkYellow
    Remove-Item -Recurse -Force $PublishDir
}

# 2. dotnet publish
Write-Host "Iniciando compilação e publicação .NET..." -ForegroundColor Gray
$PublishCmd = "dotnet publish $ProjectFile -c $Config -r win-x64 --self-contained true -p:PublishSingleFile=false -p:PublishReadyToRun=true -p:WindowsPackageType=None -p:Version=$Version -o $PublishDir"
Write-Host "Executando: $PublishCmd" -ForegroundColor DarkGray
Invoke-Expression $PublishCmd

if ($LASTEXITCODE -ne 0) {
    Write-Error "A compilação do .NET falhou."
}
Write-Host "Compilação .NET finalizada com sucesso." -ForegroundColor Green

# 3. Velopack packaging
Write-Host "Iniciando empacotamento via Velopack (vpk)..." -ForegroundColor Gray
$PackCmd = "vpk pack --packId DCC --packVersion $Version --packDir $PublishDir --mainExe DesktopCommandCenter.UI.exe --icon `"$IconFile`""
Write-Host "Executando: $PackCmd" -ForegroundColor DarkGray
Invoke-Expression $PackCmd

if ($LASTEXITCODE -ne 0) {
    Write-Error "O empacotamento via Velopack falhou."
}

Write-Host "==================================================" -ForegroundColor Green
Write-Host " RELEASE GERADA COM SUCESSO! " -ForegroundColor Green
Write-Host "==================================================" -ForegroundColor Green
Write-Host "Instalador e zip portátil disponíveis em:" -ForegroundColor Gray
Write-Host "  -> $(Resolve-Path Releases)" -ForegroundColor Cyan
Write-Host "==================================================" -ForegroundColor Green
