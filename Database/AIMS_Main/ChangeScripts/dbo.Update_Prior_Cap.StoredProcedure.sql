IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Update_Prior_Cap]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].Update_Prior_Cap
GO

/****** Object:  StoredProcedure [dbo].[Update_Prior_Cap]    Script Date: 05/01/2013 14:06:37 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO

create procedure [dbo].[Update_Prior_Cap]

as

	--Truncate the Cap Monitoring Table 	
	truncate table .dbo.MONITORING_DAILY_CAP_CHANGE
	
	BEGIN TRAN T1
	insert into MONITORING_DAILY_CAP_CHANGE(security_id,curr_date,prior_cap, curr_cap, prior_diagram, curr_diagram)
	select pf.SECURITY_ID, getdate() as CURR_DATE, pf.AMOUNT as PRIOR_CAP, 0 as CURR_CAP, pf.CALCULATION_DIAGRAM as PRIOR_DIAGRAM, '' as CURR_DIAGRAM
		from .dbo.PERIOD_FINANCIALS pf
		where pf.DATA_ID = 185
		and pf.DATA_SOURCE = 'PRIMARY'
		and pf.CURRENCY = 'USD'
		and pf.PERIOD_TYPE = 'C'	
	COMMIT TRAN T1	  
	
GO
