param (
    [string]$searchPattern,
    [string]$versionLabel
)
if( [string]::IsNullOrWhiteSpace( $searchPattern ) -eq $true ) {
    $searchPattern = '*.api.csproj';
}

function canEnumerate( $obj ) {
    try {
        $enumerated = $obj.GetEnumerator();
        return $true;
    } catch {
        return $false;
    }
}

function executeDeployment( $source ){
    $currentLocation = Get-Location
    try {
        $projectDirectory = [System.IO.Path]::GetDirectoryName( $source );
        Set-Location $projectDirectory
        dotnet eb deploy-environment -c Release --version-label $versionLabel
    } finally {
        Set-Location $currentLocation
    }
}

$workingDir = [System.IO.Path]::GetDirectoryName( $MyInvocation.MyCommand.Path );
$di = [System.IO.DirectoryInfo]::new( $workingDir );
$found = $di.GetFiles($searchPattern, [System.IO.SearchOption]::AllDirectories)

if( ( canEnumerate -obj $found ) -eq $false ) {
    $solutionFile = $found.FullName
    executeDeployment -solution $solutionFile
}
else {
    $found.GetEnumerator() | ForEach-Object {
        $solutionFile = ([system.io.fileinfo]$_).FullName
        executeDeployment -solution $solutionFile
    }
}