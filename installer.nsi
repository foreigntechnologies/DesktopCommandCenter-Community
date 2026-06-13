!ifndef VERSION
  !define VERSION "0.0.1"
!endif

Unicode true

!include "MUI2.nsh"
!include "LogicLib.nsh"

# Nome do produto e do executável do instalador
Name "Desktop Command Center"
OutFile "Releases\DCC - Desktop Command Center - v${VERSION}.exe"

# Define o modo de usuário múltiplo (Permite instalar por usuário ou por máquina)
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

# Configurações visuais (Ícone do Instalador e Desinstalador)
!define MUI_ICON "src\DesktopCommandCenter.UI\Assets\AppIcon.ico"
!define MUI_UNICON "src\DesktopCommandCenter.UI\Assets\AppIcon.ico"

# Páginas do Instalador
!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_LICENSE $(myLicenseData)
!insertmacro MULTIUSER_PAGE_INSTALLMODE
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!define MUI_FINISHPAGE_RUN "$INSTDIR\Desktop Command Center.exe"
!define MUI_FINISHPAGE_RUN_TEXT $(FinRunText)
!define MUI_FINISHPAGE_SHOWREADME ""
!define MUI_FINISHPAGE_SHOWREADME_TEXT $(FinLinkText)
!define MUI_FINISHPAGE_SHOWREADME_FUNCTION "CreateDesktopShortcut"
!insertmacro MUI_PAGE_FINISH

# Páginas do Desinstalador
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES

# Idiomas suportados
!insertmacro MUI_LANGUAGE "PortugueseBR"
!insertmacro MUI_LANGUAGE "English"

LicenseLangString myLicenseData ${LANG_PORTUGUESEBR} "LICENSE.txt"
LicenseLangString myLicenseData ${LANG_ENGLISH} "LICENSE_EN.txt"

LangString FinRunText ${LANG_PORTUGUESEBR} "Executar Desktop Command Center"
LangString FinRunText ${LANG_ENGLISH} "Run Desktop Command Center"

LangString FinLinkText ${LANG_PORTUGUESEBR} "Criar atalho na Área de Trabalho"
LangString FinLinkText ${LANG_ENGLISH} "Create Desktop Shortcut"

LangString MsgAppRunning ${LANG_PORTUGUESEBR} "O Desktop Command Center está em execução em segundo plano.$\n$\nDeseja fechá-lo automaticamente para continuar a instalação?"
LangString MsgAppRunning ${LANG_ENGLISH} "Desktop Command Center is currently running in the background.$\n$\nDo you want to close it automatically to continue the installation?"

LangString MsgInstallAborted ${LANG_PORTUGUESEBR} "Instalação cancelada pelo usuário."
LangString MsgInstallAborted ${LANG_ENGLISH} "Installation aborted by user."

!insertmacro MUI_RESERVEFILE_LANGDLL

# Inicialização do MultiUser
Function .onInit
  !insertmacro MULTIUSER_INIT
  !insertmacro MUI_LANGDLL_DISPLAY
FunctionEnd

Function un.onInit
  !insertmacro MULTIUSER_UNINIT
FunctionEnd

Section "Install"
  SetOutPath "$INSTDIR"
  
  # Limpeza preventiva de antigas versões em cache de instalações passadas
  RMDir /r "$LocalAppData\Programs\DCC - Community"
  RMDir /r "$LocalAppData\Programs\DCC - PRO"
  RMDir /r "$LocalAppData\Programs\DCCPro"
  
  # Verifica se o aplicativo está rodando antes de sobrescrever
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
  
  # Copia recursivamente todos os arquivos da compilação de publicação
  File /r "publish\v${VERSION}\*.*"
  
  # Cria o desinstalador executável no diretório de instalação
  WriteUninstaller "$INSTDIR\uninstall.exe"
  
  # Atalhos (Menu Iniciar e Área de Trabalho)
  CreateDirectory "$SMPROGRAMS\Desktop Command Center"
  CreateShortcut "$SMPROGRAMS\Desktop Command Center\Desktop Command Center.lnk" "$INSTDIR\Desktop Command Center.exe" "" "$INSTDIR\Desktop Command Center.exe" 0
  CreateShortcut "$SMPROGRAMS\Desktop Command Center\Desinstalar DCC.lnk" "$INSTDIR\uninstall.exe"
  
  # Chaves de registro de desinstalação para o Windows adicionar/remover programas
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

Function LaunchAsNormalUser
  Exec '"$WINDIR\explorer.exe" "$INSTDIR\Desktop Command Center.exe"'
FunctionEnd

Function CreateDesktopShortcut
  CreateShortcut "$DESKTOP\Desktop Command Center.lnk" "$INSTDIR\Desktop Command Center.exe" "" "$INSTDIR\Desktop Command Center.exe" 0
FunctionEnd

