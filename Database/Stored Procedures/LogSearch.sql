IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'LogSearch')
	BEGIN
		DROP Procedure dbo.LogSearch
	END
GO

CREATE Procedure dbo.LogSearch
(
	@Term NVARCHAR(300),
	@CapturedFrom DATETIME,
	@CapturedUntil DATETIME,
	@Property TINYINT,
	@Orientation TINYINT,
	@UserUID UNIQUEIDENTIFIER,
	@IPAddress VARCHAR(20),
	@Results INT
)
AS
	SET NOCOUNT ON
	INSERT INTO
		Searches
		(
			Term,
			CapturedFrom,
			CapturedUntil,
			[Property],
			Orientation,
			UserUID,
			IPAddress,
			Results
		)
		VALUES
		(
			@Term,
			@CapturedFrom,
			@CapturedUntil,
			@Property,
			@Orientation,
			@UserUID,
			@IPAddress,
			@Results
		)
GO

GRANT EXEC ON dbo.LogSearch TO PUBLIC
GO