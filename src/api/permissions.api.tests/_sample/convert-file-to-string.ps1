$src = "C:\Users\frank.thompson\Downloads\denton-sample-02.xlsx";
$dest = "C:\_g\lui\fwthompsonjr\leads-ui\src\api\permissions.api.tests\_sample\denton-excel-file-01.txt"
$bts = [System.IO.File]::ReadAllBytes($src);
$dat = [System.Convert]::ToBase64String($bts);
if ([System.IO.File]::Exists($dest) -eq $true) { [System.IO.File]::Delete($dest); }
[System.IO.File]::WriteAllText($dest, $dat);