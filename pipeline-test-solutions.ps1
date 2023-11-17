param (
    [string]
    $buildConfigruation = 'Release',
    [string]
    $searchPattern = '*test*.csproj'
)

function canEnumerate( $obj ) {
    try {
        $enumerated = $obj.GetEnumerator();
        return $true;
    } catch {
        return $false;
    }
}
function generateExecutionCommand( $solution ) {
    $arr = @();
    $shortName = [System.IO.Path]::GetFileNameWithoutExtension( $solution );
    $arr += [string]::Concat("Write-Output ", "Running tests for : $shortName", "; ");
    $arr += "dotnet test $sln --logger trx -c $buildConfigruation --no-restore --results-directory --- ";
    $command = {
        $solution = $args[0]
        $buildConfigruation = $args[1]
        $errorsFile = $args[2]
        $settingsFile = $args[3]
        $shortName = [System.IO.Path]::GetFileNameWithoutExtension( $solution );
        $pathName = [System.IO.Path]::GetDirectoryName( $solution );
        $pathName = [System.IO.Path]::Combine( $pathName, "$shortName-0" );
        if( !([System.IO.Directory]::Exists( $pathName ))) {
            [System.IO.Directory]::CreateDirectory( $pathName ) | Out-Null
        }
        Write-Output "Running tests for : $shortName"
        dotnet test $solution --logger trx -c $buildConfigruation -s $settingsFile --collect "Code Coverage" --no-restore --results-directory $pathName
        
        if ($LASTEXITCODE -ne 0) {
            ("Test $shortName failed." + [Environment]::NewLine) >> $errorsFile
        }
    }
    return @{
        "text" = [string]::Join("", $arr)
        "args" = @( $solution, $buildConfigruation, $errorsFile, $settingsFile )
        "obj" = $command
        }

}
## find all files matching user search pattern 
$startedAt = [datetime]::UtcNow
$currentDir = [System.IO.Path]::GetDirectoryName( $MyInvocation.MyCommand.Path );
$errorsFile = [System.IO.Path]::Combine( $currentDir, "test-solution-error-file.txt" );
$settingsFile = [System.IO.Path]::Combine( $currentDir, "CodeCoverage.runsettings" );
$di = [System.IO.DirectoryInfo]::new( $currentDir );
$found = $di.GetFiles($searchPattern, [System.IO.SearchOption]::AllDirectories) | Where-Object { $_.Name.IndexOf( 'integration' ) -lt 0 }
$commands = @();

if( ( canEnumerate -obj $found ) -eq $false ) {
    $solutionFile = $found.FullName
    $cmmd = generateExecutionCommand -solution $solutionFile
    $commands += $cmmd
}
else {
    $found.GetEnumerator() | ForEach-Object {
        $solutionFile = ([system.io.fileinfo]$_).FullName
        $cmmd = generateExecutionCommand -solution $solutionFile
        $commands += $cmmd
    }
}
# Start the (thread) jobs.
# and make the script run considerably longer.
$jobs = $commands | Foreach-Object { Start-Job -ScriptBlock $_["obj"] -ArgumentList $_["args"] }

# Wait until all jobs have completed, passing their output through as it
# is received, and automatically clean up afterwards.
$jobs | Receive-Job -Wait -AutoRemoveJob

"All jobs completed. Total runtime in secs.: $(([datetime]::UtcNow - $startedAt).TotalSeconds)"

if( [System.IO.File]::Exists( $errorsFile ) -eq $true ) { 
    [System.IO.File]::Delete( $errorsFile )
    [Environment]::ExitCode = 1000; 
    return 1000;
}
return 0;