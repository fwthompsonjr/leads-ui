SET @nw = utc_timestamp();
SET @accountName = 'legallead.live@mail.com';
SET @externalAuth = 'msLgDapAtA3NZjx';
SET @internalAuth = '010-msLgDapAtA3NYjx-212';

INSERT INTO ACCOUNTS
(
	`Name`, Authorization, InternalId, CreateDate
)
SELECT 
	@accountName `Name`,
    @externalAuth `Authorization`,
    @internalAuth InternalId,
    @nw CreateDate
WHERE NOT EXISTS( SELECT 1 FROM ACCOUNTS WHERE `Name` = @accountName);