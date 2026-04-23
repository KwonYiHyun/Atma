using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Empen.Migrations.PersonDb
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "person_achievement",
                columns: table => new
                {
                    person_achievement_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    person_id = table.Column<int>(type: "int", nullable: false),
                    achievement_id = table.Column<int>(type: "int", nullable: false),
                    insert_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_person_achievement", x => x.person_achievement_id);
                });

            migrationBuilder.CreateTable(
                name: "person_character",
                columns: table => new
                {
                    person_character_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    person_id = table.Column<int>(type: "int", nullable: false),
                    character_id = table.Column<int>(type: "int", nullable: false),
                    character_level = table.Column<int>(type: "int", nullable: false),
                    grade = table.Column<int>(type: "int", nullable: false),
                    friendship_level = table.Column<int>(type: "int", nullable: false),
                    friendship_exp = table.Column<int>(type: "int", nullable: false),
                    insert_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_person_character", x => x.person_character_id);
                });

            migrationBuilder.CreateTable(
                name: "person_gacha",
                columns: table => new
                {
                    person_gacha_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    person_id = table.Column<int>(type: "int", nullable: false),
                    gacha_id = table.Column<int>(type: "int", nullable: false),
                    gacha_count = table.Column<int>(type: "int", nullable: false),
                    insert_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_person_gacha", x => x.person_gacha_id);
                });

            migrationBuilder.CreateTable(
                name: "person_item",
                columns: table => new
                {
                    person_item_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    person_id = table.Column<int>(type: "int", nullable: false),
                    item_id = table.Column<int>(type: "int", nullable: false),
                    amount = table.Column<int>(type: "int", nullable: false),
                    insert_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_person_item", x => x.person_item_id);
                });

            migrationBuilder.CreateTable(
                name: "person_item_history",
                columns: table => new
                {
                    person_item_history_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    person_id = table.Column<int>(type: "int", nullable: false),
                    item_id = table.Column<int>(type: "int", nullable: false),
                    amount = table.Column<int>(type: "int", nullable: false),
                    insert_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_person_item_history", x => x.person_item_history_id);
                });

            migrationBuilder.CreateTable(
                name: "person_levelup",
                columns: table => new
                {
                    person_levelup_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    person_id = table.Column<int>(type: "int", nullable: false),
                    character_id = table.Column<int>(type: "int", nullable: false),
                    level = table.Column<int>(type: "int", nullable: false),
                    insert_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_person_levelup", x => x.person_levelup_id);
                });

            migrationBuilder.CreateTable(
                name: "person_limitbreak",
                columns: table => new
                {
                    person_limitbreak_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    person_id = table.Column<int>(type: "int", nullable: false),
                    character_id = table.Column<int>(type: "int", nullable: false),
                    grade = table.Column<int>(type: "int", nullable: false),
                    insert_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_person_limitbreak", x => x.person_limitbreak_id);
                });

            migrationBuilder.CreateTable(
                name: "person_login",
                columns: table => new
                {
                    person_login_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    person_id = table.Column<int>(type: "int", nullable: false),
                    insert_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_person_login", x => x.person_login_id);
                });

            migrationBuilder.CreateTable(
                name: "person_loginbonus",
                columns: table => new
                {
                    person_loginbonus_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    person_id = table.Column<int>(type: "int", nullable: false),
                    pre_login_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    daily_login_bonus_id = table.Column<int>(type: "int", nullable: false),
                    total_login_count = table.Column<int>(type: "int", nullable: false),
                    insert_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_person_loginbonus", x => x.person_loginbonus_id);
                });

            migrationBuilder.CreateTable(
                name: "person_mail",
                columns: table => new
                {
                    person_mail_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    person_id = table.Column<int>(type: "int", nullable: false),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    reward_id_1 = table.Column<int>(type: "int", nullable: false),
                    reward_id_1_amount = table.Column<int>(type: "int", nullable: false),
                    reward_id_2 = table.Column<int>(type: "int", nullable: true),
                    reward_id_2_amount = table.Column<int>(type: "int", nullable: true),
                    reward_id_3 = table.Column<int>(type: "int", nullable: true),
                    reward_id_3_amount = table.Column<int>(type: "int", nullable: true),
                    is_receive = table.Column<bool>(type: "bit", nullable: false),
                    expire_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    insert_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_person_mail", x => x.person_mail_id);
                });

            migrationBuilder.CreateTable(
                name: "person_product",
                columns: table => new
                {
                    person_product_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    person_id = table.Column<int>(type: "int", nullable: false),
                    product_id = table.Column<int>(type: "int", nullable: false),
                    price = table.Column<int>(type: "int", nullable: false),
                    insert_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_person_product", x => x.person_product_id);
                });

            migrationBuilder.CreateTable(
                name: "person_product_set_piece",
                columns: table => new
                {
                    person_product_set_piece_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    person_id = table.Column<int>(type: "int", nullable: false),
                    product_set_piece_id = table.Column<int>(type: "int", nullable: false),
                    price = table.Column<int>(type: "int", nullable: false),
                    insert_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_person_product_set_piece", x => x.person_product_set_piece_id);
                });

            migrationBuilder.CreateTable(
                name: "person_product_set_prism",
                columns: table => new
                {
                    person_product_set_prism_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    person_id = table.Column<int>(type: "int", nullable: false),
                    product_set_prism_id = table.Column<int>(type: "int", nullable: false),
                    price = table.Column<int>(type: "int", nullable: false),
                    insert_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_person_product_set_prism", x => x.person_product_set_prism_id);
                });

            migrationBuilder.CreateTable(
                name: "person_product_set_token",
                columns: table => new
                {
                    person_product_set_token_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    person_id = table.Column<int>(type: "int", nullable: false),
                    product_set_token_id = table.Column<int>(type: "int", nullable: false),
                    price = table.Column<int>(type: "int", nullable: false),
                    insert_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_person_product_set_token", x => x.person_product_set_token_id);
                });

            migrationBuilder.CreateTable(
                name: "person_status",
                columns: table => new
                {
                    person_status_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    person_id = table.Column<int>(type: "int", nullable: false),
                    display_person_id = table.Column<int>(type: "int", nullable: false),
                    person_hash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    person_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    level = table.Column<int>(type: "int", nullable: false),
                    exp = table.Column<int>(type: "int", nullable: false),
                    token = table.Column<int>(type: "int", nullable: false),
                    gift = table.Column<int>(type: "int", nullable: false),
                    manual = table.Column<int>(type: "int", nullable: false),
                    flim = table.Column<int>(type: "int", nullable: false),
                    prism = table.Column<int>(type: "int", nullable: false),
                    character_amount_max = table.Column<int>(type: "int", nullable: false),
                    character_storage_amount_max = table.Column<int>(type: "int", nullable: false),
                    leader_person_character_id = table.Column<int>(type: "int", nullable: false),
                    introduce = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    insert_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_person_status", x => x.person_status_id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_person_achievement_person_id_achievement_id",
                table: "person_achievement",
                columns: new[] { "person_id", "achievement_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_person_character_person_id_character_id",
                table: "person_character",
                columns: new[] { "person_id", "character_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_person_gacha_person_id_gacha_id",
                table: "person_gacha",
                columns: new[] { "person_id", "gacha_id" });

            migrationBuilder.CreateIndex(
                name: "IX_person_item_person_id_item_id",
                table: "person_item",
                columns: new[] { "person_id", "item_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_person_item_history_person_id_insert_date",
                table: "person_item_history",
                columns: new[] { "person_id", "insert_date" });

            migrationBuilder.CreateIndex(
                name: "IX_person_limitbreak_person_id_character_id",
                table: "person_limitbreak",
                columns: new[] { "person_id", "character_id" });

            migrationBuilder.CreateIndex(
                name: "IX_person_login_person_id_insert_date",
                table: "person_login",
                columns: new[] { "person_id", "insert_date" });

            migrationBuilder.CreateIndex(
                name: "IX_person_loginbonus_person_id_daily_login_bonus_id",
                table: "person_loginbonus",
                columns: new[] { "person_id", "daily_login_bonus_id" });

            migrationBuilder.CreateIndex(
                name: "IX_person_mail_person_id_insert_date",
                table: "person_mail",
                columns: new[] { "person_id", "insert_date" });

            migrationBuilder.CreateIndex(
                name: "IX_person_mail_person_id_is_receive",
                table: "person_mail",
                columns: new[] { "person_id", "is_receive" });

            migrationBuilder.CreateIndex(
                name: "IX_person_status_display_person_id",
                table: "person_status",
                column: "display_person_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_person_status_email",
                table: "person_status",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_person_status_person_id",
                table: "person_status",
                column: "person_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "person_achievement");

            migrationBuilder.DropTable(
                name: "person_character");

            migrationBuilder.DropTable(
                name: "person_gacha");

            migrationBuilder.DropTable(
                name: "person_item");

            migrationBuilder.DropTable(
                name: "person_item_history");

            migrationBuilder.DropTable(
                name: "person_levelup");

            migrationBuilder.DropTable(
                name: "person_limitbreak");

            migrationBuilder.DropTable(
                name: "person_login");

            migrationBuilder.DropTable(
                name: "person_loginbonus");

            migrationBuilder.DropTable(
                name: "person_mail");

            migrationBuilder.DropTable(
                name: "person_product");

            migrationBuilder.DropTable(
                name: "person_product_set_piece");

            migrationBuilder.DropTable(
                name: "person_product_set_prism");

            migrationBuilder.DropTable(
                name: "person_product_set_token");

            migrationBuilder.DropTable(
                name: "person_status");
        }
    }
}
