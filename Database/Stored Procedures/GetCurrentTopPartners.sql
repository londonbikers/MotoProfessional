IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'GetCurrentTopPartners')
	BEGIN
		DROP Procedure dbo.GetCurrentTopPartners
	END
GO

CREATE Procedure dbo.GetCurrentTopPartners
(
	@MaxResults INT
)
AS
	DECLARE @StartDate DATETIME
	DECLARE @EndDate DATETIME
	SET @EndDate = (SELECT MAX(Created) FROM Orders)
	SET @StartDate = DATEADD(m, -1, @EndDate)
	SELECT TOP (@MaxResults) Photos.PartnerID, COUNT(0) AS [Sales] 
	FROM OrderItems
	INNER JOIN Orders ON Orders.ID = OrderItems.OrderID
	INNER JOIN Photos ON Photos.ID = OrderItems.PhotoID
	WHERE Orders.Created BETWEEN @StartDate AND @EndDate
	GROUP BY Photos.PartnerID
	ORDER BY COUNT(OrderItems.ID)
GO

GRANT EXEC ON dbo.GetCurrentTopPartners TO PUBLIC
GO
