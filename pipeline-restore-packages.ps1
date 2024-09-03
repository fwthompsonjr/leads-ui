function isSolutionNotExcluded( $name ) {
    
    $exclusions = @('email', 'logging', '.api');
    foreach($item in $exclusions){
        if($name.IndexOf( $item ) -ge 0 ) { return $false; }
    }
    return  $true;
}

function generateRestoreCommand( $solution ) {
    $arr = @();
    $shortName = [System.IO.Path]::GetFileNameWithoutExtension( $solutionFile );
    $arr += [string]::Concat("Write-Output ", "Restoring packages for Solution: $shortName", "; ");
    $arr += "dotnet restore $sln";
    $command = {
        $shortName = $args[0]
        $sln = $args[1]
        $errorsFile = $args[2]
        Write-Output "Restoring packages for Solution: $shortName"; 
        dotnet restore $sln
        
        if ($LASTEXITCODE -ne 0) {
            ("Restore $shortName failed." + [Environment]::NewLine) >> $errorsFile
        }
    }
    return @{
        "text" = [string]::Join("", $arr)
        "args" = @( $shortName, $solution, $errorsFile )
        "obj" = $command
        }
}
## find all files matching *.sln 
$startedAt = [datetime]::UtcNow
$currentDir = [System.IO.Path]::GetDirectoryName( $MyInvocation.MyCommand.Path );
$errorsFile = [System.IO.Path]::Combine( $currentDir, "restore-error-file.txt" );
$di = [System.IO.DirectoryInfo]::new( $currentDir );
$found = $di.GetFiles('*.sln', [System.IO.SearchOption]::AllDirectories) | Where-Object {  
        $nme = $_.Name
        $isNotExcluded = ( isSolutionNotExcluded -name $nme ) 
        return $isNotExcluded;
}
$commands = @();

if( $found.Count -eq $null ) {
    $solutionFile = $found.FullName
    $cmmd = generateRestoreCommand -solution $solutionFile
    $commands += $cmmd
}
else {
    $found.GetEnumerator() | ForEach-Object {
        $solutionFile = ([system.io.fileinfo]$_).FullName
        $cmmd = generateRestoreCommand -solution $solutionFile
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