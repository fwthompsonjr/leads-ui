﻿<#
    execute postman testing
    pre-requ
#>
function is_npm_installed() {
    try {
        $g = (npm version)
        Write-Debug $g;
        return $true
    } catch {
        return $false;
    }
}
function is_newman_installed() {
    try {
        $g = (newman -v)
        Write-Debug $g;
        return $true
    } catch {
        return $false;
    }
}
function install_npm() {
    try {
        $isNpm = is_npm_installed
        if ( $isNpm -eq $true ) { return $true; }
        choco install npm
        return $true
    } catch {
        return $false;
    }
}

function install_newman() {
    try {
        $isNewman = is_newman_installed
        if ( $isNewman -eq $true ) { return $true; }
        $ms = (npm install -g newman --force)
        return $true
    } catch {
        return $false;
    }
}

function pre_req_install() {
    $npm = install_npm;
    if ( $npm -eq $false ) { return $false }
    $newman = install_newman
    if ( $newman -eq $false ) { return $false }
    return $true;
}


$workfolder = [System.IO.Path]::GetDirectoryName( $MyInvocation.MyCommand.Path );
$canTest = pre_req_install
if ($canTest -eq $false ) {
    Write-Warning "Unable to setup testing frameworks."
    return;
}
$process_01 = [System.IO.Path]::Combine( $workfolder, "postman-testing-01.ps1" );
$process_02 = [System.IO.Path]::Combine( $workfolder, "postman-testing-02.ps1" );
$processnumber = ( & $process_01 )
if ($processnumber -eq 0 ) { return }
Start-Sleep -Seconds 25
& $process_02
Get-Process -Id $processnumber | Stop-Process