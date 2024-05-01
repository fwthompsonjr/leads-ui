param(
    [string]$search = 'src\email'
)

<#
    generate coverage summary
#>

function canEnumerate( $obj ) {
    try {
        $enumerated = $obj.GetEnumerator();
        return $true;
    } catch {
        return $false;
    }
}

function installCli() {
    try {
        dotnet tool install -g dotnet-reportgenerator-globaltool
    } catch {
        Write-Warning "Unable to install report generator."
    }
}

function echoReportMarkdown( $mdfile ) {

    if ( [System.IO.File]::Exists( $mdfile ) -eq $null ) { return; }
    $content = [System.IO.File]::ReadAllText( $mdfile );
    try {
        Write-Output $content
        $content | Out-File -FilePath $Env:GITHUB_STEP_SUMMARY  -Append
    } catch {
        Write-Warning "Unable to append summary to workflow"
    }
}
function generateSummary( $report, $reportFolder )
{
    try {
        if ( [System.IO.Directory]::Exists( $reportFolder ) -eq $false ) { return; }
        $findfiles = [System.IO.DirectoryInfo]::new( $reportFolder ).GetFiles( "*.md" );
        if ( $null -eq $findfiles ) { return; }
        $ismany = canEnumerate -obj $findfiles
        if ( $ismany -eq $false ) {
            $mdfilename = $findfiles.FullName
            echoReportMarkdown -mdfile $mdfilename
            return;
        } else {
            $findfiles.GetEnumerator() | ForEach-Object {
                $mdfilename = $_.FullName
                echoReportMarkdown -mdfile $mdfilename
            }
        }
    } finally {
        if ( [System.IO.Directory]::Exists( $reportFolder ) -eq $true ) {
            [System.IO.Directory]::Delete($reportDir, $true ) | Out-Null
        }
    }
}
function processFileData( $obj ) {
    try {
        $info = ([system.io.fileinfo]$obj);
        $coverageFile = $info.FullName;
        $parentDir = [System.IO.Path]::GetDirectoryName( $coverageFile );
        $reportDir = [System.IO.Path]::Combine( $parentDir, "reports" );
        if ( [System.IO.Directory]::Exists( $reportDir ) -eq $false ) {
            [System.IO.Directory]::CreateDirectory( $reportDir ) | Out-Null
        }
        $rptFile = reportgenerator `
            -reports:$coverageFile `
            -targetDir:$reportDir `
            -reporttypes:MarkdownSummaryGithub
        generateSummary -reportFolder $reportDir
    } catch {
        return $null;
    }
}


$startedAt = [datetime]::UtcNow
try {
    installCli
    ## find all files matching *.sln 
    $currentDir = [System.IO.Path]::GetDirectoryName( $MyInvocation.MyCommand.Path );
    $di = [System.IO.DirectoryInfo]::new( $currentDir );
    $found = $di.GetFiles( "*coverage.cobertura.xml", [System.IO.SearchOption]::AllDirectories) | Where-Object {
        $_.FullName.IndexOf( $search ) -ge 0
    };

    $isArray = canEnumerate -obj $found
    if ( $isArray ) {
        $found.GetEnumerator() | ForEach-Object {
            processFileData -obj $_;
        }
        return;
    } else {
        processFileData -obj $found
    }
} catch {
    Write-Warning "Unexpected error occurred in report process."
}