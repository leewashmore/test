SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
alter procedure [dbo].[ModelRetrieveFileId]
	(
	@LOCATION VARCHAR(255)
	)
AS
BEGIN
	
	SELECT * 
	FROM FileMaster 
	WHERE Location=@LOCATION
	
END
GO
