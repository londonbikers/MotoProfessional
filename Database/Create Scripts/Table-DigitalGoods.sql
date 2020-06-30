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
CREATE TABLE dbo.DigitalGoods
	(
	ID int NOT NULL IDENTITY (1, 1),
	OrderID int NULL,
	OrderItemID int NULL,
	Type tinyint NOT NULL,
	Filename varchar(50) NULL,
	Width int NULL,
	Height int NULL,
	Filesize int NULL,
	FileExists bit NOT NULL,
	FileCreationDate datetime NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.DigitalGoods ADD CONSTRAINT
	DF_DigitalGoods_Type DEFAULT 0 FOR Type
GO
ALTER TABLE dbo.DigitalGoods ADD CONSTRAINT
	DF_DigitalGoods_FileExists DEFAULT 1 FOR FileExists
GO
ALTER TABLE dbo.DigitalGoods ADD CONSTRAINT
	PK_DigitalGoods PRIMARY KEY CLUSTERED 
	(
	ID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
COMMIT

ALTER TABLE [dbo].[DigitalGoods]  WITH CHECK ADD  CONSTRAINT [FK_DigitalGoods_OrderItems] FOREIGN KEY([OrderItemID])
REFERENCES [dbo].[OrderItems] ([ID])
GO
ALTER TABLE [dbo].[DigitalGoods] CHECK CONSTRAINT [FK_DigitalGoods_OrderItems]
GO

ALTER TABLE [dbo].[DigitalGoods]  WITH CHECK ADD  CONSTRAINT [FK_DigitalGoods_Orders] FOREIGN KEY([OrderID])
REFERENCES [dbo].[Orders] ([ID])
GO
ALTER TABLE [dbo].[DigitalGoods] CHECK CONSTRAINT [FK_DigitalGoods_Orders]

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
ALTER TABLE dbo.DigitalGoods ADD
	Created datetime NOT NULL CONSTRAINT DF_DigitalGoods_Created DEFAULT getdate()
GO
COMMIT

------

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
ALTER TABLE dbo.DigitalGoods
	DROP CONSTRAINT FK_DigitalGoods_Orders
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.DigitalGoods
	DROP CONSTRAINT FK_DigitalGoods_OrderItems
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.DigitalGoods
	DROP CONSTRAINT DF_DigitalGoods_Type
GO
ALTER TABLE dbo.DigitalGoods
	DROP CONSTRAINT DF_DigitalGoods_FileExists
GO
ALTER TABLE dbo.DigitalGoods
	DROP CONSTRAINT DF_DigitalGoods_Created
GO
CREATE TABLE dbo.Tmp_DigitalGoods
	(
	ID int NOT NULL IDENTITY (1, 1),
	OrderID int NULL,
	OrderItemID int NULL,
	Type tinyint NOT NULL,
	Filename varchar(50) NULL,
	Width int NULL,
	Height int NULL,
	Filesize bigint NULL,
	FileExists bit NOT NULL,
	FileCreationDate datetime NULL,
	Created datetime NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_DigitalGoods ADD CONSTRAINT
	DF_DigitalGoods_Type DEFAULT ((0)) FOR Type
GO
ALTER TABLE dbo.Tmp_DigitalGoods ADD CONSTRAINT
	DF_DigitalGoods_FileExists DEFAULT ((1)) FOR FileExists
GO
ALTER TABLE dbo.Tmp_DigitalGoods ADD CONSTRAINT
	DF_DigitalGoods_Created DEFAULT (getdate()) FOR Created
GO
SET IDENTITY_INSERT dbo.Tmp_DigitalGoods ON
GO
IF EXISTS(SELECT * FROM dbo.DigitalGoods)
	 EXEC('INSERT INTO dbo.Tmp_DigitalGoods (ID, OrderID, OrderItemID, Type, Filename, Width, Height, Filesize, FileExists, FileCreationDate, Created)
		SELECT ID, OrderID, OrderItemID, Type, Filename, Width, Height, CONVERT(bigint, Filesize), FileExists, FileCreationDate, Created FROM dbo.DigitalGoods WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_DigitalGoods OFF
GO
ALTER TABLE dbo.DigitalGoodsDownloadLog
	DROP CONSTRAINT FK_DigitalGoodsDownloadLog_DigitalGoods
GO
DROP TABLE dbo.DigitalGoods
GO
EXECUTE sp_rename N'dbo.Tmp_DigitalGoods', N'DigitalGoods', 'OBJECT' 
GO
ALTER TABLE dbo.DigitalGoods ADD CONSTRAINT
	PK_DigitalGoods PRIMARY KEY CLUSTERED 
	(
	ID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.DigitalGoods ADD CONSTRAINT
	FK_DigitalGoods_OrderItems FOREIGN KEY
	(
	OrderItemID
	) REFERENCES dbo.OrderItems
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.DigitalGoods ADD CONSTRAINT
	FK_DigitalGoods_Orders FOREIGN KEY
	(
	OrderID
	) REFERENCES dbo.Orders
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.DigitalGoodsDownloadLog ADD CONSTRAINT
	FK_DigitalGoodsDownloadLog_DigitalGoods FOREIGN KEY
	(
	DigitalGoodID
	) REFERENCES dbo.DigitalGoods
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
COMMIT

----

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
ALTER TABLE dbo.DigitalGoods ADD
	LastUpdated datetime NOT NULL CONSTRAINT DF_DigitalGoods_LastUpdated DEFAULT getdate()
GO
COMMIT
