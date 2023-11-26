<#
    localize = courtAddress.json
    = collinCountyCaseType.json
    collinCountyUserMap.json
    tarrantCountyMapping_2
#>
$idx = 8;
$filenames = @(
    'courtAddress.json',
    'collinCountyCaseType.json',
    'collinCountyUserMap.json', 
    'tarrantCountyMapping_2.json',
    'tarrantCountyMapping_1.json',
    'tarrantCourtSearchDropDown.json'
    'denton-settings.json',
    'dentonCaseCustomInstruction.json',
    'dentonCaseCustomInstruction_1.json');
$fname = $filenames[$idx];
$fmt = 'sbb.AppendLine("{0}");'
$srcfile = "C:\_g\lui\fwthompsonjr\leads-ui\src\shared\$fname";
$srcfile = "C:\_g\lui\fwthompsonjr\leads-ui\src\core\legallead.core\tests\legallead.records.search.tests\bin\Debug\net6.0\xml\$fname"
[string]$content = [system.io.file]::ReadAllText( $srcfile ).Replace( '"', "~" );
$sep = [Environment]::NewLine.ToCharArray();
$arr = $content.Split( $sep );
cls;
Write-Output ([string]::Concat("// ", $fname)); 
foreach($a in $arr) { 
    if([string]::IsNullOrWhiteSpace( $a )) { continue; }
    $tx = $fmt -f $a;  
    Write-Output $tx;
}