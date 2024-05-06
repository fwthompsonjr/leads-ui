<#
    summarize results
#>


<# Setup variable names #>
    $indicator = "-------------------------------------------------------------------";
    $jsOutputName = "ll-authorizations-summary.txt";

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
    if ( [System.IO.File]::Exists( $jsOutputFile ) -eq $false ) { 
        return $null
    }
<# get summary #>

function doesLineContainKey( [string]$text, [string[]]$words ) {
    foreach( $word in $words ) {
        $isfound = doesLineContainKeyWord -text $text -word $word
        if ( $isFound -eq $true ) { return $true }
    }
    return $false;
}

function doesLineContainKeyWord( [string]$text, [string]$word ) {
    return $text.Contains( $word );
}

function getFailedCount( [string]$txt, [string]$word ) {
    $pipe = "|";
    if ( $txt.Contains( $pipe ) -eq $false ) { return 0 }
    $items = $txt.Split( $pipe );
    $nn = $items.Length - 2;
    $failedCount = $items[$nn].Trim()
    $rtn = ""
    if ( [double]::TryParse($failedCount,[ref]$rtn) -eq $true ) {
        return [System.Convert]::ToInt32( $failedCount );
    }
    return 0;
}

function writeGitAction( $content )
{
    try {
        Write-Output $content
        $content | Out-File -FilePath $Env:GITHUB_STEP_SUMMARY  -Append
    } catch {
        Write-Warning "Unable to append summary to workflow"
    }
}

[string]$content = [System.IO.File]::ReadAllText( $jsOutputFile );
$aa = $content.IndexOf( $indicator );
$bb = $content.LastIndexOf( $indicator );
$snippet = $content.Substring($aa, $bb - $aa);
$markdownarray = @(
'### Integration Results   ',
'``` ',
$snippet,
$indicator,
'',
'```   '
'');
$markdown = [string]::Join( [Environment]::NewLine, $markdownarray );
writeGitAction -content $markdown
$rowHeaders = @("iterations", "requests", "test-scripts", "prerequest-scripts", "assertions" );
[int[]]$failures = @();
[System.IO.TextReader]$sreader = [System.IO.StringReader]::new( $snippet );
$line = $sreader.ReadLine();
while ( $null -ne $line ) {
    $hasKeyWord = doesLineContainKey -text $line -words $rowHeaders
    if ( $hasKeyWord -eq $true ) { 
        [int]$itemCount = getFailedCount -txt $line
        $failures += ($itemCount);
    }
    $line = $sreader.ReadLine();
}
$sreader.Dispose();
$finalFailedCount = 0;
$failures.GetEnumerator() | ForEach-Object {
    [int]$n = $_;
    $finalFailedCount += $n;
}
if ( $finalFailedCount -gt 0 ) {
    throw "Integration testing failed."
}