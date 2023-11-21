<#

test ability to list failed tests

#>

function findNode( $name ) {
    foreach( $nde in $xContent.DocumentElement.ChildNodes ) {
        if( ([System.Xml.XmlNode]$nde).Name -eq $name ) { return $nde; }
    }
    return $null;
}

$testFile = "C:\_g\_fv-az658-243_2023-11-21_15_28_02.trx";
$content = [System.IO.File]::ReadAllText( $testFile );
$xContent = [xml]$content;
$results = findNode -name 'Results'
($results -eq $null);