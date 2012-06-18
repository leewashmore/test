set noexec off




--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00007'
declare @CurrentScriptVersion as nvarchar(100) = '00008'

--if current version already in DB, just skip
if exists(select 1 from ChangeScripts  where ScriptVersion = @CurrentScriptVersion)
 set noexec on 

--check that current DB version is Ok
declare @DBCurrentVersion as nvarchar(100) = (select top 1 ScriptVersion from ChangeScripts order by DateExecuted desc)
if (@DBCurrentVersion != @RequiredDBVersion)
begin
	RAISERROR(N'DB version is "%s", required "%s".', 16, 1, @DBCurrentVersion, @RequiredDBVersion)
	set noexec on
end

GO
--PUT YOUR CODE HERE:

Alter PROCEDURE [dbo].[GetMarketSnapshotSelectionData] 
	-- Add the parameters for the stored procedure here
	  @userId nVARCHAR(100)	  
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	Select SnapshotPreferenceId,SnapshotName from tblMarketSnapshotPreference
	WHERE UserId = @userId	
END

GO





--END OF YOUR CODE.


--indicate thet current script is executed

if @@error = 0
begin
	declare @CurrentScriptVersion as nvarchar(100) = '00008'
	insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())
end

