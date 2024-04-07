$evname = "LEGALLEAD_INSTALLATION_KEY";
$workfolder = [System.IO.Path]::GetDirectoryName( $MyInvocation.MyCommand.Path );
$projectFile = [System.IO.Path]::Combine( $workfolder, "legallead.installer.csproj" );
$jsfolder = [System.IO.Path]::Combine( $workfolder, "_text" );
$jsfile = [System.IO.Path]::Combine( $jsfolder, "configuration-js.txt" );
$readMe = [System.IO.Path]::Combine( $workfolder, "README.md" );
$jsonReadMe = [System.IO.Path]::Combine( $workfolder, "x-installer-version.json" );
$backupReadMe = [System.IO.Path]::Combine( $workfolder, "README.backup.md" );
$versionLine = '| x.y.z | {{ date }} | {{ description }} |'
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

function updateInstallationKey() {
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
}

function getVersionNumber {
    if ( [System.IO.File]::Exists( $jsonReadMe ) -eq $false ) { 
        return "1.0.0"; 
    }
    $jscontent = [System.IO.File]::ReadAllText( $jsonReadMe ) | ConvertFrom-Json
    return $jscontent.Item(0).id
    
}

function updateReadMe() {
    if ( [System.IO.File]::Exists( $readMe ) -eq $false ) { 
        Write-Output "Source file is not found."
        [System.Environment]::ExitCode = 1;
        return 1; 
    }
    if ( [System.IO.File]::Exists( $jsonReadMe ) -eq $false ) { 
        Write-Output "JSON file is not found."
        [System.Environment]::ExitCode = 1;
        return 1; 
    }
    [System.IO.File]::Copy( $readMe, $backupReadMe, $true ) 
    $details = @();
    $jscontent = [System.IO.File]::ReadAllText( $jsonReadMe ) | ConvertFrom-Json
    $jscontent.GetEnumerator() | ForEach-Object {
        $item = $_;
        $index = $item.id;
        $description = $item.description;
        $dte = $item.date;
        $details += ("| $index | $dte | $description |   ")
    }
    if ($details.Length -eq 0 ) { return; }

    $rmtransform = [string]::Join( [Environment]::NewLine, $details );
    $rmcontent = [System.IO.File]::ReadAllText( $readMe )
    $rmcontent = $rmcontent.Replace( $versionLine, $rmtransform )
    [System.IO.File]::Delete( $readMe ) 
    [System.IO.File]::WriteAllText( $readMe, $rmcontent );
    
}
function getReleaseNotes() {
    try {
        $tb = "     ";
        $dashes = "$tb - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - "
        $txt = [System.IO.File]::ReadAllText( $jsonReadMe ) | ConvertFrom-Json
        $dat = @("", "$tb legallead.installer", $dashes);
        $txt.GetEnumerator() | ForEach-Object {
            $nde = $_;
            $ln = "$tb : $($nde.id) - $($nde.date) - $($nde.description)"
            $dat += $ln
        }
        $dat += $dashes
        $dat += ""
        $detail = [string]::Join( [Environment]::NewLine, $dat)
        $detail += "    "
        return $detail
    } catch {
        return $null
    }
}

function updateVersionNumbers() {

    if ( [System.IO.File]::Exists( $projectFile ) -eq $false ) { 
        Write-Output "Source file is not found."
        [System.Environment]::ExitCode = 1;
        return 1; 
    }
    $hasXmlChange = $false;
    $nodeNames = @( 'AssemblyVersion', 'FileVersion', 'Version' );
    $versionStamp = getVersionNumber
    [xml]$prj = [System.IO.File]::ReadAllText( $projectFile );
    $nde = $prj.DocumentElement.SelectSingleNode('PropertyGroup')
    if ( $nde -eq $null ) { return; }
    $nde.ChildNodes | ForEach-Object {
        [System.Xml.XmlNode]$child = $_;
        $nodeName = $child.Name;
        $nodeValue = $child.InnerText;
        if ( $nodeNames.Contains( $nodeName ) -eq $true -and $nodeValue.Equals($versionStamp) -eq $false ) {
            $hasXmlChange = $true;
            $child.InnerText = $versionStamp;
        }
        if ('PackageReleaseNotes' -eq $nodeName ) {
            $expected = getReleaseNotes
            if ($expected -ne $null -and $expected.Equals($nodeValue) -eq $false ) {
                $hasXmlChange = $true;
                $child.InnerText = $expected;
            }
        }
    }
    if ($hasXmlChange -eq $true ) {
        $prj.Save( $projectFile );
    }
}

updateVersionNumbers
updateInstallationKey
updateReadMe