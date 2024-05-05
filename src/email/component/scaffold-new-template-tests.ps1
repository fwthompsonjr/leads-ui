<#
scaffold-new-template-tests.ps1
1. setup email template
	1.1 create action coverage
	1.2 create template coverage file
#>


param (
	[string]$template = "BeginSearchRequested"
)

$workfolder = [System.IO.Path]::GetDirectoryName( $MyInvocation.MyCommand.Path );
$parentDir = [System.IO.Path]::GetDirectoryName( $workfolder );
$parentDirName = [System.IO.Path]::GetFileName( $parentDir );
while ( $parentDirName -ne "email" ) {
    $parentDir = [System.IO.Path]::GetDirectoryName( $parentDir );
    $parentDirName = [System.IO.Path]::GetFileName( $parentDir );
}
$testDir = [System.IO.Path]::Combine( $parentDir, "tests" );
Write-Host "Test directory: $testDir";
$sources = @{
    "action" = @{
		file = "actions/BaseEmailActionTemplateTests.cs"
        find = "BaseEmailActionTemplate"
    }
    "implementations" = @{
		file = "implementations\NoActionTemplateTests.cs" 
        find = "NoAction"
    }
}
#  BeginSearchRequestedTemplateTests
function createTestFile( $name, $source ) {
    $names = @( "action", "implementations" );
    if ( $names.Contains( $name ) -eq $false ) { retrun; }
    $srcFile = [System.IO.Path]::Combine( $testDir, $source.file );
    if ( [System.IO.File]::Exists( $srcFile ) -eq $false ) { return; }
    $srcDir = [System.IO.Path]::GetDirectoryName( $srcFile );
    $srcName = [System.IO.Path]::GetFileName( $srcFile );

    $testFile = $srcName.Replace( $source.find, $template );
    $testFileName = [System.IO.Path]::Combine( $srcDir, $testFile );
    if ( [System.IO.File]::Exists( $testFileName ) -eq $true ) { 
        Write-Output "Test file: $testFile is already created."
        return; 
    }
    $content = [System.IO.File]::ReadAllText( $srcFile );
    $content = $content.Replace( $source.find, $template );
    [System.IO.File]::WriteAllText( $testFileName, $content );
}

$sources.GetEnumerator() | ForEach-Object {
    $typeName = $_.Key;
    $typeValue = $_.Value;
    Write-Output "Processing $typeName : template: $($typeValue.file) "
    createTestFile -name $typeName -source $typeValue
}