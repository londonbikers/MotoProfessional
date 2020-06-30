-- DEPLOYED TO LIVE
-------------------------------------------------------

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
ALTER PROCEDURE [dbo].[aspnet_Membership_GetAllUsers]
    @ApplicationName       nvarchar(256),
    @PageIndex             int,
    @PageSize              int
AS
BEGIN
    DECLARE @ApplicationId uniqueidentifier
    SELECT  @ApplicationId = NULL
    SELECT  @ApplicationId = ApplicationId FROM dbo.aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
    IF (@ApplicationId IS NULL)
        RETURN 0


    -- Set the page bounds
    DECLARE @PageLowerBound int
    DECLARE @PageUpperBound int
    DECLARE @TotalRecords   int
    SET @PageLowerBound = @PageSize * @PageIndex
    SET @PageUpperBound = @PageSize - 1 + @PageLowerBound

    -- Create a temp table TO store the select results
    CREATE TABLE #PageIndexForUsers
    (
        IndexId int IDENTITY (0, 1) NOT NULL,
        UserId uniqueidentifier
    )

    -- Insert into our temp table
    INSERT INTO #PageIndexForUsers (UserId)
    SELECT u.UserId
    FROM   dbo.aspnet_Membership m, dbo.aspnet_Users u
    WHERE  u.ApplicationId = @ApplicationId AND u.UserId = m.UserId
    ORDER BY u.UserName

    SELECT @TotalRecords = @@ROWCOUNT

    SELECT u.UserName, m.Email, m.PasswordQuestion, m.Comment, m.IsApproved,
            m.CreateDate,
            m.LastLoginDate,
            u.LastActivityDate,
            m.LastPasswordChangedDate,
            u.UserId, m.IsLockedOut,
            m.LastLockoutDate
    FROM   dbo.aspnet_Membership m, dbo.aspnet_Users u, #PageIndexForUsers p
    WHERE  u.UserId = p.UserId AND u.UserId = m.UserId AND
           p.IndexId >= @PageLowerBound AND p.IndexId <= @PageUpperBound
    ORDER BY m.CreateDate DESC
    RETURN @TotalRecords
END 

---------------------------------------------------------------------------------------

ALTER TABLE dbo.Partners
	DROP COLUMN Url
GO

---------------------------------------------------------------------------------------

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

---------------------------------------------------------------------------------------

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

---------------------------------------------------------------------------------------

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

---------------------------------------------------------------------------------------

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

---------------------------------------------------------------------------------------

DROP INDEX [IDX_Secondary] ON [dbo].[Photos] WITH ( ONLINE = OFF )
GO
USE [Photos]
GO
CREATE NONCLUSTERED INDEX [IDX_Secondary] ON [dbo].[Photos] 
(
	[PhotographerUID] ASC,
	[Status] ASC,
	[Created] ASC,
	[PartnerID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

---------------------------------------------------------------------------------------

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

---------------------------------------------------------------------------------------