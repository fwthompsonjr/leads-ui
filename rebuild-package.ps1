<#
    repackage content
#>

$homedir = [System.IO.Path]::GetDirectoryName( $MyInvocation.MyCommand.Path );
$workdir = [System.IO.Path]::Combine( $homedir, "_work");

function canEnumerate( $obj ) {
    try {
        $enumerated = $obj.GetEnumerator();
        return $true;
    } catch {
        return $false;
    }
}
function changeNuSpec( $path ) {
    if ( [System.IO.File]::Exists( $path ) -eq $false ) { return $null; }
    [string]$content = [System.IO.File]::ReadAllText( $path );
    $token = "<version>";
    $closetoken = $token.Replace("<", "</");
    $aa = $content.IndexOf($token);
    $bb = $content.IndexOf($closetoken);
    $number = $content.Substring($aa, $bb - $aa).Replace($token, "").Replace($closetoken,"");
    $arr = $number.Split("-")
    $changed = $arr[$arr.Count - 1];
    $find = "{0}{1}{2}" -f $token, $number, $closetoken
    $replacement = "{0}{1}{2}" -f $token, $changed, $closetoken
    $newtext = $content.Replace( $find, $replacement);
    [System.IO.File]::WriteAllText( $path, $newtext );
    return $changed;
}
function processFileData( $obj ) {
    try {
        if ( [System.IO.Directory]::Exists( $workdir ) -eq $false ) {
            [System.IO.Directory]::CreateDirectory( $workdir ) | Out-Null;
        }
        $searches = @("legallead.desktop.core", "1.0.0-");
        $info = ([system.io.fileinfo]$obj);
        $blnmatch = $false;
        $packageFile = $info.FullName.ToLower();
        foreach($search in $searches) {
            if ($packageFile.Contains($search) -eq $false) {
                $blnmatch = $false;
                break;
            }
            $blnmatch = $true;
        }
        if ($blnmatch -eq $false ) { return; }
        $zippedFileName = $info.Name.ToLower().Replace(".nupkg", ".zip");
        $zippedFile = [System.IO.Path]::Combine( $workdir, $zippedFileName);
        $destinationPath = [System.IO.Path]::Combine( $workdir, "working");
        $specFile = [System.IO.Path]::Combine( $destinationPath, "legallead.desktop.core.nuspec");
        if ( [System.IO.Directory]::Exists( $destinationPath ) ) {
            [System.IO.DirectoryInfo]::new( $destinationPath ).Delete($true) | Out-Null;
        }
        [System.IO.File]::Copy( $packageFile, $zippedFile, $true ) | Out-Null;
        Expand-Archive -LiteralPath $zippedFile -DestinationPath $destinationPath
        $nwversionid = (changeNuSpec -path $specFile)
        if ($null -ne $nwversionid ) {
            $targetFile = "legallead.desktop.core-{0}.zip" -f $nwversionid
            $specFile = "legallead.desktop.core-{0}.nupkg" -f $nwversionid
            $newZipFile = [System.IO.Path]::Combine( $homedir, $targetFile );
            $newSpecFile = [System.IO.Path]::Combine( $homedir, $specFile );
            if ( [System.IO.File]::Exists( $newZipFile ) ) { [System.IO.File]::Delete( $newZipFile ) | Out-Null }
            if ( [System.IO.File]::Exists( $newSpecFile ) ) { [System.IO.File]::Delete( $newSpecFile ) | Out-Null }
            $destination = [string]::Concat( $destinationPath, "\*");
            Compress-Archive -Path $destination -DestinationPath $newZipFile
            if ( [System.IO.File]::Exists( $newZipFile ) ) {
                [System.IO.File]::Copy( $newZipFile, $newSpecFile, $true ) | Out-Null
                [System.IO.File]::Delete( $newZipFile ) | Out-Null
                Write-Host "New File has been created. $($newSpecFile)" # $packageFile
                if ( [System.IO.File]::Exists( $packageFile ) ) { [System.IO.File]::Delete( $packageFile ) | Out-Null }
            }
        }
        if ( [System.IO.Directory]::Exists( $workdir ) ) {
            [System.IO.DirectoryInfo]::new( $workdir ).Delete($true) | Out-Null;
        }
    } catch {
        return $null;
    }
}
if ( [System.IO.Directory]::Exists( $workdir ) ) {
    [System.IO.DirectoryInfo]::new( $workdir ).Delete($true) | Out-Null;
}
$dii = [System.IO.DirectoryInfo]::new( $homedir );
$found = $dii.GetFiles('*.nupkg', [System.IO.SearchOption]::AllDirectories);
$isArray = canEnumerate -obj $found
if ( $isArray ) {
    $found.GetEnumerator() | ForEach-Object {
        processFileData -obj $_;
    }
    return;
} else {
    processFileData -obj $found
}