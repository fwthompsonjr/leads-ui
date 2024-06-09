## get list of versions for application
## src/worker/_configuration/account-index.json.txt
$installed = (leadcli locals);
$available = (leadcli list);
$svc = "legallead.reader.service";
## $installed
$available
if( $available.IndexOf( $svc ) -gt 1 ) {
    Write-Output "Version check is required"
} else {
    Write-Output "Version check is NOT required"
}