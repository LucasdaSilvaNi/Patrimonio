IF NOT EXISTS (
  SELECT
    *
  FROM
    INFORMATION_SCHEMA.COLUMNS
  WHERE
    TABLE_NAME = '[User]' AND COLUMN_NAME = 'DataUltimoTreinamento')
BEGIN
 Alter table [User] Add DataUltimoTreinamento datetime
END;
