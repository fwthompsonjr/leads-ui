drop temporary table if exists tmp_WORKINGMESSAGE;

create temporary table tmp_WORKINGMESSAGE (
	`Id` int not null,
    `Message` varchar(50) not null,
	PRIMARY KEY (`Id`)
);
insert tmp_WORKINGMESSAGE ( `Id`, `Message` )
values
	( 0, 'process beginning' )
	,( 1, 'parameter evaluation' )
	,( 2, 'parameter conversion to search request' )
	,( 3, 'search request processing' )
	,( 4, 'excel content conversion' )
	,( 5, 'excel content serialization' )
	,( 6, 'process completed' );

drop table if exists SEARCHWORKINGMESSAGE;
create table SEARCHWORKINGMESSAGE (
	`Id` int not null,
    `Message` varchar(50) not null,
	PRIMARY KEY (`Id`)
);

INSERT SEARCHWORKINGMESSAGE
( `Id`, `Message` )
SELECT `Id`, `Message` FROM tmp_WORKINGMESSAGE;

SELECT * FROM SEARCHWORKINGMESSAGE;