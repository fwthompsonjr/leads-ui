<#
    get newman test files
    --reporter-json-export
#>

<# Setup variable names #>
    $collentionName = "ll-authorizations-test"
    $environmentName = "LL-TEST"
    $jsOutputName = "ll-authorizations-summary.txt";
    $collection = "$collentionName.postman_collection.json"
    $envjson = "$environmentName.postman_environment.json"

<# compute local file system locations #>
    $wkfolder = [System.IO.Path]::GetDirectoryName( $MyInvocation.MyCommand.Path );
    $parentDir = [System.IO.Path]::GetDirectoryName( $workfolder );
    $parentDirName = [System.IO.Path]::GetFileName( $parentDir );
    while ( $parentDirName -ne "leads-ui" ) {
        $parentDir = [System.IO.Path]::GetDirectoryName( $parentDir );
        $parentDirName = [System.IO.Path]::GetFileName( $parentDir );
    }
    $testDir = [System.IO.Path]::Combine( $parentDir, "postman" );
    if ( [System.IO.Directory]::Exists( $testDir ) -eq $false ) { return $null }
    Write-Output "Test folder = $testDir"

<# Build target file path(s) #>
    $jsOutputFile = [System.IO.Path]::Combine( $testDir, $jsOutputName );
    $collectionFile = [System.IO.Path]::Combine( $testDir, $collection );
    $environmentFile = [System.IO.Path]::Combine( $testDir, $envjson );
    if ( [System.IO.File]::Exists( $jsOutputFile ) -eq $true ) { 
        [System.IO.File]::Delete( $jsOutputFile ) | Out-Null
    }
    if ( [System.IO.File]::Exists( $collectionFile ) -eq $false ) { return $null }
    if ( [System.IO.File]::Exists( $environmentFile ) -eq $false ) { return $null }

$ErrorActionPreference="SilentlyContinue"
Stop-Transcript | out-null
$ErrorActionPreference = "Continue"
Start-Transcript -path $jsOutputFile -append
newman run $collectionFile -e $environmentFile -r cli -k --disable-unicode --color off
Stop-Transcript