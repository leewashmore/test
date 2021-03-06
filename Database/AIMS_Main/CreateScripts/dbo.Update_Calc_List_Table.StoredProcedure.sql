--ADD calc 296
delete from .dbo.CALC_LIST where CALC_NUM = 296
insert into .dbo.calc_list(CALC_NUM,CALC_SEQ,CALC_NAME,ACTIVE,PRICEBASED)
values(296, 1, 'Trailing Earnings', 'Y' , 'Y')

delete from .dbo.DATA_MASTER where DATA_ID = 296
insert into .dbo.DATA_MASTER
values (296,'','NONE','N','Y','Y','Y','Y','N','Trailing Earnings','','N','Y','','','Y',NULL,NULL)

--ADD calc 297
delete from .dbo.CALC_LIST where CALC_NUM = 297
insert into .dbo.calc_list(CALC_NUM,CALC_SEQ,CALC_NAME,ACTIVE,PRICEBASED)
values(297, 1, 'Trailing Dividends', 'Y' , 'Y')

delete from .dbo.DATA_MASTER where DATA_ID = 297
insert into .dbo.DATA_MASTER
values (297,'','NONE','N','Y','Y','Y','Y','N','Trailing Dividends','','N','Y','','','Y',NULL,NULL)

--ADD calc 298
delete from .dbo.CALC_LIST where CALC_NUM = 298
insert into .dbo.calc_list(CALC_NUM,CALC_SEQ,CALC_NAME,ACTIVE,PRICEBASED)
values(298, 1, 'Trailing Book Value', 'Y' , 'Y')

delete from .dbo.DATA_MASTER where DATA_ID = 298
insert into .dbo.DATA_MASTER
values (298,'','NONE','N','Y','Y','Y','Y','N','Trailing Equity','','N','Y','','','Y',NULL,NULL)

--ADD calc 299
delete from .dbo.CALC_LIST where CALC_NUM = 299
insert into .dbo.calc_list(CALC_NUM,CALC_SEQ,CALC_NAME,ACTIVE,PRICEBASED)
values(299, 2, 'Trailing Dividend Yield', 'Y' , 'Y')

delete from .dbo.DATA_MASTER where DATA_ID = 299
insert into .dbo.DATA_MASTER
values (299,'','NONE','N','Y','Y','Y','Y','N','Trailing Dividend Yield','','N','Y','','','Y',NULL,NULL)

--ADD calc 300
delete from .dbo.CALC_LIST where CALC_NUM = 300
insert into .dbo.calc_list(CALC_NUM,CALC_SEQ,CALC_NAME,ACTIVE,PRICEBASED)
values(300, 1, 'Forward Dividends', 'Y' , 'Y')

delete from .dbo.DATA_MASTER where DATA_ID = 300
insert into .dbo.DATA_MASTER
values (300,'','NONE','N','Y','Y','Y','Y','N','Forward Dividends','','N','Y','','','Y',NULL,NULL)


--Change calc_seq for 207/209/212 (Trailing P/E, Trailing P/BV and Trailing ROE)
update .dbo.CALC_LIST
set CALC_SEQ = 2 where CALC_NUM in (207,209,212)

