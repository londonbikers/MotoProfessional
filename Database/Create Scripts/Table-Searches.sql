/****** Object:  Table [dbo].[Searches]    Script Date: 08/20/2008 15:17:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Searches](
	[Term] [nvarchar](200) NOT NULL,
	[CapturedFrom] [datetime] NULL,
	[CapturedUntil] [datetime] NULL,
	[Property] [tinyint] NOT NULL,
	[UserUID] [uniqueidentifier] NULL,
	[Results] [int] NOT NULL CONSTRAINT [DF_Searches_Results]  DEFAULT ((0)),
	[Created] [datetime] NOT NULL CONSTRAINT [DF_Searches_Created]  DEFAULT (getdate())
) ON [PRIMARY]
/****** Object:  Index [IDX_Primary]    Script Date: 08/20/2008 15:18:12 ******/
CREATE NONCLUSTERED INDEX [IDX_Primary] ON [dbo].[Searches] 
(
	[Property] ASC,
	[UserUID] ASC,
	[Results] ASC,
	[Created] ASC,
	[Term] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.Searches
	DROP CONSTRAINT FK_Searches_aspnet_Users
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.Searches
	DROP CONSTRAINT DF_Searches_Results
GO
ALTER TABLE dbo.Searches
	DROP CONSTRAINT DF_Searches_Created
GO
CREATE TABLE dbo.Tmp_Searches
	(
	Term nvarchar(200) NOT NULL,
	CapturedFrom datetime NULL,
	CapturedUntil datetime NULL,
	Property tinyint NOT NULL,
	UserUID uniqueidentifier NULL,
	IPAddress varchar(20) NULL,
	Results int NOT NULL,
	Created datetime NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_Searches ADD CONSTRAINT
	DF_Searches_Results DEFAULT ((0)) FOR Results
GO
ALTER TABLE dbo.Tmp_Searches ADD CONSTRAINT
	DF_Searches_Created DEFAULT (getdate()) FOR Created
GO
IF EXISTS(SELECT * FROM dbo.Searches)
	 EXEC('INSERT INTO dbo.Tmp_Searches (Term, CapturedFrom, CapturedUntil, Property, UserUID, Results, Created)
		SELECT Term, CapturedFrom, CapturedUntil, Property, UserUID, Results, Created FROM dbo.Searches WITH (HOLDLOCK TABLOCKX)')
GO
DROP TABLE dbo.Searches
GO
EXECUTE sp_rename N'dbo.Tmp_Searches', N'Searches', 'OBJECT' 
GO
CREATE NONCLUSTERED INDEX IDX_Primary ON dbo.Searches
	(
	Property,
	UserUID,
	Results,
	Created,
	Term
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE dbo.Searches ADD CONSTRAINT
	FK_Searches_aspnet_Users FOREIGN KEY
	(
	UserUID
	) REFERENCES dbo.aspnet_Users
	(
	UserId
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
COMMIT

/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.Searches
	DROP CONSTRAINT FK_Searches_aspnet_Users
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.Searches
	DROP CONSTRAINT DF_Searches_Results
GO
ALTER TABLE dbo.Searches
	DROP CONSTRAINT DF_Searches_Created
GO
CREATE TABLE dbo.Tmp_Searches
	(
	Term nvarchar(200) NOT NULL,
	CapturedFrom datetime NULL,
	CapturedUntil datetime NULL,
	Property tinyint NOT NULL,
	Orientation tinyint NULL,
	UserUID uniqueidentifier NULL,
	IPAddress varchar(20) NULL,
	Results int NOT NULL,
	Created datetime NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_Searches ADD CONSTRAINT
	DF_Searches_Orientation DEFAULT 0 FOR Orientation
GO
ALTER TABLE dbo.Tmp_Searches ADD CONSTRAINT
	DF_Searches_Results DEFAULT ((0)) FOR Results
GO
ALTER TABLE dbo.Tmp_Searches ADD CONSTRAINT
	DF_Searches_Created DEFAULT (getdate()) FOR Created
GO
IF EXISTS(SELECT * FROM dbo.Searches)
	 EXEC('INSERT INTO dbo.Tmp_Searches (Term, CapturedFrom, CapturedUntil, Property, UserUID, IPAddress, Results, Created)
		SELECT Term, CapturedFrom, CapturedUntil, Property, UserUID, IPAddress, Results, Created FROM dbo.Searches WITH (HOLDLOCK TABLOCKX)')
GO
DROP TABLE dbo.Searches
GO
EXECUTE sp_rename N'dbo.Tmp_Searches', N'Searches', 'OBJECT' 
GO
CREATE NONCLUSTERED INDEX IDX_Primary ON dbo.Searches
	(
	Property,
	UserUID,
	Results,
	Created,
	Term
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE dbo.Searches ADD CONSTRAINT
	FK_Searches_aspnet_Users FOREIGN KEY
	(
	UserUID
	) REFERENCES dbo.aspnet_Users
	(
	UserId
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
COMMIT
