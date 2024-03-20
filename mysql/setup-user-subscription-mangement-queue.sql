/*
	in order to keep subscription data in-sync with remote subscription manager
    a view contains a list of subsciptions that have been provisioned
    that view definition can be used to populate a staging table
    so that a queue can fetch subscription details and make modifications
    as needed to keep the remote subscription manager in sync with real time
*/

SELECT 
	FALSE IsSubscriptionVerified,
	uq.Id, uq.UserId, uq.ExternalId, uq.InvoiceUri, 
	uq.LevelName, uq.SessionId, uq.IsPaymentSuccess, uq.CompletionDate, uq.CreateDate
FROM (
SELECT 
	'account-permissions' SubscriptionType,
	lr.Id, lr.UserId, lr.ExternalId, lr.InvoiceUri, 
    CAST( lr.LevelName  AS CHAR(2000)) LevelName, lr.SessionId, lr.IsPaymentSuccess, lr.CompletionDate, lr.CreateDate
FROM LEVELREQUEST lr
INNER
JOIN (
	SELECT UserId, max( CompletionDate ) CompletionDate
	FROM LEVELREQUEST
	WHERE IsPaymentSuccess = TRUE
	GROUP BY UserId ) lrmax
ON	lr.UserId = lrmax.UserId
AND lr.CompletionDate = lrmax.CompletionDate
UNION
SELECT 
	'discount' SubscriptionType,
	dr.Id, dr.UserId, dr.ExternalId, dr.InvoiceUri, dr.DiscountJs LevelName, dr.SessionId, dr.IsPaymentSuccess, dr.CompletionDate, dr.CreateDate
FROM DISCOUNTREQUEST dr
INNER
JOIN (
	SELECT UserId, max( CompletionDate ) CompletionDate
	FROM DISCOUNTREQUEST
	WHERE IsPaymentSuccess = TRUE
	GROUP BY UserId ) drmax
ON	dr.UserId = drmax.UserId
AND dr.CompletionDate = drmax.CompletionDate
) uq
ORDER BY UserId, SubscriptionType
;