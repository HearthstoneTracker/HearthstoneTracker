; Script generated with the Venis Install Wizard
!system "CompileVersion.exe"
!include "Version.nsh"

; Define your application name
!define APPNAME "HearthstoneTracker"
!define APPNAMEANDVERSION "HearthstoneTracker ${VERSION}"

; Additional script dependencies
!include WinVer.nsh

; Main Install settings
Name "${APPNAMEANDVERSION}"
Icon "..\HearthCap\Resources\hearthstone_icon.ico"
InstallDir "$PROGRAMFILES\HearthstoneTracker"
InstallDirRegKey HKLM "Software\${APPNAME}" ""
OutFile "..\package\packages\HearthstoneTracker-Setup.exe"

VIAddVersionKey ProductName "${APPNAME}"
VIAddVersionKey Comments "${APPNAME}"
VIAddVersionKey CompanyName "HearthstoneTracker.com"
VIAddVersionKey LegalCopyright "HearthstoneTracker.com"
VIAddVersionKey FileDescription "HearthstoneTracker"
VIAddVersionKey FileVersion $VERSION
VIAddVersionKey ProductVersion $VERSION
VIAddVersionKey InternalName "$APPNAME"
VIAddVersionKey LegalTrademarks "HearthstoneTracker - Copyright by Remco Ros"
VIAddVersionKey OriginalFilename "HearthstoneTracker-Setup.exe"

; Use compression
SetCompressor LZMA

; Need Admin
RequestExecutionLevel admin

; Modern interface settings
!include "MUI.nsh"

!define MUI_ABORTWARNING
!define MUI_FINISHPAGE_RUN "$INSTDIR\HearthCap.exe"
; !define MUI_FINISHPAGE_RUN
; !define MUI_FINISHPAGE_RUN_FUNCTION Launch
!define MUI_PAGE_CUSTOMFUNCTION_LEAVE PreReqCheck

!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_LICENSE "..\HearthCap\LICENSE.txt"
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH

!define MUI_PAGE_CUSTOMFUNCTION_PRE un.RunningCheck
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES

; Set languages (first is default language)
!insertmacro MUI_LANGUAGE "English"
!insertmacro MUI_RESERVEFILE_LANGDLL

Var dotNET45IsThere

;Function Launch
;  ShellExecAsUser::ShellExecAsUser $0 "$INSTDIR\HearthCap.exe"
;FunctionEnd

Function PreReqCheck
    ; Abort on XP or lower
    ${If} ${AtMostWinXP}
        MessageBox MB_OK|MB_ICONSTOP "HearthstoneTracker can only be used on Windows Vista and higher."
        Quit
    ${EndIf}
    
    ReadRegDWORD $dotNET45IsThere HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full" "Release"
    IntCmp $dotNET45IsThere 378389 is_equal is_less is_greater
is_equal:
    Goto dotnet_check_ok
is_greater:
    Goto dotnet_check_ok
is_less:
    Goto dotnet_check_notok
dotnet_check_notok:
    MessageBox MB_YESNO|MB_ICONEXCLAMATION "Your system does not have Microsoft .NET Framework 4.5 installed. Would you like to download it?" IDYES downloadtrue IDNO downloadfalse
    downloadtrue:
        ExecShell "open" "http://www.microsoft.com/download/details.aspx?id=40773"
    downloadfalse:
    Quit
dotnet_check_ok:

    ; DirectX Version Check
    ;ClearErrors
    ;GetDLLVersion "D3DX9_43.DLL" $R0 $R1
    ;GetDLLVersion "D3DX10_43.DLL" $R0 $R1
    ;GetDLLVersion "D3D10_1.DLL" $R0 $R1
    ;GetDLLVersion "DXGI.DLL" $R0 $R1
    ;GetDLLVersion "D3DCompiler_43.dll" $R0 $R1
    ;IfErrors dxMissing dxOK
    ;dxMissing:
    ;	MessageBox MB_YESNO|MB_ICONEXCLAMATION "Your system is missing DirectX components that ${APPNAME} requires. Would you like to download them?" IDYES dxtrue IDNO dxfalse
    ;	dxtrue:
    ;		ExecShell "open" "http://www.microsoft.com/en-us/download/details.aspx?id=35"
    ;	dxfalse:
    ;	Quit
    ;dxOK:
    ClearErrors
    
    ; Check previous instance
hearthcap_running_check:
    System::Call 'kernel32::OpenMutexW(i 0x100000, b 0, w "HearthCap") i .R0'
    IntCmp $R0 0 hs_running_check
    System::Call 'kernel32::CloseHandle(i $R0)'
    MessageBox MB_RETRYCANCEL|MB_ICONEXCLAMATION "${APPNAME} is already running. Please close it first before installing a new version." /SD IDCANCEL IDRETRY hearthcap_running_check
    Abort

hs_running_check:
    ReadRegStr $R0 HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "UninstallString"
    IfFileExists $R0 +1 notRunning
    FindWindow $0 "UnityWndClass" "Hearthstone"
    StrCmp $0 0 notRunning
    MessageBox MB_ABORTRETRYIGNORE|MB_ICONEXCLAMATION "It is recommended to close Hearthstone before updating Hearthstone Tracker." /SD IDCANCEL IDRETRY hs_running_check IDIGNORE notRunning
    Abort
