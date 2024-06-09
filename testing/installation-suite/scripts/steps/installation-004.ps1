
function chkFirefoxCmmd()
{
    try {
        $firefox = (Get-Item (Get-ItemProperty 'HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\firefox.exe').'(Default)').VersionInfo
        return $firefox;
    } catch {
        return $null;
    }
}

$itm = chkFirefoxCmmd
if ($null -ne $itm) {
    Write-Output "Firefox installation confirmed"
    return;
}
try {
    choco install -y firefox
    Write-Output "Firefox install completed."
} catch {
    Write-Error "Error installing Firefox software."
}