SET @destinationAccount = 'legallead.live@mail.com';
SET @destinationId = (SELECT Id FROM defaultdb.USERS WHERE Email = @destinationAccount);

SELECT 
	p.*, m.KeyName
  FROM defaultdb.USERPROFILE p
  JOIN defaultdb.PROFILEMAP m
    ON p.ProfileMapId = m.Id
  WHERE p.UserId = @destinationId
  ORDER BY m.OrderId;