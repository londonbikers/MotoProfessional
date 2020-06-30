IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'FindPartnerCollectionsByName')
	BEGIN
		DROP Procedure dbo.FindPartnerCollectionsByName
	END
GO

CREATE Procedure dbo.FindPartnerCollectionsByName
(
	@PartnerID INT,
	@Name VARCHAR(255),
	@MaxRecords INT,
	@Status TINYINT
)
AS
	SET @Name = '"' + @Name + '"'
	IF (@Status = 255)
	BEGIN
		SELECT
			ID
			FROM Collections
			WHERE PartnerID = @PartnerID AND CONTAINS([Name], @Name)
			ORDER BY Created DESC
	END
	ELSE
	BEGIN
		SELECT
			ID
			FROM Collections
			WHERE PartnerID = @PartnerID AND CONTAINS([Name], @Name) AND Status = @Status
			ORDER BY Created DESC
	END
GO

GRANT EXEC ON dbo.FindPartnerCollectionsByName TO PUBLIC
GO
