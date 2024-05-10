param(
    [string]
    $version = "3.2.0"
)

function echoZipFileName {
    try {
        Write-Output "ZIPFILE: $zipFileName is created."
        echo "ZIPFILE_NAME::$zipFileName" >> $GITHUB_ENV
    } catch { }
}
$startedAt = [datetime]::UtcNow
$workfolder = [System.IO.Path]::GetDirectoryName( $MyInvocation.MyCommand.Path );
$binfolder = [System.IO.Path]::Combine( $workfolder, "installation-suite/scripts" );
$project = "legallead.zip.installation"
$zipFileName = [string]::Concat($project, "-", $version, ".zip");
$zipFileName = [System.IO.Path]::Combine( $workfolder, $zipFileName );

if( [System.IO.Directory]::Exists( $binfolder ) -eq $false ) {
    throw "$project script folder is not found!"
}

if( [system.io.file]::Exists( $zipFileName ) -eq $true ) {
    [System.IO.File]::Delete( $zipFileName ) | Out-Null
}
Write-Output "Creating compressed file: ' $project '"

Compress-Archive -Path "$binfolder\*" -DestinationPath $zipFileName -Force

if( [system.io.file]::Exists( $zipFileName ) -eq $false ) {
    Write-Output "Compress $project to zip has failed."
    [Environment]::ExitCode = 1000;
    return 1000;
} else {
    echoZipFileName
}
