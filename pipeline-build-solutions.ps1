param(
    [string]
    $version = "1.0.0"
)


function generateBuildCommand( $solution ) {
    $arr = @();
    $shortName = [System.IO.Path]::GetFileNameWithoutExtension( $solutionFile );
    $sln = [string]::Concat( '"', $solution, '"' );
    $arr += [string]::Concat("Write-Output ", "Building Solution: $shortName $version", "; ");
    $arr += "dotnet build $sln /p:VersionPrefix=$version -t:rebuild --property:Configuration=Release";
    $command = {
        $version = $args[0]
        $shortName = $args[1]
        $sln = $args[2]
        $errorsFile = $args[3]
        Write-Output "Building Solution: $shortName $version"; 
        dotnet build $sln /p:VersionPrefix=$version -t:rebuild --property:Configuration=Release
        
        if ($LASTEXITCODE -ne 0) {
            ("Build $shortName failed." + [Environment]::NewLine) >> $errorsFile
        }
    }
    return @{
        "text" = [string]::Join("", $arr)
        "args" = @( $version, $shortName, $solution, $errorsFile )
        "obj" = $command
        }
}
$startedAt = [datetime]::UtcNow

## find all files matching *.sln 
$currentDir = [System.IO.Path]::GetDirectoryName( $MyInvocation.MyCommand.Path );
$errorsFile = [System.IO.Path]::Combine( $currentDir, "build-error-file.txt" );
$di = [System.IO.DirectoryInfo]::new( $currentDir );
$found = $di.GetFiles('*.sln', [System.IO.SearchOption]::AllDirectories)
$commands = @();

if( $found.Count -eq $null ) {
    $solutionFile = $found.FullName
    $cmmd = generateBuildCommand -solution $solutionFile
    $commands += $cmmd
}
else {
    $found.GetEnumerator() | ForEach-Object {
        $solutionFile = ([system.io.fileinfo]$_).FullName
        $cmmd = generateBuildCommand -solution $solutionFile
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