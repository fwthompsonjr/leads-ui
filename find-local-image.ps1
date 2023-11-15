
$images = (docker image ls);
$list = ($images | ConvertTo-Json | ConvertFrom-Json);


function getMatching( $find ) {
    $arr = "";
    foreach($line in $list) {
        $txt = [string]$line;
        if ( $txt.IndexOf( $find ) -lt 0 ) { continue; }
        return $txt;
    }
    return $arr;
}

function isImageInContainer( $find )
{
    $matches = getMatching -find $find
    return $matches.Length -gt 0
}


function getImageName( $find )
{
    try {
        $matches = getMatching -find $find
        if ( $matches.Length -le 0 ) { return @("") }
        $prefix = "Pushed container";
        $iname = $matches.Split( ' ' )[0].Trim()
        $tag = $matches.Replace( $iname, '' ).Trim().Split( ' ' )[0].Trim()
        return @( 
            "", 
            "  $prefix '$iname.$tag'" );
    }
    catch {
        return @("");
    }
}