using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AccessToBlazer.Web.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    CustomerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerName = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    City = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.CustomerId);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductName = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnitsInStock = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductId);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    OrderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_Orders_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "CustomerId", "City", "CustomerName", "Phone" },
                values: new object[,]
                {
                    { 1, "Springfield", "Acme Corp", "555-0101" },
                    { 2, "Shelbyville", "Globex Inc", "555-0142" },
                    { 3, "Austin", "Initech", "555-0177" },
                    { 4, "Palo Alto", "Hooli", "555-0113" },
                    { 5, "New York", "Stark Industries", "555-0190" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "ProductName", "UnitPrice", "UnitsInStock" },
                values: new object[,]
                {
                    { 1, "Widget A", 9.99m, 120 },
                    { 2, "Widget B", 14.50m, 85 },
                    { 3, "Gadget X", 39.95m, 40 },
                    { 4, "Gizmo Basic", 5.25m, 300 },
                    { 5, "Gizmo Pro", 19.99m, 150 },
                    { 6, "Turbo Chip", 79.00m, 12 }
                });

            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "OrderId", "CustomerId", "OrderDate", "ProductId", "Quantity" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 10 },
                    { 2, 1, new DateTime(2026, 1, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, 25 },
                    { 3, 2, new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 2 },
                    { 4, 3, new DateTime(2026, 2, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 8 },
                    { 5, 4, new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 6, 1 },
                    { 6, 5, new DateTime(2026, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 12 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ProductId",
                table: "Orders",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
