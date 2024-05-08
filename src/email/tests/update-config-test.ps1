<#
test transform for test file configurations
#>
function doesNodeExist( $source, $search )
{
    $isNameFound = $false;
    $source.GetEnumerator() | ForEach-Object {
        if( $_.name -eq $search ) {
            $isNameFound = $true;
        }
    }
    return $isNameFound;
}

$myfolder = [System.IO.Path]::GetDirectoryName( $MyInvocation.MyCommand.Path );
$src = [System.IO.Path]::Combine( $myfolder, "_template\template-test-json.txt" );
$name = "PermissionChangeRequested"

if ( [System.IO.File]::Exists( $src ) -eq $false ) { return; }

[string]$jsource = [System.IO.File]::ReadAllText( $src )
$json = $jsource | ConvertFrom-Json
$isItemCreated = doesNodeExist -source $json -search $name
if ( $isItemCreated -eq $true ) { return; }
$node = @{
    "name" = $name
    "isTesting" = $true
} | ConvertTo-Json
$append = [string]::Join( [System.Environment]::NewLine, @( ",", $node, "]" ));
$jsourcenew = $jsource.Replace( "]", $append )
$jsourcenew
