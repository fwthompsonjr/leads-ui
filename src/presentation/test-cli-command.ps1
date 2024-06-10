## get list of versions for application
## src/worker/_configuration/account-index.json.txt

function getInstalledVersion( $list ) {
    if ($list.Count -eq 0 ) { return "" };
    $ii = $list.Count - 1;
    $itm = ([string]($list[ $ii ])).Trim().Split(" ")[0];
    return $itm;
}

function getRemoteVersion( $list ) {
    if ($list.Count -eq 0 ) { return "" };
    $ii = $list.Count - 1;
    $itms = ([string]($list[ $ii ])).Trim().Split(" ");
    $ii = $itms.Count - 1;
    return $itms[$ii];
}

$installed = (leadcli locals);
$available = (leadcli list);
$svc = "legallead.reader.service";
$arr = @();
$locals = @();
$doubleDash = "--";
$singleDash = "- ";
foreach( $entry in $available ) {
    if ( $entry.Contains( $svc ) -eq $true ) {
        $arr += ( $entry.Replace($doubleDash, "").Trim() )
    }
}
$find = "- $svc";
$dataFound = $false;
foreach( $local in $installed ) {
    if ( $local.Contains( $find ) -eq $true ) {
        $dataFound = $true
    }
    if ( $dataFound -eq $true -and $local.Contains( $doubleDash ) -eq $true ) {
        $locals += ( $local.Replace($doubleDash, "").Trim() )
    }
	if ( $dataFound -eq $true -and $locals.Count -gt 0 -and $local.Contains( $singleDash ) -eq $true ) {
        $dataFound = $false;
    }
}
if( $arr.Count -gt 0 ) {
    $lastInstall = getInstalledVersion -list $locals
    $lastAvailable = getRemoteVersion -list $arr
    Write-Output "Installed: $lastInstall"
    Write-Output "Remote: $lastAvailable"
    if ( $lastInstall -ne $lastAvailable ) {
        Write-Output "New version is available, $lastAvailable"
    }
} else {
    Write-Output "Version check is NOT required"
}