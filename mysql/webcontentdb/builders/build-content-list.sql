-- 1000 = @{ Name = "HOME.PRE.LOGIN.000.SHELL" }

SELECT concat( '[string]::Concat( "', InternalId, '", ",", "', ContentName, '" ), ') Line
  FROM CONTENT
  WHERE IsActive = true
  GROUP BY InternalId, ContentName
  ORDER BY InternalId;