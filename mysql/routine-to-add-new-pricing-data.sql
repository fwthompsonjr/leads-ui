/*
Id = 5e2031a4-de48-11ee-b616-0af7a01f52e9

*/

SET @revised_pricing = '{
 	"product": {
 		"name": "Silver Account",
 		"decription": "Silver billing plan for record search.",
        "code": "-"
 	},
 	"priceamount": {
 		"monthly": 12,
 		"annual": 100
 	},
 	"pricecode": {
 		"monthly": "abc-def",
 		"annual": "wxy-abc"
 	}
 }
';
SET @permission_code_index = '5e2031a4-de48-11ee-b616-0af7a01f52e9';
SET @permission_group_index = (
SELECT PermissionGroupId
  FROM PERMISSIONGROUPCODES pgc
  WHERE pgc.Id = @permission_code_index);

  

SELECT	pg.KeyName,
		js.*,
        @revised_pricing KeyJs,
        TRUE IsActive
FROM (
	SELECT @permission_group_index PermissionGroupId,
    JSON_UNQUOTE( JSON_EXTRACT( @revised_pricing, '$.product.code' ) ) ProductCode,
    JSON_UNQUOTE( JSON_EXTRACT( @revised_pricing, '$.pricecode.annual' ) ) PriceCodeAnnual,
    JSON_UNQUOTE( JSON_EXTRACT( @revised_pricing, '$.pricecode.monthly' ) ) PriceCodeMonthly
    ) js
JOIN ( SELECT * FROM PERMISSIONGROUPCODES WHERE Id = @permission_code_index LIMIT 1 ) pg
ON	js.PermissionGroupId = pg.PermissionGroupId
LEFT JOIN (
	SELECT PermissionGroupId
      FROM PERMISSIONGROUPCODES
      WHERE IsActive = TRUE
      GROUP BY PermissionGroupId
) cds
ON	js.PermissionGroupId = cds.PermissionGroupId
WHERE	1 = 1
  AND	@permission_group_index IS NOT NULL
  AND	cds.PermissionGroupId IS NULL
  AND	js.ProductCode IS NOT NULL
  AND	js.PriceCodeAnnual IS NOT NULL
  AND	js.PriceCodeMonthly IS NOT NULL