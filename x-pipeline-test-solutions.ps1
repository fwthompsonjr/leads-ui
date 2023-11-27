$buildConfigruation = "Release";
$searchPattern = '*.sln';
$currentDir = [System.IO.Path]::GetDirectoryName( $MyInvocation.MyCommand.Path );
$coverageDir = [System.IO.Path]::Combine( $currentDir, "coverage" );
if( !([System.IO.Directory]::Exists( $coverageDir ))) {
    [System.IO.Directory]::CreateDirectory( $coverageDir ) | Out-Null
}
$di = [System.IO.DirectoryInfo]::new( $currentDir );
$found = $di.GetFiles($searchPattern, [System.IO.SearchOption]::AllDirectories) | Where-Object { $_.Name.IndexOf( 'integration' ) -lt 0 }


$found.GetEnumerator() | ForEach-Object {
    $solutionFile = ([system.io.fileinfo]$_).FullName
    $shortName = [System.IO.Path]::GetFileNameWithoutExtension( $solutionFile );
    $pathName = [System.IO.Path]::GetDirectoryName( $solutionFile );
    $pathName = [System.IO.Path]::Combine( $pathName, "$shortName-0" );
    if( !([System.IO.Directory]::Exists( $pathName ))) {
        [System.IO.Directory]::CreateDirectory( $pathName ) | Out-Null
    }
    Write-Output "Testing project: $shortName"
    dotnet test $solutionFile --logger trx -c $buildConfigruation --no-restore `
            --results-directory $pathName `
            /p:CollectCoverage=true `
            /p:CoverletOutput=$coverageDir `
            /p:CoverletOutputFormat=cobertura

}