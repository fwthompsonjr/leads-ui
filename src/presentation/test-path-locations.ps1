$tools = @();
$cmmds = $env:Path.Split(';');
$cmmds.GetEnumerator() | ForEach-Object {
    $name = $_;
    if($name.Contains( ".dotnet" ) -eq $true ) {
        if ( $tools.Contains( $name ) -eq $false ) {
            $tools += ( $name );
        }
    }
}
if ( $tools.Count -gt 0 ) {
    $find = "leadcli.exe";
    $parent = $tools[0];
    $search = [System.IO.Path]::Combine( $parent, $find );
    if ([system.IO.File]::Exists( $search ) -eq $true ) {
        Write-Output "Found lead cli"
    }
}