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
!insertmacro MUI_PAGE_LICENSE "LICENSE.txt"
!insertmacro MULTIUSER_PAGE_INSTALLMODE
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!define MUI_FINISHPAGE_RUN "$INSTDIR\Desktop Command Center.exe"
!define MUI_FINISHPAGE_RUN_TEXT "Executar Desktop Command Center"
!define MUI_FINISHPAGE_RUN_FUNCTION "LaunchAsNormalUser"
!define MUI_FINISHPAGE_SHOWREADME ""
!define MUI_FINISHPAGE_SHOWREADME_TEXT "Criar atalho na Área de Trabalho"
!define MUI_FINISHPAGE_SHOWREADME_FUNCTION "CreateDesktopShortcut"
!insertmacro MUI_PAGE_FINISH

# Páginas do Desinstalador
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES

# Idiomas suportados
!insertmacro MUI_LANGUAGE "PortugueseBR"
!insertmacro MUI_LANGUAGE "English"

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
  MessageBox MB_RETRYCANCEL|MB_ICONEXCLAMATION "O Desktop Command Center está em execução.$\n$\nPor favor, feche o aplicativo (verifique a bandeja do sistema perto do relógio) e clique em Repetir para continuar a instalação." IDRETRY CheckRunning IDCANCEL AbortInstall

AbortInstall:
  Abort "Instalação cancelada pelo usuário."

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
