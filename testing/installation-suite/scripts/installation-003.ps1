try {
    $name = "legallead.desktop-windows"
    leadcli install -n $name
} catch {
    Write-Warning "There was some issue with this step."
}