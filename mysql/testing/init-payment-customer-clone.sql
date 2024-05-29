/*
	test account should have stripe association
*/

DROP TEMPORARY TABLE IF EXISTS tmp_payment_user_cloned;
CREATE TEMPORARY TABLE tmp_payment_user_cloned
	SELECT u.*
	  FROM defaultdb.PAYMENTCUSTOMER u
	  LEFT
	  JOIN testing.PAYMENTCUSTOMERCLONE s
		ON u.Id = s.Id
	  JOIN testing.ACCOUNTS a
		ON u.Email = a.`Name`
	WHERE s.Id is null;

	INSERT testing.PAYMENTCUSTOMERCLONE (
		Id, UserId, CustomerId, Email, IsTest, CreateDate
	)
	SELECT
	Id, UserId, CustomerId, Email, IsTest, CreateDate
	FROM tmp_payment_user_cloned;
    
    DROP TEMPORARY TABLE IF EXISTS tmp_payment_user_cloned;