!ifndef VERSION
  !define VERSION "1.0.1"
!endif

Unicode true

!include "MUI2.nsh"
!include "LogicLib.nsh"

# Nome do produto e do executavel do instalador
Name "Desktop Command Center"
OutFile "Releases\DCC - Desktop Command Center - v${VERSION}.exe"

# Define o modo de usuario multiplo (Permite instalar por usuario ou por maquina)
!define MULTIUSER_MUI
!define MULTIUSER_EXECUTIONLEVEL Highest
!define MULTIUSER_INSTALLMODE_DEFAULT_CURRENTUSER
!define MULTIUSER_INSTALLMODE_DEFAULT_REGISTRY_KEY "Software\DCCPro"
!define MULTIUSER_INSTALLMODE_DEFAULT_REGISTRY_VALUENAME "InstallDir"
!define MULTIUSER_INSTALLMODE_COMMANDLINE
!define MULTIUSER_INSTALLMODE_INSTDIR "DCC - Desktop Command Center"
!define MULTIUSER_INSTALLMODE_FUNCTION onMultiUserModeChanged
!include "MultiUser.nsh"

Function onMultiUserModeChanged
  ${If} $MultiUser.InstallMode == "CurrentUser"
    StrCpy $INSTDIR "$LocalAppData\Programs\${MULTIUSER_INSTALLMODE_INSTDIR}"
  ${Else}
    StrCpy $INSTDIR "$PROGRAMFILES64\${MULTIUSER_INSTALLMODE_INSTDIR}"
  ${EndIf}
FunctionEnd

# Configuracoes visuais (Icone do Instalador e Desinstalador)
!define MUI_ICON "src\DesktopCommandCenter.UI\Assets\AppIcon.ico"
!define MUI_UNICON "src\DesktopCommandCenter.UI\Assets\AppIcon.ico"

# Paginas do Instalador
!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_LICENSE $(myLicenseData)
!insertmacro MULTIUSER_PAGE_INSTALLMODE
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES

# Definicoes para executar apos instalacao
!define MUI_FINISHPAGE_RUN
!define MUI_FINISHPAGE_RUN_TEXT $(FinRunText)
!define MUI_FINISHPAGE_RUN_FUNCTION "LaunchApp"

!define MUI_FINISHPAGE_SHOWREADME ""
!define MUI_FINISHPAGE_SHOWREADME_TEXT $(FinLinkText)
!define MUI_FINISHPAGE_SHOWREADME_FUNCTION "CreateDesktopShortcut"
!insertmacro MUI_PAGE_FINISH

# Paginas do Desinstalador
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES

# Idiomas suportados
!insertmacro MUI_LANGUAGE "PortugueseBR"
!insertmacro MUI_LANGUAGE "English"
!insertmacro MUI_LANGUAGE "Spanish"

LicenseLangString myLicenseData ${LANG_PORTUGUESEBR} "LICENSE.txt"
LicenseLangString myLicenseData ${LANG_ENGLISH} "LICENSE_EN.txt"
LicenseLangString myLicenseData ${LANG_SPANISH} "LICENSE_ES.txt"

LangString FinRunText ${LANG_PORTUGUESEBR} "Executar Desktop Command Center"
LangString FinRunText ${LANG_ENGLISH} "Run Desktop Command Center"
LangString FinRunText ${LANG_SPANISH} "Ejecutar Desktop Command Center"

LangString FinLinkText ${LANG_PORTUGUESEBR} "Criar atalho na Area de Trabalho"
LangString FinLinkText ${LANG_ENGLISH} "Create Desktop Shortcut"
LangString FinLinkText ${LANG_SPANISH} "Crear acceso directo en el Escritorio"

LangString MsgAppRunning ${LANG_PORTUGUESEBR} "O Desktop Command Center esta em execucao em segundo plano.$\n$\nDeseja fecha-lo automaticamente para continuar a instalacao?"
LangString MsgAppRunning ${LANG_ENGLISH} "Desktop Command Center is currently running in the background.$\n$\nDo you want to close it automatically to continue the installation?"
LangString MsgAppRunning ${LANG_SPANISH} "Desktop Command Center esta ejecutandose en segundo plano.$\n$\nDesea cerrarlo automaticamente para continuar la instalacion?"

LangString MsgInstallAborted ${LANG_PORTUGUESEBR} "Instalacao cancelada pelo usuario."
LangString MsgInstallAborted ${LANG_ENGLISH} "Installation aborted by user."
LangString MsgInstallAborted ${LANG_SPANISH} "Instalacion cancelada por el usuario."

!insertmacro MUI_RESERVEFILE_LANGDLL

