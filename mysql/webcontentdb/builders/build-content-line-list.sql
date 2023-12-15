/*
 need to append column into CONTENT
*/
SELECT c.Id, c.ContentId, c.InternalId, c.LineNbr, c.Content
FROM
(
SELECT 
a.Id, b.Id ContentId, a.InternalId, a.LineNbr, a.Content
FROM
(
SELECT '2730a503-72ed-4663-97d1-e9e7fe4ed687' Id, 1000 InternalId, 100 LineNbr, '<!doctype html>' Content
UNION SELECT '883e1d84-4608-4979-841d-192de6e1ad65' Id, 1000 InternalId, 110 LineNbr, '<html lang="en">' Content
UNION SELECT 'be9da094-730d-49b6-9235-928090aa5aa8' Id, 1000 InternalId, 120 LineNbr, '<head>' Content
UNION SELECT '835f6d45-b475-4ae2-898a-84d33da815c7' Id, 1000 InternalId, 130 LineNbr, '<meta charset="utf-8">' Content
UNION SELECT '2eb0dfda-0146-40fd-a6c1-a07e186ee37a' Id, 1000 InternalId, 140 LineNbr, '<meta name="viewport" content="width=device-width, initial-scale=1">' Content
UNION SELECT 'e032f249-6376-4ada-8d9e-c92ac87f0b78' Id, 1000 InternalId, 150 LineNbr, '<title>legallead.ui: home</title>' Content
UNION SELECT '300d476f-cc53-475c-9811-7e94ba9198a2' Id, 1000 InternalId, 160 LineNbr, '<!-- HOME.PRE.LOGIN.010.LINKS -->' Content
UNION SELECT '2f301114-1e40-4fc0-a70e-6717cf7788f1' Id, 1000 InternalId, 170 LineNbr, '<!-- HOME.PRE.LOGIN.020.STYLES -->' Content
UNION SELECT '0f1ebc6c-67aa-4ef0-aa60-11b1ea0f7f71' Id, 1000 InternalId, 180 LineNbr, '</head>' Content
UNION SELECT '5d87e455-fdbe-486e-aa17-104f09373d13' Id, 1000 InternalId, 190 LineNbr, '<body class="light-theme">' Content
UNION SELECT '9cc35cc8-8866-4387-8501-29537f3b0082' Id, 1000 InternalId, 200 LineNbr, '<!-- HOME.PRE.LOGIN.030.HEADER -->' Content
UNION SELECT '7406bd11-20d7-44fa-876e-794655db8f0d' Id, 1000 InternalId, 210 LineNbr, '<!-- HOME.PRE.LOGIN.040.NOTIFICATIONS -->' Content
UNION SELECT '56139c28-abe0-45a3-aafe-2638884ec936' Id, 1000 InternalId, 220 LineNbr, '<!-- HOME.PRE.LOGIN.050.MAIN.CONTENT -->' Content
UNION SELECT 'fc6976e5-8e9b-40c2-91ef-ffb7eb589061' Id, 1000 InternalId, 230 LineNbr, '<!-- HOME.PRE.LOGIN.070.JS.SCRIPTS -->' Content
UNION SELECT '2eeed01b-4693-474a-ae8d-e6c272234152' Id, 1000 InternalId, 240 LineNbr, '</body>' Content
UNION SELECT 'da254f56-36ab-41a1-9b4d-5caaa7063df8' Id, 1000 InternalId, 250 LineNbr, '</html>' Content
) a
INNER JOIN CONTENT b
ON a.InternalId = b.InternalId
AND b.IsActive = true
) c
LEFT JOIN CONTENTLINE d
ON c.ContentId = d.ContentId
AND c.LineNbr = d.LineNbr
WHERE d.Id IS NULL;