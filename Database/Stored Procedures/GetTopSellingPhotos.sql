IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'GetTopSellingPhotos')
	BEGIN
		DROP Procedure dbo.GetTopSellingPhotos
	END
GO

CREATE Procedure dbo.GetTopSellingPhotos
(
	@MaxRecords INT
)
AS
	SELECT TOP (@MaxRecords) OrderItems.PhotoID AS [ID], COUNT(0) AS Sales
	FROM OrderItems
	INNER JOIN CollectionPhotos ON CollectionPhotos.PhotoID = OrderItems.PhotoID
	INNER JOIN Collections ON Collections.ID = CollectionPhotos.CollectionID
	INNER JOIN Orders ON Orders.ID = OrderItems.OrderID
	WHERE Orders.ChargeStatus = 1 AND Collections.[Status] = 1
	GROUP BY OrderItems.PhotoID
	ORDER BY COUNT(0) DESC
GO

GRANT EXEC ON dbo.GetTopSellingPhotos TO PUBLIC
GO