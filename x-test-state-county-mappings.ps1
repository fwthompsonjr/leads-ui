<#
C:\_g\lui\fwthompsonjr\leads-ui\src\shared\
    localize = us-state-builder.json
    localize = settings.xml
#>
$idx = 1;
$filenames = @('us-state-builder.json','us-state-county-builder.json');
$filefmts = @('sb.AppendLine("{0}");','sb.AppendLine("{0}");');
$fname = $filenames[$idx];
$fmt = $filefmts[$idx];
$srcfile = "C:\_g\lui\fwthompsonjr\leads-ui\src\shared\$fname";
[string]$content = [system.io.file]::ReadAllText( $srcfile ).Replace( '"', "~" );
$sep = [Environment]::NewLine.ToCharArray();
$arr = $content.Split( $sep );

foreach($a in $arr) { 
    if([string]::IsNullOrWhiteSpace( $a )) { continue; }
    $tx = $fmt -f $a;  
    Write-Output $tx;
}