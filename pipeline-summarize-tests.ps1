param (
    $workingDir
)
if( $null -eq $workingDir ) {
    $workingDir = [System.IO.Path]::GetDirectoryName( $MyInvocation.MyCommand.Path );
}
$reportValues = @{
    totalpassed = 0
    totalfailed = 0
    totalskipped = 0
    totaloverall = 0
}
$assembyName = '(unknown)';
$details = @(
    "<span style='color: green'>&#x2714; [totalpassed]</span>",
    "<span style='color: red'>&#x2718; [totalfailed]</span>",
    "<span style='color: silver'>&#x2745; [totalskipped]</span>"
);
$reportMd = @( 
    '#### Test Summary  [assemblyname] ',
    '<div style="margin: 15px"> '
    ' ',
    '| Assembly | Test Count | Passed | Failed | Skipped |  ',
    '| :--  | :-- | :-- | :-- | --: |  ',
    "| [assemblyname] | **[totaloverall]** | $($details[0]) | $($details[1]) | $($details[2]) |  ",
    ' ',
    ' Percent Passing: [totalpassed] / ( [totalfailed] + [totalpassed] ) = [percentpassed]% ',
    ' ',
    ' </div>',
    ' ');

$icons =@{
"Passed" = '<span style="color: green">&#x2714; Passed</span>'
"Failed" = '<span style="color: red">&#x2718; Failed</span>'
"Inconclusive" = '<span style="color: silver">&#x2745; Unknown</span>'
"NotExecuted" = '<span style="color: silver">&#x2745; Unknown</span>'
}

function updateTotal( $node ) {
    [System.Xml.XmlNode]$counter = ([System.Xml.XmlNode]$node).GetElementsByTagName('Counters')[0]
    $total = [System.Convert]::ToInt32( $counter.Attributes.GetNamedItem('total').Value );
    $passed = [System.Convert]::ToInt32( $counter.Attributes.GetNamedItem('passed').Value );
    $failed = [System.Convert]::ToInt32( $counter.Attributes.GetNamedItem('failed').Value );
    $others = $total - ($passed + $failed)
    $reportValues["totaloverall"] += $total
    $reportValues["totalpassed"] += $passed
    $reportValues["totalfailed"] += $failed
    $reportValues["totalskipped"] += $others
}

function findTestById( $node, $name ) {
    $ndlist = ([System.Xml.XmlNode]$node).ChildNodes;
    return $ndlist.GetEnumerator().Where({ ([system.xml.xmlnode]$_).Attributes.GetNamedItem( 'id' ) -eq $name });
}

function findNodeInList( $node, $name ) {
    $ndlist = ([System.Xml.XmlNode]$node).ChildNodes;
    return $ndlist.GetEnumerator().Where({ $_.Name -eq $name });
}

function getClassName( $classid, $testlist ) {
    foreach( $unitTest in $testlist.ChildNodes ) {
        $tst = ([system.xml.xmlnode]$unitTest)
        $attr = $tst.Attributes.GetNamedItem( 'id' ).Value
        if( $attr -eq $classid ) {
            [system.xml.xmlnode]$clsnode = $tst.GetElementsByTagName("TestMethod")[0];
            if( $null -eq $clsnode ) { return $find; }
            $clsName = $clsnode.Attributes.GetNamedItem('className').Value.Split( '.' );
            return $clsName[$clsName.Length-1];
        }
    }
    return $find;
}

function getSectionSummary( $node ) {
    $template = "<summary>$assembyName Passed: [passed], Failed: [failed], Other: [other]</summary>  ";
    [System.Xml.XmlNode]$counter = ([System.Xml.XmlNode]$node).GetElementsByTagName('Counters')[0]
    $total = [System.Convert]::ToInt32( $counter.Attributes.GetNamedItem('total').Value );
    $passed = [System.Convert]::ToInt32( $counter.Attributes.GetNamedItem('passed').Value );
    $failed = [System.Convert]::ToInt32( $counter.Attributes.GetNamedItem('failed').Value );
    $others = $total - ($passed + $failed);
    $template = $template.Replace( '[passed]', $passed)
    $template = $template.Replace( '[failed]', $failed)
    $template = $template.Replace( '[other]', $others)
    return $template;
}

function getOutCome( $code ) {
    if( $icons.ContainsKey( $code ) -eq $true ) { return $icons[$code]; }
    return $code;
}

