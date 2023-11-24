<#
    sample address builder
    from file: C:\_g\lui\fwthompsonjr\leads-ui\src\core\legallead.core\tests\legallead.records.search.tests\Json\collincounty_probate.csv
#>
$rtpath = "C:\_g\lui\fwthompsonjr\leads-ui"
$itmpath = "src\core\legallead.core\tests\legallead.records.search.tests\Json\collincounty_probate.csv"
$src= [System.IO.Path]::Combine( $rtpath, $itmpath );
$exists = [System.IO.File]::Exists( $src );
$content = [System.IO.File]::ReadAllText( $src ).Replace('"', '~');
$arr = $content -split '(.{80})'
$nl = [System.Environment]::NewLine;
$enew = "+ Environment.NewLine +";
$eol = "+";
$aarr = @();
foreach($a in $arr) { 
    if( $a.Trim().Length -gt 0 ) {
    $sp = $eol;
    if ($a.IndexOf( $nl ) -ge 0 ) { $sp = $enew }
    $ln = '"{0}" {1} ' -f $a.Trim(), $sp; 
    $aarr += $ln 
    }
}
if( $aarr.Count -gt 0 ) {
    $li = $aarr.Count - 1;
    $ln = $aarr[$li].Trim();
    $aarr[$li] = $ln.Substring( 0, $ln.Length - 1).Trim()
}
[string]::Join( $nl, $aarr );