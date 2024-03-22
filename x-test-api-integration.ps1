param (
    [string]
    $buildConfigruation = 'Release',
    [string]
    $workfolder = ''
)
if ([string]::IsNullOrEmpty( $workfolder ) -eq $true -or [System.IO.Directory]::Exists( $workfolder ) -eq $false ) {
    return;
}

$project = "legallead.permissions.api"
$projectFolder = "src\api\$project"
$projectFolder = [System.IO.Path]::Combine( $workfolder, $projectFolder )
$projectFile = [System.IO.Path]::Combine( $projectFolder, "$project.csproj" )
if( [system.io.file]::Exists( $projectFile ) -eq $false ) {
    return;
}
dotnet run --project $projectFile --configuration $buildConfigruation