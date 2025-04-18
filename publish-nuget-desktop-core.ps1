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

## find all files matching *.nupkg 
$currentDir = [System.IO.Path]::GetDirectoryName( $MyInvocation.MyCommand.Path );
$di = [System.IO.DirectoryInfo]::new( $currentDir );
$found = $di.GetFiles('*.nupkg', [System.IO.SearchOption]::AllDirectories);
$hasEnumerator = canEnumerate -obj $found
if( $hasEnumerator -eq $false) {
    $package = $found.FullName
    dotnet nuget push $package --api-key $apikey --source $publishurl
    return;
}

$found.GetEnumerator() | ForEach-Object {
    $package = ([system.io.fileinfo]$_).FullName
    dotnet nuget push $package --api-key $apikey --source $publishurl
}