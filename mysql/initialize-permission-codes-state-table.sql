/*
	As a db designer,
    I want to setup a list of products 
    So that a customer can subscribe to them
    1. initialize records in PERMISSIONGROUPCODES
		- this is a one-time script to allow api to communicate to product/price services

*/
SET @key_js = '{
	"product": {
		"name": "~0 Discount",
		"description": "~3 discount program for record searches"		
	},
	"priceamount": {
		"monthly": ~1,
		"annual": ~2
	}
}';

DROP TEMPORARY TABLE IF EXISTS tmp_state_pricing;
DROP TEMPORARY TABLE IF EXISTS tmp_actv_states;
CREATE TEMPORARY TABLE tmp_actv_states
SELECT 
	c.*,
    CONCAT( LEFT( c.Id, 1 ), LOWER( RIGHT( c.Id, 1 ) ) ) ProperCaseState
  FROM aspnettds.usstate c
  WHERE c.Actv = 1;

CREATE TEMPORARY TABLE tmp_state_pricing
SELECT 
GroupId,
q.Id PermissionGroupId,
q.`Name` KeyName,
'' ProductCode, 
'' PriceCodeAnnual, 
'' PriceCodeMonthly,
	REPLACE( 
	REPLACE( @key_js , 
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
    WHERE `Name` = 'State.Discount.Pricing'
      AND GroupId >= 2000
      AND GroupId <  3000
      AND `PerMonth` > 0
      AND PerYear > 0
      AND cde.PermissionGroupId IS NULL ) q
ORDER BY q.GroupId;

INSERT PERMISSIONGROUPCODES (
	PermissionGroupId, KeyName, ProductCode, PriceCodeAnnual, PriceCodeMonthly, KeyJs, IsActive
)
SELECT 
	PermissionGroupId,
    CONCAT( p.KeyName, ".", tc.ProperCaseState) KeyName,
    ProductCode,
    PriceCodeAnnual,
    PriceCodeMonthly,
    REPLACE(
    REPLACE( KeyJs, 
		"~0", CONCAT( tc.`Name`, ' - State - ', Id ) ),
        "~3", CONCAT( tc.`Name`, ' state' ) ) KeyJs,
	IsActive
FROM tmp_actv_states tc
CROSS JOIN tmp_state_pricing p;