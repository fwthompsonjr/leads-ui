/*
	for any items where a zero invoice exists
    there must be a session record to allow download
*/

CALL USP_CREATE_SESSION_FOR_ZERO_INVOICES();
/*


SET @bkslash = "\\";
SET @bsfalse = '"base64:type16:AA=="';
SET @ui_index = '33ccbabf-cb59-11ee-be26-0af7a01f52e9';
CALL PRC__GET_INVOICE_JSON_SUMMARY( @ui_index, @js_data );

SELECT 
	REPLACE(
	REPLACE(
	REPLACE(
	REPLACE( @js_data 
		, @bkslash, "" ),
        '"[', "[" ),
        ']"', "]" ),
        @bsfalse, "false" )  js, 
    @bkslash Slash;
    
SET @blank_type = 'none';
SET @refernce_index = '558dcb84-c3a7-11ee-be26-0af7a01f52e9';

drop temporary table if exists tmp_zero_invoice;
create temporary table tmp_zero_invoice
SELECT *
  FROM USERINVOICE
  WHERE LineId = 0
    AND PurchaseDate IS NOT NULL
    AND Price = 0
    AND Id = @ui_index;
SET @jdata = (    
SELECT JSON_ARRAYAGG(JSON_OBJECT(
'LineId', ui.LineId, 
'UserId', ui.UserId,
'ItemType', ui.ItemType,
'ItemCount', ui.ItemCount,
'UnitPrice', ui.UnitPrice,
'Price', ui.Price,
'ui.ReferenceId', ui.ReferenceId,
'ExternalId', ui.ExternalId,
'PurchaseDate', ui.PurchaseDate,
'IsDeleted', ui.IsDeleted,
'CreateDate', ui.CreateDate)) 
  FROM USERINVOICE ui
  JOIN tmp_zero_invoice t
    ON ( t.ExternalId = ui.ExternalId OR t.ReferenceId = ui.ReferenceId));

SET @search_index = (SELECT ReferenceId FROM tmp_zero_invoice LIMIT 1);
SET @external_index = (SELECT ExternalId FROM tmp_zero_invoice LIMIT 1);
SET @success_fmt = REPLACE("{0}/payment-result?sts=success&id=~1", '~1', @external_index);
CALL PRC__GET_INVOICE_DESCRIPTION( @search_index, @inv_description);
SET @jso =
-- REPLACE(somecolumn, '\"', '"')
JSON_OBJECT( 'Data', @jdata,
	'SuccessUrl', @success_fmt,
    'ExternalId',  @external_index,
    'Description', @inv_description );
SELECT 
	REPLACE(
    CONVERT( @jso USING utf8mb4 ), '\"', '"') js;
    */