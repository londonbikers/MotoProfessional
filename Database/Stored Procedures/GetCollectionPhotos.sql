IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'GetCollectionPhotos')
	BEGIN
		DROP Procedure dbo.GetCollectionPhotos
	END
GO

CREATE Procedure dbo.GetCollectionPhotos
(
	@CollectionID int
)
AS
	SELECT 
		p.*, 
		cp.[Order]
		FROM Photos p
		INNER JOIN CollectionPhotos cp ON cp.PhotoID = p.ID
		WHERE cp.CollectionID = @CollectionID
		ORDER BY cp.[Order]
GO

GRANT EXEC ON dbo.GetCollectionPhotos TO PUBLIC
GO