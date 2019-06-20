SetCompressor /SOLID /FINAL lzma

!define APPNAME "NoPipeline"
!define INSTALLERVERSION "1.0.0.1"

!define MUI_ICON "pics\icon.ico"
!define MUI_UNICON "pics\icon.ico"


!include "Sections.nsh"
!include "MUI2.nsh"
!include "FileAssociation.nsh"

!define MUI_WELCOMEFINISHPAGE_BITMAP "pics\panel.bmp"

!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH

!insertmacro MUI_LANGUAGE "English"

Name '${APPNAME} ${INSTALLERVERSION}'
OutFile 'NoPipelineSetup.exe'
InstallDir '$PROGRAMFILES\${APPNAME}\' ; Main install directory.
!define MSBuildInstallDir '$PROGRAMFILES32\MSBuild\${APPNAME}' ; MSBuild directory.

VIProductVersion "${INSTALLERVERSION}"
VIAddVersionKey /LANG=${LANG_ENGLISH} "ProductName" "${APPNAME}"
VIAddVersionKey /LANG=${LANG_ENGLISH} "CompanyName" "Chai Foxes"
VIAddVersionKey /LANG=${LANG_ENGLISH} "FileVersion" "${INSTALLERVERSION}"
VIAddVersionKey /LANG=${LANG_ENGLISH} "ProductVersion" "${INSTALLERVERSION}"
VIAddVersionKey /LANG=${LANG_ENGLISH} "FileDescription" "${APPNAME} Installer"
VIAddVersionKey /LANG=${LANG_ENGLISH} "LegalCopyright" "Copyright © Chai Foxes"


; Request application privileges.
RequestExecutionLevel admin



; UI stuff.
!define MUI_HEADERIMAGE
!define MUI_HEADERIMAGE_BITMAP "pics\NoPipeline.bmp"
!define MUI_ABORTWARNING
; UI stuff.


; The stuff to install.
Section "Main" Main
  SectionIn RO

  ; Installing main program.
  SetOutPath '$INSTDIR'
  File /r '..\NoPipeline\NoPipeline\bin\Release\*.exe'
  File /r '..\NoPipeline\NoPipeline\bin\Release\*.dll'
  File /r '..\NoPipeline\NoPipeline\bin\Release\*.npl'
  ${registerExtension} "$INSTDIR\${APPNAME}.exe" ".npl" "NoPipeline Config"

  ; Installing .targets file.
  SetOutPath '${MSBuildInstallDir}'
  File /r '..\NoPipeline\NoPipeline\bin\Release\*.targets'

  ; Uninstaller
  WriteUninstaller "uninstall.exe"

SectionEnd


Function .onInit
  IntOp $0 $0 | ${SF_RO}
  IntOp $0 ${SF_SELECTED} | ${SF_RO}
FunctionEnd

;--------------------------------
; Uninstaller Section

Section "Uninstall"
  ${unregisterExtension} ".npl" "NoPipeline Config"
  Delete "$INSTDIR\Uninstall.exe"
  RMDir /r "$INSTDIR"
  RMDir /r "${MSBuildInstallDir}"
SectionEnd

