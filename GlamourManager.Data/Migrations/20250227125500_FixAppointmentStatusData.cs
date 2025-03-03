using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GlamourManager.Data.Migrations
{
    /// <summary>
    /// This migration fixes the data issue with the existing appointments 
    /// having StatusId values that don't match the new AppointmentStatuses table.
    /// </summary>
    public partial class FixAppointmentStatusData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // First check if AppointmentStatuses table exists but is empty
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AppointmentStatuses') 
                AND NOT EXISTS (SELECT * FROM AppointmentStatuses)
                BEGIN
                    -- Insert the status values
                    INSERT INTO AppointmentStatuses (Id, Name)
                    VALUES (1, 'Pending'),
                           (2, 'Accepted'),
                           (3, 'Refused'),
                           (4, 'Cancelled')
                END
            ");

            // If the table doesn't exist, the previous migration will create it
            
            // Update existing appointments to map from the string Status to StatusId
            // This assumes there are existing appointments with string Status values
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                           WHERE TABLE_NAME = 'Appointments' AND COLUMN_NAME = 'Status' AND COLUMN_NAME = 'StatusId')
                BEGIN
                    -- Update StatusId based on Status string
                    UPDATE Appointments SET StatusId = 1 WHERE Status = 'Pending'
                    UPDATE Appointments SET StatusId = 2 WHERE Status = 'Accepted'
                    UPDATE Appointments SET StatusId = 3 WHERE Status = 'Refused'
                    UPDATE Appointments SET StatusId = 4 WHERE Status = 'Cancelled'
                    
                    -- Set default for any other values
                    UPDATE Appointments SET StatusId = 1 WHERE StatusId NOT IN (1, 2, 3, 4)
                END
                ELSE IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                                WHERE TABLE_NAME = 'Appointments' AND COLUMN_NAME = 'StatusId')
                BEGIN
                    -- Set default StatusId = 1 (Pending) for any existing records
                    UPDATE Appointments SET StatusId = 1 WHERE StatusId NOT IN (1, 2, 3, 4)
                END
            ");

            migrationBuilder.InsertData(
                table: "AppointmentStatuses",
                columns: new[] { "Id", "Name" },
                values: new object[] { 5, "Done" }
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Can't really undo this data fix without additional information about previous state

            migrationBuilder.DeleteData(
                table: "AppointmentStatuses",
                keyColumn: "Id",
                keyValue: 5);
        }
    }
}