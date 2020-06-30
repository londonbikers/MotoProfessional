IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'IncrementPhotoViewCount')
	BEGIN
		DROP Procedure dbo.IncrementPhotoViewCount
	END
GO

CREATE Procedure dbo.IncrementPhotoViewCount
(
		@PhotoID int
)
AS
	SET NOCOUNT ON
	UPDATE
		Photos
		SET
		CustomerViews = CustomerViews + 1
		WHERE
		ID = @PhotoID
GO

GRANT EXEC ON dbo.IncrementPhotoViewCount TO PUBLIC
GO