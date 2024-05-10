<#
.\leads-ui\x-installer-version.json
#>

$startedAt = [datetime]::UtcNow
$workfolder = [System.IO.Path]::GetDirectoryName( $MyInvocation.MyCommand.Path );
$jsfile = [System.IO.Path]::Combine( $workfolder, "x-installer-version.json" );
if( [system.io.file]::Exists( $jsfile ) -eq $false ) {
    return;
}
$content = [System.IO.File]::ReadAllText( $jsfile ) | ConvertFrom-Json
try {
    $title = $content.Item(0).description
    echo "RELEASE_TITLE=$title" >> $env:GITHUB_ENV
} catch {}