using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace crafts_api.Migrations
{
    /// <inheritdoc />
    public partial class AddCraftmans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Crafters_Categories_CategoryPublicId",
                table: "Crafters");

            migrationBuilder.DropForeignKey(
                name: "FK_Crafters_Users_UserPublicId",
                table: "Crafters");

            migrationBuilder.DropIndex(
                name: "IX_Crafters_CategoryPublicId",
                table: "Crafters");

            migrationBuilder.DropIndex(
                name: "IX_Crafters_UserPublicId",
                table: "Crafters");

            migrationBuilder.DropColumn(
                name: "CategoryPublicId",
                table: "Crafters");

            migrationBuilder.DropColumn(
                name: "UserPublicId",
                table: "Crafters");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Crafters",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Crafters",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Crafters",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "IdentityId",
                table: "Crafters",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Crafters",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Crafters",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Crafters",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "Crafters",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Crafters_PublicId",
                table: "Crafters",
                column: "PublicId");

            migrationBuilder.CreateTable(
                name: "CraftsmanProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CraftsmanPublicId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Bio = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PhoneNumber = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Address = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    City = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Country = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Street = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Number = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PostalCode = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProfilePicture = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CraftsmanProfiles", x => x.Id);
                    table.UniqueConstraint("AK_CraftsmanProfiles_CraftsmanPublicId", x => x.CraftsmanPublicId);
                    table.ForeignKey(
                        name: "FK_CraftsmanProfiles_Crafters_CraftsmanPublicId",
                        column: x => x.CraftsmanPublicId,
                        principalTable: "Crafters",
                        principalColumn: "PublicId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PublicId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CategoryPublicId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.Id);
                    table.UniqueConstraint("AK_Services_PublicId", x => x.PublicId);
                    table.ForeignKey(
                        name: "FK_Services_Categories_CategoryPublicId",
                        column: x => x.CategoryPublicId,
                        principalTable: "Categories",
                        principalColumn: "PublicId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CraftsmanServices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CraftsmanProfileCraftsmanPublicId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ServicePublicId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Price = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    Duration = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CraftsmanServices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CraftsmanServices_CraftsmanProfiles_CraftsmanProfileCraftsma~",
                        column: x => x.CraftsmanProfileCraftsmanPublicId,
                        principalTable: "CraftsmanProfiles",
                        principalColumn: "CraftsmanPublicId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CraftsmanServices_Services_ServicePublicId",
                        column: x => x.ServicePublicId,
                        principalTable: "Services",
                        principalColumn: "PublicId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Crafters_IdentityId",
                table: "Crafters",
                column: "IdentityId");

            migrationBuilder.CreateIndex(
                name: "IX_CraftsmanServices_CraftsmanProfileCraftsmanPublicId",
                table: "CraftsmanServices",
                column: "CraftsmanProfileCraftsmanPublicId");

            migrationBuilder.CreateIndex(
                name: "IX_CraftsmanServices_ServicePublicId",
                table: "CraftsmanServices",
                column: "ServicePublicId");

            migrationBuilder.CreateIndex(
                name: "IX_Services_CategoryPublicId",
                table: "Services",
                column: "CategoryPublicId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CraftsmanServices");

            migrationBuilder.DropTable(
                name: "CraftsmanProfiles");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Crafters_PublicId",
                table: "Crafters");

            migrationBuilder.DropIndex(
                name: "IX_Crafters_IdentityId",
                table: "Crafters");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Crafters");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Crafters");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Crafters");

            migrationBuilder.DropColumn(
                name: "IdentityId",
                table: "Crafters");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Crafters");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Crafters");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Crafters");

            migrationBuilder.DropColumn(
                name: "Username",
                table: "Crafters");

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryPublicId",
                table: "Crafters",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "UserPublicId",
                table: "Crafters",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_Crafters_CategoryPublicId",
                table: "Crafters",
                column: "CategoryPublicId");

            migrationBuilder.CreateIndex(
                name: "IX_Crafters_UserPublicId",
                table: "Crafters",
                column: "UserPublicId");

            migrationBuilder.AddForeignKey(
                name: "FK_Crafters_Categories_CategoryPublicId",
                table: "Crafters",
                column: "CategoryPublicId",
                principalTable: "Categories",
                principalColumn: "PublicId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Crafters_Users_UserPublicId",
                table: "Crafters",
                column: "UserPublicId",
                principalTable: "Users",
                principalColumn: "PublicId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
