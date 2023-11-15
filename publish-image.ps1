<#
    publish project Output to local docker
    note: 
        if image is already in container, it will not overwrite.
        therefore, user must manually remove image to rebuild.

#>
$current_dir = [System.IO.Path]::GetDirectoryName( $MyInvocation.MyCommand.Path );
$publish_file = [System.IO.Path]::Combine( $current_dir, "build-local-container.ps1");

. $publish_file
$lookfor = "Pushed container '";
Write-Output "Building local container image...";
$response = publishLocal
if( $null -eq $response ) { return; }

$c = $response.Count - 1;
$message = [string]($response[$c]);
if( $message.IndexOf( $lookfor ) -lt 0 ) { return; }
$imageName = $message.Split("'")[1]
Write-Output "Local container image := $imageName";