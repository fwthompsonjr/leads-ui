try {
    $toolpath = "C:\Tools";
    if ( [System.IO.Directory]::Exists( $toolpath ) -eq $false ) {
        [System.IO.Directory]::CreateDirectory( $toolpath );
    }
    $name = 'legallead.installer';
    $feed = 'https://www.myget.org/F/fwthompsonjr/api/v3/index.json';
    dotnet tool install $name --tool-path $toolpath --add-source $feed
} catch {
    Write-Warning "There was some issue with this step."
}