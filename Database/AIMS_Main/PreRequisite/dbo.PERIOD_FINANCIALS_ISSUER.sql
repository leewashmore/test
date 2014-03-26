
/****** Object:  Table [dbo].[PERIOD_FINANCIALS]    Script Date: 03/13/2014 09:54:45 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PERIOD_FINANCIALS_ISSUER]') AND type in (N'U'))
DROP TABLE [dbo].[PERIOD_FINANCIALS_ISSUER]
GO


/****** Object:  Table [dbo].[PERIOD_FINANCIALS]    Script Date: 03/13/2014 09:54:49 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[PERIOD_FINANCIALS_ISSUER](
	[ISSUER_ID] [varchar](20) NOT NULL,
	[SECURITY_ID] [varchar](20) NOT NULL,
	[COA_TYPE] [varchar](3) NOT NULL,
	[DATA_SOURCE] [varchar](10) NOT NULL,
	[ROOT_SOURCE] [varchar](10) NOT NULL,
	[ROOT_SOURCE_DATE] [datetime] NOT NULL,
	[PERIOD_TYPE] [char](2) NOT NULL,
	[PERIOD_YEAR] [int] NOT NULL,
	[PERIOD_END_DATE] [datetime] NOT NULL,
	[FISCAL_TYPE] [char](8) NOT NULL,
	[CURRENCY] [char](3) NOT NULL,
	[DATA_ID] [int] NOT NULL,
	[AMOUNT] [decimal](32, 6) NOT NULL,
	[CALCULATION_DIAGRAM] [varchar](255) NULL,
	[SOURCE_CURRENCY] [char](3) NOT NULL,
	[AMOUNT_TYPE] [char](10) NOT NULL
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


