/*
 need to append column into CONTENT
*/
INSERT INTO CONTENTLINE ( Id, ContentId, InternalId, LineNbr, Content )
SELECT c.Id, c.ContentId, c.InternalId, c.LineNbr, c.Content
FROM
(
SELECT 
a.Id, b.Id ContentId, a.InternalId, a.LineNbr, a.Content
FROM
(
SELECT 'cc6bc92b-9dbd-4295-81a2-612b551fc1a9' Id, 1000 InternalId, 100 LineNbr, '<!doctype html>' Content
UNION SELECT '8bfbeed3-59c2-4222-9a34-e0cc7578c82d' Id, 1000 InternalId, 110 LineNbr, '<html lang="en">' Content
UNION SELECT '2a0d6066-943f-4765-923b-8d633062c36f' Id, 1000 InternalId, 120 LineNbr, '<head>' Content
UNION SELECT 'f5fbf1fc-acf2-4e2d-9ed7-e6e66f97dfb0' Id, 1000 InternalId, 130 LineNbr, '<meta charset="utf-8">' Content
UNION SELECT '8ad5c6c3-01a0-4195-81a3-e93af89827a1' Id, 1000 InternalId, 140 LineNbr, '<meta name="viewport" content="width=device-width, initial-scale=1">' Content
UNION SELECT '3fbf2e95-990f-4872-b05f-55a984f3836f' Id, 1000 InternalId, 150 LineNbr, '<title>legallead.ui: home</title>' Content
UNION SELECT 'e52979b5-4f89-4ebc-90db-64be91de7c55' Id, 1000 InternalId, 160 LineNbr, '<!-- HOME.PRE.LOGIN.010.LINKS -->' Content
UNION SELECT '57720d3d-59ce-4a04-98fc-db7dd0249b3e' Id, 1000 InternalId, 170 LineNbr, '<!-- HOME.PRE.LOGIN.020.STYLES -->' Content
UNION SELECT '96787fd1-4f23-4b8e-af80-a6a33735be95' Id, 1000 InternalId, 180 LineNbr, '</head>' Content
UNION SELECT '224463af-3f20-4f5d-b0f4-c922df13fa3b' Id, 1000 InternalId, 190 LineNbr, '<body class="light-theme">' Content
UNION SELECT 'd8685d74-6abe-4fdc-a2e1-630f64f2da3b' Id, 1000 InternalId, 200 LineNbr, '<!-- HOME.PRE.LOGIN.030.HEADER -->' Content
UNION SELECT '23b90b8a-341e-49e3-adf2-e728bde6beb9' Id, 1000 InternalId, 210 LineNbr, '<!-- HOME.PRE.LOGIN.040.NOTIFICATIONS -->' Content
UNION SELECT '9756af1d-f641-421a-afe4-e1fb36d4ffa1' Id, 1000 InternalId, 220 LineNbr, '<!-- HOME.PRE.LOGIN.050.MAIN.CONTENT -->' Content
UNION SELECT '5b8e1090-f3d0-4339-ae3d-39910ddf97ef' Id, 1000 InternalId, 230 LineNbr, '<!-- HOME.PRE.LOGIN.070.JS.SCRIPTS -->' Content
UNION SELECT '877c92f5-28e4-4798-b783-e616ee7426f6' Id, 1000 InternalId, 240 LineNbr, '</body>' Content
UNION SELECT '86351455-9574-4506-907a-0ffe5be8d8fa' Id, 1000 InternalId, 250 LineNbr, '</html>' Content
) a
INNER JOIN CONTENT b
ON a.InternalId = b.InternalId
AND b.IsActive = true
) c
LEFT JOIN CONTENTLINE d
ON c.ContentId = d.ContentId
AND c.LineNbr = d.LineNbr
WHERE d.Id IS NULL;