/****** Object:  View [dbo].[LatestSearches]    Script Date: 08/21/2008 10:19:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[LatestSearches]
AS
SELECT     TOP (200) Term, CapturedFrom, CapturedUntil, Property, Orientation, UserUID, IPAddress, Results, Created
FROM         dbo.Searches
ORDER BY Created DESC

GO