<#
    test:
    given the current application folder.
    i can locate the latest version of the reader server
#>
$appfolder = "C:\Users\frank.thompson\AppData\Local\_ll-applications\legallead.desktop-windows\3.2.15\_legallead_desktop_windows"
if ( [System.IO.Directory]::Exists( $appfolder ) -eq $false ) { return;}
$find = @{
    token = "_ll-applications"
    exname = "legallead.reader.service"
}

$indx = $appfolder.IndexOf( $find.token );
if ( $indx -lt 0 ) { return; }
$parentFolder = $appfolder.Substring(0, $indx + ($find.token.Length));
if ( [System.IO.Directory]::Exists( $parentFolder ) -eq $false ) { return; }
$search = [string]::Concat( $find.exname, ".exe" )
$files = [System.IO.DirectoryInfo]::new( $parentFolder ).GetFiles( $search, [System.IO.SearchOption]::AllDirectories ) | Sort-Object -Property CreationTime;
$files | ForEach-Object {
    $fi = $_;
    $fi.CreationTime
}

return;
[System.IO.FileInfo]$tmp = $files.Item(0);
$tmp.CreationTime