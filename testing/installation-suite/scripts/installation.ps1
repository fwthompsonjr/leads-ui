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
	"presentation" = @{
		"file" = "installation-003.ps1"
		"description" = "Install Legal Lead Application"
	}
}
function executeCommand( $path ) {
	try {
		& $path
		$result = [System.Environment]::ExitCode;
        Write-Host $result
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
		$target = [System.IO.Path]::Combine( $currentDir, $item.file );
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