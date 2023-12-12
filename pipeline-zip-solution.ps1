$searchPattern = '*.sln';


function canEnumerate( $obj ) {
    try {
        $enumerated = $obj.GetEnumerator();
        return $true;
    } catch {
        return $false;
    }
}

function executeZip( $solution ) {
    $shortName = [System.IO.Path]::GetFileNameWithoutExtension( $solution );
    Write-Output "Building solution: $shortName"
    dotnet build $solution -c Zip --no-restore 
}

$currentDir = [System.IO.Path]::GetDirectoryName( $MyInvocation.MyCommand.Path );
$di = [System.IO.DirectoryInfo]::new( $currentDir );
$found = $di.GetFiles($searchPattern, [System.IO.SearchOption]::AllDirectories) | Where-Object { $_.Name.IndexOf( 'integration' ) -ge 0 }

if( ( canEnumerate -obj $found ) -eq $false ) {
    $solutionFile = $found.FullName
    executeZip -solution $solutionFile
}
else {
    $found.GetEnumerator() | ForEach-Object {
        $solutionFile = ([system.io.fileinfo]$_).FullName
        executeZip -solution $solutionFile
    }
}