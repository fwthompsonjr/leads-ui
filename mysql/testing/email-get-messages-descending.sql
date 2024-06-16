/*
	email-get-message-list
    design-note
    api method is post
    require authorization header
    require body { "id": "MessageId",  "lastCreatedDate" : null }
    
*/

set @user_index = 'bcb2aebb-60ab-4eb2-aba9-11618ea8b3ea';
set @last_created_date = date_add( utc_timestamp(), INTERVAL -10 DAY );
drop temporary table if exists tmp_user_correspondence_filter;
drop temporary table if exists tmp_user_correspondence;

create temporary table tmp_user_correspondence
SELECT 
row_number() over ( order by CreateDate DESC) RowId,
 DATE_FORMAT(`CreateDate`, '%Y-%m-%d %H:%i') CreateDt,
 c.*
 FROM CORRESPONDENCE c
 WHERE UserId = @user_index
 ORDER BY CreateDate DESC;
 
 create temporary table tmp_user_correspondence_filter
	SELECT max( RowId ) RowId, CreateDt, `Subject`
    FROM tmp_user_correspondence
    GROUP BY CreateDt, `Subject`;
    
SELECT 
	uc.Id, 
    uc.UserId, 
    uc.FromAddress, 
    uc.ToAddress, 
    uc.`Subject`, 
    uc.StatusId, 
    uc.CreateDate
  FROM tmp_user_correspondence uc
  JOIN tmp_user_correspondence_filter ucf 
    ON uc.RowId = ucf.RowId
  WHERE uc.CreateDate >= CASE WHEN @last_created_date IS NULL THEN uc.CreateDate ELSE @last_created_date END;