set noexec off

--declare  current and required version
declare @RequiredDBVersion as nvarchar(100) = '00079'
declare @CurrentScriptVersion as nvarchar(100) = '00080'
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

CREATE PROCEDURE [dbo].[SetICPresentationAttachedFileInfo]
	@UserName VARCHAR(50),
	@PresentationId BIGINT,
	@Name VARCHAR(255),
	@SecurityName VARCHAR(255),
	@SecurityTicker VARCHAR(50),
	@Location VARCHAR(255),
	@MetaTags VARCHAR(255),
	@Category VARCHAR(50),
	@Type  VARCHAR(50),
	@FileId BIGINT = 0,
	@DeletionFlag BIT = 'False'
AS
BEGIN
	SET NOCOUNT ON;
	BEGIN TRANSACTION	
					
		IF @FileId = 0
		BEGIN	
			INSERT INTO FileMaster ([Name], [SecurityName], [SecurityTicker], [Location], [MetaTags], [Category], [Type]
			, [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn])
			VALUES (@Name, @SecurityName, @SecurityTicker, @Location, @MetaTags, @Category, @Type
			, @UserName, GETUTCDATE(), @UserName, GETUTCDATE())
			
			SET @FileId = @@IDENTITY
			
			IF @@ERROR <> 0
			BEGIN
				ROLLBACK TRANSACTION
				SELECT -1
				RETURN
			END	
		
			INSERT INTO dbo.PresentationAttachedFileInfo ( [FileID], [PresentationID], [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn] )
			VALUES ( @FileId, @PresentationId, @UserName, GETUTCDATE(), @UserName, GETUTCDATE() )
			
			IF @@ERROR <> 0
			BEGIN
				ROLLBACK TRANSACTION
				SELECT -2
				RETURN
			END	
		END
		ELSE
		BEGIN
			IF @DeletionFlag = 'True'
			BEGIN			
				DELETE FROM FileMaster WHERE [FileID] = @FileId
				IF @@ERROR <> 0
				BEGIN
					ROLLBACK TRANSACTION
					SELECT -3
					RETURN
				END	
			END
			ELSE
			BEGIN
				UPDATE FileMaster SET
				[Name] = @Name, 
				[SecurityName] = @SecurityName,
				[SecurityTicker] = @SecurityTicker,
				[Location] = @Location,
				[MetaTags] = @MetaTags,
				[Category] = @Category,
				[Type] = @Type,
				[ModifiedBy] = @UserName,
				[ModifiedOn] = GETUTCDATE()
				WHERE [FileID] = @FileId
				
				IF @@ERROR <> 0
				BEGIN
					ROLLBACK TRANSACTION
					SELECT -4
					RETURN
				END	
			END
		END							
		
	COMMIT TRANSACTION	
	SELECT 0
    
END
GO

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00080'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())
