SET @externalIndex = 'PUFhs8AvOQmC6Orz';
SET @levelName = (
SELECT LevelName
  FROM defaultdb.LEVELREQUEST
  WHERE 'ERROR' <> InvoiceUri 
  AND ExternalId = @externalIndex
  ORDER BY CreateDate DESC);
SET @selector = CONCAT( @levelName, '.Pricing' );
DROP TEMPORARY TABLE IF EXISTS tp_permission_pricing;
DROP TEMPORARY TABLE IF EXISTS tp_permission_pricing_summary;
CREATE TEMPORARY TABLE tp_permission_pricing
  SELECT p.`Id`,
    p.`Name`,
    p.`GroupId`,
    p.`OrderId`,
    p.`PerRequest`,
    p.`PerMonth`,
    p.`PerYear`,
    p.`IsActive`,
    p.`IsVisible`,
    p.`CreateDate`,
    p.`ProductCode`
FROM `defaultdb`.`PERMISSIONGROUP` p
WHERE p.`Name` = @selector
LIMIT 1;
SET @priceMonth = CAST( (SELECT `PerMonth` FROM tp_permission_pricing) AS DECIMAL(9,4));
SET @priceYear = CAST( (SELECT `PerYear` FROM tp_permission_pricing) AS DECIMAL(9,4));
SET @taxAmount = CAST( '0.0825' AS DECIMAL(9,4));
SET @serviceFee = CAST( '0.0525' AS DECIMAL(9,4));
SET @priceMonthTax = @priceMonth * @taxAmount * 12;
SET @priceYearTax = @priceYear * @taxAmount;
CREATE TEMPORARY TABLE tp_permission_pricing_summary
SELECT 
	Id,
	UserId, 
    ExternalId, 
    LevelName,
    CompletionDate,
    CreateDate
  FROM defaultdb.LEVELREQUEST
  WHERE 'ERROR' <> InvoiceUri 
  AND ExternalId = @externalIndex
  ORDER BY CreateDate DESC;
  
  SELECT
   pmt.*,
   ( Price + TaxAmount + ServiceFee ) SubscriptionAmount
   FROM
   (
	  SELECT 
		s.Id,
		s.UserId, 
		s.ExternalId, 
		s.LevelName,
		fx.*
		FROM tp_permission_pricing_summary s
		CROSS
		JOIN (
			SELECT 'Monthly' PriceType, ROUND( @priceMonth, 2 ) Price, ROUND( @priceMonthTax, 2 ) TaxAmount, ROUND( (@priceMonthTax + @priceMonth) * @serviceFee , 2 ) ServiceFee
			UNION
			SELECT 'Yearly' PriceType, ROUND( @priceYear, 2 ) Price, ROUND( @priceYearTax, 2 ) TaxAmount, ROUND( (@priceYearTax + @priceYear) * @serviceFee , 2 ) ServiceFee
		) fx
	) pmt
