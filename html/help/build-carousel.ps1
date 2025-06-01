$id = 'changePasswordCarousel';
$indicatorMainTemplate = "<button type='button' data-bs-target='#$id' data-bs-slide-to='0' class='active' aria-current='true' aria-label='{0}'></button>";
$indicatorMinorTemplate = "<button type='button' data-bs-target='#$id' data-bs-slide-to='{1}' aria-label='{0}'></button>";
$baseUrl = "../img/help-account-settings-change-password"
$names = @(
'Choose Settings',
'Select [Change Password] Menu',
'Enter Current Password',
'Enter New Password And Confirm',
'Click [Change Password] Button',
'Correct Errors',
'Change Confirmation'
);
$captions = @(
'From top menu click Settings',
'Choose [Change Password] and the Settings Window will open.',
'Enter your current password in the first text box',
'Enter your new password in the second text box and confirm in the third text box',
'Click [Change Password] Button to submit and change password.',
'Correct any errors if found, and click [Change Password] again to retry.',
'If password change succeeds you will see a confirmation message'
);
$slideMain = "<div class=`"carousel-item active`">
            <img src=`"$baseUrl-00.png`" class=`"d-block w-100`" alt=`"$($names[0])`">
            <div class=`"carousel-caption d-none d-md-block`">
              <h5>$($names[0])</h5>
              <p>$($captions[0])</p>
            </div>
          </div>";
$slideChild = "<div class=`"carousel-item`">
            <img src=`"$baseUrl-{0}.png`" class=`"d-block w-100`" alt=`"{1}`">
            <div class=`"carousel-caption d-none d-md-block`">
              <h5>{1}</h5>
              <p>{2}</p>
            </div>
          </div>";

$indicators = @(
 [string]::Format( $indicatorMainTemplate, $names[0] );
);
$divs = @(
 $slideMain
);
for($n = 1; $n -lt $names.Count; $n++) {
    $indx = $n.ToString("00");
    $item = [string]::Format( $indicatorMinorTemplate, $names[$n], $n );
    $dvitem = [string]::Format( $slideChild, $indx, $names[$n], $captions[$n]);
    $indicators += $item
    $divs += $dvitem
}
$dvs = [string]::Join( [Environment]::NewLine, $divs );
$buttons = [string]::Join( [Environment]::NewLine, $indicators );
Write-Host $buttons
Write-Host $dvs
Set-Clipboard $dvs