# Inicializacao do MultiUser
Function .onInit
  !insertmacro MULTIUSER_INIT
  !insertmacro MUI_LANGDLL_DISPLAY
FunctionEnd

Function un.onInit
  !insertmacro MULTIUSER_UNINIT
FunctionEnd

Section "Install"
  SetOutPath "$INSTDIR"
  
  # Limpeza preventiva de antigas versoes em cache de instalacoes passadas
  RMDir /r "$LocalAppData\Programs\DCC - Community"
  RMDir /r "$LocalAppData\Programs\DCC - PRO"
  RMDir /r "$LocalAppData\Programs\DCCPro"
  
  # Verifica se o aplicativo esta rodando antes de sobrescrever
  IfFileExists "$INSTDIR\Desktop Command Center.exe" 0 SkipCheck
CheckRunning:
  ClearErrors
  FileOpen $0 "$INSTDIR\Desktop Command Center.exe" a
  IfErrors IsRunning IsNotRunning

IsRunning:
  MessageBox MB_YESNO|MB_ICONEXCLAMATION $(MsgAppRunning) IDYES KillApp IDNO AbortInstall

KillApp:
  nsExec::Exec 'taskkill /F /IM "Desktop Command Center.exe"'
  Sleep 1000
  Goto CheckRunning

AbortInstall:
  Quit

IsNotRunning:
  FileClose $0
SkipCheck:

  # === LIMPEZA COMPLETA DA INSTALACAO ANTERIOR ===
  # Remove TODOS os arquivos da pasta de instalacao para evitar conflito de versoes de DLL
  # Os dados do usuario ficam em AppData\Local\DCC e nao sao afetados
  RMDir /r "$INSTDIR"
  CreateDirectory "$INSTDIR"
  # ================================================
  
  # === LIMPEZA DE CACHE E LOGS ===
  # Limpa os logs antigos, caches e a pasta temporaria do WebView2, 
  # mas preserva sessões (login) e banco de dados do usuario
  RMDir /r "$LocalAppData\DCC\logs"
  RMDir /r "$LocalAppData\Desktop Command Center.exe.WebView2"
  # ================================================
  
  # Copia recursivamente todos os arquivos da compilacao de publicacao
  File /r "publish\v${VERSION}\*.*"
  
  # Cria o desinstalador executavel no diretorio de instalacao
  WriteUninstaller "$INSTDIR\uninstall.exe"
  
  # Atalhos (Menu Iniciar e Area de Trabalho)
  CreateDirectory "$SMPROGRAMS\Desktop Command Center"
  CreateShortcut "$SMPROGRAMS\Desktop Command Center\Desktop Command Center.lnk" "$INSTDIR\Desktop Command Center.exe" "" "$INSTDIR\Desktop Command Center.exe" 0
  CreateShortcut "$SMPROGRAMS\Desktop Command Center\Desinstalar DCC.lnk" "$INSTDIR\uninstall.exe"
  
  # Chaves de registro de desinstalacao para o Windows adicionar/remover programas
  WriteRegStr SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\DCC" "DisplayName" "Desktop Command Center"
  WriteRegStr SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\DCC" "UninstallString" '"$INSTDIR\uninstall.exe"'
  WriteRegStr SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\DCC" "DisplayIcon" '"$INSTDIR\Desktop Command Center.exe"'
  WriteRegStr SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\DCC" "DisplayVersion" "${VERSION}"
  WriteRegStr SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\DCC" "Publisher" "Foreign Technologies"
SectionEnd

Section "Uninstall"
  # Deleta os atalhos criados
  Delete "$SMPROGRAMS\Desktop Command Center\Desktop Command Center.lnk"
  Delete "$SMPROGRAMS\Desktop Command Center\Desinstalar DCC.lnk"
  RMDir "$SMPROGRAMS\Desktop Command Center"
  Delete "$DESKTOP\Desktop Command Center.lnk"
  
  # Deleta os arquivos e subpastas recursivamente
  RMDir /r "$INSTDIR"
  
  # Remove as chaves de registro do Windows
  DeleteRegKey SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\DCC"
  DeleteRegKey SHCTX "Software\DCCPro"
SectionEnd



Function LaunchApp
  SetOutPath "$INSTDIR"
  ExecShell "" "$INSTDIR\Desktop Command Center.exe"
FunctionEnd

Function CreateDesktopShortcut
  CreateShortcut "$DESKTOP\Desktop Command Center.lnk" "$INSTDIR\Desktop Command Center.exe" "" "$INSTDIR\Desktop Command Center.exe" 0
FunctionEnd
