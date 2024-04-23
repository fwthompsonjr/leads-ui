
$workfolder = [System.IO.Path]::GetDirectoryName( $MyInvocation.MyCommand.Path );
$jsfolder = [System.IO.Path]::Combine( $workfolder, "_settings" );
$jsfile = [System.IO.Path]::Combine( $jsfolder, "smtp-settings.txt" );
$mail = @{
	"account" = "ses-smtp-user.legallead.2024.04"
	"uid" = ""
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