$WixRoot = "$PSScriptRoot\wix"
$InstallFileswsx = "..\Template.wxs"
$InstallFilesWixobj = "..\Sampleappx.wixobj"

if(!(Test-Path "$WixRoot\candle.exe"))
{
    
	Write-Host Downloading Wixtools..
    New-Item $WixRoot -type directory -force | Out-Null
    # Download Wix version 3.10.2 - https://wix.codeplex.com/releases/view/619491
    Invoke-WebRequest -Uri https://wix.codeplex.com/downloads/get/1540241 -Method Get -OutFile $WixRoot\WixTools.zip

    Write-Host Extracting Wixtools..
    Add-Type -AssemblyName System.IO.Compression.FileSystem
    [System.IO.Compression.ZipFile]::ExtractToDirectory("$WixRoot\WixTools.zip", $WixRoot)
}

pushd "$WixRoot"
.\candle.exe $InstallFileswsx -ext WixUtilExtension -o "$PSScriptRoot\Sampleappx.wixobj" 
.\light.exe $InstallFilesWixobj -ext WixUtilExtension -b "$PSScriptRoot" -o "$PSScriptRoot\Sampleappx.msi" 
popd

#msiexec.exe /i Sampleappx.msi /log log.txt
#msiexec.exe /x Sampleappx.msi /log log.txt