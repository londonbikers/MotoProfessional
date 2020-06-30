[DEPLOYED TO LIVE]


-- ------------------------------------------------------------------------
-- INSERT SQL STATEMENTS BELOW IN A TRANSACTION TO MODIFY STRUCTURE OR DATA
-- ------------------------------------------------------------------------

-- [ MIGRATE TO COMPANY PAYMENT TERMS COLUMN ]-----------------------------

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
ALTER TABLE dbo.Companies
	DROP CONSTRAINT FK_Companies_Countries
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.Companies
	DROP CONSTRAINT DF_Companies_Invoicable
GO
ALTER TABLE dbo.Companies
	DROP CONSTRAINT DF_Companies_Status
GO
ALTER TABLE dbo.Companies
	DROP CONSTRAINT DF_Companies_Created
GO
ALTER TABLE dbo.Companies
	DROP CONSTRAINT DF_Companies_LastUpdated
GO
CREATE TABLE dbo.Tmp_Companies
	(
	ID int NOT NULL IDENTITY (1, 1),
	Name varchar(50) NOT NULL,
	Description text NULL,
	Telephone varchar(20) NULL,
	Fax varchar(20) NULL,
	Address text NULL,
	PostalCode varchar(10) NULL,
	CountryID int NULL,
	Url varchar(255) NULL,
	PaymentTerms tinyint NOT NULL,
	Status tinyint NOT NULL,
	Created datetime NOT NULL,
	LastUpdated datetime NOT NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_Companies ADD CONSTRAINT
	DF_Companies_PaymentTerms DEFAULT 0 FOR PaymentTerms
GO
ALTER TABLE dbo.Tmp_Companies ADD CONSTRAINT
	DF_Companies_Status DEFAULT ((0)) FOR Status
GO
ALTER TABLE dbo.Tmp_Companies ADD CONSTRAINT
	DF_Companies_Created DEFAULT (getdate()) FOR Created
GO
ALTER TABLE dbo.Tmp_Companies ADD CONSTRAINT
	DF_Companies_LastUpdated DEFAULT (getdate()) FOR LastUpdated
GO
SET IDENTITY_INSERT dbo.Tmp_Companies ON
GO
IF EXISTS(SELECT * FROM dbo.Companies)
	 EXEC('INSERT INTO dbo.Tmp_Companies (ID, Name, Description, Telephone, Fax, Address, PostalCode, CountryID, Url, Status, Created, LastUpdated)
		SELECT ID, Name, Description, Telephone, Fax, Address, PostalCode, CountryID, Url, Status, Created, LastUpdated FROM dbo.Companies WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_Companies OFF
GO
ALTER TABLE dbo.CompanyStaff
	DROP CONSTRAINT FK_CompanyStaff_Companies
GO
ALTER TABLE dbo.Partners
	DROP CONSTRAINT FK_Partners_Companies
GO
DROP TABLE dbo.Companies
GO
EXECUTE sp_rename N'dbo.Tmp_Companies', N'Companies', 'OBJECT' 
GO
ALTER TABLE dbo.Companies ADD CONSTRAINT
	PK_Companies PRIMARY KEY CLUSTERED 
	(
	ID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
CREATE NONCLUSTERED INDEX IDX_Secondary ON dbo.Companies
	(
	Created,
	Name
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE dbo.Companies ADD CONSTRAINT
	FK_Companies_Countries FOREIGN KEY
	(
	CountryID
	) REFERENCES dbo.Countries
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.Partners ADD CONSTRAINT
	FK_Partners_Companies FOREIGN KEY
	(
	CompanyID
	) REFERENCES dbo.Companies
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.CompanyStaff ADD CONSTRAINT
	FK_CompanyStaff_Companies FOREIGN KEY
	(
	CompanyID
	) REFERENCES dbo.Companies
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
COMMIT

-- ************************************************************************

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
EXECUTE sp_rename N'dbo.Companies.PaymentTerms', N'Tmp_ChargeMethod', 'COLUMN' 
GO
EXECUTE sp_rename N'dbo.Companies.Tmp_ChargeMethod', N'ChargeMethod', 'COLUMN' 
GO
COMMIT

-- ************************************************************************

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
ALTER TABLE dbo.OrderTransactions
	DROP CONSTRAINT FK_OrderTransactions_Orders
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.OrderTransactions
	DROP CONSTRAINT DF_OrderTransactions_Created
GO
CREATE TABLE dbo.Tmp_OrderTransactions
	(
	ID int NOT NULL IDENTITY (1, 1),
	OrderID int NOT NULL,
	Type tinyint NOT NULL,
	GC_OrderNumber varchar(50) NULL,
	GC_NewFinanceState varchar(50) NULL,
	GC_NewFulfillmentState varchar(50) NULL,
	GC_PrevFinanceState varchar(50) NULL,
	GC_PrevFulfillmentState varchar(50) NULL,
	GC_ChargedAmount money NULL,
	GC_RefundedAmount money NULL,
	GC_ChargebackAmount money NULL,
	MemberUID uniqueidentifier NULL,
	Operation varchar(1000) NULL,
	ClientIPAddress char(15) NULL,
	Created datetime NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_OrderTransactions ADD CONSTRAINT
	DF_OrderTransactions_Created DEFAULT (getdate()) FOR Created
GO
SET IDENTITY_INSERT dbo.Tmp_OrderTransactions ON
GO
IF EXISTS(SELECT * FROM dbo.OrderTransactions)
	 EXEC('INSERT INTO dbo.Tmp_OrderTransactions (ID, OrderID, Type, GC_OrderNumber, GC_NewFinanceState, GC_NewFulfillmentState, GC_PrevFinanceState, GC_PrevFulfillmentState, GC_ChargedAmount, GC_RefundedAmount, GC_ChargebackAmount, MemberUID, Operation, Created)
		SELECT ID, OrderID, Type, GC_OrderNumber, GC_NewFinanceState, GC_NewFulfillmentState, GC_PrevFinanceState, GC_PrevFulfillmentState, GC_ChargedAmount, GC_RefundedAmount, GC_ChargebackAmount, MemberUID, Operation, Created FROM dbo.OrderTransactions WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_OrderTransactions OFF
GO
DROP TABLE dbo.OrderTransactions
GO
EXECUTE sp_rename N'dbo.Tmp_OrderTransactions', N'OrderTransactions', 'OBJECT' 
GO
ALTER TABLE dbo.OrderTransactions ADD CONSTRAINT
	PK_OrderTransactions PRIMARY KEY CLUSTERED 
	(
	ID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
CREATE NONCLUSTERED INDEX IDX_Secondary ON dbo.OrderTransactions
	(
	OrderID,
	Created
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE dbo.OrderTransactions ADD CONSTRAINT
	FK_OrderTransactions_Orders FOREIGN KEY
	(
	OrderID
	) REFERENCES dbo.Orders
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
COMMIT

-- ************************************************************************

IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'GetCollectionPhotos')
	BEGIN
		DROP Procedure dbo.GetCollectionPhotos
	END
GO

CREATE Procedure dbo.GetCollectionPhotos
(
	@CollectionID int
)
AS
	SELECT 
		p.*, 
		cp.[Order]
		FROM Photos p
		INNER JOIN CollectionPhotos cp ON cp.PhotoID = p.ID
		WHERE cp.CollectionID = @CollectionID
		ORDER BY cp.[Order]
GO

GRANT EXEC ON dbo.GetCollectionPhotos TO PUBLIC
GO

-- ************************************************************************