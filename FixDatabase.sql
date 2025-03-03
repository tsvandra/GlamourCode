-- SQL Script to fix database relationship issues
-- Save this as FixDatabase.sql and run it directly in SQL Server Management Studio
-- or via sqlcmd -S NITRO -d Glamour -i FixDatabase.sql

-- Create AppointmentStatuses table if it doesn't exist
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AppointmentStatuses')
BEGIN
    CREATE TABLE AppointmentStatuses (
        Id INT PRIMARY KEY,
        Name NVARCHAR(50) NOT NULL
    );
    
    -- Insert the status values
    INSERT INTO AppointmentStatuses (Id, Name)
    VALUES (1, 'Pending'),
           (2, 'Accepted'),
           (3, 'Refused'),
           (4, 'Cancelled');
    
    PRINT 'AppointmentStatuses table created and populated';
END
ELSE
BEGIN
    -- Make sure the AppointmentStatuses table has the correct values
    IF NOT EXISTS (SELECT * FROM AppointmentStatuses)
    BEGIN
        INSERT INTO AppointmentStatuses (Id, Name)
        VALUES (1, 'Pending'),
               (2, 'Accepted'),
               (3, 'Refused'),
               (4, 'Cancelled');
        
        PRINT 'AppointmentStatuses table populated with required values';
    END
    ELSE
    BEGIN
        PRINT 'AppointmentStatuses table already exists with data';
    END
END

-- Check if Appointments table has StatusId column, if not add it
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Appointments')
    AND NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                 WHERE TABLE_NAME = 'Appointments' AND COLUMN_NAME = 'StatusId')
BEGIN
    ALTER TABLE Appointments ADD StatusId INT DEFAULT 1;
    PRINT 'StatusId column added to Appointments table';
END

-- If Status column exists, map from text status to StatusId values
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
          WHERE TABLE_NAME = 'Appointments' AND COLUMN_NAME = 'Status')
BEGIN
    UPDATE Appointments SET StatusId = 1 WHERE Status = 'Pending';
    UPDATE Appointments SET StatusId = 2 WHERE Status = 'Accepted';
    UPDATE Appointments SET StatusId = 3 WHERE Status = 'Refused';
    UPDATE Appointments SET StatusId = 4 WHERE Status = 'Cancelled';
    
    -- Make sure all records have a valid StatusId
    UPDATE Appointments SET StatusId = 1 
    WHERE StatusId IS NULL OR StatusId NOT IN (1, 2, 3, 4);
    
    PRINT 'Updated StatusId values based on Status text';
END
ELSE
BEGIN
    -- Make sure all records have a valid StatusId
    UPDATE Appointments SET StatusId = 1 
    WHERE StatusId IS NULL OR StatusId NOT IN (1, 2, 3, 4);
    
    PRINT 'Ensured all Appointments have valid StatusId values';
END

-- Check for and remove any existing foreign key that might cause problems
DECLARE @constraint_name NVARCHAR(128);
SELECT @constraint_name = name
FROM sys.foreign_keys
WHERE parent_object_id = OBJECT_ID('Appointments')
  AND referenced_object_id = OBJECT_ID('AppointmentStatuses');

IF @constraint_name IS NOT NULL
BEGIN
    DECLARE @sql NVARCHAR(MAX) = N'ALTER TABLE Appointments DROP CONSTRAINT ' + @constraint_name;
    EXEC sp_executesql @sql;
    PRINT 'Removed existing foreign key constraint: ' + @constraint_name;
END

-- Add the foreign key constraint
BEGIN TRY
    ALTER TABLE Appointments 
    ADD CONSTRAINT FK_Appointments_AppointmentStatuses 
    FOREIGN KEY (StatusId) REFERENCES AppointmentStatuses(Id);
    
    PRINT 'Added foreign key constraint successfully';
END TRY
BEGIN CATCH
    PRINT 'Error adding foreign key constraint: ' + ERROR_MESSAGE();
END CATCH

-- Remove Status column if it exists and migration succeeded
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
          WHERE TABLE_NAME = 'Appointments' AND COLUMN_NAME = 'Status')
    AND EXISTS (SELECT * FROM sys.foreign_keys
               WHERE name = 'FK_Appointments_AppointmentStatuses')
BEGIN
    ALTER TABLE Appointments DROP COLUMN Status;
    PRINT 'Removed deprecated Status column';
END

PRINT 'Database update completed';