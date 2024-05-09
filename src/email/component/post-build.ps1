
$workfolder = [System.IO.Path]::GetDirectoryName( $MyInvocation.MyCommand.Path );
$jsfolder = [System.IO.Path]::Combine( $workfolder, "_settings" );
$jsfile = [System.IO.Path]::Combine( $jsfolder, "smtp-settings.txt" );
$readMe = [System.IO.Path]::Combine( $workfolder, "README.md" );
$backupReadMe = [System.IO.Path]::Combine( $workfolder, "README.backup.md" );
$mail = @{
	"account" = "ses-smtp-user.legallead.2024.04"
	"uid" = ""
	"copy-admin" = $true
	"secret" = ""
	"type" = "aws-ses"
	"settings" = @{
		"endpoint" = "email-smtp.us-east-2.amazonaws.com"
		"port" = 587
		"from" = @{
			"email" = "admin@legallead.co"
			"display" = "Legal Lead Admin"
		}
	}
}

$jcontent = ($mail | ConvertTo-Json);
[System.IO.File]::WriteAllText( $jsfile, $jcontent );
# clean up for README file
if ( [System.IO.File]::Exists( $readMe ) -eq $false ) { 
    Write-Output "README file is not found."
    [System.Environment]::ExitCode = 1;
    return 1; 
}
if ( [System.IO.File]::Exists( $backupReadMe ) -eq $false ) { 
    Write-Output "README back up file is not found."
    [System.Environment]::ExitCode = 1;
    return 1; 
}
[System.IO.File]::Copy( $backupReadMe, $readMe, $true ) 
[System.IO.File]::Delete( $backupReadMe ) 