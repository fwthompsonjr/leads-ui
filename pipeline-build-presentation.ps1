param(
    [string]
    $version = "1.0.0"
)
function isFileNameValid($fullName) {
    try {
        $blnIsGood = ($null -ne $fullName);
        if ($blnIsGood -eq $true ) { $blnIsGood = $fullName.Contains($version) -and $fullName.ToLower().Contains('release') }
        if ($blnIsGood -eq $true ) { $blnIsGood = [System.IO.File]::Exists( $fullName ); }
        if ($blnIsGood -eq $true ) { $blnIsGood = ($fullName.ToLower().Contains('legallead.installer') -eq $false) }
        return $blnIsGood;
    } catch { return $false; }
}

function sanitizeFileName($fullName) {
    try {
        if ( [System.IO.File]::Exists( $fullName ) -eq $false ) { return $fullName }
		$dirname = [System.IO.Path]::GetDirectoryName( $fullName );
		$shortname = [System.IO.Path]::GetFileName( $fullName );
		$find = "1.0.0-$version"
		if ($shortname.Contains( $find ) ) {
			$newname = [System.IO.Path]::Combine( $dirname, $shortname.Replace( $find, $version) );
			[System.IO.File]::Copy( $fullName, $newname, $true )
			[System.IO.File]::Delete( $fullName ) | Out-Null
			return $newname;
		}
		return $fullName;
    } catch { 
		return $fullName; 
	}
}
function generateBuildCommand( $solution ) {
    $arr = @();
    $shortName = [System.IO.Path]::GetFileNameWithoutExtension( $solutionFile );
    $sln = [string]::Concat( '"', $solution, '"' );
    $arr += [string]::Concat("Write-Output ", "Building Solution: $shortName $version", "; ");
    $arr += "dotnet build $sln /p:AssemblyVersion=$version /p:VersionPrefix=$version -t:rebuild --property:Configuration=Release";
    $command = {
        $version = $args[0]
        $shortName = $args[1]
        $sln = $args[2]
        $errorsFile = $args[3]
        Write-Output "Building Solution: $shortName $version"; 
        dotnet build $sln /p:AssemblyVersion=$version /p:VersionPrefix=$version -t:rebuild --property:Configuration=Release
        
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

function isSolutionIncluded( $name ) {
    
    $exclusions = @('presentation');
    foreach($item in $exclusions){
        if($name.IndexOf( $item ) -ge 0 ) { return $true; }
    }
    return  $false;
}

function deleteNuPkgFiles( $homedir ) {
    try {
        $dii = [System.IO.DirectoryInfo]::new( $homedir );
        $packages = $dii.GetFiles('*.nupkg', [System.IO.SearchOption]::AllDirectories)
        if ( $packages.Count -eq $null ) {
            $pname = $packages.FullName;
            $shouldExecute = isFileNameValid -fullName $pname
            if ( $shouldExecute -eq $false ) { 
                [System.IO.File]::Delete( $pname ) | Out-Null
            } else {
                sanitizeFileName -fullName $pname
            }
        } else {
            $packages.GetEnumerator() | ForEach-Object {
                $pname = ([system.io.fileinfo]$_).FullName
                $shouldExecute = isFileNameValid -fullName $pname
                if ( $shouldExecute -eq $false ) { 
                    [System.IO.File]::Delete( $pname ) | Out-Null
                } else {
                    sanitizeFileName -fullName $pname
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
        $isIncluded = ( isSolutionIncluded -name $nme ) 
        return $isIncluded;
}

$commands = @();
$solutionFile = $found.FullName
$cmmd = generateBuildCommand -solution $solutionFile
$commands += $cmmd

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
    throw "One or more errors occured during build process."
}

return 0;