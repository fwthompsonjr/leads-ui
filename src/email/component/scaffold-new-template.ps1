<#
scaffold-new-template.ps1
1. setup email template
	1.1 create new enum member
	1.2 create action
	1.3 create template text file
	1.4 di
		1.4.1 add keyed transient
		1.4.2 add action registry
#>

param (
	[string]$template = "BeginSearchRequested",
    [string]$templateToken = "begin-search-requested"
)

$workfolder = [System.IO.Path]::GetDirectoryName( $MyInvocation.MyCommand.Path );
$htmlText = @(
    '<h4 style="color: green" name="template-token-heading"><!-- Template Heading --></h4>',
    '<table name="template-token-detail-table" role="presentation" border="0" cellspacing="0" style="margin-left: 15px; margin-bottom= 15px;" width="95%">',
    '	<tr>',
    '		<td style="width: 25%">',
    '			<span>User Name:</span>',
    '		</td>',
    '		<td style="width: 75%">',
    '			<span name="template-token-user-name"> - </span>',
    '		</td>',
    '	</tr>',
    '	<tr>',
    '		<td style="width: 25%">',
    '			<span>Email:</span>',
    '		</td>',
    '		<td style="width: 75%">',
    '			<span name="template-token-email"> - </span>',
    '		</td>',
    '	</tr>',
    '	<tr>',
    '		<td name="template-token-detail-spacer" colspan="2">',
    '			<hr style="background: #444; height: 1px; border: 0px;" />',
    '		</td>',
    '	</tr>',
    '	<!-- Add custom replacements here -->',
    '</table>');
$sources = @{
	"enumeration" = @{
		file = "utility/TemplateNames.cs"
		eof = "        // end of enumeration"
	}
	"action" = @{
		file = "actions/BaseEmailActionTemplate.cs"
		find = "TemplateNames.None"
	}
	"implementation" = @{
		file = "implementations/NoActionTemplate.cs"
		find = "NoActionTemplate"
	}
    "dependency" = @{
		file = "utility/ServiceInfrastructure.cs"
        transientEnd = "                // end keyed transients"
        registrationEnd = "                // end register actions"
    }
}
function getTemplateFileName() {
    $shortName = $sources.enumeration.file;
    $templateFile = [System.IO.Path]::Combine( $workfolder, $shortName );
    if ( [System.IO.File]::Exists( $templateFile ) -eq $false ) {
        Write-Warning "Unable to locate template file: $shortName"
        return $null;
    }
    return $templateFile;
}

function getActionFileName() {
    $shortName = $sources.action.file;
    $templateFile = [System.IO.Path]::Combine( $workfolder, $shortName );
    if ( [System.IO.File]::Exists( $templateFile ) -eq $false ) {
        Write-Warning "Unable to locate template file: $shortName"
        return $null;
    }
    return $templateFile;
}

function getHtmlFileName() {
    $shortName = "_html\email-template-html-body-$templateToken.txt"
    $templateFile = [System.IO.Path]::Combine( $workfolder, $shortName );
    if ( [System.IO.File]::Exists( $templateFile ) -eq $true ) {
        Write-Warning "template file: $shortName is already created"
        return $null;
    }
    return $true;
}

function getImplementationFileName() {
    $shortName = $sources.implementation.file;
    $templateFile = [System.IO.Path]::Combine( $workfolder, $shortName );
    if ( [System.IO.File]::Exists( $templateFile ) -eq $false ) {
        Write-Warning "Unable to locate template file: $shortName"
        return $null;
    }
    return $templateFile;
}

function getDependencyFileName() {
    $shortName = $sources.dependency.file;
    $templateFile = [System.IO.Path]::Combine( $workfolder, $shortName );
    if ( [System.IO.File]::Exists( $templateFile ) -eq $false ) {
        Write-Warning "Unable to locate dependency file: $shortName"
        return $null;
    }
    return $templateFile;
}

function isTemplateNameUnique( $name ) {
    $sourceFile = getTemplateFileName
    if ( $null -eq $sourceFile ) { return $null; }
    [string]$content = [System.IO.File]::ReadAllText( $sourceFile );
    $indx = $content.IndexOf( $template );
    return $indx -lt 0;
}


function isActionNameUnique( $name ) {
    $sourceFile = getActionFileName
    if ( $null -eq $sourceFile ) { return $null; }
	$actionPath = [System.IO.Path]::GetDirectoryName( $sourceFile );
    $actionFile = [System.IO.Path]::Combine( $actionPath, "$name.cs");
    if ( [System.IO.File]::Exists( $actionFile ) -eq $true ) {
        $shortName = [System.IO.Path]::GetFileName( $actionFile );
        Write-Warning "action file: $shortName is already created."
        return $null;
    }
    return $actionFile
}

function addTemplateAction( $name, $testing ) {    
    $sourceFile = getActionFileName
    if ( $null -eq $sourceFile ) { return $null; }
    $locator = $sources.action.find;
    $additions = @(
        "BaseEmailActionTemplate",
        $locator
    )
    $replacements = @(
        $name,
        "TemplateNames.$name"
    )
    [string]$content = [System.IO.File]::ReadAllText( $sourceFile );
    for($i = 0; $i -lt $additions.Count; $i++) {
        $search = $additions[$i];
        $replacement = $replacements[$i];
        $content = $content.Replace( $search, $replacement);
    }
    if ($testing -eq $false ) {
        $rootDir = [System.IO.Path]::GetDirectoryName( $sourceFile );
        $outFile = [string]::Concat( $template, ".cs")
        $csfile = [System.IO.Path]::Combine( $rootDir, $outFile );
        [System.IO.File]::WriteAllText( $csfile, $content );
    }
    return $true;
}



