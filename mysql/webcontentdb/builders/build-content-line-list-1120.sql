INSERT INTO CONTENTLINE ( Id, ContentId, InternalId, LineNbr, Content )
SELECT c.Id, c.ContentId, c.InternalId, c.LineNbr, c.Content
FROM
(
SELECT 
a.Id, b.Id ContentId, a.InternalId, a.LineNbr, a.Content
FROM
(
SELECT '629281d6-802c-4141-8c7d-48054b75e267' Id, 1120 InternalId, 100 LineNbr, '<form id="frm-register">' Content
UNION SELECT '484c8e42-8e9d-4876-b649-3305b9fb076a' Id, 1120 InternalId, 110 LineNbr, '<div class="group">' Content
UNION SELECT 'c8dd1013-6bf2-4ed8-a0d3-b4e061ee0647' Id, 1120 InternalId, 120 LineNbr, '<div class="field">' Content
UNION SELECT '650e18f7-3713-4b12-8870-2c3760e402cc' Id, 1120 InternalId, 130 LineNbr, '<rux-input id="frm-register-username" placeholder="username" label="User Name:" type="text" />' Content
UNION SELECT '3582c755-3398-47b5-a02e-a7ca7c2ddbac' Id, 1120 InternalId, 140 LineNbr, '</div>' Content
UNION SELECT '2451227f-f8a3-4f79-ad46-e22df52f488c' Id, 1120 InternalId, 150 LineNbr, '</div>' Content
UNION SELECT '46cbfed1-a899-436e-bbec-13c5742e47fd' Id, 1120 InternalId, 160 LineNbr, '<div class="group">' Content
UNION SELECT '6229c2a1-1a74-4caf-a8dd-f688f78b7725' Id, 1120 InternalId, 170 LineNbr, '<div class="field">' Content
UNION SELECT 'f3fc4a6a-c66e-4bd2-aefb-8eb589c16124' Id, 1120 InternalId, 180 LineNbr, '<rux-input id="frm-register-email" placeholder="email@leagalleads.com" label="Email" type="email" />' Content
UNION SELECT '299665e4-9512-464d-a536-4f02acdd565c' Id, 1120 InternalId, 190 LineNbr, '</div>' Content
UNION SELECT '456fb554-cf69-4495-8991-c23bbeec07a6' Id, 1120 InternalId, 200 LineNbr, '</div>' Content
UNION SELECT 'c16e9479-a107-47cd-981c-e7b327cbed78' Id, 1120 InternalId, 210 LineNbr, '<div class="group">' Content
UNION SELECT '1d6f4ee3-fc47-4a77-b99d-b247c0ea5eac' Id, 1120 InternalId, 220 LineNbr, '<div class="field">' Content
UNION SELECT '481b4c09-89a2-4779-b14b-009545c8a2a6' Id, 1120 InternalId, 230 LineNbr, '<rux-input id="frm-register-pw" label="Password" type="password"/>' Content
UNION SELECT '81122296-cb81-4c4a-a9b4-fc9f7b91566d' Id, 1120 InternalId, 240 LineNbr, '</div>' Content
UNION SELECT '01be1bd2-255c-4f22-bd62-2f7278326510' Id, 1120 InternalId, 250 LineNbr, '</div>' Content
UNION SELECT '73e33469-fd1b-44a2-8133-dd5d8c14daca' Id, 1120 InternalId, 260 LineNbr, '<div class="group">' Content
UNION SELECT 'ebe357ed-6b7e-42ea-bb24-8f2f37193b9c' Id, 1120 InternalId, 270 LineNbr, '<div class="field">' Content
UNION SELECT '2e63b8b1-ef67-4284-9e12-493f851a91a3' Id, 1120 InternalId, 280 LineNbr, '<rux-input id="frm-register-pw-confirm" label="Confirm Password" type="password"/>' Content
UNION SELECT '03a36f0d-0b8c-44df-99fb-04af743abe07' Id, 1120 InternalId, 290 LineNbr, '</div>' Content
UNION SELECT '83801697-2192-42ca-adba-28c3380c0f99' Id, 1120 InternalId, 300 LineNbr, '</div>' Content
UNION SELECT 'd7c71250-7d97-4e57-928b-bd20c20e8273' Id, 1120 InternalId, 310 LineNbr, '<div class="field">' Content
UNION SELECT 'a2a752cd-8722-4b19-b877-437a5b19003f' Id, 1120 InternalId, 320 LineNbr, '<rux-button id="sign-in-btn" class="sign-in-btn" type="button">Create Account</rux-button>' Content
UNION SELECT '23ecd678-5b00-43bf-9531-adc4abbe7512' Id, 1120 InternalId, 330 LineNbr, '</div>' Content
UNION SELECT '30dc8c76-59cd-4ace-b118-fac7380d7a91' Id, 1120 InternalId, 340 LineNbr, '</form>' Content
) a
INNER JOIN CONTENT b
ON a.InternalId = b.InternalId
AND b.IsActive = true
) c
LEFT JOIN CONTENTLINE d
ON c.ContentId = d.ContentId
AND c.LineNbr = d.LineNbr
WHERE d.Id IS NULL;