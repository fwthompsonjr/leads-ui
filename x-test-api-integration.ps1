param (
    [string]
    $buildConfigruation = 'Release'
)
$project = "legallead.permissions.api"
$workfolder = [System.IO.Path]::GetDirectoryName( $MyInvocation.MyCommand.Path );
$find = [System.IO.DirectoryInfo]::new( $workfolder ).GetFiles( "*$project.csproj", [System.IO.SearchOption]::AllDirectories );
$projectFile = $find.Item(0).FullName
if( [system.io.file]::Exists( $projectFile ) -eq $false ) {
    return;
}
$projectDir = [System.IO.Path]::GetDirectoryName( $projectFile );
dotnet build $projectFile --configuration $buildConfigruation
$search = "*legallead.permissions.api.exe"
$exes = [System.IO.DirectoryInfo]::new( $projectDir ).GetFiles( $search, [System.IO.SearchOption]::AllDirectories );
$exeFile = $null;
$exes.GetEnumerator() | ForEach-Object {
    $execinfo = $_;
    if ($null -eq $exeFile -and $execinfo.FullName.Contains($buildConfigruation) -eq $true ) {
        $exeFile = $execinfo.FullName
    }
}
if ($null -eq $exeFile ) { 
    Write-Warning "Unable to find $project executable"
    return 
}
## if ($null -ne $exeFile ) { Write-Output $exeFile; }
$myProcess = [System.Diagnostics.Process]::new();
$info = $myProcess.StartInfo;
$info.WorkingDirectory = [System.IO.Path]::GetDirectoryName($exeFile);
$info.FileName = $exeFile;
$myProcess.Start();