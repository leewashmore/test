SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
alter procedure [dbo].[AIMS_Calc_236](
	@ISSUER_ID			varchar(20) = NULL			-- The company identifier		
,	@CALC_LOG			char		= 'Y'			-- write calculation errors to the log table
)
as
	
	-- Get the data
	select distinct pf.* 
	  into #A
	  from dbo.PERIOD_FINANCIALS pf  with (nolock)
	 inner join dbo.GF_SECURITY_BASEVIEW sb on sb.SECURITY_ID = pf.SECURITY_ID
	 where DATA_ID = 185			--Market Capitalization
	   and sb.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'C'
	   
 ---- Total amount for all the fiscal quarters within an year --- 

	-- Calculate the percentage of the amount to use.
	declare @PERCENTAGE decimal(32,6)
	declare @PERIOD_END_DATE datetime
	declare @PERIOD_YEAR integer
	select @PERCENTAGE = cast(datediff(day, getdate(), MIN(period_end_date)) as decimal(32,6)) / 365.0
	   ,   @PERIOD_END_DATE = MIN(period_end_date)
	  from PERIOD_FINANCIALS with (nolock)
	 where ISSUER_ID = @ISSUER_ID
	   and DATA_ID = 124
	   and PERIOD_END_DATE > GETDATE()
	   and PERIOD_TYPE = 'A'

	select @PERIOD_YEAR = PERIOD_YEAR
	  from PERIOD_FINANCIALS with (nolock)
	 where ISSUER_ID = @ISSUER_ID
	   and DATA_ID = 124
	   and PERIOD_END_DATE = @PERIOD_END_DATE
	   and PERIOD_TYPE = 'A'
   
	select distinct ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE
		  , (AMOUNT * @PERCENTAGE)*-1 as AMOUNT, -1*AMOUNT as AMOUNT100
	  into #B
	  from dbo.PERIOD_FINANCIALS pf  with (nolock)
	 where DATA_ID = 124		-- Dividends
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'
	   and FISCAL_TYPE = 'FISCAL'
	   and pf.PERIOD_END_DATE = @PERIOD_END_DATE
	
	select distinct ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE
		  , (AMOUNT * (1-@PERCENTAGE))*-1 as AMOUNT
	  into #C
	  from dbo.PERIOD_FINANCIALS pf  with (nolock)
	 where DATA_ID = 124		-- Dividends
	   and pf.ISSUER_ID = @ISSUER_ID
	   and pf.PERIOD_TYPE = 'A'
	   and FISCAL_TYPE = 'FISCAL'
	   and pf.PERIOD_YEAR = @PERIOD_YEAR + 1

	-- Add the data to the table
	-- When dealing with 'C'urrent PERIOD_TYPE there should be only one value... the current one.  
	-- No PERIOD_YEAR not PERIOD_END_DATE is needed.
	BEGIN TRAN T1
	insert into PERIOD_FINANCIALS(ISSUER_ID, SECURITY_ID, COA_TYPE, DATA_SOURCE, ROOT_SOURCE
		  , ROOT_SOURCE_DATE, PERIOD_TYPE, PERIOD_YEAR, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY
		  , DATA_ID, AMOUNT, CALCULATION_DIAGRAM, SOURCE_CURRENCY, AMOUNT_TYPE)
	select distinct a.ISSUER_ID, a.SECURITY_ID, a.COA_TYPE, a.DATA_SOURCE, a.ROOT_SOURCE
		,  a.ROOT_SOURCE_DATE, 'C', 0, '01/01/1900'				-- These are specific for PERIOD_TYPE = 'C'
		,  a.FISCAL_TYPE, a.CURRENCY
		,  236 as DATA_ID										-- DATA_ID:236 Forward Dividend Yield
		,  case when c.AMOUNT is NULL then (-1*b.AMOUNT100) else -1*(b.AMOUNT +c.AMOUNT) end /a.AMOUNT as AMOUNT	 					--  Sum of next 4 quarters FCDP**/Current Market Cap* 
		,  case when c.AMOUNT is NULL then  'Dividends(' + CAST(-1*b.AMOUNT100 as varchar(32)) + ') / Mktcap(' + CAST(a.AMOUNT as varchar(32)) + ')' 
				else 'Dividends(' + CAST(-1*b.AMOUNT as varchar(32)) + ' + ' +CAST(-1*c.AMOUNT as varchar(32)) + ') / Mktcap(' + CAST(a.AMOUNT as varchar(32)) + ')' end as CALCULATION_DIAGRAM
		,  a.SOURCE_CURRENCY
		,  a.AMOUNT_TYPE
	  from #A a
	 inner join dbo.GF_SECURITY_BASEVIEW sb on sb.SECURITY_ID = a.SECURITY_ID
	 inner join	#B b on b.ISSUER_ID = sb.ISSUER_ID 					
					and b.CURRENCY = a.CURRENCY
					and b.DATA_SOURCE = a.DATA_SOURCE
	  left join	#C c on c.ISSUER_ID = sb.ISSUER_ID 					
					and c.CURRENCY = a.CURRENCY
					and c.DATA_SOURCE = a.DATA_SOURCE
	 where 1=1 	  
	   and isnull(b.AMOUNT, 0.0) <> 0.0	-- Data validation
