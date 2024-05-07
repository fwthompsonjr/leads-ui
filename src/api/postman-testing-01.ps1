
$myfolder = [System.IO.Path]::GetDirectoryName( $MyInvocation.MyCommand.Path );


function canEnumerate( $obj ) {
    try {
        $enumerated = $obj.GetEnumerator();
        return $true;
    } catch {
        return $false;
    }
}
function verifyExecutable( $name )
{
    $canExecute =(
        ( $name.IndexOf( "permissions.api.tests" ) -lt 0 ) -and 
        ( $name.IndexOf( "Debug" ) -gt 0 ) -and 
        ( $name.IndexOf( "net8.0" ) -gt 0 ));
    return $canExecute
}

$executables = @();
$files = [System.IO.DirectoryInfo]::new( $myfolder ).GetFiles( "*legallead.permissions.api.exe", [System.IO.SearchOption]::AllDirectories );
if ( $null -eq $files -or $files.Length -eq 0 ) { return; }
$isEnumerable = canEnumerate -obj $files

if ( $isEnumerable -eq $false ) {
    $fullName = $files.FullName;
    $isvalid = verifyExecutable -name $fullName
    if ( $isvalid -eq $true ) { $executables+=($fullName); }
} else {
    $files.GetEnumerator() | ForEach-Object {
        [System.IO.FileInfo]$fi = $_;
        $fullName = $fi.FullName;
        $isvalid = verifyExecutable -name $fullName
        if ( $isvalid -eq $true ) { $executables+=($fullName); }    
    }
}
if( $executables.Count -eq 0 ) { return 0; }
$sampleExe = $executables.Item(0);
$workingFolder = [System.IO.Path]::GetDirectoryName( $sampleExe )
$prc = (Start-Process $sampleExe -WorkingDirectory $workingFolder -WindowStyle Normal -PassThru)
$pindex = $prc.Id;
Write-Host "Process $pindex has started"
return $pindex