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

function executeRemove( $source ){
    if ( [System.IO.File]::Exists( $source ) -eq $true ) { 
        [System.IO.File]::Delete( $source );
    }
}

function executeCopy( $source, $target ){
    if( [System.IO.File]::Exists( $source ) -eq $false ) { return; }
    if( [System.IO.Directory]::Exists( $target ) -eq $false ) { return; }
    $shortName = [System.IO.Path]::GetFileName( $source )
    $targetName = [System.IO.Path]::Combine( $target, $shortName )
    if ( [System.IO.File]::Exists( $targetName ) -eq $true ) { 
        [System.IO.File]::Delete( $targetName );
    }
    [System.IO.File]::Copy( $source, $targetName, $true );
    [System.IO.File]::Delete( $source );
}

$findZipPattern = "*.api.zip"
$currentDir = [System.IO.Path]::GetDirectoryName( $MyInvocation.MyCommand.Path );
$di = [System.IO.DirectoryInfo]::new( $currentDir );
$zipped = $di.GetFiles($findZipPattern, [System.IO.SearchOption]::AllDirectories);
$found = $di.GetFiles($searchPattern, [System.IO.SearchOption]::AllDirectories) | Where-Object { $_.Name.IndexOf( 'integration' ) -ge 0 }

if ( $null -ne $zipped -and ( canEnumerate -obj $zipped ) -eq $false ) {
    $zipFile = $zipped.FullName
    executeRemove -source $zipFile
}

if ($null -ne $zipped -and ( canEnumerate -obj $zipped ) -eq $true ) {
    $zipped.GetEnumerator() | ForEach-Object {
        $zipFile = ([system.io.fileinfo]$_).FullName
        executeRemove -source $zipFile
    }
}

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
# find any zip files created
$di = [System.IO.DirectoryInfo]::new( $currentDir );
$found = $di.GetFiles($findZipPattern, [System.IO.SearchOption]::AllDirectories);

if ( $null -ne $found -and ( canEnumerate -obj $found ) -eq $false ) {
    $zipFile = $found.FullName
    executeCopy -source $zipFile -target $currentDir
}

if ($null -ne $found -and ( canEnumerate -obj $found ) -eq $true ) {
    $found.GetEnumerator() | ForEach-Object {
        $zipFile = ([system.io.fileinfo]$_).FullName
        executeCopy -source $zipFile -target $currentDir
    }
}