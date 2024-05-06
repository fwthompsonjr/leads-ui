<#
    get newman test files
#>

$wkfolder = [System.IO.Path]::GetDirectoryName( $MyInvocation.MyCommand.Path );
$parentDir = [System.IO.Path]::GetDirectoryName( $workfolder );
$parentDirName = [System.IO.Path]::GetFileName( $parentDir );
while ( $parentDirName -ne "leads-ui" ) {
    $parentDir = [System.IO.Path]::GetDirectoryName( $parentDir );
    $parentDirName = [System.IO.Path]::GetFileName( $parentDir );
}
$testDir = [System.IO.Path]::Combine( $parentDir, "postman" );
Write-Output "Test folder = $testDir"
$collection = "ll-authorizations.postman_collection.json"
$envjson = "LL-TEST.postman_environment.json"
$collectionFile = [System.IO.Path]::Combine( $testDir, $collection );
$environmentFile = [System.IO.Path]::Combine( $testDir, $envjson );

newman run $collectionFile -e $environmentFile -r cli