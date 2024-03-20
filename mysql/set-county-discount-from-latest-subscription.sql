-- synchronize county account discount
drop temporary table if exists tmp_discounted_user;
drop temporary table if exists tmp_discounted_user_indx;
create temporary table tmp_discounted_user
SELECT 
	ROW_NUMBER() OVER (ORDER BY UserId, CreateDate) RowIndex,
    dr.*
  FROM defaultdb.DISCOUNTREQUEST dr
  WHERE dr.IsPaymentSuccess = 1
  AND	dr.UserId = user_index
  ORDER BY UserId, CreateDate desc;
create temporary table tmp_discounted_user_indx
SELECT MAX( RowIndex ) MnIndex, UserId
FROM tmp_discounted_user
GROUP BY UserId;

SET @user_id = (SELECT UserId
	FROM tmp_discounted_user_indx ORDER BY UserId LIMIT 1);
SET @rw_idx = (SELECT MnIndex
	FROM tmp_discounted_user_indx WHERE UserId = @user_id LIMIT 1);
SET @jsdata = (SELECT tu.DiscountJs
	FROM tmp_discounted_user tu
    WHERE RowIndex = @rw_idx
    LIMIT 1);
SET @jsdata = CAST( REPLACE( @jsdata, '\\', '' ) AS JSON);

SET @county_names =(
SELECT GROUP_CONCAT(uc.Id ORDER BY uc.Id ASC SEPARATOR ', ') `Names`
FROM JSON_TABLE(@jsdata, '$.Choices[*]' 
    COLUMNS(
      StateName VARCHAR(5) PATH '$.StateName',      
      CountyName VARCHAR(75) PATH '$.CountyName',      
      IsSelected boolean PATH '$.IsSelected'
	)
) jt
INNER JOIN  aspnettds.uscounty uc
ON  jt.CountyName = uc.`Name`
AND jt.StateName = uc.StateId
WHERE jt.IsSelected = TRUE);

UPDATE 	defaultdb.USERPERMISSION p
JOIN 	defaultdb.PERMISSIONMAP mp
ON 		p.PermissionMapId = mp.Id
SET		p.KeyValue = IFNULL(@county_names, '')
WHERE	p.UserId = @user_id
AND		p.Id != ''
AND 	mp.KeyName = 'Setting.State.County.Subscriptions'
AND		p.KeyValue <> IFNULL(@county_names, '');


drop temporary table if exists tmp_discounted_user;
drop temporary table if exists tmp_discounted_user_indx;