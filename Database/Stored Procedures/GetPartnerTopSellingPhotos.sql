IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'GetPartnerTopSellingPhotos')
	BEGIN
		DROP Procedure dbo.GetPartnerTopSellingPhotos
	END
GO

CREATE Procedure dbo.GetPartnerTopSellingPhotos
(
	@PartnerID INT,
	@MaxRecords INT
)
AS
	SELECT TOP (@MaxRecords) Photos.ID, COUNT(0) AS Sales
	FROM Photos
	INNER JOIN CollectionPhotos ON CollectionPhotos.PhotoID = Photos.ID
	INNER JOIN Collections ON Collections.ID = CollectionPhotos.CollectionID
	INNER JOIN OrderItems ON OrderItems.PhotoID = Photos.ID
	INNER JOIN Orders ON Orders.ID = OrderItems.OrderID
	WHERE Photos.PartnerID = @PartnerID AND Orders.ChargeStatus = 1 AND Collections.[Status] = 1
	GROUP BY Photos.ID
	ORDER BY COUNT(0) DESC
GO

GRANT EXEC ON dbo.GetPartnerTopSellingPhotos TO PUBLIC
GO