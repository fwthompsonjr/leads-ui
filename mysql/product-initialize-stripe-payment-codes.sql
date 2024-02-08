SET @base_name = "~0 Record Search";
SET @base_desc = "~0: Execution a date bound search against a legal lead county repository.";

DROP TEMPORARY TABLE IF EXISTS tb_pricing_map;
CREATE TEMPORARY TABLE tb_pricing_map AS
SELECT
 a.Id,
 a.`Name`,
 REPLACE( @base_name, "~0", NamePrefix ) PricingName,
 REPLACE( @base_desc, "~0", UPPER( NamePrefix ) ) `Description`,
 CAST( (0.01 * PerRequest) as DECIMAL(5,2) ) Price,
 ProductCode,
 CASE 
	WHEN a.`Name` = 'Admin.Pricing' 	THEN 'prod_PWhqrj7DuB24bb'
	WHEN a.`Name` = 'Gold.Pricing' 		THEN 'prod_PWhzUEFu5AiRgc'
	WHEN a.`Name` = 'Guest.Pricing' 	THEN 'prod_PWi04u9GUFKbwh'
	WHEN a.`Name` = 'Platinum.Pricing' 	THEN 'prod_PWi0sVZnnKFXjg'
	WHEN a.`Name` = 'Silver.Pricing' 	THEN 'prod_PWi205m7MO8geU'
    ELSE NULL END StripeCode
 FROM
 (
	SELECT 
		Id,
		`Name`,
		SUBSTRING_INDEX( `Name`, ".", 1) NamePrefix,
		PerRequest,
        ProductCode
	  FROM  PERMISSIONGROUP g
	  WHERE 1 = 1
		AND	`Name` LIKE '%.Pricing'
		AND	`Name` NOT LIKE 'None%'
		AND	`Name` NOT LIKE 'State%'
		AND	`Name` NOT LIKE 'County%'
 ) a
ORDER BY `Name`;

SELECT *
  FROM tb_pricing_map;
  
UPDATE PERMISSIONGROUP PG JOIN tb_pricing_map M ON PG.Id = M.Id
SET PG.ProductCode = M.StripeCode
WHERE PG.ProductCode IS NULL;

