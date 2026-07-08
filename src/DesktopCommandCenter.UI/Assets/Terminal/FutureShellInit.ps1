param(
    [string]$LangArg = ""
)

Clear-Host
$lang = if ([string]::IsNullOrWhiteSpace($LangArg)) { (Get-Culture).TwoLetterISOLanguageName } else { $LangArg }

if ($lang -eq "pt") {
    $banner = "O Future Shell foi desenvolvido por Foreign Technologies..."
    $tip = @"
Dica: Para usar uma CLI, Ferramenta ou Biblioteca especifica, digite seu comando especifico e pressione Enter, por exemplo:
- "bash";
- "wsl";
- "node";
- "nvm";
- "cmd";
- "gcloud";
- "fly";
- "vercel";

Para usar os poderes da Inteligencia Artificial do FutureShell, digite 'fs' seguido do seu prompt entre aspas. Ex: fs "listar arquivos"

Antes de utilizar um CLI Especifico, verifique se o mesmo esta instalado ou nao.
Digite 'help' para ajuda ou 'principal-commands' para uma lista de comandos.
"@
    $helpTitle = "=== Ajuda do FutureShell ==="
    $helpContent = "O FutureShell suporta qualquer CLI instalada no seu sistema.`n- Para entrar no Linux/WSL: digite 'wsl' ou 'bash'`n- Para executar comandos nativos do CMD: digite 'cmd'`n- Para ver os comandos principais: digite 'principal-commands'`n- Para ajuda nativa do PowerShell: digite 'help <comando>'"
    $pcTitle = "=== Principais Comandos de CLIs Globais ==="
    $pcContent = @"
Node.js: node -v, npm install, npm start, npx
Python: python --version, pip install, pip list
Git: git status, git clone, git commit, git push
Cloud: gcloud auth login, fly deploy
Frameworks: ng serve (Angular)
Gerenciadores: choco install, nvm use
"@
} elseif ($lang -eq "es") {
    $banner = "FutureShell fue desarrollado por Foreign Technologies..."
    $tip = @"
Consejo: Para usar una CLI, Herramienta o Biblioteca especifica, escriba su comando especifico y presione Enter, por ejemplo:
- "bash";
- "wsl";
- "node";
- "nvm";
- "cmd";
- "gcloud";
- "fly";
- "vercel";

Para usar los poderes de la Inteligencia Artificial de FutureShell, escriba 'fs' seguido de su instruccion entre comillas. Ej: fs "listar archivos"

Antes de utilizar una CLI especifica, verifique si esta instalada o no.
Escriba 'help' para ayuda o 'principal-commands' para una lista de comandos.
"@
    $helpTitle = "=== Ayuda de FutureShell ==="
    $helpContent = "FutureShell soporta cualquier CLI instalada en su sistema.`n- Para entrar en Linux/WSL: escriba 'wsl' o 'bash'`n- Para comandos CMD: escriba 'cmd'`n- Para comandos principales: escriba 'principal-commands'`n- Para ayuda nativa de PowerShell: escriba 'help <comando>'"
    $pcTitle = "=== Comandos Principales de CLIs Globales ==="
    $pcContent = @"
Node.js: node -v, npm install, npm start, npx
Python: python --version, pip install, pip list
Git: git status, git clone, git commit, git push
Cloud: gcloud auth login, fly deploy
Frameworks: ng serve (Angular)
Gestores: choco install, nvm use
"@
} else {
    $banner = "The FutureShell was developed by Foreign Technologies..."
    $tip = @"
Tip: To use a specific CLI, Tool, or Library, type its specific command and press Enter, for example:
- "bash";
- "wsl";
- "node";
- "nvm";
- "cmd";
- "gcloud";
- "fly";
- "vercel";

To use the Artificial Intelligence powers of FutureShell, type 'fs' followed by your prompt in quotes. Ex: fs "list files"

Before using a specific CLI, make sure it is installed.
Type 'help' for help or 'principal-commands' for a list of commands.
"@
    $helpTitle = "=== FutureShell Help ==="
    $helpContent = "FutureShell supports any CLI installed on your system.`n- To enter Linux/WSL: type 'wsl' or 'bash'`n- To execute CMD commands: type 'cmd'`n- For main commands: type 'principal-commands'`n- For native PowerShell help: type 'help <command>'"
    $pcTitle = "=== Main Commands for Global CLIs ==="
    $pcContent = @"
Node.js: node -v, npm install, npm start, npx
Python: python --version, pip install, pip list
Git: git status, git clone, git commit, git push
Cloud: gcloud auth login, fly deploy
Frameworks: ng serve (Angular)
Managers: choco install, nvm use
"@
}

Write-Host $banner -ForegroundColor Cyan
Write-Host ""
Write-Host $tip -ForegroundColor Yellow
Write-Host ""

Remove-Item alias:help -ErrorAction SilentlyContinue
Remove-Item function:help -ErrorAction SilentlyContinue

function FutureShell-Help {
    param(
        [Parameter(Position=0, ValueFromPipelineByPropertyName=$true)]
        [string]$Name
    )
    if ([string]::IsNullOrWhiteSpace($Name)) {
        Write-Host ""
        Write-Host $helpTitle -ForegroundColor Cyan
        Write-Host $helpContent
        Write-Host ""
    } else {
        Get-Help $Name | Out-String | Write-Host
    }
}
Set-Alias help FutureShell-Help -Scope Global -Force

function principal-commands {
    Write-Host ""
    Write-Host $pcTitle -ForegroundColor Cyan
    Write-Host $pcContent
    Write-Host ""
}

# FutureShell AI CLI Registration
$AppRoot = (Get-Item $PSCommandPath).Directory.Parent.Parent.FullName

function future-shell {
    param(
        [Parameter(ValueFromRemainingArguments=$true)]
        $ArgsList
    )
    $cliPath = Join-Path $AppRoot "CLI\future-shell.exe"
    if (Test-Path $cliPath) {
        & $cliPath $ArgsList
    } else {
        Write-Host "AI CLI (future-shell.exe) não foi encontrado no pacote!" -ForegroundColor Red
    }
}

Set-Alias fs future-shell -Scope Global -Force
