<#
    setup web content data
    iterate all files *.txt in folder
#>


function canEnumerate( $obj ) {
    try {
        $enumerated = $obj.GetEnumerator();
        return $true;
    } catch {
        return $false;
    }
}

$dir = [System.IO.Path]::GetDirectoryName( $MyInvocation.MyCommand.Path );
$targetDir = [System.IO.Path]::Combine( $dir, 'components' );
$exists = [System.IO.Directory]::Exists( $targetDir );
if ( $exists -ne $true ) { return; }

$stmts = @();
$initsql = "CALL USP_INSERT_CONTENT( '{0}' );"
$di = [System.IO.DirectoryInfo]::new( $targetDir );
$found = $di.GetFiles( "*.txt" );

if ( $found -eq $null ) { return; }
if ( (canEnumerate -obj $found ) -eq $true ) {
    $found.GetEnumerator() | ForEach-Object {
        $textFile = ([system.io.fileinfo]$_).FullName
        [string] $shortName = [System.IO.Path]::GetFileNameWithoutExtension( $textFile );
        $shortName = $shortName.ToUpper();
        $stmt = ( $initsql -f $shortName );
        $stmts += $stmt;
    }
} else {
        $textFile = $found.FullName
        [string] $shortName = [System.IO.Path]::GetFileNameWithoutExtension( $textFile );
        $shortName = $shortName.ToUpper();
        $stmt = ( $initsql -f $shortName );
        $stmts += $stmt;
}
$content = [string]::Join( [Environment]::NewLine, $stmts );
Write-Output $content