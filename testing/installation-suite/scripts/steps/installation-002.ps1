
$name = 'legallead.installer';
$feed = 'https://www.myget.org/F/fwthompsonjr/api/v3/index.json';

function listTools() {
    $isUninstallNeeded = $false
    try {
        $tools = (dotnet tool list -g);
        $tools.GetEnumerator() | ForEach-Object {
            if ($_.Contains($name) -eq $true) {
                $isUninstallNeeded = $true
            }
        }
        return $isUninstallNeeded;
    } catch {
        return $false;
    }

}

function tryUnInstall()
{
    try {
        dotnet tool uninstall $name -g
    } catch {
        return
    }
}

function tryInstall()
{
    try {
        dotnet tool install $name --add-source $feed -g
    } catch {
        return
    }
}
try {
    $isNeeded = listTools
    Write-Output "Uninstall required := $isNeeded"
    if ( $isNeeded -eq $true ) {
        tryUnInstall
    }
    tryInstall
} catch {
    Write-Warning "There was some issue with this step."
}