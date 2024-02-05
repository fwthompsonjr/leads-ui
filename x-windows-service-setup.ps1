<#
    Setup windows service
    note: you might need to upgrade to powershell 7 ..

    iex "& { $(irm https://aka.ms/install-powershell.ps1) } -UseMSI"

#>
param (
    [int]$stepid = -1
)
$saccount = @{
    app = "legallead-reader"
    name = "svc-legallead"
    secret = "CVO8Axc2044aEwc31aie"
    user = "legallead-windows-ws-01\Administrator"
    exename = "legallead.reader.component.exe"
    exefolder = "C:\Svc\reader"
    }
$exeFolder = $saccount.exefolder
$exeLocation = [System.IO.Path]::Combine( $saccount.exefolder, $saccount.exename );

if ($stepid -eq 0) {
  New-LocalUser -Name $saccount.name;
  $paths = @( "C:\Svc", "C:\Svc\reader", "C:\Svc\setup")
    foreach($p in $paths) {
        if ( [System.IO.Directory]::Exists( $p ) -eq $false ) {
            [System.IO.Directory]::CreateDirectory( $p );
        }
    }
}
if ($stepid -eq 1) {
    <#
    THis command failed, had to setup ACL manually
    $acl = Get-Acl $saccount.exefolder
    $aclRuleArgs = $saccount.user, "Read,Write,ReadAndExecute", "ContainerInherit,ObjectInherit", "None", "Allow"
    $accessRule = New-Object System.Security.AccessControl.FileSystemAccessRule($aclRuleArgs)
    $acl.SetAccessRule($accessRule)
    $acl | Set-Acl $saccount.exefolder
    #>
}
if ($stepid -eq 2) {
    # this installation worked, 
    # but needed to assign service account manually
    New-Service -Name $saccount.app `
    -BinaryPathName "$exeLocation --contentRoot $exeFolder" `
    -Description "Legal Lead Background Queue Processor" `
    -DisplayName "Legal Lead Queue Processor" `
    -StartupType Automatic
}
if ($stepid -eq 3) {
    #uninstall
    Remove-Service -Name $saccount.app
}