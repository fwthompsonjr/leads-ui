$searchPattern = "*.sln"
$currentDir = [System.IO.Path]::GetDirectoryName( $MyInvocation.MyCommand.Path );
$errorsFile = [System.IO.Path]::Combine( $currentDir, "test-solution-error-file.txt" );
$settingsFile = [System.IO.Path]::Combine( $currentDir, "src" );
$settingsFile = [System.IO.Path]::Combine( $settingsFile, "CodeCoverage.runsettings" );
$di = [System.IO.DirectoryInfo]::new( $currentDir );
$di.GetFiles($searchPattern, [System.IO.SearchOption]::AllDirectories) | Where-Object { $_.Name.IndexOf( 'integration' ) -lt 0 }