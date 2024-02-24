/*
	can i get the invoice record from sproc

set @invoice_id = (SELECT ReferenceId FROM USERINVOICE WHERE ExternalId = external_id);
    INSERT PAYMENTSESSION
    (
		`Id`, `UserId`, `InvoiceId`, `SessionType`, `SessionId`, `IntentId`, `ClientId`, `ExternalId`
    )
    SELECT
		idx `Id`, 
        user_id `UserId`, 
        @invoice_id `InvoiceId`, 
        session_type `SessionType`, 
        session_id `SessionId`, 
        intent_id `IntentId`, 
        client_id `ClientId`, 
        external_id `ExternalId`
        WHERE @invoice_id IS NOT NULL;
*/
SET @ext_id = '32CBC942AAED-WPIU-2B4A8291';
CALL USP_GET_PAYMENT_SESSION( @ext_id );
