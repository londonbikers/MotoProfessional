IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'FindCollectionsByName')
	BEGIN
		DROP Procedure dbo.FindCollectionsByName
	END
GO

CREATE Procedure dbo.FindCollectionsByName
(
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
			WHERE CONTAINS([Name], @Name)
			ORDER BY Created DESC
	END
	ELSE
	BEGIN
		SELECT
			ID
			FROM Collections
			WHERE CONTAINS([Name], @Name) AND Status = @Status
			ORDER BY Created DESC
	END
GO

GRANT EXEC ON dbo.FindCollectionsByName TO PUBLIC
GO