function timeSpanToSeconds( $timestring ) {
    [System.TimeSpan]$ts = [System.TimeSpan]::Parse( $timestring );
    return $ts.TotalSeconds.ToString("0.000");
}

function buildResults( $node, $testlist, $numbers ) {
    $ssline = getSectionSummary -node $numbers
    $header = '| Result | Class Name | Test Name | Duration (seconds) |  '
    $subheader = '| :-- | :-- | :-- | --: |  '
    $line = '| [outcome] | [class] | [testName] | [duration] |  '
    $arr = @();
    $arr += '<details>  '
    $arr += $ssline
    $arr += '  '
    $arr += "###### Test Results - $assembyName "
    $arr += '  '
    $arr += '<small>  '
    $arr += '  '
    $arr += $header
    $arr += $subheader
    $tmptable = @{};
    $indexes = @();
    foreach($nde in $node.ChildNodes) {
        $nn = [system.xml.xmlnode]$nde
        $testid = $nn.Attributes.GetNamedItem( 'testId' ).Value
        $outcomeid = $nn.Attributes.GetNamedItem( 'outcome' ).Value
        $outcome = getOutCome -code $outcomeid
        $classNamed = getClassName -classid $testid -testlist $testlist
        $duration = timeSpanToSeconds -timestring $nn.Attributes.GetNamedItem( 'duration' ).Value
        $tmp = @{ 
            outcome = $outcome
            className = $classNamed.Trim()
            testName = $nn.Attributes.GetNamedItem( 'testName' ).Value.Trim()
            duration = $duration
        };

        $indexes+= ( "$($tmp["className"]) $($tmp["testName"]) | $($tmptable.Count)" )
        $tmptable.Add( $tmptable.Count, $tmp );
    }
    $indexes = ($indexes | sort);
    
    $indexes.GetEnumerator() | ForEach-Object { 
        $s =   [System.Convert]::ToInt32( ([string]$_).Split('|')[1] );
        $tmp = $tmptable[$s];
        $addline = $line.Replace('[outcome]', $tmp['outcome'] );
        $addline = $addline.Replace('[testName]', $tmp['testName'] );
        $addline = $addline.Replace('[duration]', $tmp['duration'] );
        $addline = $addline.Replace('[class]', $tmp['className'] );
        $arr += $addline
        }
    
    $arr += '  '
    $arr += '</small>  '
    $arr += '</details>  '
    $assembyName = '(unknown)';
    return ( [string]::Join( [environment]::NewLine, $arr ) );
}

