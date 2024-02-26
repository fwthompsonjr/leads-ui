/* let's add time-zome to user profile */
INSERT PROFILEMAP (  Id, OrderId, KeyName )
	SELECT 
    'bd86e443-7ce4-450f-aca5-aba38c701097' Id,
    17 OrderId,
    'Time Zone' KeyName
    WHERE NOT EXISTS( SELECT 1 FROM PROFILEMAP WHERE KeyName = 'Time Zone');
    
SELECT * 
FROM defaultdb.PROFILEMAP
ORDER BY OrderId;