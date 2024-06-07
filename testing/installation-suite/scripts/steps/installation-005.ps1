<#
    installation-005.ps1
    Source:
        Get Download information from configuration file
    Destination:
        Environment.SpecialFolder.LocalApplicationData
        subfolder: jsfile.name
        install-folder: jsfile.version
#>

function generateDownloadUri( $js ) {
    try
    {
        $formatted = $js.url;
        $runIndex =  [Convert]::ToString( $js.runId );
        $itemIndex =  [Convert]::ToString( $js.artifactId );
        $response = $formatted -f $runIndex, $itemIndex
        return $response;
    } catch {
        return $null;
    }

}

function stopRunningProcesses( $js )
{
    try {
        $appname = $js.name;
        $processes = (Get-Process | Where { $_.ProcessName -eq $appname });
        if ($null -eq $processes ) { return $true; }
        
        $processes | Stop-Process -Force
        return $true
    } catch {
        return $null;
    }
}

function getUserFolders ( $js ) {
    try {
        $appname = $js.name;
        $appversion = $js.version;
        $localDataPath = $env:LOCALAPPDATA
        $folders = @( 
            $localDataPath,
            [System.IO.Path]::Combine( $localDataPath, $appname ),
            [System.IO.Path]::Combine( $localDataPath, "$appname\$appversion" )
        );
        return $folders;
    } catch {
        return $null;
    }
}

function buildUserFolders ( $js )
{
    [string]$dest = "";
    try {
        $destinationPaths = getUserFolders
        $destinationPaths.GetEnumerator() | ForEach-Object {
            $dest = $_;
            if ( [System.IO.Directory]::Exists( $dest ) -eq $false ) {
                [System.IO.Directory]::CreateDirectory( $dest )
            }
        }
        return $dest;
    } catch {
        return $null;
    }
}

function getDestinationFileName()
{
    try {
        $tmpfolder = $env:LOCALAPPDATA
        $tmbsubfolder = [System.IO.Path]::Combine( $tmpfolder, "ll-tmp" );
        $tmpname = [string]::Concat( [System.IO.Path]::GetFileNameWithoutExtension([System.IO.Path]::GetRandomFileName()), ".zip" );
        if ( [System.IO.Directory]::Exists( $tmbsubfolder ) -eq $false ) {
            [System.IO.Directory]::CreateDirectory( $tmbsubfolder )
        }
        $tmpfullname = [System.IO.Path]::Combine( $tmbsubfolder, $tmpname );
        if ( [System.IO.File]::Exists( $tmpfullname ) -eq $true ) {
            [System.IO.File]::Delete( $tmpfullname ) | Out-Null
        }
        return $tmpfullname;
    } catch {
        return $null;
    }
}

function downloadZipFile( $webaddress, $downloadpath ) {
    try {
        if ( [string]::IsNullOrWhiteSpace( $downloadpath ) -eq $true ) { return $null }
        $key = $env:LEGALLEAD_INSTALLATION_KEY
        if ( [string]::IsNullOrWhiteSpace( $key ) -eq $true ) { return $null }
        $hdr = @{ 
            "Authorization" = "token $key"
            "UserAgent" = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.77 Safari/537.36"
            "Accept" = "application/octet-stream" }
        Invoke-RestMethod -Uri $webaddress -OutFile $downloadpath -Headers $hdr -Method GET
        if ( [System.IO.File]::Exists( $downloadpath ) -eq $false ) { return $null }
        return $downloadpath
    } catch {
        Write-Error $_.Exception.Message
        return $null;
    }
}

$itemDir = [System.IO.Path]::GetDirectoryName( $MyInvocation.MyCommand.Path );
$configFile = [System.IO.Path]::Combine( $itemDir, "reader.config.json" );
if ( [System.IO.File]::Exists( $configFile ) -eq $false ) { return; }
$stepId = 1;
Write-Output " ( $stepId ) Reading configuration"
$config = [System.IO.File]::ReadAllText( $configFile );
$jsconfig = $config | ConvertFrom-Json
$applicationName = $jsconfig.name;
$downloadUri = generateDownloadUri -js $jsconfig
if ( $null -eq $downloadUri )
{
    Write-Warning "Failed to read configuration for: $applicationName";
    return;
}

$stepId++;
Write-Output " ( $stepId ) Stopping any running applications"
$stopped = stopRunningProcesses -js $jsconfig
if ( $null -eq $stopped )
{
    Write-Warning "Failed to stop processes for: $applicationName";
    return;
}

$stepId++;
Write-Output " ( $stepId ) Building target folders"
$targetFolder = buildUserFolders -js $jsconfig
if ( $null -eq $targetFolder )
{
    Write-Warning "Failed to create folders for: $applicationName";
    return;
}

$stepId++;
Write-Output " ( $stepId ) Downloading package $applicationName"
$destinationFile = getDestinationFileName
$zipFile = (downloadZipFile -webaddress $downloadUri -downloadpath $destinationFile)
if ( $null -eq $zipFile )
{
    Write-Warning "Failed to downloading package for: $applicationName";
    return;
}
$stepId++;
Write-Output " ( $stepId ) Installing package $applicationName"
