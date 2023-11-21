<#
    localize = courtAddress.json

    
#>
$idx = 0;
$filenames = @('courtAddress.json','settings.xml');
$fname = $filenames[$idx];
$fmt = 'sbb.AppendLine("{0}");'
$srcfile = "C:\_g\lui\fwthompsonjr\leads-ui\src\shared\$fname";
[string]$content = [system.io.file]::ReadAllText( $srcfile ).Replace( '"', "~" );
$sep = [Environment]::NewLine.ToCharArray();
$arr = $content.Split( $sep );
cls;
foreach($a in $arr) { 
    if([string]::IsNullOrWhiteSpace( $a )) { continue; }
    $tx = $fmt -f $a;  
    Write-Output $tx;
}