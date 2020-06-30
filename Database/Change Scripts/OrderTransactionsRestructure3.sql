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
	Created datetime NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_OrderTransactions ADD CONSTRAINT
	DF_OrderTransactions_Created DEFAULT (getdate()) FOR Created
GO
SET IDENTITY_INSERT dbo.Tmp_OrderTransactions ON
GO
IF EXISTS(SELECT * FROM dbo.OrderTransactions)
	 EXEC('INSERT INTO dbo.Tmp_OrderTransactions (ID, OrderID, Type, GC_OrderNumber, GC_NewFinanceState, GC_NewFulfillmentState, GC_PrevFinanceState, GC_PrevFulfillmentState, GC_ChargedAmount, GC_RefundedAmount, GC_ChargebackAmount, Created)
		SELECT ID, OrderID, Type, GC_OrderNumber, GC_NewFinanceState, GC_NewFulfillmentState, GC_PrevFinanceState, GC_PrevFulfillmentState, GC_ChargedAmount, GC_RefundedAmount, GC_ChargebackAmount, Created FROM dbo.OrderTransactions WITH (HOLDLOCK TABLOCKX)')
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
