function getEnvironment($name) {
    try {
        return [System.Environment]::GetEnvironmentVariable($name);
    } catch {
        return $null
    }
}
function getEnvironmentUser($name) {
    try {
        return [System.Environment]::GetEnvironmentVariable($name, [System.EnvironmentVariableTarget]::User);
    } catch {
        return $null
    }
}

function getEnvironmentProcess($name) {
    try {
        return [System.Environment]::GetEnvironmentVariable($name, [System.EnvironmentVariableTarget]::Process);
    } catch {
        return $null
    }
}

function getEnvironmentMachine($name) {
    try {
        return [System.Environment]::GetEnvironmentVariable($name, [System.EnvironmentVariableTarget]::Machine);
    } catch {
        return $null
    }
}
function getEnvironmentSetting($name) {
    [string]$setting = $null;
    $searchId = 0;
    while ( [string]::IsNullOrEmpty( $setting ) -eq $true -and $searchId -lt 4 ) {
        if( $searchId -eq 0 ) { $setting = (getEnvironment -name $name); }
        if( $searchId -eq 1 ) { $setting = (getEnvironmentUser -name $name); }
        if( $searchId -eq 2 ) { $setting = (getEnvironmentProcess -name $name); }
        if( $searchId -eq 3 ) { $setting = (getEnvironmentMachine -name $name); }
        $searchId++;
    }
    return $setting;
}

$evname = "LEGALLEAD_INSTALLATION_KEY";
$workfolder = [System.IO.Path]::GetDirectoryName( $MyInvocation.MyCommand.Path );
$jsfolder = [System.IO.Path]::Combine( $workfolder, "_text" );
$jsfile = [System.IO.Path]::Combine( $jsfolder, "configuration-js.txt" );
if ( [System.IO.File]::Exists( $jsfile ) -eq $false ) { 
    Write-Output "Source file is not found."
    [System.Environment]::ExitCode = 1;
    return 1; 
}
$evkey = getEnvironmentSetting -name $evname
if ( [string]::IsNullOrEmpty( $evkey ) -eq $true ) { 
    Write-Output "Environment key $evname is not found."
    [System.Environment]::ExitCode = 1;
    return 1; 
}
$jcontent = [System.IO.File]::ReadAllText( $jsfile ) | ConvertFrom-Json
if ($jcontent.key -eq $evkey) { return; }
$jcontent.key = $evkey
$transform = ($jcontent | ConvertTo-Json)
[System.IO.File]::WriteAllText( $jsfile, $transform );