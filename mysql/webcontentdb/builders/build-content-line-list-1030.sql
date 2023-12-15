INSERT INTO CONTENTLINE ( Id, ContentId, InternalId, LineNbr, Content )
SELECT c.Id, c.ContentId, c.InternalId, c.LineNbr, c.Content
FROM
(
SELECT 
a.Id, b.Id ContentId, a.InternalId, a.LineNbr, a.Content
FROM
(
SELECT '08e35aad-2623-4234-98fa-53a87165f3b7' Id, 1030 InternalId, 100 LineNbr, '<style>' Content
UNION SELECT 'd4edecc6-131b-4a44-b09d-57bab3ea3137' Id, 1030 InternalId, 110 LineNbr, 'div.global-notification {' Content
UNION SELECT 'a60c06e4-d21c-4504-866d-678af5fb5da3' Id, 1030 InternalId, 120 LineNbr, 'margin-top: 5px;' Content
UNION SELECT '07bd12a5-bb50-4ba8-ab83-cf239ec4f4dc' Id, 1030 InternalId, 130 LineNbr, '}' Content
UNION SELECT 'dfc072ad-6d82-4bc1-809a-f935ef12cc8c' Id, 1030 InternalId, 140 LineNbr, 'div.global-header {' Content
UNION SELECT 'd2bd6b76-215f-4abc-b010-fc1a2f136bfb' Id, 1030 InternalId, 150 LineNbr, 'display: flex;' Content
UNION SELECT 'dbb25cc8-cc22-49af-8c1f-ca8c5403ff62' Id, 1030 InternalId, 160 LineNbr, 'justify-content: center;' Content
UNION SELECT '90d58a1a-2f81-43b1-a3aa-53b4709c9d7b' Id, 1030 InternalId, 170 LineNbr, '}' Content
UNION SELECT '12a2c1bc-d18c-4ef8-821a-df92ce85d789' Id, 1030 InternalId, 180 LineNbr, 'rux-container[name="main-container"] {' Content
UNION SELECT 'f921827e-7a4b-4784-a834-ff1d68dd1b7b' Id, 1030 InternalId, 190 LineNbr, 'padding: 10px;' Content
UNION SELECT 'd12f8ba4-0769-4d35-b3b3-792a4022bf61' Id, 1030 InternalId, 200 LineNbr, '}' Content
UNION SELECT 'bea4e1b9-32f0-4eae-8b2e-3c9d7583bdb0' Id, 1030 InternalId, 210 LineNbr, 'div.main-content {' Content
UNION SELECT '05f56996-8a3c-40c3-89cd-fc09e7987833' Id, 1030 InternalId, 220 LineNbr, 'padding: 10px;' Content
UNION SELECT 'b9e59f66-608c-4a09-aa60-780a8d7558f5' Id, 1030 InternalId, 230 LineNbr, '}' Content
UNION SELECT '5fc33509-163e-4c61-9554-324a3dadfe3b' Id, 1030 InternalId, 240 LineNbr, 'div.page-menu {' Content
UNION SELECT 'd4ba8e52-c863-4f3b-8214-bb66f2de5ca0' Id, 1030 InternalId, 250 LineNbr, 'display: flex;' Content
UNION SELECT '20b94c6e-017f-4711-8d6e-ece6649e90ed' Id, 1030 InternalId, 260 LineNbr, 'width: 100%;' Content
UNION SELECT '23c08da8-1283-4341-a114-61c8126b7129' Id, 1030 InternalId, 270 LineNbr, 'justify-content: left;' Content
UNION SELECT 'a11067c6-ff85-41c7-a0a9-bdaca5af4486' Id, 1030 InternalId, 280 LineNbr, 'margin-bottom: 10px;' Content
UNION SELECT 'e0296804-2cd8-4b33-a914-b90a720ae517' Id, 1030 InternalId, 290 LineNbr, '}' Content
UNION SELECT 'bdb7a46f-1476-438b-811d-f6626598d9ee' Id, 1030 InternalId, 300 LineNbr, 'div[name="register-now"] {' Content
UNION SELECT '22cec20e-b0a4-441c-a1ee-b3bec81dfe84' Id, 1030 InternalId, 310 LineNbr, 'width: 50%;' Content
UNION SELECT '3fec0cfa-7ee3-40ff-ac62-42dd30779770' Id, 1030 InternalId, 320 LineNbr, 'min-width: 300px;' Content
UNION SELECT '1938f202-7a07-42e1-b08e-733ba84f2b90' Id, 1030 InternalId, 330 LineNbr, 'max-width: 500px;' Content
UNION SELECT '0cfd0e09-c774-424e-9186-d8ff4ffd2343' Id, 1030 InternalId, 340 LineNbr, 'padding: 10px;' Content
UNION SELECT 'd7b8aade-70bc-4e54-877a-66ad59de043a' Id, 1030 InternalId, 350 LineNbr, 'border: 1px dotted var(--color-border-interactive-muted);' Content
UNION SELECT '4d90c780-3fa2-4ec4-85e9-a4e03c319a56' Id, 1030 InternalId, 360 LineNbr, '}' Content
UNION SELECT 'df1b3c12-e42d-4dc4-ac06-9ac405d057f4' Id, 1030 InternalId, 370 LineNbr, 'div[name="register-now"] p.not-yet { color: var(--color-text-secondary); }' Content
UNION SELECT '987b7590-b23d-473e-a232-9d3d6fb3c907' Id, 1030 InternalId, 380 LineNbr, '' Content
UNION SELECT '314587f3-8de0-4cb5-b98a-f0e9a44946e5' Id, 1030 InternalId, 390 LineNbr, '' Content
UNION SELECT 'e861c030-4f50-4526-bd63-83b6640570de' Id, 1030 InternalId, 400 LineNbr, 'form {' Content
UNION SELECT 'f780891b-a0bf-47fa-9ed8-04d59f6eb6ff' Id, 1030 InternalId, 410 LineNbr, 'margin: 0 auto;' Content
UNION SELECT 'daf6579d-46bc-4a79-ba9a-837e6ea1092e' Id, 1030 InternalId, 420 LineNbr, 'max-width: 330px;' Content
UNION SELECT '3d6cf9ce-39da-4f98-b267-c09c1aef4b2f' Id, 1030 InternalId, 430 LineNbr, 'display: block;' Content
UNION SELECT '041ec5bf-df0b-427b-9ec6-2e1d76e48d7c' Id, 1030 InternalId, 440 LineNbr, '}' Content
UNION SELECT 'dfc2f080-35cf-4609-8962-b2472c8e484e' Id, 1030 InternalId, 450 LineNbr, '.group {' Content
UNION SELECT 'b2e91b73-28f8-4faa-b524-a743a8d1001c' Id, 1030 InternalId, 460 LineNbr, 'margin-bottom: 2.25rem;' Content
UNION SELECT '32a51ef1-dd68-4e83-aa8b-5bd1f5d71a40' Id, 1030 InternalId, 470 LineNbr, '}' Content
UNION SELECT '8eb45fe1-2fdb-4d4e-8f15-117f853662c6' Id, 1030 InternalId, 480 LineNbr, '.field {' Content
UNION SELECT '7cba0fcc-0e8c-46e0-ac07-f0093061dec6' Id, 1030 InternalId, 490 LineNbr, 'display: flex;' Content
UNION SELECT '619bec4d-0fd4-45d5-bb81-49848390ae70' Id, 1030 InternalId, 500 LineNbr, 'flex-flow: row wrap;' Content
UNION SELECT '0c6b7e9c-d811-4292-b3a1-e96624f08b4b' Id, 1030 InternalId, 510 LineNbr, 'align-items: flex-start;' Content
UNION SELECT '78c1fb85-4018-4591-871d-3f73ad46fef8' Id, 1030 InternalId, 520 LineNbr, '}' Content
UNION SELECT '53aec017-8df3-4a68-beea-2e16849a3195' Id, 1030 InternalId, 530 LineNbr, '.checkbox {' Content
UNION SELECT '4336995e-4687-4e4b-87d5-56c7340b217f' Id, 1030 InternalId, 540 LineNbr, 'margin: 0.5rem 0 1rem 0 !important;' Content
UNION SELECT '4982aa6c-f3b1-4ae3-bc37-c05771bac7a5' Id, 1030 InternalId, 550 LineNbr, 'line-height: 1.2;' Content
UNION SELECT 'b015bba8-8b20-433d-9a99-af1aa42f27dd' Id, 1030 InternalId, 560 LineNbr, '}' Content
UNION SELECT '5e29f5cd-c5cf-402a-b7d5-1a2dde379816' Id, 1030 InternalId, 570 LineNbr, 'rux-input {' Content
UNION SELECT '2d4848de-3425-43d7-9825-fc6858574d35' Id, 1030 InternalId, 580 LineNbr, 'width: 100%;' Content
UNION SELECT 'd2fc7002-218b-4061-a64a-1684daddf89a' Id, 1030 InternalId, 590 LineNbr, '}' Content
UNION SELECT '9717dba5-cadf-4b2d-b494-1c5f7c071458' Id, 1030 InternalId, 600 LineNbr, '.sign-in-btn {' Content
UNION SELECT 'fc4f5941-e52e-4c6c-ac3e-68a2932c545a' Id, 1030 InternalId, 610 LineNbr, 'margin-left: auto;' Content
UNION SELECT '031729ed-9240-4af8-a373-68e61f7dc390' Id, 1030 InternalId, 620 LineNbr, '}' Content
UNION SELECT 'b03033ac-f603-4516-a1dc-2b95db548f68' Id, 1030 InternalId, 630 LineNbr, '</style>' Content
) a
INNER JOIN CONTENT b
ON a.InternalId = b.InternalId
AND b.IsActive = true
) c
LEFT JOIN CONTENTLINE d
ON c.ContentId = d.ContentId
AND c.LineNbr = d.LineNbr
WHERE d.Id IS NULL;