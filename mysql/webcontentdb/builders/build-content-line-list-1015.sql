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
SELECT 'ecebd7bb-29e8-4035-9761-41a028e5d325' Id, 1015 InternalId, 100 LineNbr, '<link rel="preconnect" href="https://fonts.gstatic.com" />' Content
UNION SELECT '72948087-5bb3-445b-847f-df8645424c86' Id, 1015 InternalId, 110 LineNbr, '<link href="https://fonts.googleapis.com/css2?family=Roboto:wght@300;400;500;700&display=swap" rel="stylesheet" />' Content
UNION SELECT '6a0d3012-2f21-4892-a061-fb344d8e0faf' Id, 1015 InternalId, 120 LineNbr, '<link rel="stylesheet" href="https://unpkg.com/@astrouxds/astro-web-components/dist/astro-web-components/astro-web-components.css" />' Content
UNION SELECT '494fba07-59ae-481e-b724-8562ee031cbe' Id, 1015 InternalId, 130 LineNbr, '<script type="module" src="https://unpkg.com/@astrouxds/astro-web-components/dist/astro-web-components/astro-web-components.esm.js"></script>' Content
) a
INNER JOIN CONTENT b
ON a.InternalId = b.InternalId
AND b.IsActive = true
) c
LEFT JOIN CONTENTLINE d
ON c.ContentId = d.ContentId
AND c.LineNbr = d.LineNbr
WHERE d.Id IS NULL;