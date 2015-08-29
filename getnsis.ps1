function Expand-ZIPFile($file, $destination)
{
  $shell = new-object -com shell.application
  $zip = $shell.NameSpace($file)
  foreach($item in $zip.items())
  {
    $shell.Namespace($destination).copyhere($item)
  }
}

$toolspath = "c:\buildtools"
$nsispath = "$toolspath\nsis-3.0b2"
$makensis = "$nsispath\makensis.exe"

If((Test-Path -Path $makensis) -eq $false)
{
	New-Item -ItemType Directory -Force -Path $toolspath
	$output = "$toolspath\nsis-3.0b2.zip"
	$url = "http://freefr.dl.sourceforge.net/project/nsis/NSIS%203%20Pre-release/3.0b2/nsis-3.0b2.zip"
	Invoke-WebRequest -Uri $url -OutFile $output
	Expand-ZIPFile -File $output -Destination $toolspath
}

&$makensis "$env:APPVEYOR_BUILD_FOLDER\installer\HearthstoneTracker.nsi"