param (
    [Parameter(Position=0)]
    [string]$Version = "0.0.1",
    
    [Parameter(Position=1)]
    [string]$Edition = "COMMUNITY"
)

Write-Host "======================================"
Write-Host " Iniciando Build via Docker Container "
Write-Host " Edicao: $Edition                     "
Write-Host " Versao: $Version                     "
Write-Host "======================================"

if ($Edition.ToUpper() -eq "PRO") {
    .\build_pro.ps1 -Version $Version
} else {
    .\build_community.ps1 -Version $Version
}

Write-Host "======================================"
Write-Host " Build finalizada com sucesso!        "
Write-Host " Verifique a pasta 'Releases'.        "
Write-Host "======================================"
