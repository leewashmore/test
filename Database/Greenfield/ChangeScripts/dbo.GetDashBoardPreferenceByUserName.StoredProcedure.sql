SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
alter procedure [dbo].[GetDashBoardPreferenceByUserName] 
	-- Add the parameters for the stored procedure here
	@UserName VARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM tblDashboardPreference WHERE UserName = @UserName
END
GO
