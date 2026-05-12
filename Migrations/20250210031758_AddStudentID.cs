using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CS395SI_Spring2023_Group1.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Spring2023_Group1_Profile_Sys",
                columns: table => new
                {
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ID = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNum = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LicenseNum = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LicenseState = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LicenseIssue = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LicenseExp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LicenseClass = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Under18 = table.Column<string>(type: "nvarchar(1)", nullable: true),
                    PGName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HomePhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AltPhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    USCitizen = table.Column<string>(type: "nvarchar(1)", nullable: true),
                    Race = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sex = table.Column<string>(type: "nvarchar(1)", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HSGrad = table.Column<string>(type: "nvarchar(1)", nullable: true),
                    SchoolName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SchoolAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Grade = table.Column<int>(type: "int", nullable: true),
                    Employed = table.Column<string>(type: "nvarchar(1)", nullable: true),
                    BusinessName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BusinessAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BusinessPhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Talents = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmerName2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmerName3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmerRelation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmerRelation2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmerRelation3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmerPhoneNum1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmerPhoneNum2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmerPhoneNum3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmerAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmerAddress2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmerAddress3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApplicationStatus = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spring2023_Group1_Profile_Sys", x => x.Email);
                });

            // the below were freshly added to update the student id 
            migrationBuilder.AddColumn<int>(
            name: "ID",
            table: "Spring2023_Group1_Profile_Sys",
            type: "int",
            nullable: false,
            defaultValue: 0);


            migrationBuilder.CreateTable(
                name: "Spring2023_Group1_Schedules",
                columns: table => new
                {
                    ScheduleID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ServiceID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SectionID = table.Column<int>(type: "int", nullable: true),
                    ScheduleStatus = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spring2023_Group1_Schedules", x => x.ScheduleID);
                });

            migrationBuilder.CreateTable(
                name: "Spring2023_Group1_Scheduling_Form",
                columns: table => new
                {
                    RequestID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsOnSunday = table.Column<bool>(type: "bit", nullable: false),
                    IsOnMonday = table.Column<bool>(type: "bit", nullable: false),
                    IsOnTuesday = table.Column<bool>(type: "bit", nullable: false),
                    IsOnWednesday = table.Column<bool>(type: "bit", nullable: false),
                    IsOnThursday = table.Column<bool>(type: "bit", nullable: false),
                    IsOnFriday = table.Column<bool>(type: "bit", nullable: false),
                    IsOnSaturday = table.Column<bool>(type: "bit", nullable: false),
                    StartMonth = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EndMonth = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spring2023_Group1_Scheduling_Form", x => x.RequestID);
                });

            migrationBuilder.CreateTable(
                name: "Spring2023_Group1_Services",
                columns: table => new
                {
                    ServiceID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ServiceName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ServiceFrequency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ServiceDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ServiceHours = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spring2023_Group1_Services", x => x.ServiceID);
                });

            migrationBuilder.CreateTable(
                name: "Spring2024_Group2_Schedule",
                columns: table => new
                {
                    ScheduleID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ServiceID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ServiceName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SectionID = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WeekDay = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spring2024_Group2_Schedule", x => x.ScheduleID);
                });

            migrationBuilder.CreateTable(
                name: "Spring2024_Group2_Sections",
                columns: table => new
                {
                    sectionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    serviceID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    serviceName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    weekDay = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    startTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    endTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spring2024_Group2_Sections", x => x.sectionID);
                });

            migrationBuilder.CreateTable(
                name: "Spring2024_Group2_Session",
                columns: table => new
                {
                    SessionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceID = table.Column<int>(type: "int", nullable: true),
                    ServiceName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SectionID = table.Column<int>(type: "int", nullable: true),
                    MeetingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spring2024_Group2_Session", x => x.SessionID);
                });
        
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Spring2023_Group1_Profile_Sys");

            migrationBuilder.DropColumn(
                name: "ID",
                table: "Spring2023_Group1_Profile_Sys");

            migrationBuilder.DropTable(
                name: "Spring2023_Group1_Schedules");

            migrationBuilder.DropTable(
                name: "Spring2023_Group1_Scheduling_Form");

            migrationBuilder.DropTable(
                name: "Spring2023_Group1_Services");

            migrationBuilder.DropTable(
                name: "Spring2024_Group2_Schedule");

            migrationBuilder.DropTable(
                name: "Spring2024_Group2_Sections");

            migrationBuilder.DropTable(
                name: "Spring2024_Group2_Session");
        }
    }
}