function changeVariableName($definitions) {
    try {
        $assembyName = '(unknown)';
        $definitionNodes = $null;
        if( $null -eq $definitions ) { return; }
        if( $definitions.HasChildNodes -eq $false ) { return; }
        if( $null -eq $definitions.ChildNodes[0] -eq $false ) { return; }
        if( $definitions.ChildNodes[0].Attributes.Length -le 0 ) { return; }
        if( $null -eq $definitions.ChildNodes[0].Attributes.GetNamedItem( 'storage' ) ) { return; }
        $storage = $definitions.ChildNodes[0].Attributes.GetNamedItem( 'storage' );
        if( $null -eq $storage.Value ) { return; }
        $str = $storage.Value;
        $delimiters = @( '\', '/');
        foreach( $d in $delimiters) {
            if ( $str.IndexOf($d) -lt 0 ) { continue; }
            $definitionNodes = $str.Split( $d );
            break;
        }
        if ($null -eq $definitionNodes) { return; }
        if ($definitionNodes.Count - 1 -lt 0 ) { return; }
        $assembyName = $assembyName.Replace( 'unknown', $definitionNodes[ $definitionNodes.Count - 1 ] );

    } catch {
        return;
    }
}

function processTestResultFile( $name ) {
    
    $assembyName = '(unknown)';
    $testfile = [string]$name
    $x = [xml](Get-Content $testfile)
    $doc = $x.DocumentElement;
    $nc = $doc.ChildNodes.Count;
    for($n = 0; $n -lt $nc; $n++ ) {
        if( $doc.ChildNodes[$n].Name -eq 'Results' ) { $results = $doc.ChildNodes[$n] }
        if( $doc.ChildNodes[$n].Name -eq 'TestDefinitions' ) { $definitions = $doc.ChildNodes[$n] }
        if( $doc.ChildNodes[$n].Name -eq 'ResultSummary' ) { $summary = $doc.ChildNodes[$n] }
    }
    changeVariableName -definitions $definitions

    $sbset = buildResults -node $results -testlist $definitions -numbers $summary
    if( $null -ne $sbset ) { $reportMd += $sbset }

    updateTotal -node $summary
    $assembyName = '(unknown)';
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

function isEnumerable( $obj ) {
    try {
        $enumerated = $obj.GetEnumerator();
        return $true;
    } catch {
        return $false;
    }
}
function findNode( $name ) {
    foreach( $nde in $xContent.DocumentElement.ChildNodes ) {
        if( ([System.Xml.XmlNode]$nde).Name -eq $name ) { return $nde; }
    }
    return $null;
}

function getFailedTestList( $solution ) {
    try {
        $txtcontent = [System.IO.File]::ReadAllText( $solution );
        $xContent = [xml]$txtcontent;
        $results = findNode -name 'Results'
        $errors = "".Split(' ');
        foreach( $nde in $results.ChildNodes ) {
            $xmnode = ([System.Xml.XmlNode]$nde);
            if ($xmnode.Attributes.Length -le 0) { continue; }
            $outcome = $xmnode.Attributes.GetNamedItem('outcome');
            $testName = $xmnode.Attributes.GetNamedItem('testName');
            if ( $outcome -eq $null ) { continue; }
            if ( $outcome.Value -eq "Passed") { continue; }
            if ( $outcome.Value -eq "NotExecuted") { continue; }
            $errstring = " - $($outcome.Value) : $($testName.Value)   ";
            $errors += $errstring;
        }
        $sorted = ($errors | Sort-Object);
        $statstring = [string]::Join([Environment]::NewLine, $sorted);
        $dvstats = "<div> <small>$statstring</small> </div>"
        writeGitAction -content $dvstats
    } catch {
         Write-Warning "ERROR: $($_.Exception.Message)"
    }
}

function describeTest( $solution ) {
    $shortName = [system.io.path]::GetFileNameWithoutExtension( $solution )
    [string]$testProject = [System.IO.Path]::GetFileName( [System.IO.Path]::GetDirectoryName( $solution ) );
    $tpi = $testProject.LastIndexOf( '-' );
    if( $tpi -gt 0 ) { $testProject = $testProject.Substring(0, $tpi); }

    Write-Output "Evaluating test project $testProject"
    processTestResultFile -name $solution
    $failedTestCount += $reportValues['totalfailed'];
    $pctpassed = ( $reportValues['totalpassed'] + 0.00001 ) / ( ( $reportValues['totalfailed'] + $reportValues['totalpassed'] ) + 0.00001 ) * 100.00000001
    $reportFinal = [string]::Join( [environment]::NewLine, $reportMd );
    $reportFinal = $reportFinal.Replace( "[assemblyname]", $testProject );
    $reportFinal = $reportFinal.Replace( "[percentpassed]", $pctpassed.ToString("F2") );
    @('totalpassed', 'totalfailed', 'totalskipped', 'totaloverall') | ForEach-Object {
        $fname = ([string]$_);
        $reportFinal = $reportFinal.Replace( "[$fname]", [system.convert]::ToString( $reportValues[$fname] ) )   
    }
    writeGitAction -content $reportFinal
}

$reportFinal = [string]::Join( [environment]::NewLine, $reportMd );


function echoFailedTestCount {
    try {
        echo "FAILED_TEST_COUNT::$failedTestCount" >> $GITHUB_ENV
    } catch { }
}

## find all files matching *.trx 
try {
    $failedTestCount = 0;
    
    $di = [System.IO.DirectoryInfo]::new( $workingDir );
    $found = $di.GetFiles('*.trx', [System.IO.SearchOption]::AllDirectories)
    if( ( isEnumerable -obj $found ) -eq $false ) {
        $solutionFile = $found.FullName
        describeTest -solution $solutionFile
        getFailedTestList -solution $solutionFile
        echoFailedTestCount
        return;
    }
    $found.GetEnumerator() | ForEach-Object {
        $solutionFile = ([System.IO.FileInfo]$_).FullName
        describeTest -solution $solutionFile
        getFailedTestList -solution $solutionFile
    }
    echoFailedTestCount
} catch {
    Write-Warning "ERROR: $($_.Exception.Message)"
}

