use testing;

DROP TABLE IF EXISTS RANDOMEMAIL;

CREATE TABLE RANDOMEMAIL (
	`Id` int NOT NULL primary KEY,
    `EmailAddress` varchar(75)
);

SET @js ='[
"yug_eweyato9@outlook.com",
"caxitu_diwi11@hotmail.com",
"hakuvo-fodo89@hotmail.com",
"xewe-gohari12@gmail.com",
"gefepup-ubo90@gmail.com",
"lifamin_eca37@outlook.com",
"juyimaj-uni81@yahoo.com",
"tiwogag_oru40@mail.com",
"zelaf-aveco33@mail.com",
"mayupi-gafi56@gmail.com",
"curobaz-uya91@aol.com",
"giwice_huri21@aol.com",
"nopoc_umufi76@outlook.com",
"zice-rarehu71@hotmail.com",
"tefo-codilo76@yahoo.com",
"zezuvu_dema50@mail.com",
"xuy-enegiwu29@yahoo.com",
"paz_itocudo89@mail.com",
"kijod_usiru18@hotmail.com",
"wakor-aberu66@yahoo.com",
"vuvewec_iti67@yahoo.com",
"tesiz_exuvi9@outlook.com",
"kucuy_obaga91@mail.com",
"heva_rowevo59@outlook.com",
"girafa-yepi91@outlook.com",
"kig_edulira64@yahoo.com",
"pugimo_noti81@mail.com",
"futa-biyupu52@outlook.com",
"heyisej-ofi82@gmail.com",
"feh-uyigedi74@yahoo.com",
"coca_xawilo67@outlook.com",
"nohirab-uja77@outlook.com",
"kegap-edoki76@gmail.com",
"lor_uniweni49@yahoo.com",
"gefuf_ucojo45@gmail.com",
"kiz-iluwewu18@hotmail.com",
"cuyi-zicuja82@gmail.com",
"gakatu_doro94@aol.com",
"yekuh-osese20@gmail.com",
"satew_upova72@hotmail.com",
"sutuyaf_ena25@outlook.com",
"kewe-saduju12@gmail.com",
"movofu-tavo89@outlook.com",
"cejed_ugugo87@mail.com",
"gey_ixukuxi69@hotmail.com",
"yedisus_eyo51@outlook.com",
"wakali_hixe69@aol.com",
"yujilew-ohi50@outlook.com",
"loyafac-ezi54@hotmail.com",
"daxojir_ocu41@yahoo.com",
"yunawi-noyi11@gmail.com",
"fixe-pazila60@yahoo.com",
"yap_oyayiyu88@yahoo.com",
"radum_ojoni14@mail.com",
"vukog-okexe53@aol.com",
"fojuxu-lozu73@outlook.com",
"wer_ubuhewo44@mail.com",
"foyo-tevace60@outlook.com",
"deh_ufidiwa6@aol.com",
"yoyam_ejajo82@aol.com",
"fesunu-vosu89@gmail.com",
"ribocek-oka46@hotmail.com",
"bofoce-mexa79@aol.com",
"pohula-hado71@aol.com",
"tek-eyaxajo85@aol.com",
"xuhefa-naro79@yahoo.com",
"kiyefec_awe54@mail.com",
"xiye_tugupu81@hotmail.com",
"cone_wetopo48@outlook.com",
"jelovav-ifu99@aol.com",
"vaweb-usegu55@gmail.com",
"dop_iyozepi65@aol.com",
"gejecac-aji55@aol.com",
"ziyolim_elo17@outlook.com",
"kul-imozuni18@outlook.com"
]';

INSERT RANDOMEMAIL ( Id, EmailAddress )
SELECT j.Id, j.`Email`
  FROM json_table(@js, 
  '$[*]' COLUMNS( 
	Id For ordinality,
	`Email` VARCHAR(75) PATH '$' ) ) j
  LEFT JOIN RANDOMEMAIL r
  ON j.`Email` = r.EmailAddress
WHERE r.Id IS NULL;