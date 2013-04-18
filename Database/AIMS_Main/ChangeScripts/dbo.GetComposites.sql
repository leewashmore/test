/****** Object:  StoredProcedure [dbo].[GetComposites]    Script Date: 04/18/2013 15:05:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
------------------------------------------------------------------------
-- Purpose:	Retrieve list of composites
--
-- Author:	Krish J
-- Date:	04-08-2013
------------------------------------------------------------------------
ALTER procedure [dbo].[GetComposites]

As

	Select *
	From	COMPOSITE_MASTER