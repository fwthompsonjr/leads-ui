param($sourceFolder, $configuration)
# get the bin folder as targetFolder
$name = "bin";
$tconfig = "Zip";


function checkConfiguration($cfg){
	if ($null -eq $cfg) { return $false }
	if ($cfg -ne $tconfig) { return $false }
	return $true;
}

$canExec = checkConfiguration -cfg $configuration
if ($canExec -eq $false) { return; }

if([System.IO.Directory]::Exists($sourceFolder) -eq $false){
	return;
}
write-host "Zip process is executing."
write-host " folder is $sourceFolder"
write-host " configuration is $configuration"

$mypath = [System.IO.Path]::GetDirectoryName( $MyInvocation.MyCommand.Path )
$awszipname = [System.IO.Path]::Combine( $mypath, "deploy.zip" );
if( [System.IO.File]::Exists( $awszipname ) -eq $true ) { [System.IO.File]::Delete( $awszipname ) | Out-Null }
$subFolder = [System.IO.Path]::GetDirectoryName($sourceFolder);
while([System.IO.Path]::GetFileName($subFolder) -ne $name) {
		$subFolder = [System.IO.Path]::GetDirectoryName($subFolder);
		$projectName = [System.IO.Path]::GetDirectoryName($subFolder);
		$projectName = [System.IO.Path]::GetFileName($projectName)
}

write-host " .. Ziping file content from $sourceFolder"
$zipName = "$subFolder\$projectName.zip"
$zipShortName = [System.IO.Path]::GetFileName($zipName)
if([System.IO.File]::Exists($zipName) -eq $true) {
	[System.IO.File]::Delete($zipName)
}
try {
	Compress-Archive -Path $sourceFolder -DestinationPath $zipName
	write-host " .... Zip file: $zipShortName created."
	# copy the created file to root
	[System.IO.File]::Copy( $zipName, $awszipname, $true)
} catch {
	write-host " .... Zip file: $zipShortName creation failed."
	#throw;
}