--	   and isnull(c.AMOUNT, 0.0) <> 0.0	-- Data validation
	-- order by a.ISSUER_ID, a.COA_TYPE, a.DATA_SOURCE, a.PERIOD_TYPE, a.PERIOD_YEAR,  a.FISCAL_TYPE, a.CURRENCY
	COMMIT TRAN T1

	
	if @CALC_LOG = 'Y'
		BEGIN
			-- Error conditions - NULL or Zero data 
			insert into CALC_LOG( LOG_DATE, DATA_ID, ISSUER_ID, PERIOD_TYPE, PERIOD_YEAR
							, PERIOD_END_DATE, FISCAL_TYPE, CURRENCY, TXT )
			(
			select GETDATE() as LOG_DATE, 236 as DATA_ID, a.ISSUER_ID, 'C'
				,  0, '01/01/1900', a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 236 Forward Dividend Yield. DATA_ID:124 is NULL or ZERO'
			  from #B a
			 where isnull(a.AMOUNT, 0.0) = 0.0	 -- Data error	 
			) union (	
			
			-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 236 as DATA_ID, sb.ISSUER_ID, 'C'
				,  0, '01/01/1900', a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 236 Forward Dividend Yield .  DATA_ID:185 is missing' as TXT
			  from #B a
			 inner join dbo.GF_SECURITY_BASEVIEW sb on sb.ISSUER_ID = a.ISSUER_ID
			 left join #A b on  b.SECURITY_ID = sb.SECURITY_ID
							and b.CURRENCY = a.CURRENCY
							and b.DATA_SOURCE = a.DATA_SOURCE
			 where 1=1 and b.ISSUER_ID is NULL	  
			) union (	

			-- Error conditions - missing data 
			select GETDATE() as LOG_DATE, 236 as DATA_ID, sb.ISSUER_ID, 'C'
				,  0, '01/01/1900', a.FISCAL_TYPE, a.CURRENCY
				, 'ERROR calculating 236 Forward Dividend Yield .  DATA_ID:124 is missing' as TXT
			  from #A a
			 inner join dbo.GF_SECURITY_BASEVIEW sb on sb.SECURITY_ID = a.SECURITY_ID
			 left join #B b on b.ISSUER_ID = sb.ISSUER_ID 					
							and b.CURRENCY = a.CURRENCY
							and b.DATA_SOURCE = a.DATA_SOURCE
			 where 1=1 and b.ISSUER_ID is NULL	  
			) union (	
			
				-- ERROR - No data at all available
			select GETDATE() as LOG_DATE, 236 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 236 Forward Dividend Yield .  DATA_ID:185 no data' as TXT
			  from (select COUNT(*) CNT from #A having COUNT(*) = 0) z
			) union (	

			select GETDATE() as LOG_DATE, 236 as DATA_ID, isnull(@ISSUER_ID, ' ') as ISSUER_ID, ' ' as PERIOD_TYPE
				,  0 as PERIOD_YEAR,  '1/1/1900' as PERIOD_END_DATE,  ' ' as FISCAL_TYPE,  ' ' as CURRENCY
				, 'ERROR calculating 236 Forward Dividend Yield.  DATA_ID:124 No data or missing quarters' as TXT
			  from (select COUNT(*) CNT from #B having COUNT(*) = 0) z
			)
		END


	-- Clean up
	drop table #A
	drop table #B
	drop table #C
GO
