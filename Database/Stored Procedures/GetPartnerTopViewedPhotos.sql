IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'GetPartnerTopViewedPhotos')
	BEGIN
		DROP Procedure dbo.GetPartnerTopViewedPhotos
	END
GO

CREATE Procedure dbo.GetPartnerTopViewedPhotos
(
	@PartnerID INT,
	@MaxRecords INT
)
AS
	SELECT TOP 10 Photos.ID, Photos.CustomerViews
		FROM Photos
		INNER JOIN CollectionPhotos ON CollectionPhotos.PhotoID = Photos.ID
		INNER JOIN Collections ON Collections.ID = CollectionPhotos.CollectionID
		WHERE Collections.[Status] = 1 AND Collections.PartnerID = @PartnerID
		GROUP BY Photos.ID, Photos.CustomerViews
		ORDER BY Photos.CustomerViews DESC
GO

GRANT EXEC ON dbo.GetPartnerTopViewedPhotos TO PUBLIC
GO