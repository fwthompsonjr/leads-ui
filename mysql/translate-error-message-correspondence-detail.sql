SET @js = '
[
"Vel vel incidunt ea alias voluptatibus. |Enim architecto quisquam. |Sint explicabo nulla et dolores voluptatem. |Ducimus deserunt ipsam quibusdam",
"quisquam fugit libero architecto adipisci. |Dolorem ad illo maiores. |In facilis doloremque fugit est libero ratione fuga sequi. |Est facere qui ipsam",
"deserunt totam accusamus quasi sint. |Maiores culpa est molestiae. |Ea veniam optio id et reiciendis dolor temporibus atque facere. |Quo sit iste ratione eos",
"aliquam."
]';
SET @indx = '8a187519-d82d-4fb0-9e10-31b71f3404cc';

UPDATE wlogpermissions.CORRESPONDENCE
SET
	StatusId = 1
WHERE Id = @indx;

INSERT wlogpermissions.CORRESPONDENCEDETAIL
(
	CorrespondenceId, LineId, Line
)
SELECT src.CorrespondenceId, src.LineId, src.`Line`
FROM
(
	SELECT @indx CorrespondenceId,
    0 LineId,
    'Error occurred sending email' `Line`
	UNION SELECT 
	@indx CorrespondenceId,
	j.LineId,
    j.Line
	FROM JSON_TABLE( @js, '$[*]' COLUMNS ( 
	`LineId` FOR ORDINALITY,
	`Line` VARCHAR(500) PATH '$' ) ) j
) src
INNER JOIN ( SELECT Id FROM wlogpermissions.CORRESPONDENCE WHERE Id = @indx LIMIT 1) c
ON src.CorrespondenceId = c.Id
ORDER BY src.LineId;