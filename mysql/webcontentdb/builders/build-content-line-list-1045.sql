INSERT INTO CONTENTLINE ( Id, ContentId, InternalId, LineNbr, Content )
SELECT c.Id, c.ContentId, c.InternalId, c.LineNbr, c.Content
FROM
(
SELECT 
a.Id, b.Id ContentId, a.InternalId, a.LineNbr, a.Content
FROM
(
SELECT '889431c5-188c-42d3-8600-970ed7d09a88' Id, 1045 InternalId, 100 LineNbr, '<div class="global-header">' Content
UNION SELECT '571ef49c-b109-49ab-bad4-cecbf92a62ca' Id, 1045 InternalId, 110 LineNbr, '<rux-global-status-bar' Content
UNION SELECT 'acb3f06c-adc0-4051-be68-53525f5d5828' Id, 1045 InternalId, 120 LineNbr, 'id="header-status-bar"' Content
UNION SELECT '8b32fb65-264f-41a7-8ad0-c6110abc3358' Id, 1045 InternalId, 130 LineNbr, 'include-icon="true"' Content
UNION SELECT 'ee1ac8ae-d38e-4f33-a97e-b7e8a7e9bf88' Id, 1045 InternalId, 140 LineNbr, 'app-domain="Legal Lead"' Content
UNION SELECT 'd5a21dd0-e92c-4551-b7a3-9b886f7b19a2' Id, 1045 InternalId, 150 LineNbr, 'app-name="UI"' Content
UNION SELECT '4bcc8bc6-0c08-45ee-ab26-b48e359e7936' Id, 1045 InternalId, 160 LineNbr, 'menu-icon="school"' Content
UNION SELECT 'ac46b0e1-b678-4859-96c1-3e39823162e8' Id, 1045 InternalId, 170 LineNbr, 'username=""' Content
UNION SELECT '624e361d-b8f3-44ba-aa94-111b954da0df' Id, 1045 InternalId, 180 LineNbr, 'app-state-color="{{app-tag-color}}"' Content
UNION SELECT '52af694b-0b51-49f0-9ce8-4641db73bf8d' Id, 1045 InternalId, 190 LineNbr, 'app-state="{{app-environment}}"' Content
UNION SELECT '99f499e6-b1c2-4663-818d-45ddfdcda56e' Id, 1045 InternalId, 200 LineNbr, 'app-version="{{app-version}}">' Content
UNION SELECT '067fa07e-4b6e-43de-a3cb-32fab52f1453' Id, 1045 InternalId, 210 LineNbr, '<rux-tabs id="tab-set-id-1" small="true">' Content
UNION SELECT 'a8c38c8f-13b0-42bf-afd0-fabd0353e9e2' Id, 1045 InternalId, 220 LineNbr, '<rux-tab id="tab-id-1" selected>Welcome</rux-tab>' Content
UNION SELECT '1e73f154-11a7-4b67-9ebf-d59f2a736344' Id, 1045 InternalId, 230 LineNbr, '<rux-tab id="tab-id-2">Login</rux-tab>' Content
UNION SELECT 'ad102e16-2c52-42f4-b6d7-086f7f5e2d80' Id, 1045 InternalId, 240 LineNbr, '<rux-tab id="tab-id-3">Register</rux-tab>' Content
UNION SELECT '8e4dc2d4-fac1-4295-a6a0-f6dde63c32a0' Id, 1045 InternalId, 250 LineNbr, '</rux-tabs>' Content
UNION SELECT '66a539c3-ad21-4147-80f3-12ff93d3fdc1' Id, 1045 InternalId, 260 LineNbr, '</rux-global-status-bar>' Content
UNION SELECT '0e7cbc95-742b-4430-9560-7da0ea35613f' Id, 1045 InternalId, 270 LineNbr, '</div>' Content
) a
INNER JOIN CONTENT b
ON a.InternalId = b.InternalId
AND b.IsActive = true
) c
LEFT JOIN CONTENTLINE d
ON c.ContentId = d.ContentId
AND c.LineNbr = d.LineNbr
WHERE d.Id IS NULL;