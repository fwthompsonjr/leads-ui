$currentDir = [System.IO.Path]::GetDirectoryName( $MyInvocation.MyCommand.Path );
$commands = [ordered]@{ 
	"dotnet" = @{
		"file" = "installation-001.ps1"
		"description" = "Download and Install Latest DOTNET SDK"
	}
	"installer" = @{
		"file" = "installation-002.ps1"
		"description" = "Download and Install Legal Lead Installer"
	}
	"chocolatey" = @{
		"file" = "installation-003.ps1"
		"description" = "Download and Install Chocolatey"
	}
	"firefox" = @{
		"file" = "installation-004.ps1"
		"description" = "Download and Install Firefox"
	}
	"search" = @{
		"file" = "installation-005.ps1"
		"description" = "Setup Search Folders"
	}
}
function executeCommand( $path ) {
	try {
		& $path
		return $true;
	} catch {
		Write-Error $_.Exception.Message
		return $false
	}
}

$hasError = $false;

$commands.GetEnumerator() | ForEach-Object {
	if ( $hasError -eq $false ) {
		$item = $_.Value;
		$scriptLocation = "steps/$($item.file)";
		$target = [System.IO.Path]::Combine( $currentDir, $scriptLocation );
		$comment = $item.description
		if ( [System.IO.File]::Exists( $target ) -eq $true ) {
			Write-Output "Executing: $comment"
			$isExecuted = executeCommand -path $target
			if ($isExecuted -eq $false ) { $hasError = $true; }
		} else {
			$hasError = $true;
		}
	}
}
if ( $hasError -eq $false ) {
	try {
		$name = "legallead.desktop-windows"
		leadcli install -n $name
		(New-Object -ComObject shell.application).toggleDesktop()
	} catch {
		Write-Warning "There was an issue installing application."
		Write-Warning "Please check logs for additional information."
	}
}