notRunning:
FunctionEnd

Function un.RunningCheck
    ; Check previous instance
hearthcap_running_check:
    System::Call 'kernel32::OpenMutexW(i 0x100000, b 0, w "HearthCap") i .R0'
    IntCmp $R0 0 hs_running_check
    System::Call 'kernel32::CloseHandle(i $R0)'
    MessageBox MB_RETRYCANCEL|MB_ICONEXCLAMATION "${APPNAME} is running. Please close it first before uninstalling" /SD IDCANCEL IDRETRY hearthcap_running_check
    Quit

hs_running_check:
    FindWindow $0 "UnityWndClass" "Hearthstone"
    StrCmp $0 0 notRunning
    MessageBox MB_RETRYCANCEL|MB_ICONEXCLAMATION "Hearthstone is running. Please close Hearthstone before uninstalling Hearthstone Tracker." /SD IDCANCEL IDRETRY hs_running_check
    Quit
notRunning:
FunctionEnd

Section "HearthstoneTracker" Section1

    ; Set Section properties
    SetOverwrite on

    ; Set Section Files and Shortcuts
    SetOutPath "$INSTDIR\"
    File "..\HearthCap\bin\Release\LICENSE.txt"
    File "..\HearthCap\bin\Release\Capture.dll"
    File "..\HearthCap\bin\Release\D3DX9_43.dll"
    File "..\HearthCap\bin\Release\EasyHook.dll"
    File "..\HearthCap\bin\Release\EasyLoad32.dll"
    File "..\HearthCap\bin\Release\EasyLoad64.dll"
    File "..\HearthCap\bin\Release\EasyHook32.dll"
    File "..\HearthCap\bin\Release\EasyHook64.dll"
    File "..\HearthCap\bin\Release\HearthCap.exe"
    File "..\HearthCap\bin\Release\HearthCap.ico"
    File "..\HearthCap\bin\Release\HearthCap.exe.config"
    File "..\HearthCap\bin\Release\System.Runtime.Remoting.dll"
    File "..\HearthCap\bin\Release\System.Runtime.Serialization.Formatters.Soap.dll"
    File "..\HearthCap\bin\Release\HearthCap.Logger.Interface.dll"
    File "..\HearthCap\bin\Release\HearthCapLogger.dll"
    SetOutPath "$INSTDIR\amd64\"
    File "..\HearthCap\bin\Release\amd64\sqlceca40.dll"
    File "..\HearthCap\bin\Release\amd64\sqlcecompact40.dll"
    File "..\HearthCap\bin\Release\amd64\sqlceer40EN.dll"
    File "..\HearthCap\bin\Release\amd64\sqlceme40.dll"
    File "..\HearthCap\bin\Release\amd64\sqlceqp40.dll"
    File "..\HearthCap\bin\Release\amd64\sqlcese40.dll"
    SetOutPath "$INSTDIR\amd64\Microsoft.VC90.CRT\"
    File "..\HearthCap\bin\Release\amd64\Microsoft.VC90.CRT\Microsoft.VC90.CRT.manifest"
    File "..\HearthCap\bin\Release\amd64\Microsoft.VC90.CRT\msvcr90.dll"
    File "..\HearthCap\bin\Release\amd64\Microsoft.VC90.CRT\README_ENU.txt"
    SetOutPath "$INSTDIR\x86\"
    File "..\HearthCap\bin\Release\x86\sqlceca40.dll"
    File "..\HearthCap\bin\Release\x86\sqlcecompact40.dll"
    File "..\HearthCap\bin\Release\x86\sqlceer40EN.dll"
    File "..\HearthCap\bin\Release\x86\sqlceme40.dll"
    File "..\HearthCap\bin\Release\x86\sqlceqp40.dll"
    File "..\HearthCap\bin\Release\x86\sqlcese40.dll"
    SetOutPath "$INSTDIR\x86\Microsoft.VC90.CRT\"
    File "..\HearthCap\bin\Release\x86\Microsoft.VC90.CRT\Microsoft.VC90.CRT.manifest"
    File "..\HearthCap\bin\Release\x86\Microsoft.VC90.CRT\msvcr90.dll"
    File "..\HearthCap\bin\Release\x86\Microsoft.VC90.CRT\README_ENU.txt"
    SetOutPath "$INSTDIR\"
    CreateShortCut "$DESKTOP\HearthstoneTracker.lnk" "$INSTDIR\HearthCap.exe"
    CreateDirectory "$SMPROGRAMS\HearthstoneTracker"
    CreateShortCut "$SMPROGRAMS\HearthstoneTracker\HearthstoneTracker.lnk" "$INSTDIR\HearthCap.exe"
    CreateShortCut "$SMPROGRAMS\HearthstoneTracker\Uninstall.lnk" "$INSTDIR\uninstall.exe"
    Delete "$INSTDIR\Capture.dll.tmp"
    Delete "$INSTDIR\HearthCapLoader.dll"
SectionEnd

