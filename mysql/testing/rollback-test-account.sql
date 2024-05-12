SET @accountEmail = (SELECT `Name` FROM testing.ACCOUNTS WHERE `Name` LIKE '%testing%' LIMIT 1);
SET @user_index = (SELECT Id FROM defaultdb.USERS WHERE Email = @accountEmail);
-- START TRANSACTION;
SET SQL_SAFE_UPDATES = 0;

DELETE FROM defaultdb.USERTOKENS WHERE UserId = @user_index;
DELETE FROM defaultdb.USERSUBSCRIPTION WHERE UserId = @user_index;
DELETE FROM defaultdb.USERPROFILEHISTORY WHERE UserId = @user_index;
DELETE FROM defaultdb.USERPROFILECHANGE WHERE UserId = @user_index;
DELETE FROM defaultdb.USERPERMISSIONHISTORY WHERE UserId = @user_index;
DELETE FROM defaultdb.USERPERMISSIONCHANGE WHERE UserId = @user_index;
DELETE FROM defaultdb.USERPERMISSION WHERE UserId = @user_index;
DELETE FROM defaultdb.USERLOCKHISTORY WHERE UserId = @user_index;
DELETE FROM defaultdb.USERINVOICE WHERE UserId = @user_index;
DELETE  D
FROM defaultdb.USERDOWNLOAD D
JOIN defaultdb.USERDOWNLOADHISTORY H
ON D.DownloadId = H.Id
WHERE H.UserId = @user_index;
DELETE FROM defaultdb.USERDOWNLOADHISTORY WHERE UserId = @user_index;
DELETE FROM defaultdb.PAYMENTCUSTOMER WHERE UserId = @user_index;
DELETE FROM defaultdb.LEVELREQUEST WHERE UserId = @user_index;
DELETE FROM defaultdb.DISCOUNTREQUEST WHERE UserId = @user_index;
DELETE FROM defaultdb.USERS WHERE Id = @user_index;