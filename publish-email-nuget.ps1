param(
	$apikey,
	$publishurl
)

function canEnumerate( $obj ) {
    try {
        $enumerated = $obj.GetEnumerator();
        return $true;
    } catch {
        return $false;
    }
}
$isTestMode = [string]::IsNullOrEmpty($apikey) -or [string]::IsNullOrEmpty($publishurl)
## find all files matching *.nupkg 
$currentDir = [System.IO.Path]::GetDirectoryName( $MyInvocation.MyCommand.Path );
$di = [System.IO.DirectoryInfo]::new( $currentDir );
$found = $di.GetFiles('*.nupkg', [System.IO.SearchOption]::AllDirectories) | Where-Object { 
    $_.FullName -like '*legallead.email*'
};
$hasEnumerator = canEnumerate -obj $found
if( $hasEnumerator -eq $false) {
    $package = $found.FullName
    if ($package.IndexOf("1.0.0") -lt 0) {
        Write-Output "Publishing $package"
        if ( $isTestMode -eq $false ) { dotnet nuget push $package --api-key $apikey --source $publishurl }
    }
    return;
}

$found.GetEnumerator() | ForEach-Object {
    $package = ([system.io.fileinfo]$_).FullName
    if ($package.IndexOf("1.0.0") -lt 0) {
        Write-Output "Publishing $package"
        if ( $isTestMode -eq $false ) { dotnet nuget push $package --api-key $apikey --source $publishurl }
    }
}