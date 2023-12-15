INSERT INTO CONTENTLINE ( Id, ContentId, InternalId, LineNbr, Content )
SELECT c.Id, c.ContentId, c.InternalId, c.LineNbr, c.Content
FROM
(
SELECT 
a.Id, b.Id ContentId, a.InternalId, a.LineNbr, a.Content
FROM
(
SELECT '823a65a2-d622-4014-958e-7cbc3d3ebeca' Id, 1075 InternalId, 100 LineNbr, '<rux-container name="main-container">' Content
UNION SELECT '9865a80d-086d-40d4-87c2-f12f4bb409d0' Id, 1075 InternalId, 110 LineNbr, '<div name="page-title" slot="header">' Content
UNION SELECT '29690458-0757-4eaa-ad15-e33478eb0fe9' Id, 1075 InternalId, 120 LineNbr, '<span name="page-title-text">Home</span>' Content
UNION SELECT '093c3e99-e42c-4ae9-9aa5-0e99ef820cf8' Id, 1075 InternalId, 130 LineNbr, '</div>' Content
UNION SELECT '64f99382-84ff-4750-a3c2-0a3bcb661f7b' Id, 1075 InternalId, 140 LineNbr, '<div name="page-content">' Content
UNION SELECT '6037c2da-3e36-4f2d-901c-99daa8414ba3' Id, 1075 InternalId, 150 LineNbr, '<rux-tab-panels aria-labelledby="tab-set-id-1">' Content
UNION SELECT '8fab37c5-ec9c-4025-b4eb-3afe5db7623d' Id, 1075 InternalId, 160 LineNbr, '<rux-tab-panel aria-labelledby="tab-id-1">' Content
UNION SELECT '96935cb8-24ca-4f37-bdad-608b8d803062' Id, 1075 InternalId, 170 LineNbr, '<!-- HOME.PRE.LOGIN.060.TAB.1.WELCOME -->' Content
UNION SELECT 'a54129ee-f4d1-40f6-89d1-74f2753261a2' Id, 1075 InternalId, 180 LineNbr, '</rux-tab-panel>' Content
UNION SELECT 'd24bdde0-6201-44e2-aea9-073f4949edc3' Id, 1075 InternalId, 190 LineNbr, '<rux-tab-panel aria-labelledby="tab-id-2">' Content
UNION SELECT '9fe7bdc1-8d44-47bf-abef-0d409c325c8b' Id, 1075 InternalId, 200 LineNbr, '<!-- HOME.PRE.LOGIN.060.TAB.2.LOGIN -->' Content
UNION SELECT '8ee25680-dd31-44b5-a8c8-282f4464b7fe' Id, 1075 InternalId, 210 LineNbr, '</rux-tab-panel>' Content
UNION SELECT 'c9eb80c2-461e-492b-9b84-658fc687c9c0' Id, 1075 InternalId, 220 LineNbr, '<rux-tab-panel aria-labelledby="tab-id-3">' Content
UNION SELECT 'a9c62291-4d8f-4d88-969d-9eb30d5f27ce' Id, 1075 InternalId, 230 LineNbr, '<!-- HOME.PRE.LOGIN.060.TAB.3.REGISTER -->' Content
UNION SELECT 'c71b3d60-6a41-4139-a37b-f67329816b11' Id, 1075 InternalId, 240 LineNbr, '</rux-tab-panel>' Content
UNION SELECT '1d3b9ff4-4be0-4c68-8687-7674043001e6' Id, 1075 InternalId, 250 LineNbr, '</rux-tab-panels>' Content
UNION SELECT 'caa77343-b708-4dd4-812c-0ded57a0a9d4' Id, 1075 InternalId, 260 LineNbr, '</div>' Content
UNION SELECT 'e4e85b22-f77f-47ec-810a-306a002b1aae' Id, 1075 InternalId, 270 LineNbr, '</rux-container>' Content
UNION SELECT '5d8418b5-3ef0-4a48-a862-1091a0251c33' Id, 1075 InternalId, 280 LineNbr, '' Content
) a
INNER JOIN CONTENT b
ON a.InternalId = b.InternalId
AND b.IsActive = true
) c
LEFT JOIN CONTENTLINE d
ON c.ContentId = d.ContentId
AND c.LineNbr = d.LineNbr
WHERE d.Id IS NULL;