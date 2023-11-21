<#

test ability to list failed tests

#>

function findNode( $name ) {
    foreach( $nde in $xContent.DocumentElement.ChildNodes ) {
        if( ([System.Xml.XmlNode]$nde).Name -eq $name ) { return $nde; }
    }
    return $null;
}

$testFile = "C:\_g\_fv-az915-694_2023-11-21_17_32_54.trx";
$content = [System.IO.File]::ReadAllText( $testFile );
$xContent = [xml]$content;
$results = findNode -name 'Results'
$errors = "".Split(' ');
foreach( $nde in $results.ChildNodes ) {
    $outcome = ([System.Xml.XmlNode]$nde).Attributes.GetNamedItem('outcome');
    $testName = ([System.Xml.XmlNode]$nde).Attributes.GetNamedItem('testName');
    if ( $outcome -eq $null ) { continue; }
    if ($outcome.Value -eq "Passed") { continue; }
    $errstring = "$($outcome.Value) : $($testName.Value)";
    $errors += $errstring;
}
$errors | Sort-Object