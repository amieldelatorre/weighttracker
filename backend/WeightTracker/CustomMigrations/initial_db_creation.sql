BEGIN;
	CREATE TABLE IF NOT EXISTS user (
		Id                  SERIAL PRIMARY KEY
		,FirstName			varchar(255)		NOT NULL
		,LastName			varchar(255)		NOT NULL
		,Email				varchar(255)		NOT NULL UNIQUE
		,Password			varchar				NOT NULL
		,DateOfBirth		DATE				NOT NULL
		,Gender				varchar(255)		NOT NULL
		,Height				DOUBLE PRECISION	NOT NULL
	);

-- ROLLBACK;
COMMIT;