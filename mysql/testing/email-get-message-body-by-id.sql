/*
	email-get-message-body-by-id
    design-note
    api method is post
    require authorization header
    require body { "Id": "MessageId" }
*/
set @message_id = '5a5ed892-2218-11ef-99ce-0af7a01f52e9';
set @user_id = 'bcb2aebb-60ab-4eb2-aba9-11618ea8b3ea';

SELECT 
	uc.Id, 
    uc.`Body`
    FROM wlogpermissions.CORRESPONDENCE uc
    WHERE uc.Id = @message_id
      AND uc.UserId = @user_id;