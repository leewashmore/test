SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
alter procedure [dbo].[SaveCustomScreeningDataPointsPreference] 
	-- Add the parameters for the stored procedure here
	@userPreference varchar(MAX), 
	@username nvarchar(50)
AS
DECLARE @listName nvarchar(50);	
DECLARE @accessibility nvarchar(10);	
DECLARE @tempTable TABLE
(
UserName nvarchar(50) not null, 
ListName nvarchar(50) not null, 
ScreeningId varchar(50) not null, 
DataDescription nvarchar(MAX) not null,
DataSource varchar(50), 
PeriodType varchar(10),
YearType char(8),
FromDate int,
ToDate int,
DataPointsOrder int not null,
CreatedBy nvarchar(50) not null,
CreatedOn datetime not null,
ModifiedBy nvarchar(50) not null,
ModifiedOn datetime not null
)
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	BEGIN TRANSACTION

    --TSQL Script to parse update xml
		
		DECLARE @XML XML
		SELECT @XML = @userPreference
		DECLARE @idoc int
		
		EXEC sp_xml_preparedocument @idoc OUTPUT, @XML
		
		SELECT @listName = ListName FROM OPENXML(@idoc, '/Root/CreateRow', 2)
			WITH ( ListName nvarchar(50) '@ListName')
			
		SELECT @accessibility = Accessibilty FROM OPENXML(@idoc, '/Root/CreateRow/CreateRowEntity', 2)
			WITH (Accessibilty nvarchar(10) '@Accessibilty')	
			
		INSERT INTO dbo.UserCustomisedListInfo
			(UserName, ListName, Accessibilty, CreatedOn, ModifiedBy, ModifiedOn)
		VALUES(@username,@listName,@accessibility,GETUTCDATE(),@username,GETUTCDATE())
				
				IF @@ERROR <> 0
			BEGIN
				ROLLBACK TRANSACTION
				SELECT -1				
				RETURN
			END	
			
			INSERT INTO @tempTable
			SELECT *,
			@username as [CreatedBy],
			GETUTCDATE() as [CreatedOn],
			@username as [ModifiedBy],
			GETUTCDATE() as [ModifiedOn]
			FROM OPENXML(@idoc, '/Root/CreateRow/CreateRowPreference', 2)
			WITH (
				UserName nvarchar(50) '@UserName', 
				ListName nvarchar(50) '@ListName', 
				ScreeningId nvarchar(10) '@ScreeningId', 
				DataDescription nvarchar(MAX) '@DataDescription',
				DataSource varchar(50) '@DataSource', 
				PeriodType varchar(10) '@PeriodType',
				YearType nvarchar(50) '@YearType',
				FromDate int '@FromDate',
				ToDate int '@ToDate',
				DataPointsOrder int '@DataPointsOrder')
				
			IF @@ERROR <> 0
			BEGIN
				ROLLBACK TRANSACTION
				SELECT -1
				RETURN
			END	
			
		SELECT 
		l.ListId,
		t.ScreeningId,
		t.DataDescription,
		t.DataSource,
		t.PeriodType,
		t.YearType,
		t.FromDate,
		t.ToDate,
		t.DataPointsOrder,
		t.CreatedBy,
		t.CreatedOn,
		t.ModifiedBy,
		t.ModifiedOn
		INTO #finalTable
		FROM
		(SELECT ListId,ListName,UserName 
		FROM UserCustomisedListInfo 
		WHERE ListName = @listName)l
		
		left join
		
		(SELECT * FROM @tempTable)t
		ON l.ListName = t.ListName AND
		l.UserName = t.UserName	
		
		INSERT INTO UserListDataPointMappingInfo
		(ListId,
		ScreeningId,
		DataDescription,
		DataSource,
		PeriodType,
		YearType,
		FromDate,
		ToDate,
		DataPointsOrder,
		CreatedBy,
		CreatedOn,
		ModifiedBy,
		ModifiedOn) 
		SELECT ListId,
		ScreeningId,
		DataDescription,
		DataSource,
		PeriodType,
		YearType,
		FromDate,
		ToDate,
		DataPointsOrder,
		CreatedBy,
		CreatedOn,
		ModifiedBy,
		ModifiedOn 
		FROM #finalTable
		
		IF @@ERROR <> 0
			BEGIN
				ROLLBACK TRANSACTION
				SELECT -1
				RETURN
			END	
		
		DROP TABLE #finalTable;	
		--EXEC sp_xml_removedocument @idoc;
		
		COMMIT TRANSACTION;
		SELECT 0	
END
GO
