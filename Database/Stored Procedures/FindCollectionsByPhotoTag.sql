IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'FindCollectionsByPhotoTag')
	BEGIN
		DROP Procedure dbo.FindCollectionsByPhotoTag
	END
GO

CREATE Procedure dbo.FindCollectionsByPhotoTag
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
			c.ID
			FROM Photos p
			INNER JOIN CollectionPhotos cp ON cp.PhotoID = p.ID
			INNER JOIN Collections c on c.ID = cp.CollectionID
			WHERE CONTAINS(p.Tags, @Tag)
			GROUP BY c.ID, c.Created
			ORDER BY c.Created DESC
	END
	ELSE
	BEGIN
		SELECT
			TOP (@MaxRecords)
			c.ID
			FROM Photos p
			INNER JOIN CollectionPhotos cp ON cp.PhotoID = p.ID
			INNER JOIN Collections c on c.ID = cp.CollectionID
			WHERE CONTAINS(p.Tags, @Tag) AND c.Status = @Status
			GROUP BY c.ID, c.Created
			ORDER BY c.Created DESC
	END
GO

GRANT EXEC ON dbo.FindCollectionsByPhotoTag TO PUBLIC
GO