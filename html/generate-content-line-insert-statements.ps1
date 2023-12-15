<#
    setup web content line data
    fetch content from *.txt file in folder

#>
$keyindex = 9;
$contents = @(
    [string]::Concat( "1000", ",", "HOME.PRE.LOGIN.000.SHELL" ), 
    [string]::Concat( "1015", ",", "HOME.PRE.LOGIN.010.LINKS" ), 
    [string]::Concat( "1030", ",", "HOME.PRE.LOGIN.020.STYLES" ), 
    [string]::Concat( "1045", ",", "HOME.PRE.LOGIN.030.HEADER" ), 
    [string]::Concat( "1060", ",", "HOME.PRE.LOGIN.040.NOTIFICATIONS" ), 
    [string]::Concat( "1075", ",", "HOME.PRE.LOGIN.050.MAIN.CONTENT" ), 
    [string]::Concat( "1090", ",", "HOME.PRE.LOGIN.060.TAB.1.WELCOME" ), 
    [string]::Concat( "1105", ",", "HOME.PRE.LOGIN.060.TAB.2.LOGIN" ), 
    [string]::Concat( "1120", ",", "HOME.PRE.LOGIN.060.TAB.3.REGISTER" ), 
    [string]::Concat( "1135", ",", "HOME.PRE.LOGIN.070.JS.SCRIPTS" )
);

function translateArrayItem( [string]$arraymember ) {
    $items = ([string]$arraymember).Split(",")
    $id = $items[0];
    $name = $items[1];
    $shortName = [string]::Concat( $name.ToLower(), ".txt" );
    return @{
        internalid = $id
        name = $name
        shortname = $shortName
    }
}

function getUniqueIndex() {
    $guid = [System.Guid]::NewGuid().ToString("D");
    return $guid;
}

function getLineStatement( $name, $index, $number ) {
    $sqlstatement = $initsql;
    $id = getUniqueIndex
    # '{0}' Id, {1} InternalId, {2} LineNbr, '{3}' Content
    if ($number -gt $startingLineNumber ) { $sqlstatement = [string]::Concat( "UNION ", $sqlstatement ); }
    $lineSQL = $sqlstatement -f $id, $index, $number, $name
    return $lineSQL
}
$dir = [System.IO.Path]::GetDirectoryName( $MyInvocation.MyCommand.Path );
$targetDir = [System.IO.Path]::Combine( $dir, 'components' );
$kvp = $contents[$keyindex];
$item = translateArrayItem -arraymember $kvp
$targetPath = [System.IO.Path]::Combine( $targetDir, $item.shortname );

$linenumber = 100;
$startingLineNumber = $linenumber;
$increment = 10;
$initsql = "SELECT '{0}' Id, {1} InternalId, {2} LineNbr, '{3}' Content"

## Id, ContentId, InternalId, LineNbr, Content
$outersql = @(
"INSERT INTO CONTENTLINE ( Id, ContentId, InternalId, LineNbr, Content )",
'SELECT c.Id, c.ContentId, c.InternalId, c.LineNbr, c.Content',
'FROM',
'(',
'SELECT ',
'a.Id, b.Id ContentId, a.InternalId, a.LineNbr, a.Content',
'FROM',
'(',
'{0}',
') a',
'INNER JOIN CONTENT b',
'ON a.InternalId = b.InternalId',
'AND b.IsActive = true',
') c',
'LEFT JOIN CONTENTLINE d',
'ON c.ContentId = d.ContentId',
'AND c.LineNbr = d.LineNbr',
'WHERE d.Id IS NULL;');

if ( [System.IO.File]::Exists( $targetPath ) -eq $false ) {
    Write-Output "Target file is not found."
    return
}
$arr = @();
foreach($line in Get-Content $targetPath) {
    $txt = ([string]$line).Trim();
    $txt = $txt.Replace( "'", '"' );
    $stmt = ( getLineStatement -name $txt -index $item.internalid -number $linenumber );
    $linenumber += $increment;
    $arr += $stmt
}
Clear-Host
$content = [string]::Join( [Environment]::NewLine, $arr );
$joined = [string]::Join( [Environment]::NewLine, $outersql );
$joined = $joined -f $content
Write-Output $joined