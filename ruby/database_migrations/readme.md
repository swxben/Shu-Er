# Database migrations

This has only been tested and developed on Windows for SQL Server databases using SQL Server authentication.

Migrations go in the `upsql` directory. Stick to the naming convention, increase the migration number by one for each migration file.

The script includes a bootstrapper which creates the `DBMigrations` table. The 0th migration creates a test table. This should be replaced with your initial database setup - it could be a dump of the current database schema which would let you run up a full database from scratch, and manually create the 0th migration in the production database so it isn't run again.

The `migratedb.rb` script should be modified to match the testing environment. Mupltiple databases can be migrated at the same time by separating the database names with a comma. This runs the entire migration procedure on each specified database. I use this to run the migrations against two local development databases - one for user acceptance testing (containing test data) and the other for running integration tests (data gets inserted and cleared by tests).

