/*
	for live account
    reset 
		account pword
        account level = guest
        delete any searches
        delete any user-subscriptions

*/
-- CALL `testing`.`PRC_CLONE_TEST_ACCOUNTS`();
SELECT *
  FROM defaultdb.PAYMENTCUSTOMER pc;