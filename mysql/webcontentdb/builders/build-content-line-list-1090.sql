INSERT INTO CONTENTLINE ( Id, ContentId, InternalId, LineNbr, Content )
SELECT c.Id, c.ContentId, c.InternalId, c.LineNbr, c.Content
FROM
(
SELECT 
a.Id, b.Id ContentId, a.InternalId, a.LineNbr, a.Content
FROM
(
SELECT '2cc1f293-daa1-4cff-83f8-055b36d97194' Id, 1090 InternalId, 100 LineNbr, '<div>' Content
UNION SELECT '40f37144-35fa-4cbb-8258-cb52ae12483f' Id, 1090 InternalId, 110 LineNbr, '<br/>' Content
UNION SELECT 'c312d068-b7d4-4894-8367-61cc8fc13af7' Id, 1090 InternalId, 120 LineNbr, 'Legal Lead is an application used to locate public data from county courts.<br />' Content
UNION SELECT '9012bb29-4be3-4242-a639-e628b126bd60' Id, 1090 InternalId, 130 LineNbr, 'From this application you can retrieve: <br />' Content
UNION SELECT '9d84bfc7-c864-4fc2-baff-2e95a9696d03' Id, 1090 InternalId, 140 LineNbr, '<ol>' Content
UNION SELECT 'cdf0e395-3881-4ff8-a989-daf9f70b370b' Id, 1090 InternalId, 150 LineNbr, '<li>Case Details</li>' Content
UNION SELECT '42b49fdc-af90-40d7-9962-99c98aeb4f09' Id, 1090 InternalId, 160 LineNbr, '<li>Defendant Information</li>' Content
UNION SELECT '6ec8df51-cdd7-4af5-91ba-d3fdfa63b82c' Id, 1090 InternalId, 170 LineNbr, '<li>Court Information</li>' Content
UNION SELECT 'c2d7ba98-3822-4c05-9a6f-89ab627f93a2' Id, 1090 InternalId, 180 LineNbr, '</ol>' Content
UNION SELECT 'cd0f6cf4-1085-4dc8-a24f-92480ec48080' Id, 1090 InternalId, 190 LineNbr, '' Content
UNION SELECT '2875e9cc-6ea5-4957-a02b-34c4f517d0b0' Id, 1090 InternalId, 200 LineNbr, '<div name="page-call-to-action">' Content
UNION SELECT 'ae936983-fc7d-4650-899b-7243a01ac9c7' Id, 1090 InternalId, 210 LineNbr, '<p><a href="#">Login</a>, if you already have an account.</p>' Content
UNION SELECT '35826d80-941f-432e-9851-7c2237039c45' Id, 1090 InternalId, 220 LineNbr, '</div>' Content
UNION SELECT '4cd9a1a7-6ef8-4829-ba73-299aa33d0c38' Id, 1090 InternalId, 230 LineNbr, '' Content
UNION SELECT 'fb95d40b-afd9-4cca-81a3-7c88c08a5a3f' Id, 1090 InternalId, 240 LineNbr, '' Content
UNION SELECT 'a07ad2f1-f741-4bb1-a0fe-501cef5cfe64' Id, 1090 InternalId, 250 LineNbr, '<div name="register-now">' Content
UNION SELECT '07ce370e-7457-4929-b996-503a9b866bb6' Id, 1090 InternalId, 260 LineNbr, '<p class="not-yet">Not registered yet?</p>' Content
UNION SELECT 'aa331619-d875-47d1-a668-750eb77a08ea' Id, 1090 InternalId, 270 LineNbr, '<p>' Content
UNION SELECT 'c3430755-2298-4d43-bbfa-8f2c58246374' Id, 1090 InternalId, 280 LineNbr, '<a href="#">Register now!</a> <br/>' Content
UNION SELECT '9886e687-408e-4f8b-9d70-16f809f6e317' Id, 1090 InternalId, 290 LineNbr, 'It is easy, done in 1 minute and gives you access to product demos and much more!' Content
UNION SELECT '93bf7be6-74cc-4c6b-a766-4b82ca90422a' Id, 1090 InternalId, 300 LineNbr, '</p>' Content
UNION SELECT '57f1eaa8-64fb-4420-8793-0ae956794212' Id, 1090 InternalId, 310 LineNbr, '</div>' Content
UNION SELECT 'bc1fb0fc-ed5e-44b2-9dce-8f856f274c80' Id, 1090 InternalId, 320 LineNbr, '</div>' Content
) a
INNER JOIN CONTENT b
ON a.InternalId = b.InternalId
AND b.IsActive = true
) c
LEFT JOIN CONTENTLINE d
ON c.ContentId = d.ContentId
AND c.LineNbr = d.LineNbr
WHERE d.Id IS NULL;