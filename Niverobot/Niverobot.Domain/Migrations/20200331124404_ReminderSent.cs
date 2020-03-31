using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niverobot.Domain.Migrations
{
    public partial class ReminderSent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<bool>(
                name: "Sent",
                table: "Reminders",
                nullable: false,
                defaultValue: false);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sent",
                table: "Reminders");
        }
    }
}
