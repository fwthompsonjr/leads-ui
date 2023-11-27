$buildConfigruation = "Release";
$searchPattern = '*.sln';


function canEnumerate( $obj ) {
    try {
        $enumerated = $obj.GetEnumerator();
        return $true;
    } catch {
        return $false;
    }
}

function executeCoverage( $solution ) {
    $shortName = [System.IO.Path]::GetFileNameWithoutExtension( $solution );
    $pathName = [System.IO.Path]::GetDirectoryName( $solution );
    $pathName = [System.IO.Path]::Combine( $pathName, "$shortName-0" );
    if( !([System.IO.Directory]::Exists( $pathName ))) {
        [System.IO.Directory]::CreateDirectory( $pathName ) | Out-Null
    }
    Write-Output "Testing solution: $shortName"
    dotnet test $solution --logger trx `
            -c $buildConfigruation --no-restore `
            --results-directory $pathName `
            /p:CollectCoverage=true `
            /p:CoverletOutput=$coverageDir `
            /p:CoverletOutputFormat=cobertura
}

$currentDir = [System.IO.Path]::GetDirectoryName( $MyInvocation.MyCommand.Path );
$coverageDir = [System.IO.Path]::Combine( $currentDir, "coverage" );


if( !([System.IO.Directory]::Exists( $coverageDir ))) {
    [System.IO.Directory]::CreateDirectory( $coverageDir ) | Out-Null
}
$di = [System.IO.DirectoryInfo]::new( $currentDir );
$found = $di.GetFiles($searchPattern, [System.IO.SearchOption]::AllDirectories) | Where-Object { $_.Name.IndexOf( 'integration' ) -ge 0 }

if( ( canEnumerate -obj $found ) -eq $false ) {
    $solutionFile = $found.FullName
    executeCoverage -solution $solutionFile
}
else {
    $found.GetEnumerator() | ForEach-Object {
        $solutionFile = ([system.io.fileinfo]$_).FullName
        executeCoverage -solution $solutionFile
    }
}