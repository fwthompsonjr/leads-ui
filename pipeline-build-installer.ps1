param(
    [string]
    $version = "1.0.0"
)

function generateBuildCommand( $solution ) {
    $arr = @();
    $shortName = [System.IO.Path]::GetFileNameWithoutExtension( $solutionFile );
    $sln = [string]::Concat( '"', $solution, '"' );
    $arr += [string]::Concat("Write-Output ", "Building Solution: $shortName $version", "; ");
    $arr += "dotnet build $sln --property:Configuration=Release";
    $arr += "dotnet test $sln --property:Configuration=Release";
    $command = {
        $version = $args[0]
        $shortName = $args[1]
        $sln = $args[2]
        $errorsFile = $args[3]
        Write-Output "Building Solution: $shortName $version"; 
        dotnet build $sln --property:Configuration=Release
        $dir = [System.IO.Path]::GetDirectoryName($sln);
        $di = [System.IO.DirectoryInfo]::new( $dir )
        $files = $di.GetFiles("*installer.tests.csproj", [System.IO.SearchOption]::AllDirectories);
        $testProj = $files.Item(0).FullName
        dotnet test $testProj --configuration Release --no-restore
        
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
            if ( $pname.IndexOf("legallead.installer") -ge 0 ) { return; }
            [System.IO.File]::Delete( $pname ) | Out-Null
        } else {
            $packages.GetEnumerator() | ForEach-Object {
                $pname = ([system.io.fileinfo]$_).FullName
                if ( $pname.IndexOf("legallead.installer") -lt 0 ) {
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
        $isIncluded = ( isSolutionIncluded -name $nme ) 
        return $isIncluded;
}

$commands = @();
$solutionFile = $found.FullName

dotnet build $solutionFile --configuration Release
$dir = [System.IO.Path]::GetDirectoryName($solutionFile);
$di = [System.IO.DirectoryInfo]::new( $dir )
$files = $di.GetFiles("*installer.tests.csproj", [System.IO.SearchOption]::AllDirectories);
$testProj = $files.Item(0).FullName
dotnet test $testProj --configuration Release --no-restore


deleteNuPkgFiles -homedir $currentDir
if( [System.IO.File]::Exists( $errorsFile ) -eq $true ) { 
    [System.IO.File]::Delete( $errorsFile )
    [Environment]::ExitCode = 1000; 
    echo "FAILED_TEST_COUNT=1000" >> $env:GITHUB_ENV
    return 1000;
}

return 0;