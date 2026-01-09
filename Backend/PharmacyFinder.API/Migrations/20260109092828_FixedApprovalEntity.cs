using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacyFinder.API.Migrations
{
    /// <inheritdoc />
    public partial class FixedApprovalEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ApprovedByUserId",
                table: "PharmacyApprovalHistories",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "ApprovalStatus",
                table: "PharmacyApprovalHistories",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Pending");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "PharmacyApprovalHistories",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovalStatus",
                table: "PharmacyApprovalHistories");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "PharmacyApprovalHistories");

            migrationBuilder.AlterColumn<int>(
                name: "ApprovedByUserId",
                table: "PharmacyApprovalHistories",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
