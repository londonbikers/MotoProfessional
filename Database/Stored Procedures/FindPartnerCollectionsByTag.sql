 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'FindPartnerCollectionsByTag')
	BEGIN
		DROP Procedure dbo.FindPartnerCollectionsByTag
	END
GO

CREATE Procedure dbo.FindPartnerCollectionsByTag
(
	@PartnerID INT,
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
			WHERE c.PartnerID = @PartnerID AND CONTAINS(p.Tags, @Tag)
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
			WHERE c.PartnerID = @PartnerID AND CONTAINS(p.Tags, @Tag) AND c.Status = @Status
			GROUP BY c.ID, c.Created
			ORDER BY c.Created DESC
	END
GO

GRANT EXEC ON dbo.FindPartnerCollectionsByTag TO PUBLIC
GO