function addTemplateName( $name, $testing ) {    
    $sourceFile = getTemplateFileName
    if ( $null -eq $sourceFile ) { return $null; }
    $locator = $sources.enumeration.eof;
    $additions = @(
        "        $name,",
        $locator
    )
    [string]$content = [System.IO.File]::ReadAllText( $sourceFile );
    $replacement = [string]::Join( [Environment]::NewLine, $additions );
    $content = $content.Replace( $locator, $replacement);
    if ($testing -eq $false ) {
        [System.IO.File]::WriteAllText( $sourceFile, $content );
    }
    return $true;
}

function addHtmlAction( $testing ) {
    $shortName = "_html\email-template-html-body-$templateToken.txt"
    $sourceFile = [System.IO.Path]::Combine( $workfolder, $shortName );
    if ( [System.IO.File]::Exists( $sourceFile ) -eq $true ) {
        Write-Warning "template file: $shortName is already created"
        return $false;
    }
    $additions = @(
        "template-token"
    )
    $replacements = @(
        $templateToken
    )
    [string]$content = [string]::Join( [System.Environment]::NewLine, $htmlText)
    for($i = 0; $i -lt $additions.Count; $i++) {
        $search = $additions[$i];
        $replacement = $replacements[$i];
        $content = $content.Replace( $search, $replacement);
    }
    if ($testing -eq $false ) {
        [System.IO.File]::WriteAllText( $sourceFile, $content );
    }
    return $sourceFile;
}

function addImplementationFile( $name ) {    
    $sourceFile = getImplementationFileName
    if ( $null -eq $sourceFile ) { return $null; }
    $locator = $sources.action.find;
    $additions = @(
        "NoActionTemplate"
    )
    $replacements = @(
        [string]::Concat( $name, "Template" )
    )
    [string]$content = [System.IO.File]::ReadAllText( $sourceFile );
    for($i = 0; $i -lt $additions.Count; $i++) {
        $search = $additions[$i];
        $replacement = $replacements[$i];
        $content = $content.Replace( $search, $replacement);
    }
    
    if ($testing -eq $false ) {
        $rootDir = [System.IO.Path]::GetDirectoryName( $sourceFile );
        $outFile = [string]::Concat( $template, "Template.cs")
        $csfile = [System.IO.Path]::Combine( $rootDir, $outFile );
        [System.IO.File]::WriteAllText( $csfile, $content );
    }
    return $true;
}


function addDependencyFile( $name, $testing ) {    
    $sourceFile = getDependencyFileName
    if ( $null -eq $sourceFile ) { return $null; }
    $locator = $sources.action.find;
    $additions = @(
        $sources.dependency.transientEnd,
        $sources.dependency.registrationEnd
    )
	$spacer = "               "
	$transient = [string]::Format( "$spacer services.AddKeyedTransient<IHtmlTransformDetailBase, {0}Template>(TemplateNames.{0}).ToString());", $name)
	$registration = "$spacer services.AddTransient<{0}Completed>();" -f $name
    $replacements = @(
        [string]::Join( [Environment]::NewLine, @( $transient, $sources.dependency.transientEnd)),
        [string]::Join( [Environment]::NewLine, @( $registration, $sources.dependency.registrationEnd))
    )
    [string]$content = [System.IO.File]::ReadAllText( $sourceFile );
    for($i = 0; $i -lt $additions.Count; $i++) {
        $search = $additions[$i];
        $replacement = $replacements[$i];
        $content = $content.Replace( $search, $replacement);
    }
    if ($testing -eq $false ) {
        [System.IO.File]::WriteAllText( $sourceFile, $content );
    }
    return $true;
}
if ( [string]::IsNullOrWhiteSpace( $template ) -eq $true ) {
    Write-Warning "Template name is invalid."
    return;
}

if ( [string]::IsNullOrWhiteSpace( $templateToken ) -eq $true ) {
    Write-Warning "Template token name is invalid."
    return;
}
$isNameUnique = isTemplateNameUnique -name $template
if ( $null -eq $isNameUnique -or $isNameUnique -eq $false ) {
    Write-Warning "Template name : $template has name violation. Unable to add"
    return;
}
$conditions = @( $true, $false )
Write-Output "Creating template constant: $template"
foreach($condition in $conditions) {
    $resp = addTemplateName -name $template -testing $condition
    if ($true -ne $resp ) { throw "Unable to test addTemplateName function" }

    $resp = addTemplateAction -name $template -testing $condition
    if ($true -ne $resp ) { throw "Unable to test addTemplateAction function" }

    $resp = addHtmlAction -testing $condition
    if ($true -ne $resp ) { throw "Unable to test addHtmlAction function" }

    $resp = addImplementationFile -name $template -testing $condition
    if ($true -ne $resp ) { throw "Unable to test addImplementationFile function" }

    $resp = addDependencyFile -name $template -testing $condition
    if ($true -ne $resp ) { throw "Unable to test addDependencyFile function" }
}