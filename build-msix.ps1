param (
    [Parameter(Mandatory=$false)]
    [string]$Version = "1.0.0"
)

$ErrorActionPreference = "Stop"
$CleanVersion = $Version -replace '^v', ''

Write-Host "==================================================" -ForegroundColor Cyan
Write-Host "        DCC MSIX STORE PACKAGER                   " -ForegroundColor Cyan
Write-Host "==================================================" -ForegroundColor Cyan
Write-Host "Versão: $CleanVersion" -ForegroundColor Yellow

$ProjectFile = "src/DesktopCommandCenter.UI/DesktopCommandCenter.UI.csproj"
$ManifestFile = "src/DesktopCommandCenter.UI/Package.appxmanifest"
$ReleasesDir = "Releases"

# 1. VALIDAÇÃO PARA A MICROSOFT STORE (Resolvendo o problema apontado)
Write-Host "Validando configurações do Package.appxmanifest para a Microsoft Store..." -ForegroundColor Gray

if (!(Test-Path $ManifestFile)) {
    Write-Error "Arquivo de manifesto não encontrado em: $ManifestFile"
}

[xml]$manifest = Get-Content $ManifestFile
$identity = $manifest.Package.Identity

if ($identity.Publisher -eq "CN=AppPublisher" -or $identity.Name -eq "714F4A21-A499-43DA-B801-AE3D7ADDEE6C") {
    Write-Host ""
    Write-Host "ERRO CRÍTICO DE CERTIFICAÇÃO DETECTADO!" -ForegroundColor Red
    Write-Host "O seu arquivo Package.appxmanifest ainda possui a identidade temporária gerada pelo Visual Studio:" -ForegroundColor Yellow
    Write-Host "  Name: $($identity.Name)" -ForegroundColor Yellow
    Write-Host "  Publisher: $($identity.Publisher)" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "A Microsoft Store rejeitará esse pacote imediatamente, pois a Identidade não bate com a reservada na loja." -ForegroundColor Red
    Write-Host "A loja até assina seu app automaticamente para resolver o problema do 'Unsigned', mas exige que a Identidade do pacote seja EXATA." -ForegroundColor Red
    Write-Host ""
    Write-Host "== COMO RESOLVER ISSO AGORA ==" -ForegroundColor Cyan
    Write-Host "Passo 1: Acesse a Partner Center (dashboard da Microsoft Store) -> Selecione o seu App." -ForegroundColor Cyan
    Write-Host "Passo 2: Vá em 'Gerenciamento do Produto' (Product Management) -> 'Identidade do Produto' (Product Identity)." -ForegroundColor Cyan
    Write-Host "Passo 3: Abra o arquivo src\DesktopCommandCenter.UI\Package.appxmanifest" -ForegroundColor Cyan
    Write-Host "Passo 4: Substitua o atributo Name e Publisher pelo que está lá." -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Se você usa o Visual Studio, o jeito mais fácil de arrumar é:" -ForegroundColor Magenta
    Write-Host "Clicar com botão direito no projeto DesktopCommandCenter.UI -> Pacote e Publicação -> Criar Pacotes de Aplicativos -> E vincular à Loja." -ForegroundColor Magenta
    Write-Host ""
    Write-Host "O script foi pausado para evitar que você gere um pacote inválido." -ForegroundColor Red
    exit 1
}

Write-Host "-> Identidade do pacote parece OK." -ForegroundColor Green

# Opcional: Sincroniza a versão no manifesto (Windows Store exige versão em 4 partes numéricas)
# Caso a sua versão seja "1.0.1", ele atualiza no AppxManifest
$manifest.Package.Identity.Version = "$CleanVersion.0"
$manifest.Save($ManifestFile)

# 2. Prepara os diretórios
if (!(Test-Path $ReleasesDir)) {
    New-Item -ItemType Directory -Path $ReleasesDir | Out-Null
}

$MsixDest = Join-Path $ReleasesDir "MSIX"
if (Test-Path $MsixDest) {
    Remove-Item -Recurse -Force $MsixDest
}
New-Item -ItemType Directory -Path $MsixDest | Out-Null

# 3. Empacotamento do MSIX via .NET
Write-Host "Iniciando compilação MSIX via dotnet publish..." -ForegroundColor Gray
# Flags importantes:
# /p:WindowsPackageType=MSIX -> Força a compilação do pacote
# /p:AppxPackageSigningEnabled=false -> Nós não queremos assinar localmente, pois a Store assina (.msixupload não precisa ser assinado se não for teste local)
# /p:UapAppxPackageBuildMode=StoreUpload -> Gera o arquivo ideal para Partner Center (.msixupload)
# /p:AppxBundle=Always -> Opcional, cria pacote para múltiplas arquiteturas

$PublishCmd = "dotnet publish $ProjectFile -c Release -r win-x64 /p:WindowsPackageType=MSIX /p:AppxPackageSigningEnabled=false /p:UapAppxPackageBuildMode=StoreUpload /p:AppxBundle=Always /p:GenerateAppxPackageOnBuild=true /p:Version=$CleanVersion"

Write-Host "Executando: $PublishCmd" -ForegroundColor DarkGray
Invoke-Expression $PublishCmd

if ($LASTEXITCODE -ne 0) {
    Write-Error "A compilação do pacote MSIX falhou. Verifique os logs acima."
}

# 4. Copiando o pacote gerado para a pasta Releases
$AppPackagesDir = "src\DesktopCommandCenter.UI\AppPackages"
if (Test-Path $AppPackagesDir) {
    $MsixUploadFiles = Get-ChildItem -Path $AppPackagesDir -Filter "*.msixupload" -Recurse
    
    if ($MsixUploadFiles.Count -gt 0) {
        foreach ($file in $MsixUploadFiles) {
            Copy-Item -Path $file.FullName -Destination $MsixDest -Force
            Write-Host "-> MSIXUpload salvo para upload na loja em: $(Join-Path $MsixDest $file.Name)" -ForegroundColor Cyan
        }
        
        Write-Host "==================================================" -ForegroundColor Green
        Write-Host "          MSIX GERADO COM SUCESSO!                " -ForegroundColor Green
        Write-Host "==================================================" -ForegroundColor Green
        Write-Host "Envie o(s) arquivo(s) '.msixupload' localizados em Releases/MSIX no Partner Center." -ForegroundColor Yellow
        Write-Host "Isso solucionará o problema de certificado pendente do seu EXE!" -ForegroundColor Yellow
        Write-Host "==================================================" -ForegroundColor Green
    } else {
        Write-Host "Aviso: O pacote foi compilado, mas o arquivo .msixupload não foi encontrado em AppPackages." -ForegroundColor Yellow
    }
} else {
    Write-Host "Aviso: A pasta de saída do pacote não foi gerada." -ForegroundColor Yellow
}
