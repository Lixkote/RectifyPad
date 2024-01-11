$WixRoot = "$PSScriptRoot\wix"
$InstallFileswsx = "..\Template.wxs"
$InstallFilesWixobj = "..\Sampleappx.wixobj"

if(!(Test-Path "$WixRoot\candle.exe"))
{
    
	Write-Host Downloading Wixtools..
    New-Item $WixRoot -type directory -force | Out-Null
    # Download Wix version 3.10.2 - https://wixtoolset.org/downloads/v3.14.0.6526/wix314-binaries.zip
    Invoke-WebRequest -Uri https://wixtoolset.org/downloads/v3.14.0.6526/wix314-binaries.zip -Method Get -OutFile $WixRoot\WixTools.zip

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