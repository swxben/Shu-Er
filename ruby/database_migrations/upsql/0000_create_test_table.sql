DECLARE @Migration INT
DECLARE @DateDeveloped DATETIME
DECLARE @Title NVARCHAR(1000)

SET @Migration = 0
SET @DateDeveloped = '2012-01-01'
SET @Title = 'create test table'

INSERT INTO DBMigrations(Migration, DateDeveloped, DateApplied, Title) VALUES(@Migration, @DateDeveloped, GETDATE(), @Title)

/* Migration script: */

CREATE TABLE TestTable(
	FirstName NVARCHAR(100) NOT NULL,
	Surname NVARCHAR(100) NOT NULL
)
GO

INSERT INTO TestTable(FirstName, Surname) VALUES('Lucius', 'Fox')
GO
