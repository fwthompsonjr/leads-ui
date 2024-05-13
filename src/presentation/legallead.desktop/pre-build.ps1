# C:\_g\lui\fwthompsonjr\leads-ui\src\presentation\legallead.desktop\AppSettings.json
$workfolder = [System.IO.Path]::GetDirectoryName( $MyInvocation.MyCommand.Path );
$jsfile = [System.IO.Path]::Combine( $workfolder, "AppSettings.json" );

if ( [System.IO.File]::Exists( $jsfile ) -eq $false ) { 
    Write-Output "Source file is not found."
    [System.Environment]::ExitCode = 1;
    return 1; 
}
$hasChanged = $false;
$kvremote = "remote";
$jcontent = [System.IO.File]::ReadAllText( $jsfile ) | ConvertFrom-Json
if ($jcontent.'api.permissions'.destination -ne $kvremote) {
    $jcontent.'api.permissions'.destination = $kvremote;
    $hasChanged = $true;
}
if ( $hasChanged -eq $false) { return; }
$txt = [System.IO.File]::ReadAllText( $jsfile ).Replace('"destination": "local"', '"destination": "remote"');
[System.IO.File]::WriteAllText( $jsfile, $txt );