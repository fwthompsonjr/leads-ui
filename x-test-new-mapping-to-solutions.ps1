<#
    localize = settings.xml
#>
$srcfile = "C:\_g\lui\fwthompsonjr\leads-ui\src\shared\settings.xml";
[string]$content = [system.io.file]::ReadAllText( $srcfile ).Replace( '"', "~" );
$sep = [Environment]::NewLine.ToCharArray();
$arr = $content.Split( $sep );
$fmt = 'sb.AppendLine("{0}");'
foreach($a in $arr) { 
    if([string]::IsNullOrWhiteSpace( $a )) { continue; }
    $tx = $fmt -f $a;  
    Write-Output $tx;
}