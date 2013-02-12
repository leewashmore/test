/****** Object:  StoredProcedure [dbo].[SET_COA_TYPE]    Script Date: 02/11/2013 17:25:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--------------------------------------------------------------------------------------
-- Name:	SET_COA_TYPE
--
-- Purpose:	Makes sure there is a record in INTERNAL_ISSUER table for this issuer.  The table
--          is used by GET_DATA to determine what type of company the issuer is:  IND (Industrial),
--			BNK (Bank),  FIN (Insurance) or UTL (Utility).  The hierarchy is to use REUTERS designation
--			first, if it exists overwriting current value when it does.  If it does not, uses the GICS codes
--			in GF_SECURITY_BASEVIEW to set the template.
--
--------------------------------------------------------------------------------------
Alter procedure [dbo].[SET_COA_TYPE](
	@ISSUER_ID varchar(20) = NULL 
)
as

	declare @ReutersCOA varchar(3) = NULL
	declare @GICSIndustry nvarchar(255) = NULL
	declare @GICSCOA varchar(3) = NULL
	declare @Update integer

	--update internal_issuer table with REUTERS COA Type
	select @GICSIndustry = MAX(sb.GICS_INDUSTRY)
		from dbo.GF_SECURITY_BASEVIEW sb
		where sb.ISSUER_ID = @ISSUER_ID
		 group by sb.ISSUER_ID

	select top 1 case when @GICSIndustry in ('401010','401020','402010','402020','402030') then 'BNK' 
				when @GICSIndustry = '403010' then 'FIN'
				when @GICSIndustry in ('551010','551020','551030','551040','551050') then 'UTL'
				else 'IND' end as GICS_COA
	into #GICSCOA
	from GF_SECURITY_BASEVIEW where ISSUER_ID = @ISSUER_ID			

	select @GICSCOA = gc.GICS_COA 
		from #GICSCOA gc
		where 1=1

	drop table #GICSCOA
		 
	select @ReutersCOA = max(sci.COAType)
		  from dbo.GF_SECURITY_BASEVIEW sb 
		 inner join Reuters.dbo.tblStdCompanyInfo sci on sci.ReportNumber = sb.REPORTNUMBER
		 where sb.ISSUER_ID = @ISSUER_ID
		 group by sb.ISSUER_ID
	
--	print @ReutersCOA
		 
	if @ReutersCOA is not null
		BEGIN
			begin transaction
				Update INTERNAL_ISSUER 
				set COA_TYPE = @ReutersCOA where ISSUER_ID = @ISSUER_ID
				set @Update = @@ROWCOUNT
			commit
			
			if @Update = 0
			BEGIN
				begin transaction
					Insert into INTERNAL_ISSUER values (@ISSUER_ID, @ReutersCOA, NULL, NULL)
				commit
			END
		END
	ELSE		
		BEGIN	
			begin transaction
				Update INTERNAL_ISSUER 
				set COA_TYPE = @GICSCOA where ISSUER_ID = @ISSUER_ID
				set @Update = @@ROWCOUNT
			commit
						
			if @Update =0
			BEGIN
				begin transaction
					Insert into INTERNAL_ISSUER values (@ISSUER_ID, @GICSCOA, NULL, NULL)
				commit
			END
		END

GO
