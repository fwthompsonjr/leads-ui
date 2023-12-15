INSERT INTO CONTENTLINE ( Id, ContentId, InternalId, LineNbr, Content )
SELECT c.Id, c.ContentId, c.InternalId, c.LineNbr, c.Content
FROM
(
SELECT 
a.Id, b.Id ContentId, a.InternalId, a.LineNbr, a.Content
FROM
(
SELECT 'f6c0ada0-97a4-4bf2-940e-5cd8e96cf52f' Id, 1060 InternalId, 100 LineNbr, '<div class="global-notification">' Content
UNION SELECT '91845bb7-e8e1-4e85-94df-a55c962f6ad5' Id, 1060 InternalId, 110 LineNbr, '<rux-notification id="notification" status="caution" small="true">' Content
UNION SELECT 'abf7e762-10c2-42de-babb-78b3c0e99ce2' Id, 1060 InternalId, 120 LineNbr, '<div id="notification-content"></div>' Content
UNION SELECT '82dd9fa5-5ce5-424e-b2a0-c45c72d04652' Id, 1060 InternalId, 130 LineNbr, '</rux-notification>' Content
UNION SELECT '90bcc6aa-ca04-43a0-b276-7e3a321461e6' Id, 1060 InternalId, 140 LineNbr, '</div>' Content
) a
INNER JOIN CONTENT b
ON a.InternalId = b.InternalId
AND b.IsActive = true
) c
LEFT JOIN CONTENTLINE d
ON c.ContentId = d.ContentId
AND c.LineNbr = d.LineNbr
WHERE d.Id IS NULL;