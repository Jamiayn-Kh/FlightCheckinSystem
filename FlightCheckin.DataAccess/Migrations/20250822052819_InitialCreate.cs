using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FlightCheckin.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BoardingPasses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FlightId = table.Column<int>(type: "INTEGER", nullable: false),
                    FlightNumber = table.Column<string>(type: "TEXT", nullable: false),
                    PassengerId = table.Column<int>(type: "INTEGER", nullable: false),
                    PassengerName = table.Column<string>(type: "TEXT", nullable: false),
                    PassportNumber = table.Column<string>(type: "TEXT", nullable: false),
                    SeatCode = table.Column<string>(type: "TEXT", nullable: false),
                    IssuedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoardingPasses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Flights",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FlightNumber = table.Column<string>(type: "TEXT", nullable: false),
                    Destination = table.Column<string>(type: "TEXT", nullable: false),
                    DepartureTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flights", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Passengers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PassportNumber = table.Column<string>(type: "TEXT", nullable: false),
                    FullName = table.Column<string>(type: "TEXT", nullable: false),
                    FlightId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Passengers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Passengers_Flights_FlightId",
                        column: x => x.FlightId,
                        principalTable: "Flights",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Seats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FlightId = table.Column<int>(type: "INTEGER", nullable: false),
                    Row = table.Column<int>(type: "INTEGER", nullable: false),
                    Column = table.Column<string>(type: "TEXT", nullable: false),
                    IsTaken = table.Column<bool>(type: "INTEGER", nullable: false),
                    AssignedPassengerId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Seats_Flights_FlightId",
                        column: x => x.FlightId,
                        principalTable: "Flights",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Seats_Passengers_AssignedPassengerId",
                        column: x => x.AssignedPassengerId,
                        principalTable: "Passengers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "Flights",
                columns: new[] { "Id", "DepartureTime", "Destination", "FlightNumber", "Status" },
                values: new object[] { 1, new DateTime(2025, 8, 22, 11, 28, 18, 899, DateTimeKind.Utc).AddTicks(96), "Tokyo (NRT)", "MGL101", 0 });

            migrationBuilder.InsertData(
                table: "Seats",
                columns: new[] { "Id", "AssignedPassengerId", "Column", "FlightId", "IsTaken", "Row" },
                values: new object[,]
                {
                    { 1, null, "A", 1, false, 1 },
                    { 2, null, "B", 1, false, 1 },
                    { 3, null, "C", 1, false, 1 },
                    { 4, null, "D", 1, false, 1 },
                    { 5, null, "E", 1, false, 1 },
                    { 6, null, "F", 1, false, 1 },
                    { 7, null, "A", 1, false, 2 },
                    { 8, null, "B", 1, false, 2 },
                    { 9, null, "C", 1, false, 2 },
                    { 10, null, "D", 1, false, 2 },
                    { 11, null, "E", 1, false, 2 },
                    { 12, null, "F", 1, false, 2 },
                    { 13, null, "A", 1, false, 3 },
                    { 14, null, "B", 1, false, 3 },
                    { 15, null, "C", 1, false, 3 },
                    { 16, null, "D", 1, false, 3 },
                    { 17, null, "E", 1, false, 3 },
                    { 18, null, "F", 1, false, 3 },
                    { 19, null, "A", 1, false, 4 },
                    { 20, null, "B", 1, false, 4 },
                    { 21, null, "C", 1, false, 4 },
                    { 22, null, "D", 1, false, 4 },
                    { 23, null, "E", 1, false, 4 },
                    { 24, null, "F", 1, false, 4 },
                    { 25, null, "A", 1, false, 5 },
                    { 26, null, "B", 1, false, 5 },
                    { 27, null, "C", 1, false, 5 },
                    { 28, null, "D", 1, false, 5 },
                    { 29, null, "E", 1, false, 5 },
                    { 30, null, "F", 1, false, 5 },
                    { 31, null, "A", 1, false, 6 },
                    { 32, null, "B", 1, false, 6 },
                    { 33, null, "C", 1, false, 6 },
                    { 34, null, "D", 1, false, 6 },
                    { 35, null, "E", 1, false, 6 },
                    { 36, null, "F", 1, false, 6 },
                    { 37, null, "A", 1, false, 7 },
                    { 38, null, "B", 1, false, 7 },
                    { 39, null, "C", 1, false, 7 },
                    { 40, null, "D", 1, false, 7 },
                    { 41, null, "E", 1, false, 7 },
                    { 42, null, "F", 1, false, 7 },
                    { 43, null, "A", 1, false, 8 },
                    { 44, null, "B", 1, false, 8 },
                    { 45, null, "C", 1, false, 8 },
                    { 46, null, "D", 1, false, 8 },
                    { 47, null, "E", 1, false, 8 },
                    { 48, null, "F", 1, false, 8 },
                    { 49, null, "A", 1, false, 9 },
                    { 50, null, "B", 1, false, 9 },
                    { 51, null, "C", 1, false, 9 },
                    { 52, null, "D", 1, false, 9 },
                    { 53, null, "E", 1, false, 9 },
                    { 54, null, "F", 1, false, 9 },
                    { 55, null, "A", 1, false, 10 },
                    { 56, null, "B", 1, false, 10 },
                    { 57, null, "C", 1, false, 10 },
                    { 58, null, "D", 1, false, 10 },
                    { 59, null, "E", 1, false, 10 },
                    { 60, null, "F", 1, false, 10 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Passengers_FlightId",
                table: "Passengers",
                column: "FlightId");

            migrationBuilder.CreateIndex(
                name: "IX_Passengers_PassportNumber",
                table: "Passengers",
                column: "PassportNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Seats_AssignedPassengerId",
                table: "Seats",
                column: "AssignedPassengerId");

            migrationBuilder.CreateIndex(
                name: "IX_Seats_FlightId_Row_Column",
                table: "Seats",
                columns: new[] { "FlightId", "Row", "Column" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BoardingPasses");

            migrationBuilder.DropTable(
                name: "Seats");

            migrationBuilder.DropTable(
                name: "Passengers");

            migrationBuilder.DropTable(
                name: "Flights");
        }
    }
}
