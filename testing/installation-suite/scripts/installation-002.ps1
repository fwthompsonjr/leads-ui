try {
$name = 'legallead.installer';
$feed = 'https://www.myget.org/F/fwthompsonjr/api/v3/index.json';
dotnet tool install -g $name --add-source $feed
} catch {
    Write-Warning "There was some issue with this step."
}