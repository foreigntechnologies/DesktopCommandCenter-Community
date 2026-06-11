!ifndef VERSION
  !define VERSION "0.0.1"
!endif

Unicode true

!include "MUI2.nsh"
!include "LogicLib.nsh"

# Nome do produto e do executável do instalador
Name "DCC - PRO"
OutFile "Releases\DCC - PRO.exe"

# Define o modo de usuário múltiplo (Permite instalar por usuário ou por máquina)
!define MULTIUSER_MUI
!define MULTIUSER_EXECUTIONLEVEL Highest
!define MULTIUSER_INSTALLMODE_DEFAULT_CURRENTUSER
!define MULTIUSER_INSTALLMODE_DEFAULT_REGISTRY_KEY "Software\DCCPro"
!define MULTIUSER_INSTALLMODE_DEFAULT_REGISTRY_VALUENAME "InstallDir"
!define MULTIUSER_INSTALLMODE_COMMANDLINE
!define MULTIUSER_INSTALLMODE_INSTDIR "DCC - PRO"
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
!define MUI_FINISHPAGE_RUN "$INSTDIR\DesktopCommandCenter.UI.exe"
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
  
  # Copia recursivamente todos os arquivos da compilação de publicação
  File /r "publish\v${VERSION}\*.*"
  
  # Cria o desinstalador executável no diretório de instalação
  WriteUninstaller "$INSTDIR\uninstall.exe"
  
  # Atalhos (Menu Iniciar e Área de Trabalho)
  CreateDirectory "$SMPROGRAMS\DCC - PRO"
  CreateShortcut "$SMPROGRAMS\DCC - PRO\DCC - PRO.lnk" "$INSTDIR\DesktopCommandCenter.UI.exe" "" "$INSTDIR\DesktopCommandCenter.UI.exe" 0
  CreateShortcut "$SMPROGRAMS\DCC - PRO\Desinstalar DCC.lnk" "$INSTDIR\uninstall.exe"
  CreateShortcut "$DESKTOP\DCC - PRO.lnk" "$INSTDIR\DesktopCommandCenter.UI.exe" "" "$INSTDIR\DesktopCommandCenter.UI.exe" 0
  
  # Chaves de registro de desinstalação para o Windows adicionar/remover programas
  WriteRegStr SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\DCCPro" "DisplayName" "DCC - PRO"
  WriteRegStr SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\DCCPro" "UninstallString" '"$INSTDIR\uninstall.exe"'
  WriteRegStr SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\DCCPro" "DisplayIcon" "$INSTDIR\DesktopCommandCenter.UI.exe,0"
  WriteRegStr SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\DCCPro" "DisplayVersion" "${VERSION}"
  WriteRegStr SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\DCCPro" "Publisher" "Foreign Technologies"
SectionEnd

Section "Uninstall"
  # Deleta os atalhos criados
  Delete "$SMPROGRAMS\DCC - PRO\DCC - PRO.lnk"
  Delete "$SMPROGRAMS\DCC - PRO\Desinstalar DCC.lnk"
  RMDir "$SMPROGRAMS\DCC - PRO"
  Delete "$DESKTOP\DCC - PRO.lnk"
  
  # Deleta os arquivos e subpastas recursivamente
  RMDir /r "$INSTDIR"
  
  # Remove as chaves de registro do Windows
  DeleteRegKey SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\DCCPro"
  DeleteRegKey SHCTX "Software\DCCPro"
SectionEnd
