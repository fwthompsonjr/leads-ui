<#
    get last update for .cs files
#> 
<# Utilities #>
    function formatDateNicely( [string]$gitdate ) {
        $parts = $gitdate.Split(" ");
        $nwdate = [string]::Concat( $parts[1], " ", $parts[2], ",", $parts[4] );
        $times = $parts[3].Split(':');
        [DateTime]$dte = [DateTime]::Parse( $nwdate );
        [DateTime]$d = [DateTime]$dte;
        $d = $d.AddHours( [int]::Parse( $times[0] ));
        $d = $d.AddMinutes( [int]::Parse( $times[1] ));
        $d = $d.AddSeconds( [int]::Parse( $times[2] ));
        return $d
    }

<# compute local file system locations #>
    $wkfolder = [System.IO.Path]::GetDirectoryName( $MyInvocation.MyCommand.Path );
    $parentDir = [System.IO.Path]::GetDirectoryName( $wkfolder );
    $parentDirName = [System.IO.Path]::GetFileName( $parentDir );
    while ( $parentDirName -ne "leads-ui" ) {
        $parentDir = [System.IO.Path]::GetDirectoryName( $parentDir );
        $parentDirName = [System.IO.Path]::GetFileName( $parentDir );
    }
    $postmanFile = [System.IO.Path]::Combine( $parentDir, "postman\ll-authorizations-summary.txt" );
    if ( [System.IO.File]::Exists( $postmanFile ) -eq $false ) { 
        throw "Postman integration result is not found."
    }
    $epochid = ( git log -1 --format=%ad $postmanFile)
    $postmanFileDate = formatDateNicely -gitdate $epochid
    $findTest = [System.IO.DirectoryInfo]::new( $parentDir )
    $curentLocation = (Get-Location).Path;
try {
    $appFolder = [System.IO.Path]::Combine( $parentDir, "src\api\legallead.permissions.api" );
    Write-Output $appFolder;
    Set-Location $parentDir
    $di = [System.IO.DirectoryInfo]::new( $appFolder );
    $files = $di.GetFiles( "*.cs", [System.IO.SearchOption]::AllDirectories ) `
    | Where-Object { $_.FullName.Contains( '\bin\' ) -eq $false } `
    | Where-Object { $_.FullName.Contains( '\obj\' ) -eq $false }
    $minDate = [DateTime]::MinValue;
    $files.GetEnumerator() | ForEach-Object {
        [System.IO.FileInfo]$info = $_;
        $fullname = $info.FullName
        $epochid = ( git log -1 --format=%ad $fullname)
        $gitDate = formatDateNicely -gitdate $epochid
        if ($gitDate -gt $minDate ) {
            $minDate = $gitDate
            $cleaned = $gitDate.ToString("s");
            Write-Output "$($info.Name) | $cleaned"
        }
    }
    if( $postmanFileDate -gt $minDate ) {
        Write-Host "Postman file is compliant!";
        return
    } else {
        throw "Postman file date is prior to lastest file change!";
    }
} finally {
    Set-Location $curentLocation
}