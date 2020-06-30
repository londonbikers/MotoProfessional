IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'GetTopViewedPhotos')
	BEGIN
		DROP Procedure dbo.GetTopViewedPhotos
	END
GO

CREATE Procedure dbo.GetTopViewedPhotos
(
	@MaxRecords INT
)
AS
	SELECT TOP (@MaxRecords) Photos.ID, Photos.CustomerViews
		FROM Photos
		INNER JOIN CollectionPhotos ON CollectionPhotos.PhotoID = Photos.ID
		INNER JOIN Collections ON Collections.ID = CollectionPhotos.CollectionID
		WHERE Collections.[Status] = 1
		GROUP BY Photos.ID, Photos.CustomerViews
		ORDER BY Photos.CustomerViews DESC
GO

GRANT EXEC ON dbo.GetTopViewedPhotos TO PUBLIC
GO