SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
alter procedure [dbo].[RetrieveCommodityForecasts] 
	
AS
BEGIN

	Select Distinct(COMMODITY_ID) from [AIMS_Config].[dbo].COMMODITY_FORECASTS
	
END
GO
