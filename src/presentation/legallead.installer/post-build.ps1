$evname = "-- no data --";
$workfolder = [System.IO.Path]::GetDirectoryName( $MyInvocation.MyCommand.Path );
$jsfolder = [System.IO.Path]::Combine( $workfolder, "_text" );
$jsfile = [System.IO.Path]::Combine( $jsfolder, "configuration-js.txt" );
$readMe = [System.IO.Path]::Combine( $workfolder, "README.md" );
$backupReadMe = [System.IO.Path]::Combine( $workfolder, "README.backup.md" );

if ( [System.IO.File]::Exists( $jsfile ) -eq $false ) { 
    Write-Output "Source file is not found."
    [System.Environment]::ExitCode = 1;
    return 1; 
}
$jcontent = [System.IO.File]::ReadAllText( $jsfile ) | ConvertFrom-Json
if ($jcontent.key -eq $evname) { return; }
$jcontent.key = $evname
$transform = ($jcontent | ConvertTo-Json)
[System.IO.File]::WriteAllText( $jsfile, $transform );
if ( [System.IO.File]::Exists( $readMe ) -eq $false ) { 
    Write-Output "README file is not found."
    [System.Environment]::ExitCode = 1;
    return 1; 
}
if ( [System.IO.File]::Exists( $backupReadMe ) -eq $false ) { 
    Write-Output "README back up file is not found."
    [System.Environment]::ExitCode = 1;
    return 1; 
}
[System.IO.File]::Copy( $backupReadMe, $readMe, $true ) 
[System.IO.File]::Delete( $backupReadMe ) 