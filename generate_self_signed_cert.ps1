$ErrorActionPreference = "Stop"

Write-Host "==================================================" -ForegroundColor Cyan
Write-Host "  GERADOR DE CERTIFICADO AUTOASSINADO (TESTE)     " -ForegroundColor Cyan
Write-Host "==================================================" -ForegroundColor Cyan

$Subject = "CN=Foreign Technologies"
$CertName = "ForeignTechnologies_TestCert"
$PfxPath = Join-Path $PWD "$CertName.pfx"
$Password = "DCC123!"
$SecurePassword = ConvertTo-SecureString -String $Password -Force -AsPlainText

Write-Host "Gerando o certificado (Pode levar alguns segundos)..." -ForegroundColor Gray
$Cert = New-SelfSignedCertificate -Subject $Subject -Type CodeSigningCert -CertStoreLocation "Cert:\CurrentUser\My" -HashAlgorithm SHA256

Write-Host "Exportando para arquivo .pfx..." -ForegroundColor Gray
Export-PfxCertificate -Cert $Cert -FilePath $PfxPath -Password $SecurePassword

Write-Host "Instalando o certificado na raiz confiável local para evitar o aviso no SEU PC..." -ForegroundColor Gray
$Store = New-Object System.Security.Cryptography.X509Certificates.X509Store "Root","LocalMachine"
$Store.Open("ReadWrite")
$Store.Add($Cert)
$Store.Close()

Write-Host "==================================================" -ForegroundColor Green
Write-Host "Certificado gerado com sucesso!" -ForegroundColor Green
Write-Host "Caminho: $PfxPath" -ForegroundColor Yellow
Write-Host "Senha: $Password" -ForegroundColor Yellow
Write-Host "==================================================" -ForegroundColor Green
Write-Host ""
Write-Host "Para compilar usando este certificado e remover a tela azul no SEU PC, execute:" -ForegroundColor Cyan
Write-Host ".\build_community.ps1 -Version `"0.0.1`" -CertPath `"$PfxPath`" -CertPassword `"$Password`"" -ForegroundColor White
Write-Host ""
Write-Host "AVISO: Este certificado é AUTOASSINADO. Ele tirará o aviso de 'Desconhecido' e a tela azul" -ForegroundColor Red
Write-Host "apenas no SEU computador (onde ele foi instalado na Raiz Confiável). Para clientes reais," -ForegroundColor Red
Write-Host "você precisará comprar um certificado oficial (DigiCert, Sectigo, etc)." -ForegroundColor Red
