INSERT INTO CONTENTLINE ( Id, ContentId, InternalId, LineNbr, Content )
SELECT c.Id, c.ContentId, c.InternalId, c.LineNbr, c.Content
FROM
(
SELECT 
a.Id, b.Id ContentId, a.InternalId, a.LineNbr, a.Content
FROM
(
SELECT 'a329f435-beac-4e53-95b2-92976542f913' Id, 1105 InternalId, 100 LineNbr, '<form id="frm-login">' Content
UNION SELECT '1f01dbac-6619-4354-a0ed-a9339473d3ad' Id, 1105 InternalId, 110 LineNbr, '<div class="group">' Content
UNION SELECT 'bd820c9a-11cf-4bab-ba39-6e09fb81ae02' Id, 1105 InternalId, 120 LineNbr, '<div class="field">' Content
UNION SELECT '50f0c1bd-120b-418d-b3da-ddab33605902' Id, 1105 InternalId, 130 LineNbr, '<rux-input id="frm-login-username" placeholder="username" label="User Name / Email:" type="text" />' Content
UNION SELECT '139d53dc-4b3c-4661-b90e-d62f72d504b9' Id, 1105 InternalId, 140 LineNbr, '</div>' Content
UNION SELECT '69a8565a-7b01-467c-ad62-d0ce23637b2f' Id, 1105 InternalId, 150 LineNbr, '</div>' Content
UNION SELECT '898671cc-fc2d-4074-ace1-beeea8f1067e' Id, 1105 InternalId, 160 LineNbr, '<div class="group">' Content
UNION SELECT '1bf4065a-6cd6-45cc-af40-a6703e63e474' Id, 1105 InternalId, 170 LineNbr, '<div class="field">' Content
UNION SELECT '157016f5-744e-4bdb-8d03-11b3877a018d' Id, 1105 InternalId, 180 LineNbr, '<rux-input id="frm-login-pw" label="Password" type="password"/>' Content
UNION SELECT 'da8b3ee7-1dfa-4b1b-b675-527522898624' Id, 1105 InternalId, 190 LineNbr, '</div>' Content
UNION SELECT '4f777f2c-45e4-4756-8d09-8bc2988f96d0' Id, 1105 InternalId, 200 LineNbr, '</div>' Content
UNION SELECT '7488820b-3e9d-4a50-b800-e2b0847cb05e' Id, 1105 InternalId, 210 LineNbr, '<div class="field">' Content
UNION SELECT '045f0dea-eb93-43a4-b914-318fe5dde004' Id, 1105 InternalId, 220 LineNbr, '<rux-button id="sign-in-btn" class="sign-in-btn" type="button">Sign In</rux-button>' Content
UNION SELECT '5ed16210-a18b-44f2-a29d-be617475ee43' Id, 1105 InternalId, 230 LineNbr, '</div>' Content
UNION SELECT 'bbf23f35-5867-4286-ba90-56758062dcba' Id, 1105 InternalId, 240 LineNbr, '</form>' Content
) a
INNER JOIN CONTENT b
ON a.InternalId = b.InternalId
AND b.IsActive = true
) c
LEFT JOIN CONTENTLINE d
ON c.ContentId = d.ContentId
AND c.LineNbr = d.LineNbr
WHERE d.Id IS NULL;