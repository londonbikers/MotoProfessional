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
ALTER TABLE dbo.Licenses
	DROP CONSTRAINT DF_Licenses_PrimaryDimension
GO
ALTER TABLE dbo.Licenses
	DROP CONSTRAINT DF_Licenses_Status
GO
ALTER TABLE dbo.Licenses
	DROP CONSTRAINT DF_Licenses_Created
GO
ALTER TABLE dbo.Licenses
	DROP CONSTRAINT DF_Licenses_LastUpdated
GO
CREATE TABLE dbo.Tmp_Licenses
	(
	ID int NOT NULL IDENTITY (1, 1),
	Name varchar(100) NOT NULL,
	ShortDescription varchar(300) NULL,
	Description text NULL,
	PrimaryDimension int NOT NULL,
	Status tinyint NOT NULL,
	Created datetime NOT NULL,
	LastUpdated datetime NOT NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_Licenses ADD CONSTRAINT
	DF_Licenses_PrimaryDimension DEFAULT ((-1)) FOR PrimaryDimension
GO
ALTER TABLE dbo.Tmp_Licenses ADD CONSTRAINT
	DF_Licenses_Status DEFAULT ((0)) FOR Status
GO
ALTER TABLE dbo.Tmp_Licenses ADD CONSTRAINT
	DF_Licenses_Created DEFAULT (getdate()) FOR Created
GO
ALTER TABLE dbo.Tmp_Licenses ADD CONSTRAINT
	DF_Licenses_LastUpdated DEFAULT (getdate()) FOR LastUpdated
GO
SET IDENTITY_INSERT dbo.Tmp_Licenses ON
GO
IF EXISTS(SELECT * FROM dbo.Licenses)
	 EXEC('INSERT INTO dbo.Tmp_Licenses (ID, Name, Description, PrimaryDimension, Status, Created, LastUpdated)
		SELECT ID, Name, Description, PrimaryDimension, Status, Created, LastUpdated FROM dbo.Licenses WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_Licenses OFF
GO
ALTER TABLE dbo.RateCardItems
	DROP CONSTRAINT FK_RateCardItems_Licenses
GO
ALTER TABLE dbo.OrderItems
	DROP CONSTRAINT FK_OrderItems_Licenses
GO
ALTER TABLE dbo.BasketItems
	DROP CONSTRAINT FK_BasketItems_Licenses
GO
DROP TABLE dbo.Licenses
GO
EXECUTE sp_rename N'dbo.Tmp_Licenses', N'Licenses', 'OBJECT' 
GO
ALTER TABLE dbo.Licenses ADD CONSTRAINT
	PK_Licenses PRIMARY KEY CLUSTERED 
	(
	ID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.BasketItems ADD CONSTRAINT
	FK_BasketItems_Licenses FOREIGN KEY
	(
	LicenseID
	) REFERENCES dbo.Licenses
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.OrderItems ADD CONSTRAINT
	FK_OrderItems_Licenses FOREIGN KEY
	(
	LicenseID
	) REFERENCES dbo.Licenses
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.RateCardItems ADD CONSTRAINT
	FK_RateCardItems_Licenses FOREIGN KEY
	(
	LicenseID
	) REFERENCES dbo.Licenses
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
COMMIT


update licenses set shortdescription = cast(Description as varchar(300))
GO

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
ALTER TABLE dbo.Licenses
	DROP CONSTRAINT DF_Licenses_PrimaryDimension
GO
ALTER TABLE dbo.Licenses
	DROP CONSTRAINT DF_Licenses_Status
GO
ALTER TABLE dbo.Licenses
	DROP CONSTRAINT DF_Licenses_Created
GO
ALTER TABLE dbo.Licenses
	DROP CONSTRAINT DF_Licenses_LastUpdated
GO
CREATE TABLE dbo.Tmp_Licenses
	(
	ID int NOT NULL IDENTITY (1, 1),
	Name varchar(100) NOT NULL,
	ShortDescription varchar(300) NOT NULL,
	Description text NOT NULL,
	PrimaryDimension int NOT NULL,
	Status tinyint NOT NULL,
	Created datetime NOT NULL,
	LastUpdated datetime NOT NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_Licenses ADD CONSTRAINT
	DF_Licenses_PrimaryDimension DEFAULT ((-1)) FOR PrimaryDimension
GO
ALTER TABLE dbo.Tmp_Licenses ADD CONSTRAINT
	DF_Licenses_Status DEFAULT ((0)) FOR Status
GO
ALTER TABLE dbo.Tmp_Licenses ADD CONSTRAINT
	DF_Licenses_Created DEFAULT (getdate()) FOR Created
GO
ALTER TABLE dbo.Tmp_Licenses ADD CONSTRAINT
	DF_Licenses_LastUpdated DEFAULT (getdate()) FOR LastUpdated
GO
SET IDENTITY_INSERT dbo.Tmp_Licenses ON
GO
IF EXISTS(SELECT * FROM dbo.Licenses)
	 EXEC('INSERT INTO dbo.Tmp_Licenses (ID, Name, ShortDescription, Description, PrimaryDimension, Status, Created, LastUpdated)
		SELECT ID, Name, ShortDescription, Description, PrimaryDimension, Status, Created, LastUpdated FROM dbo.Licenses WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_Licenses OFF
GO
ALTER TABLE dbo.RateCardItems
	DROP CONSTRAINT FK_RateCardItems_Licenses
GO
ALTER TABLE dbo.OrderItems
	DROP CONSTRAINT FK_OrderItems_Licenses
GO
ALTER TABLE dbo.BasketItems
	DROP CONSTRAINT FK_BasketItems_Licenses
GO
DROP TABLE dbo.Licenses
GO
EXECUTE sp_rename N'dbo.Tmp_Licenses', N'Licenses', 'OBJECT' 
GO
ALTER TABLE dbo.Licenses ADD CONSTRAINT
	PK_Licenses PRIMARY KEY CLUSTERED 
	(
	ID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.BasketItems ADD CONSTRAINT
	FK_BasketItems_Licenses FOREIGN KEY
	(
	LicenseID
	) REFERENCES dbo.Licenses
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.OrderItems ADD CONSTRAINT
	FK_OrderItems_Licenses FOREIGN KEY
	(
	LicenseID
	) REFERENCES dbo.Licenses
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.RateCardItems ADD CONSTRAINT
	FK_RateCardItems_Licenses FOREIGN KEY
	(
	LicenseID
	) REFERENCES dbo.Licenses
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
COMMIT
