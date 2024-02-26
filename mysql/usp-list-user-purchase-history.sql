/*
	USP_QUERY_USER_PURCHASE_HISTORY
*/
SET @user_index = 'cf35094a-ad64-41dd-9f2d-32cbc942aaed';
DROP TEMPORARY TABLE IF EXISTS tmp_user_purchases;
CREATE TEMPORARY TABLE tmp_user_purchases
SELECT 
	CONVERT(sr.Line USING UTF8MB4) LineJs,
	ui.PurchaseDate,
	ui.ReferenceId,
    ui.ExternalId,
    'SEARCH :' ItemType,
    ui.ItemCount,
    ( SELECT SUM( Price ) FROM USERINVOICE s  WHERE s.ReferenceId = ui.ReferenceId ) Price,
    CASE 
		WHEN hh.Id IS NULL THEN 'Purchased'
        WHEN hh.Id IS NOT NULL AND hh.PurchaseDate IS NOT NULL THEN 'Downloaded'
        ELSE 'Purchased' END StatusText
  FROM USERINVOICE ui
  LEFT
  JOIN USERDOWNLOADHISTORY hh
	ON ui.ReferenceId = hh.SearchId
  LEFT 
  JOIN SEARCHREQUEST sr
    ON ui.ReferenceId = sr.SearchId
  WHERE ui.PurchaseDate IS NOT NULL
    AND ui.LineId = 0
    AND ui.UserId = @user_index
  ORDER BY ui.CreateDate;
SELECT
    PurchaseDate, 
    ReferenceId, 
    ExternalId, 
    concat( ItemType, ' ', CountyName, ', ', StateName, ' from ', StartingDate, ' to ', EndingDate) ItemType,
    ItemCount, 
    Price, 
    StatusText
FROM
(  
SELECT
	UPPER( JSON_UNQUOTE(JSON_EXTRACT(LineJs, '$.state') ) ) StateName,
	UPPER( JSON_UNQUOTE(JSON_EXTRACT(LineJs, '$.county.name') ) ) CountyName,
    CAST( from_unixtime(floor( CAST(( JSON_UNQUOTE(JSON_EXTRACT(LineJs, '$.end') ) ) AS UNSIGNED) /1000)) AS DATE )EndingDate,
    CAST( from_unixtime(floor( CAST(( JSON_UNQUOTE(JSON_EXTRACT(LineJs, '$.start') ) ) AS UNSIGNED) /1000)) AS DATE ) StartingDate,
	t.*
FROM tmp_user_purchases t
) q;