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
	"environment" = @{
		"file" = "installation-005.ps1"
		"description" = "Organize environment path variable"
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
	$appnames = @( "legallead.desktop-windows", "legallead.reader.service");
	$appnames.GetEnumerator() | ForEach-Object {
		$aname = $_;
		try {
			leadcli install -n $aname
		} catch {
			$hasError = $true;
			Write-Warning "There was an issue installing $aname application."
			Write-Warning "Please check logs for additional information."
		}
	}
}
if ( $hasError -eq $false ) {
	(New-Object -ComObject shell.application).toggleDesktop()
}