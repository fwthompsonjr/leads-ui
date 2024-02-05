<#
    Get version stamp
    script location: $/leads-ui/src/core/legallead.core/component/worker
    json location: $/leads-ui/release/legallead.reader.component.release.txt
#>

function findParentDirectory( $starting, $name ) {
    if ([System.IO.Directory]::Exists( $starting ) -eq $false ) { return $null; }
    $di = [System.IO.DirectoryInfo]::new( $starting );
    [string]$fullName = $di.FullName;
    $directoryName = $di.Name;
    while( $directoryName -ne $null -and $directoryName -ne $name -and $di.Parent -ne $null ) {
        $di = [System.IO.DirectoryInfo]::new( $di.Parent.FullName );
        $directoryName = $di.Name;
        $fullName = $di.FullName;
        if ( $di.Parent -eq $null -and $directoryName -ne $name ) {
            $directoryName = $null;
            $fullName = $null;
            break;
        }
    }
    return $fullName;
}
$prjname = "legallead.reader.component.csproj";
$jsname = "legallead.reader.component.release.txt";
$currentDir = [System.IO.Path]::GetDirectoryName( $MyInvocation.MyCommand.Path );
$leadsDir = (findParentDirectory -starting $currentDir -name "leads-ui");
$prjfile = [System.IO.Path]::Combine( $currentDir, $prjname );
if ( $leadsDir -eq $null ) {
    Write-Host "Unable to find lead-ui parent directory."
    return;
}

if ([System.IO.File]::Exists( $prjfile ) -eq $false ) { 
    Write-Host "Unable to find project file $prjname"
    return; 
}
$releaseDir = [System.IO.Path]::Combine( $leadsDir, "release" );
if ([System.IO.Directory]::Exists( $releaseDir ) -eq $false ) { return; }

$jsfile = [System.IO.Path]::Combine( $releaseDir, $jsname );
if ([System.IO.File]::Exists( $jsfile ) -eq $false ) { return; }

$js = [System.IO.File]::ReadAllText( $jsfile ) | ConvertFrom-Json

if( $js -eq $null -or $js.Count -eq 0 ) { return; }
$jsline = $js.Item( $js.Count - 1 );
$xproject = [xml]([System.IO.File]::ReadAllText( $prjfile ));
$nde = $xproject.DocumentElement.FirstChild;
$ndasm = $nde.SelectSingleNode("AssemblyVersion");

if ( $ndasm.InnerText -ne $jsline.label ) {
    Write-Host "Js File version: $($jsline.label)";
    $ndasm.InnerText = $jsline.label;
    $xproject.Save( $prjfile );
    Write-Host "Project Node updated successfully to : $($jsline.label)"
}
else
{
    Write-Host "Project Node version: $($ndasm.InnerText)";
    Write-Host "Project Node is in sync."
}