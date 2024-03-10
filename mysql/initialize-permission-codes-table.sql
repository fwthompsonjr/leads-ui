/*
	As a db designer,
    I want to setup a list of products 
    So that a customer can subscribe to them
    1. initialize records in PERMISSIONGROUPCODES
		- this is a one-time script to allow api to communicate to product/price services

*/
SET @key_js = '{
	"product": {
		"name": "~0 Account",
		"decription": "~0 billing plan for record search."		
	},
	"priceamount": {
		"monthly": ~1,
		"annual": ~2
	}
}';
INSERT PERMISSIONGROUPCODES (
	PermissionGroupId, KeyName, ProductCode, PriceCodeAnnual, PriceCodeMonthly, KeyJs, IsActive
)
SELECT 
q.Id PermissionGroupId,
q.`Name` KeyName,
'' ProductCode, 
'' PriceCodeAnnual, 
'' PriceCodeMonthly,
	REPLACE( 
	REPLACE( 
	REPLACE( @key_js , 
	'~0', q.ShortName ), 
	'~1', q.PerMonth ), 
	'~2', q.PerYear ) KeyJs,
  FALSE IsActive
FROM (
	SELECT  
		pg.Id,
		pg.`Name`, 
		SUBSTRING_INDEX(pg.`Name`, ".", 1) ShortName, 
        GroupId,
        PerMonth,
        PerYear
	FROM defaultdb.PERMISSIONGROUP pg
    LEFT JOIN (
		SELECT 	PermissionGroupId
        FROM	PERMISSIONGROUPCODES
        WHERE	IsActive = FALSE
        GROUP
           BY 	PermissionGroupId
    ) cde
    ON	pg.Id = cde.PermissionGroupId
    WHERE `Name` LIKE '%.Pricing'
      AND GroupId >= 1000
      AND GroupId <  2000
      AND `PerMonth` > 0
      AND PerYear > 0
      AND cde.PermissionGroupId IS NULL ) q
ORDER BY q.GroupId;