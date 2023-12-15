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

function getUniqueIndex() {
    $guid = [System.Guid]::NewGuid().ToString("D");
    return $guid;
}

function getStatement( $name, $number ) {
    $sqlstatement = $initsql;
    $id = getUniqueIndex
    # '{0}' Id, {1} InternalId, '{2}' ContentName
    if ($number -gt $startingInternalId ) { $sqlstatement = [string]::Concat( "UNION ", $sqlstatement ); }
    $lineSQL = $sqlstatement -f $id, $number, $name
    return $lineSQL
}

function parseFileName( $itemName, $number ) {
    [string] $shortName = [System.IO.Path]::GetFileNameWithoutExtension( $itemName );
    $shortName = $shortName.ToUpper();
    $stmt = ( getStatement -name $shortName -number $number );
    return $stmt
}

$increment = 15;
$internalId = 1000;
$startingInternalId = $internalId;
$dir = [System.IO.Path]::GetDirectoryName( $MyInvocation.MyCommand.Path );
$targetDir = [System.IO.Path]::Combine( $dir, 'components' );
$exists = [System.IO.Directory]::Exists( $targetDir );
$stmts = @();
$initsql = "SELECT '{0}' Id, {1} InternalId, 0 VersionId, '{2}' ContentName"
$outersql = @(
"INSERT INTO CONTENT ( Id, InternalId, VersionId, ContentName, IsActive )",
"SELECT src.Id, src.InternalId, src.VersionId, src.ContentName, true IsActive ",
"FROM ( ",
"{0}",
") src ",
"LEFT JOIN CONTENT dest ",
"ON src.ContentName = dest.ContentName ",
"WHERE dest.Id IS NULL;"
);
if ( $exists -ne $true ) { return; }


$di = [System.IO.DirectoryInfo]::new( $targetDir );
$found = $di.GetFiles( "*.txt" );

if ( $found -eq $null ) { return; }
if ( (canEnumerate -obj $found ) -eq $true ) {
    $found.GetEnumerator() | ForEach-Object {
        $textFile = ([system.io.fileinfo]$_).FullName
        $command = parseFileName -itemName $textFile -number $internalId
        $internalId = ($internalId + $increment);
        if( $null -ne $command ) { $stmts += $command; }
    }
} else {
        $textFile = $found.FullName
        $command = parseFileName -itemName $textFile -number $internalId
        $internalId = ($internalId + $increment);
        if( $null -ne $command ) { $stmts += $command; }
}

$content = [string]::Join( [Environment]::NewLine, $stmts );
$ostatement = [string]::Join( [Environment]::NewLine, $outersql );
$query = ( $ostatement -f $content )
Write-Output $query