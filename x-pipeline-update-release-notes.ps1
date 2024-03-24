param(
    [string]
    $version = "3.2.0"
)

$startedAt = [datetime]::UtcNow
$workfolder = [System.IO.Path]::GetDirectoryName( $MyInvocation.MyCommand.Path );
$readmeFile = [System.IO.Path]::Combine( $workfolder, "README.md" );
if( [system.io.file]::Exists( $readmeFile ) -eq $false ) {
    return;
}
$find1 = "## vX.Y.Z";
$find2 = "- ReleaseDate"
$mdcontent = [System.IO.File]::ReadAllText( $readmeFile );
$dte = [string]::Concat( " ( ", [DateTime]::UtcNow.ToString("s").Replace("T", " "), " )");
if ( $mdcontent.IndexOf( $find1 ) -ge 0 ) {
    $mdcontent = $mdcontent.Replace( $find1, $version );
}
if ( $mdcontent.IndexOf( $find2 ) -ge 0 ) {
    $mdcontent = $mdcontent.Replace( $find2, $dte );
}