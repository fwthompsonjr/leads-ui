SET @court_name = 'Probate Court #2';
SET @county_index = 26730;
SET @internal_index = 1;
SET @court_address = '3900 Morse St #100, Denton, TX 76208';
INSERT aspnettds.mds_court (
	StateId, CountyId, InternalId, `Name`
)
	SELECT 
		'TX',
        @county_index,
        @internal_index,
        @court_name
        WHERE NOT EXISTS( SELECT 1 FROM aspnettds.mds_court WHERE `Name` = @court_name AND CountyId = @county_index);
    

SET @mxindx = (SELECT Id
	FROM aspnettds.mds_court
    WHERE `Name` = @court_name AND CountyId = @county_index );

INSERT aspnettds.mds_court_address (
	CourtId, `Name`, FullName, Address
)
SELECT
	@mxindx,
    @court_name,
    @court_name,
	@court_address
 WHERE NOT EXISTS ( SELECT 1 FROM aspnettds.mds_court_address WHERE FullName = @court_name);