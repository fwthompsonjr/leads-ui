
function chkChocoCmmd()
{
    try {
        $obj = (Get-Command -Name choco.exe -ErrorAction SilentlyContinue)
        return $obj;
    } catch {
        return $null;
    }
}

$itm = chkChocoCmmd
if ($null -ne $itm) {
    Write-Output "Choco installation confirmed"
    return;
}
try {
    Set-ExecutionPolicy Bypass -Scope Process -Force; 
    [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072; 
    iex ((New-Object System.Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'));
    Write-Output "Choco install completed."
} catch {
    Write-Error "Error installing choco software."
}