Section -FinishSection

    WriteRegStr HKLM "Software\${APPNAME}" "" "$INSTDIR"
    WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "DisplayName" "${APPNAME}"
    WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "DisplayIcon" "$INSTDIR\HearthCap.ico"
    WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "DisplayVersion" "${VERSION}"
    WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "Publisher" "HearthstoneTracker.com"
    WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "URLInfoAbout" "http://hearthstonetracker.com"
    WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "EstimatedSize" 13721
    WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "UninstallString" "$INSTDIR\uninstall.exe"
    WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "NoModify" 1
    WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "NoRepair" 1
    WriteUninstaller "$INSTDIR\uninstall.exe"

SectionEnd

; Modern install component descriptions
!insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
    !insertmacro MUI_DESCRIPTION_TEXT ${Section1} ""
!insertmacro MUI_FUNCTION_DESCRIPTION_END

;Uninstall section
Section Uninstall

    ;Remove from registry...
    DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}"
    DeleteRegKey HKLM "SOFTWARE\${APPNAME}"
    DeleteRegValue HKCU "Software\Microsoft\Windows\CurrentVersion\Run" "${APPNAME}"

    ; Delete self
    Delete "$INSTDIR\uninstall.exe"

    ; Delete Shortcuts
    Delete "$DESKTOP\HearthstoneTracker.lnk"
    Delete "$SMPROGRAMS\HearthstoneTracker\HearthstoneTracker.lnk"
    Delete "$SMPROGRAMS\HearthstoneTracker\Uninstall.lnk"

    ; Clean up HearthstoneTracker
    Delete "$INSTDIR\LICENSE.txt"
    Delete "$INSTDIR\Capture.dll"
    Delete "$INSTDIR\Capture.dll.tmp"
    Delete "$INSTDIR\D3DX9_43.dll"
    Delete "$INSTDIR\EasyHook.dll"
    Delete "$INSTDIR\EasyHook.dll.tmp"
    Delete "$INSTDIR\EasyLoad32.dll"
    Delete "$INSTDIR\EasyLoad64.dll"
    Delete "$INSTDIR\EasyHook32.dll"
    Delete "$INSTDIR\EasyHook32.dll.tmp"
    Delete "$INSTDIR\EasyHook32Svc.exe"
    Delete "$INSTDIR\EasyHook64.dll"
    Delete "$INSTDIR\EasyHook64.dll.tmp"
    Delete "$INSTDIR\EasyHook64Svc.exe"
    Delete "$INSTDIR\EasyHookSvc.exe"
    Delete "$INSTDIR\HearthCap.exe"
    Delete "$INSTDIR\HearthCap.ico"
    Delete "$INSTDIR\HearthCap.exe.config"
    Delete "$INSTDIR\HearthCap.Updater.exe"
    Delete "$INSTDIR\System.Runtime.Remoting.dll"
    Delete "$INSTDIR\System.Runtime.Serialization.Formatters.Soap.dll"
    Delete "$INSTDIR\HearthCap.Logger.Interface.dll"
    Delete "$INSTDIR\HearthCapLogger.dll"
    Delete "$INSTDIR\HearthCapLoader.dll"
    Delete "$INSTDIR\amd64\sqlceca40.dll"
    Delete "$INSTDIR\amd64\sqlcecompact40.dll"
    Delete "$INSTDIR\amd64\sqlceer40EN.dll"
    Delete "$INSTDIR\amd64\sqlceme40.dll"
    Delete "$INSTDIR\amd64\sqlceqp40.dll"
    Delete "$INSTDIR\amd64\sqlcese40.dll"
    Delete "$INSTDIR\amd64\Microsoft.VC90.CRT\Microsoft.VC90.CRT.manifest"
    Delete "$INSTDIR\amd64\Microsoft.VC90.CRT\msvcr90.dll"
    Delete "$INSTDIR\amd64\Microsoft.VC90.CRT\README_ENU.txt"
    Delete "$INSTDIR\x86\sqlceca40.dll"
    Delete "$INSTDIR\x86\sqlcecompact40.dll"
    Delete "$INSTDIR\x86\sqlceer40EN.dll"
    Delete "$INSTDIR\x86\sqlceme40.dll"
    Delete "$INSTDIR\x86\sqlceqp40.dll"
    Delete "$INSTDIR\x86\sqlcese40.dll"
    Delete "$INSTDIR\x86\Microsoft.VC90.CRT\Microsoft.VC90.CRT.manifest"
    Delete "$INSTDIR\x86\Microsoft.VC90.CRT\msvcr90.dll"
    Delete "$INSTDIR\x86\Microsoft.VC90.CRT\README_ENU.txt"

    ; Remove remaining directories
    RMDir "$SMPROGRAMS\HearthstoneTracker"
    RMDir "$INSTDIR\x86\Microsoft.VC90.CRT\"
    RMDir "$INSTDIR\x86\"
    RMDir "$INSTDIR\amd64\Microsoft.VC90.CRT\"
    RMDir "$INSTDIR\amd64\"
    RMDir "$INSTDIR\"

SectionEnd

BrandingText "HearthstoneTracker.com"

; eof