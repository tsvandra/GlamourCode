-- Complete database setup script
-- Run this script to set up the entire database structure correctly

-- Create AppointmentStatuses table if it doesn't exist (seems like this one already exists)
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

-- Create Clients table if it doesn't exist
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Clients')
BEGIN
    CREATE TABLE Clients (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Name NVARCHAR(100) NOT NULL,
        PhoneNumber NVARCHAR(20) NOT NULL,
        Email NVARCHAR(100) NOT NULL
    );
    
    PRINT 'Clients table created';
END
ELSE
BEGIN
    PRINT 'Clients table already exists';
END

-- Create Services table if it doesn't exist
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Services')
BEGIN
    CREATE TABLE Services (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Name NVARCHAR(100) NOT NULL,
        Description NVARCHAR(500) NULL,
        Price DECIMAL(10, 2) NOT NULL,
        DurationMinutes INT NOT NULL
    );
    
    -- Insert default services
    INSERT INTO Services (Name, Description, Price, DurationMinutes)
    VALUES ('Haircut', 'Basic haircut service', 30.00, 30),
           ('Hair Coloring', 'Full hair coloring service', 80.00, 120),
           ('Styling', 'Hair styling service', 40.00, 45);
    
    PRINT 'Services table created and populated with default services';
END
ELSE
BEGIN
    PRINT 'Services table already exists';
END

-- Create Stylists table if it doesn't exist
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Stylists')
BEGIN
    CREATE TABLE Stylists (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Name NVARCHAR(100) NOT NULL,
        Specialization NVARCHAR(100) NULL
    );
    
    -- Insert default stylists
    INSERT INTO Stylists (Name, Specialization)
    VALUES ('John Smith', 'Cutting'),
           ('Maria Garcia', 'Coloring');
    
    PRINT 'Stylists table created and populated with default stylists';
END
ELSE
BEGIN
    PRINT 'Stylists table already exists';
END

-- Create a join table for the many-to-many relationship between Stylists and Services
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'StylistService')
BEGIN
    CREATE TABLE StylistService (
        StylistId INT NOT NULL,
        ServiceId INT NOT NULL,
        PRIMARY KEY (StylistId, ServiceId),
        FOREIGN KEY (StylistId) REFERENCES Stylists(Id),
        FOREIGN KEY (ServiceId) REFERENCES Services(Id)
    );
    
    PRINT 'StylistService join table created';
END
ELSE
BEGIN
    PRINT 'StylistService table already exists';
END

-- Create Appointments table if it doesn't exist
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Appointments')
BEGIN
    CREATE TABLE Appointments (
        Id INT PRIMARY KEY IDENTITY(1,1),
        DateTime DATETIME2 NOT NULL,
        ClientId INT NOT NULL,
        StylistId INT NOT NULL,
        ServiceId INT NOT NULL,
        StatusId INT NOT NULL DEFAULT 1,
        FOREIGN KEY (ClientId) REFERENCES Clients(Id),
        FOREIGN KEY (StylistId) REFERENCES Stylists(Id),
        FOREIGN KEY (ServiceId) REFERENCES Services(Id),
        FOREIGN KEY (StatusId) REFERENCES AppointmentStatuses(Id)
    );
    
    PRINT 'Appointments table created with proper foreign keys';
END
ELSE
BEGIN
    PRINT 'Appointments table already exists';
    
    -- Make sure StatusId column exists and has appropriate foreign key
    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                WHERE TABLE_NAME = 'Appointments' AND COLUMN_NAME = 'StatusId')
    BEGIN
        ALTER TABLE Appointments ADD StatusId INT DEFAULT 1;
        PRINT 'StatusId column added to Appointments table';
    END
    
    -- Check for and add the foreign key if it doesn't exist
    IF NOT EXISTS (SELECT * FROM sys.foreign_keys
                WHERE parent_object_id = OBJECT_ID('Appointments')
                AND referenced_object_id = OBJECT_ID('AppointmentStatuses'))
    BEGIN
        ALTER TABLE Appointments 
        ADD CONSTRAINT FK_Appointments_AppointmentStatuses 
        FOREIGN KEY (StatusId) REFERENCES AppointmentStatuses(Id);
        
        PRINT 'Added foreign key constraint';
    END
END

PRINT 'Database setup completed';