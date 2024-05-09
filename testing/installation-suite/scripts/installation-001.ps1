# Run a separate PowerShell process
# because the script calls exit,
# so it will end the current PowerShell session.
$currentDir = [System.IO.Path]::GetDirectoryName( $MyInvocation.MyCommand.Path );

$url = 'https://dot.net/v1/dotnet-install.ps1'
$dest = [System.IO.Path]::Combine( $currentDir, 'dotnet-install.ps1');
if ( [System.IO.File]::Exists( $dest ) -eq $false )
{
    Invoke-WebRequest $url -OutFile $dest
}
try
{
    & $dest -Channel LTS
} catch {
    $mssg = $_.Exception.Message;
    Write-Warning $mssg;
}