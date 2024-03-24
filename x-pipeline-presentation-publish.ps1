param(
    [string]
    $version = "3.2.0"
)

$startedAt = [datetime]::UtcNow
$workfolder = [System.IO.Path]::GetDirectoryName( $MyInvocation.MyCommand.Path );
$binfolder = [System.IO.Path]::Combine( $workfolder, "_legallead_desktop_windows" );
$project = "legallead.desktop"
$zipFileName = [string]::Concat($project, "-windows-", $version, ".zip");
$zipFileName = [System.IO.Path]::Combine( $workfolder, $zipFileName );
$errorsFile = [System.IO.Path]::Combine( $workfolder, [string]::Concat( $project.Replace(".", "-"), "-error-file.txt" ));
$projectFolder = "src\presentation\$project"
$projectFolder = [System.IO.Path]::Combine( $workfolder, $projectFolder )
$projectFile = [System.IO.Path]::Combine( $projectFolder, "$project.csproj" )
if( [system.io.file]::Exists( $projectFile ) -eq $false ) {
    return;
}

if( [System.IO.Directory]::Exists( $binfolder ) -eq $true ) {
    $di = [System.IO.DirectoryInfo]::new( $binfolder );
    $di.Delete( $true ) | Out-Null;
}
if( [System.IO.Directory]::Exists( $binfolder ) -eq $false ) {
    [System.IO.Directory]::CreateDirectory( $binfolder ) | Out-Null;
}
Write-Output "Publishing project: ' $project ' to temp directory"
dotnet publish $projectFile -o $binfolder --sc $true --version-suffix $version -c Release

if ($LASTEXITCODE -ne 0) {
    Write-Output "Publish $project failed."
    [Environment]::ExitCode = 1000;
    return 1000;
}

if( [system.io.file]::Exists( $zipFileName ) -eq $true ) {
    [System.IO.File]::Delete( $zipFileName ) | Out-Null
}
Write-Output "Creating compressed file: ' $project '"

Compress-Archive -Path $binfolder -DestinationPath $zipFileName -Force

if ($LASTEXITCODE -ne 0) {
    Write-Output "Compress $project to zip has failed."
    [Environment]::ExitCode = 1000;
    return 1000;
}
