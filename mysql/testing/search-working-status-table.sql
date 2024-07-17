drop temporary table if exists tmp_WORKINGSTATUS;

create temporary table tmp_WORKINGSTATUS (
	`Id` int not null,
    `Message` varchar(15) not null,
	PRIMARY KEY (`Id`)
);
insert tmp_WORKINGSTATUS ( `Id`, `Message` )
values
	( 0, 'begin' )
	,( 1, 'complete' )
	,( 2, 'failed' );

drop table if exists SEARCHWORKINGSTATUS;
create table SEARCHWORKINGSTATUS (
	`Id` int not null,
    `Message` varchar(15) not null,
	PRIMARY KEY (`Id`)
);

INSERT SEARCHWORKINGSTATUS
( `Id`, `Message` )
SELECT `Id`, `Message` FROM tmp_WORKINGSTATUS;

SELECT * FROM SEARCHWORKINGSTATUS;