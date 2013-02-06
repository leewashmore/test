SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
alter procedure [dbo].[SetICPPresentationStatus] 
	@UserName VARCHAR(50),
	@PresentationId BIGINT,	
	@Status VARCHAR(50)	
AS
BEGIN
	SET NOCOUNT ON;
	BEGIN TRANSACTION
	
		UPDATE PresentationInfo 
		SET StatusType = @Status,
			ModifiedBy = @UserName,
			ModifiedOn = GETUTCDATE()
		WHERE PresentationID = @PresentationId		
		
		IF	@Status = 'Withdrawn'
		BEGIN
			DELETE FROM MeetingPresentationMappingInfo
			WHERE [PresentationID] = @PresentationId
		END
		
		IF @@ERROR <> 0
		BEGIN
			ROLLBACK TRANSACTION
			SELECT -1
			RETURN
		END	
		
	COMMIT TRANSACTION	
	SELECT 0    
END
GO
