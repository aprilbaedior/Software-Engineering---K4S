using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CS395SI_Spring2023_Group1.Migrations
{
    /// <inheritdoc />
    public partial class SyncModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Certificate table: rename IssueDate -> IssuedDate
            migrationBuilder.RenameColumn(
                name: "IssueDate",
                table: "Spring2026_Group1_Certificate",
                newName: "IssuedDate");

            // Certificate table: make CertificateStatus nullable
            migrationBuilder.AlterColumn<string>(
                name: "CertificateStatus",
                table: "Spring2026_Group1_Certificate",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldNullable: false);

            // Certificate table: add new columns
            migrationBuilder.AddColumn<string>(
                name: "IssuedBy",
                table: "Spring2026_Group1_Certificate",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Spring2026_Group1_Certificate",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SectionID",
                table: "Spring2026_Group1_Certificate",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ServiceName",
                table: "Spring2026_Group1_Certificate",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StudentEmail",
                table: "Spring2026_Group1_Certificate",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StudentName",
                table: "Spring2026_Group1_Certificate",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            // Sections table: make ServiceName nullable
            migrationBuilder.AlterColumn<string>(
                name: "ServiceName",
                table: "Spring2026_Group1_Sections",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IssuedBy",
                table: "Spring2026_Group1_Certificate");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Spring2026_Group1_Certificate");

            migrationBuilder.DropColumn(
                name: "SectionID",
                table: "Spring2026_Group1_Certificate");

            migrationBuilder.DropColumn(
                name: "ServiceName",
                table: "Spring2026_Group1_Certificate");

            migrationBuilder.DropColumn(
                name: "StudentEmail",
                table: "Spring2026_Group1_Certificate");

            migrationBuilder.DropColumn(
                name: "StudentName",
                table: "Spring2026_Group1_Certificate");

            migrationBuilder.RenameColumn(
                name: "IssuedDate",
                table: "Spring2026_Group1_Certificate",
                newName: "IssueDate");

            migrationBuilder.AlterColumn<string>(
                name: "ServiceName",
                table: "Spring2026_Group1_Sections",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true);
        }
    }
}
