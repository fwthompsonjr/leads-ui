param(
	$apikey,
	$publishurl
)
## find all files matching *.nupkg 
$currentDir = [System.IO.Path]::GetDirectoryName( $MyInvocation.MyCommand.Path );
$di = [System.IO.DirectoryInfo]::new( $currentDir );
$found = $di.GetFiles('*.nupkg', [System.IO.SearchOption]::AllDirectories) | Where-Object { $_.FullName -like '*release*' };

if( $found.Count -eq $null) {
    $package = $found.FullName
    dotnet nuget push $package --api-key $apikey --source $publishurl
    return;
}

$found.GetEnumerator() | ForEach-Object {
    $package = ([system.io.fileinfo]$_).FullName
    dotnet nuget push $package --api-key $apikey --source $publishurl
}