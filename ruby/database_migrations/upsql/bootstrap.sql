SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
	
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id=OBJECT_ID(N'DBMigrations') AND OBJECTPROPERTY(id, N'IsUserTable') = 1) BEGIN
	CREATE TABLE DBMigrations(
		Migration INT NOT NULL,
		DateDeveloped DATETIME NOT NULL,
		DateApplied DATETIME NOT NULL,
		Title NVARCHAR(1000)
	)
END
GO
