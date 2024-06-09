try  {
	$orginalpath = $env:Path
    $orginalarr = $orginalpath.Split(';');
    $arr = @();
    $orginalarr.GetEnumerator() | ForEach-Object {
        $itm = $_;
        if ( $arr.Contains( $itm ) -eq $false ) {
            $arr += ( $itm );
        }
    }
    if ( $arr.Count -ne $orginalarr.Count ) {
        Write-Output "Duplicates found."
        $rewrite = [string]::Join( ';', $arr );
        $env:Path = $rewrite
    }
} catch {
	Write-Warning "Failed to update environment"
}