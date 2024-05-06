
param (
	[string]$template = "LockedAccountResponse",
    [string]$templateToken = "locked-account"
)

$myfolder = [System.IO.Path]::GetDirectoryName( $MyInvocation.MyCommand.Path );
$implementationScript = [System.IO.Path]::Combine( $myfolder, "scaffold-new-template.ps1" );
$testingScript = [System.IO.Path]::Combine( $myfolder, "scaffold-new-template-tests.ps1" );
if ( [IO.File]::Exists( $implementationScript ) -eq $false ) { return; }
if ( [IO.File]::Exists( $testingScript ) -eq $false ) { return; }

$creation = (& $implementationScript -template $template -templateToken $templateToken);
if ($null -eq $creation -or $false -eq $creation ) { return; }
& $testingScript -template $template