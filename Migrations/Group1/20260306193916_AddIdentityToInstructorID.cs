using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CS395SI_Spring2023_Group1.Migrations
{
    /// <inheritdoc />
    public partial class AddIdentityToInstructorID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Only convert InstructorID to IDENTITY - ignore all other schema differences
            migrationBuilder.Sql(@"
                -- Store FK constraint information before dropping
                DECLARE @FKs TABLE (
                    FKName NVARCHAR(255), 
                    TableName NVARCHAR(255), 
                    ColumnName NVARCHAR(255)
                );
                
                INSERT INTO @FKs
                SELECT 
                    fk.name,
                    OBJECT_NAME(fk.parent_object_id),
                    COL_NAME(fkc.parent_object_id, fkc.parent_column_id)
                FROM sys.foreign_keys AS fk
                INNER JOIN sys.foreign_key_columns AS fkc 
                    ON fk.object_id = fkc.constraint_object_id
                WHERE OBJECT_NAME(fk.referenced_object_id) = 'Spring2026_Group1_Instructor';

                -- Drop all foreign keys referencing Instructor table
                DECLARE @dropFKSQL NVARCHAR(MAX) = '';
                SELECT @dropFKSQL += 'ALTER TABLE ' + QUOTENAME(TableName) + 
                                    ' DROP CONSTRAINT ' + QUOTENAME(FKName) + ';' + CHAR(13)
                FROM @FKs;
                
                IF LEN(@dropFKSQL) > 0
                    EXEC sp_executesql @dropFKSQL;

                -- Create new table with IDENTITY column
                CREATE TABLE Spring2026_Group1_Instructor_New (
                    InstructorID INT IDENTITY(1,1) NOT NULL,
                    Email NVARCHAR(128) NOT NULL,
                    FullName NVARCHAR(200) NOT NULL,
                    HireDate DATETIME2 NULL,
                    IsActive BIT NULL,
                    EndDate DATETIME2 NULL,
                    Speciality NVARCHAR(1000) NULL,
                    ApprovedDate DATETIME2 NOT NULL,
                    ApprovedBy NVARCHAR(128) NOT NULL,
                    Status NVARCHAR(50) NOT NULL,
                    CONSTRAINT PK_Spring2026_Group1_Instructor PRIMARY KEY (InstructorID)
                );

                -- Copy existing data if table has records
                IF EXISTS (SELECT 1 FROM Spring2026_Group1_Instructor)
                BEGIN
                    SET IDENTITY_INSERT Spring2026_Group1_Instructor_New ON;
                    
                    INSERT INTO Spring2026_Group1_Instructor_New 
                        (InstructorID, Email, FullName, HireDate, IsActive, EndDate, 
                         Speciality, ApprovedDate, ApprovedBy, Status)
                    SELECT 
                        InstructorID, Email, FullName, HireDate, IsActive, EndDate, 
                        Speciality, ApprovedDate, ApprovedBy, Status
                    FROM Spring2026_Group1_Instructor;
                    
                    SET IDENTITY_INSERT Spring2026_Group1_Instructor_New OFF;
                END

                -- Drop old table and rename new one
                DROP TABLE Spring2026_Group1_Instructor;
                EXEC sp_rename 'Spring2026_Group1_Instructor_New', 'Spring2026_Group1_Instructor';

                -- Recreate all foreign keys
                DECLARE @createFKSQL NVARCHAR(MAX) = '';
                SELECT @createFKSQL += 
                    'ALTER TABLE ' + QUOTENAME(TableName) + 
                    ' ADD CONSTRAINT ' + QUOTENAME(FKName) + 
                    ' FOREIGN KEY (' + QUOTENAME(ColumnName) + ') ' +
                    'REFERENCES Spring2026_Group1_Instructor(InstructorID) ON DELETE CASCADE;' + CHAR(13)
                FROM @FKs;
                
                IF LEN(@createFKSQL) > 0
                    EXEC sp_executesql @createFKSQL;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            throw new NotSupportedException(
                "Rolling back an IDENTITY column change is not supported.");
        }
    }
}
