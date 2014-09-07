OutFile "CompileVersion.exe"
SilentInstall silent
RequestExecutionLevel user

Var VERSION

Section InitVersion
  FileOpen $R0 "..\HearthCap\Version.txt" r
	FileRead $R0 $VERSION
	FileClose $R0
	
  FileOpen $R1 "Version.nsh" w
	FileWrite $R1 '!define VERSION "$VERSION"$\r$\n'
	FileWrite $R1 'VIProductVersion $VERSION'

	FileClose $R1	
SectionEnd