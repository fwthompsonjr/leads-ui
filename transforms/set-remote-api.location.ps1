$locations = @(
    "postman/ll-authorizations-test.postman_collection.json",
    "postman/ll-authorizations.postman_collection.json",
    "src/presentation/legallead.desktop/AppSettings.json"
);
$find = "http://legalleadpermissionsapi-dev.us-east-2.elasticbeanstalk.com";
$replace = "http://api.legallead.co"
$wfolder = [System.IO.Path]::GetDirectoryName( $MyInvocation.MyCommand.Path );
$parent = [System.IO.Path]::GetDirectoryName( $wfolder );

function hasToken( $source ) {
    if ( [System.IO.File]::Exists( $source ) -eq $false ) { return $false; }
    [string]$content = [System.IO.File]::ReadAllText( $source );
    if( $content.IndexOf( $find ) -lt 0 ) { return $false; }
    return $true;
}

function makeReplacement( $source ) {
    if ( [System.IO.File]::Exists( $source ) -eq $false ) { return; }
    [string]$content = [System.IO.File]::ReadAllText( $source );
    if( $content.IndexOf( $find ) -lt 0 ) { return; }
    $content = $content.Replace( $find, $replace );
    [System.IO.File]::WriteAllText( $source, $content );
}


if ( [System.IO.Directory]::Exists( $parent ) -eq $false ) {
    Write-Error "Parent folder is not found."
    return;
}

$locations.GetEnumerator() | ForEach-Object {
    $subfolder = $_;
    $path = [System.IO.Path]::Combine( $parent, $subfolder );
    $shortName = [System.IO.Path]::GetFileName( $path );
    $needsReplacement = hasToken -source $path
    if ( $needsReplacement -eq $true ) {
        Write-Output "File found: $shortName"
        makeReplacement -source $path
    }
}