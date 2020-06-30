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
CREATE TABLE dbo.DigitalGoodsDownloadLog
	(
	ID int NOT NULL IDENTITY (1, 1),
	DigitalGoodID int NOT NULL,
	IPAddress varchar(15) NOT NULL,
	CustomerUID uniqueidentifier NOT NULL,
	HttpReferrer varchar(512) NULL,
	ClientName varchar(512) NULL,
	Created nchar(10) NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.DigitalGoodsDownloadLog ADD CONSTRAINT
	PK_DigitalGoodsDownloadLog PRIMARY KEY CLUSTERED 
	(
	ID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

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
ALTER TABLE dbo.DigitalGoodsDownloadLog
	DROP CONSTRAINT FK_DigitalGoodsDownloadLog_DigitalGoods
GO
COMMIT
BEGIN TRANSACTION
GO
CREATE TABLE dbo.Tmp_DigitalGoodsDownloadLog
	(
	ID int NOT NULL IDENTITY (1, 1),
	DigitalGoodID int NOT NULL,
	IPAddress varchar(15) NOT NULL,
	CustomerUID uniqueidentifier NOT NULL,
	HttpReferrer varchar(512) NULL,
	ClientName varchar(512) NULL,
	Created datetime NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_DigitalGoodsDownloadLog ADD CONSTRAINT
	DF_DigitalGoodsDownloadLog_Created DEFAULT getdate() FOR Created
GO
SET IDENTITY_INSERT dbo.Tmp_DigitalGoodsDownloadLog ON
GO
IF EXISTS(SELECT * FROM dbo.DigitalGoodsDownloadLog)
	 EXEC('INSERT INTO dbo.Tmp_DigitalGoodsDownloadLog (ID, DigitalGoodID, IPAddress, CustomerUID, HttpReferrer, ClientName, Created)
		SELECT ID, DigitalGoodID, IPAddress, CustomerUID, HttpReferrer, ClientName, CONVERT(datetime, Created) FROM dbo.DigitalGoodsDownloadLog WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_DigitalGoodsDownloadLog OFF
GO
DROP TABLE dbo.DigitalGoodsDownloadLog
GO
EXECUTE sp_rename N'dbo.Tmp_DigitalGoodsDownloadLog', N'DigitalGoodsDownloadLog', 'OBJECT' 
GO
ALTER TABLE dbo.DigitalGoodsDownloadLog ADD CONSTRAINT
	PK_DigitalGoodsDownloadLog PRIMARY KEY CLUSTERED 
	(
	ID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

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
