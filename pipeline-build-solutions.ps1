param(
    [string]
    $version = "1.0.0"
)


function generateBuildCommand( $solution ) {
    $arr = @();
    $shortName = [System.IO.Path]::GetFileNameWithoutExtension( $solutionFile );
    $sln = [string]::Concat( '"', $solution, '"' );
    $arr += [string]::Concat("Write-Output ", "Building Solution: $shortName $version", "; ");
    $arr += "dotnet build $sln /p:AssemblyVersion=$version /p:VersionPrefix=$version --property:Configuration=Release";
    $command = {
        $version = $args[0]
        $shortName = $args[1]
        $sln = $args[2]
        $errorsFile = $args[3]
        Write-Output "Building Solution: $shortName $version"; 
        dotnet build $sln /p:AssemblyVersion=$version /p:VersionPrefix=$version --property:Configuration=Release
        
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

function isSolutionNotExcluded( $name ) {
    
    $exclusions = @('email', 'logging', '.api');
    foreach($item in $exclusions){
        if($name.IndexOf( $item ) -ge 0 ) { return $false; }
    }
    return  $true;
}

function deleteNuPkgFiles( $homedir ) {
    try {
        $dii = [System.IO.DirectoryInfo]::new( $homedir );
        $packages = $dii.GetFiles('*.nupkg', [System.IO.SearchOption]::AllDirectories)
        if ( $packages.Count -eq $null ) {
            $pname = $packages.FullName;
            if ( $pname.IndexOf("1.0.0") -lt 0 ) { return; }
            [System.IO.File]::Delete( $pname ) | Out-Null
        } else {
            $packages.GetEnumerator() | ForEach-Object {
                $pname = ([system.io.fileinfo]$_).FullName
                if ( $pname.IndexOf("1.0.0") -ge 0 ) {
                    [System.IO.File]::Delete( $pname ) | Out-Null
                }
            }
        }
    } catch {
        ## no action on failed
    }
}

$startedAt = [datetime]::UtcNow
## find all files matching *.sln 
$currentDir = [System.IO.Path]::GetDirectoryName( $MyInvocation.MyCommand.Path );
$errorsFile = [System.IO.Path]::Combine( $currentDir, "build-error-file.txt" );
$di = [System.IO.DirectoryInfo]::new( $currentDir );
$found = $di.GetFiles('*.sln', [System.IO.SearchOption]::AllDirectories) | Where-Object {  
        $nme = $_.Name
        $isNotExcluded = ( isSolutionNotExcluded -name $nme ) 
        return $isNotExcluded;
}
$prjfound = $di.GetFiles('*.legallead.desktop.core.csproj', [System.IO.SearchOption]::AllDirectories);
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
	
if( $prjfound.Count -eq $null ) {
    $solutionFile = $prjfound.FullName
    $cmmd = generateBuildCommand -solution $solutionFile
    $commands += $cmmd
}
else {
    $prjfound.GetEnumerator() | ForEach-Object {
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
deleteNuPkgFiles -homedir $currentDir
if( [System.IO.File]::Exists( $errorsFile ) -eq $true ) { 
    [System.IO.File]::Delete( $errorsFile )
    [Environment]::ExitCode = 1000; 
    echo "FAILED_TEST_COUNT=1000" >> $env:GITHUB_ENV
    return 1000;
}

return 0;