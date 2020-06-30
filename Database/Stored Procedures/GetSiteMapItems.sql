IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'GetSiteMapItems')
	BEGIN
		DROP Procedure dbo.GetSiteMapItems
	END
GO

CREATE Procedure dbo.GetSiteMapItems
AS
	SELECT
		ID,
		[Name] AS Title,
		LastUpdated AS LastModified
		FROM
		Collections
		WHERE
		Status = 1
GO

GRANT EXEC ON dbo.GetSiteMapItems TO PUBLIC
GO