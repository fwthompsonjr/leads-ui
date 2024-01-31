<#
$location = @{
    "region" = "us-central1"
    "project" = "tdscience-worksheets"
    "repo" = "tdsstorage"
}
C:\_g\lui\fwthompsonjr\leads-ui\x-image-version.json
docker login us-central1-docker.pkg.dev/tdscience-worksheets/tdsstorage
docker tag legalleadsearchapi:dev us-central1-docker.pkg.dev/tdscience-worksheets/tdsstorage/legalleadsearchapi:latest
docker push us-central1-docker.pkg.dev/tdscience-worksheets/tdsstorage/legalleadsearchapi:latest
#>

$currentDir = [System.IO.Path]::GetDirectoryName( $MyInvocation.MyCommand.Path );
$jsonFile = [System.IO.Path]::Combine( $currentDir, "x-image-version.json" );
$js = [System.IO.File]::ReadAllText( $jsonFile ) | ConvertFrom-Json;

$location = @{
    "region" = "us-central1"
    "project" = "tdscience-worksheets"
    "repo" = "tdsstorage"
}
$image = @{
    "name"= "legalleadsearchapi"
    "source" = "dev"
    "tags" = @( "latest", $js[$js.Count-1].tag )
}

$locationName = [string]::Concat( $location.region, "-docker.pkg.dev/", $location.project, "/", $location.repo);
$devimage = [string]::Concat( $image.name, ":", $image.source )
$src = $image.name;
$dest = [string]::Concat( $locationName, "/", $image.name, ":latest" )
docker login $locationName

$image.tags.GetEnumerator() | ForEach-Object {
    $tg = [string]::Concat(":", $_);
    $tagtarget = "$locationName/$src$tg"
    $cmmd = "Execute command: docker tag $devimage $tagtarget"
    Write-Host $cmmd
    docker tag $devimage $tagtarget
}
$cmmd = "Execute command: docker push $dest";
Write-Host $cmmd
# docker push $dest