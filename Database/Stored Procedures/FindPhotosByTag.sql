 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'FindPhotosByTag')
	BEGIN
		DROP Procedure dbo.FindPhotosByTag
	END
GO

CREATE Procedure [dbo].[FindPhotosByTag]
(
	@Tag VARCHAR(255),
	@MaxRecords INT,
	@Status TINYINT
)
AS
	SET @Tag = '"' + @Tag + '"'
	
	IF (@Status = 255)
	BEGIN
		SELECT
			TOP (@MaxRecords)
			p.ID
			FROM Photos P
			WHERE CONTAINS(p.Tags, @Tag)
			ORDER BY p.Created DESC
	END
	ELSE
	BEGIN
		SELECT
			TOP (@MaxRecords)
			p.ID
			FROM Photos P
			INNER JOIN CollectionPhotos cp on cp.PhotoID = p.ID
			INNER JOIN Collections c on c.ID = cp.CollectionID
			WHERE CONTAINS(p.Tags, @Tag) AND (p.Status = @Status AND c.Status = @Status)
			ORDER BY p.Created DESC
	END
GO

GRANT EXEC ON dbo.FindPhotosByTag TO PUBLIC
GO