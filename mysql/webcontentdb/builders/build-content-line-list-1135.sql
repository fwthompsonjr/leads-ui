INSERT INTO CONTENTLINE ( Id, ContentId, InternalId, LineNbr, Content )
SELECT c.Id, c.ContentId, c.InternalId, c.LineNbr, c.Content
FROM
(
SELECT 
a.Id, b.Id ContentId, a.InternalId, a.LineNbr, a.Content
FROM
(
SELECT 'e16b0d6c-aafb-4cc6-9a9e-0f85cdd403c4' Id, 1135 InternalId, 100 LineNbr, '<script>' Content
UNION SELECT 'f508a49f-e53b-47f5-a240-2bcfbec5d9ab' Id, 1135 InternalId, 110 LineNbr, 'function initializeStatusBar() {' Content
UNION SELECT '75a608d7-9f84-4e88-bf87-089a784cd87d' Id, 1135 InternalId, 120 LineNbr, 'const headerid = "header-status-bar";' Content
UNION SELECT '6c5e8337-1252-4a5d-8f40-0e775a3d9f89' Id, 1135 InternalId, 130 LineNbr, 'const element = document.getElementById(headerid);' Content
UNION SELECT '22ba60b9-7ce1-4967-b037-93cc3ee6ded7' Id, 1135 InternalId, 140 LineNbr, 'if (undefined === element || null === element ) { return; }' Content
UNION SELECT 'e734c10a-0f41-4f11-a805-4b2d0fa91456' Id, 1135 InternalId, 150 LineNbr, 'const keynames = "app-state-color,app-state,app-version".split(",");' Content
UNION SELECT 'e32e1eed-461e-44ba-880e-b99ee07b1f53' Id, 1135 InternalId, 160 LineNbr, 'const defaultvalues = "tag1,Development,3.2 Beta".split(",");' Content
UNION SELECT 'e77d131a-348c-4069-a3f5-da04cc05416b' Id, 1135 InternalId, 170 LineNbr, 'for (let i = 0; i < keynames.length; i++) {' Content
UNION SELECT '48a64924-469f-4f9d-8bc2-6e24e833df2f' Id, 1135 InternalId, 180 LineNbr, 'let attr = element.getAttribute(keynames[i]);' Content
UNION SELECT '8605f26c-3875-42f7-8159-5e30a0e7cfff' Id, 1135 InternalId, 190 LineNbr, 'if (undefined === attr || null === attr ) {' Content
UNION SELECT 'e090aec8-bfc3-4d72-9186-85034830e92a' Id, 1135 InternalId, 200 LineNbr, 'let attnw = document.createAttribute(keynames[i]);' Content
UNION SELECT '2f168a0d-cfd7-4066-9de1-9d582b399c43' Id, 1135 InternalId, 210 LineNbr, 'attnw.value = defaultvalues[i];' Content
UNION SELECT '91abbc2d-f050-4e6b-884b-dcab9b834051' Id, 1135 InternalId, 220 LineNbr, 'element.setAttributeNode(attnw);' Content
UNION SELECT 'ab9a140c-2f4f-4524-b39a-5102c22e6052' Id, 1135 InternalId, 230 LineNbr, 'continue;' Content
UNION SELECT '138b8906-1364-41ec-bcf9-866129035776' Id, 1135 InternalId, 240 LineNbr, '}' Content
UNION SELECT '5672ca65-5bc2-43ce-b372-20a0ff03fc20' Id, 1135 InternalId, 250 LineNbr, 'if (attr.indexOf("{") < 0 ) { continue; }' Content
UNION SELECT 'edf13d7f-890e-4494-87c4-545b28e3fb86' Id, 1135 InternalId, 260 LineNbr, 'element.setAttribute( keynames[i], defaultvalues[i] );' Content
UNION SELECT '8deb3cac-31bf-4251-8af0-eba6c1a92456' Id, 1135 InternalId, 270 LineNbr, '}' Content
UNION SELECT '15c293a0-7da2-4fe4-8120-116388433726' Id, 1135 InternalId, 280 LineNbr, '}' Content
UNION SELECT '085b1298-6dc5-4315-b65b-4ceda4766d18' Id, 1135 InternalId, 290 LineNbr, '' Content
UNION SELECT '7244aa52-34b2-4086-aabf-ed0bd3bbccaa' Id, 1135 InternalId, 300 LineNbr, 'function docReady(fn) {' Content
UNION SELECT '38a68b1e-502e-49b6-b003-59e49aa15d63' Id, 1135 InternalId, 310 LineNbr, '// see if DOM is already available' Content
UNION SELECT '6630aa27-ae04-423f-a6d7-328d6937a59e' Id, 1135 InternalId, 320 LineNbr, 'if (document.readyState === "complete" || document.readyState === "interactive") {' Content
UNION SELECT '88d9b5f7-f948-4e93-9ce0-957194a7c40a' Id, 1135 InternalId, 330 LineNbr, '// call on next available tick' Content
UNION SELECT '62dc5c26-24ce-482b-b5ce-9f1ce0177f4d' Id, 1135 InternalId, 340 LineNbr, 'setTimeout(fn, 10);' Content
UNION SELECT 'ac07950e-d2d4-44ad-b0fd-c59dcb91ebc6' Id, 1135 InternalId, 350 LineNbr, '} else {' Content
UNION SELECT '46d776c9-4662-4695-a1d6-4b8da6e1c4a8' Id, 1135 InternalId, 360 LineNbr, 'document.addEventListener("DOMContentLoaded", fn);' Content
UNION SELECT 'f39b97e2-b8c2-4e77-b244-aaa2117f115a' Id, 1135 InternalId, 370 LineNbr, '}' Content
UNION SELECT '6f728f3b-009d-4fa0-8415-5fbba09c1132' Id, 1135 InternalId, 380 LineNbr, '}' Content
UNION SELECT '6829f406-e30d-44a6-881e-0bdd1add065e' Id, 1135 InternalId, 390 LineNbr, '' Content
UNION SELECT '55b9f2e0-36a4-46e0-adbb-8d2b9f32842c' Id, 1135 InternalId, 400 LineNbr, 'docReady(function() {' Content
UNION SELECT '2289b305-be52-4d6d-ab58-4b18910533a3' Id, 1135 InternalId, 410 LineNbr, 'initializeStatusBar();' Content
UNION SELECT '3db10fdb-ba4c-400a-8c5a-110d29fc0959' Id, 1135 InternalId, 420 LineNbr, '});' Content
UNION SELECT '9536b3d6-4868-4e25-bb8c-2e2a3f2e5514' Id, 1135 InternalId, 430 LineNbr, '</script>' Content
) a
INNER JOIN CONTENT b
ON a.InternalId = b.InternalId
AND b.IsActive = true
) c
LEFT JOIN CONTENTLINE d
ON c.ContentId = d.ContentId
AND c.LineNbr = d.LineNbr
WHERE d.Id IS NULL;