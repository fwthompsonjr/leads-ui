$evname = "LEGALLEAD_EMAIL_TOKEN";
$evuid = "LEGALLEAD_EMAIL_USER";
$workfolder = [System.IO.Path]::GetDirectoryName( $MyInvocation.MyCommand.Path );
$jsfolder = [System.IO.Path]::Combine( $workfolder, "_settings" );
$jsfile = [System.IO.Path]::Combine( $jsfolder, "smtp-settings.txt" );
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

function updateEmailToken() {
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
    $evuser = getEnvironmentSetting -name $evuid
    if ( [string]::IsNullOrEmpty( $evuser ) -eq $true ) { 
        Write-Output "Environment key $evuid is not found."
        [System.Environment]::ExitCode = 1;
        return 1; 
    }
    $jcontent = [System.IO.File]::ReadAllText( $jsfile ) | ConvertFrom-Json
    if ($jcontent.secret -eq $evkey -and $jcontent.uid -eq $evuser ) { return; }
    $jcontent.secret = $evkey
    $jcontent.uid = $evuser
    $transform = ($jcontent | ConvertTo-Json -Depth 2)
    [System.IO.File]::WriteAllText( $jsfile, $transform );
}

updateEmailToken