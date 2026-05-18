using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Empen.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "master_achievement",
                columns: table => new
                {
                    achievement_id = table.Column<int>(type: "int", nullable: false),
                    show_order = table.Column<int>(type: "int", nullable: false),
                    achievement_category_id = table.Column<int>(type: "int", nullable: false),
                    achievement_title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    achievement_type = table.Column<int>(type: "int", nullable: false),
                    achievement_param_1 = table.Column<int>(type: "int", nullable: false),
                    achievement_param_2 = table.Column<int>(type: "int", nullable: false),
                    achievement_param_3 = table.Column<int>(type: "int", nullable: false),
                    achievement_description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    reward_id_1 = table.Column<int>(type: "int", nullable: false),
                    reward_id_2 = table.Column<int>(type: "int", nullable: true),
                    reward_id_3 = table.Column<int>(type: "int", nullable: true),
                    start_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    end_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    insert_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_master_achievement", x => x.achievement_id);
                });

            migrationBuilder.CreateTable(
                name: "master_achievement_category",
                columns: table => new
                {
                    achievement_category_id = table.Column<int>(type: "int", nullable: false),
                    show_order = table.Column<int>(type: "int", nullable: false),
                    category_title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    flag = table.Column<int>(type: "int", nullable: false),
                    start_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    end_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    insert_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_master_achievement_category", x => x.achievement_category_id);
                });

            migrationBuilder.CreateTable(
                name: "master_banner",
                columns: table => new
                {
                    banner_id = table.Column<int>(type: "int", nullable: false),
                    show_order = table.Column<int>(type: "int", nullable: false),
                    show_place_type = table.Column<int>(type: "int", nullable: false),
                    banner_image = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    banner_action_type = table.Column<int>(type: "int", nullable: false),
                    banner_action_param = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    limited_type = table.Column<int>(type: "int", nullable: false),
                    limited_param = table.Column<int>(type: "int", nullable: false),
                    os_type = table.Column<int>(type: "int", nullable: false),
                    start_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    end_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    insert_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_master_banner", x => x.banner_id);
                });

            migrationBuilder.CreateTable(
                name: "master_character",
                columns: table => new
                {
                    character_id = table.Column<int>(type: "int", nullable: false),
                    show_order = table.Column<int>(type: "int", nullable: false),
                    character_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    character_level_max = table.Column<int>(type: "int", nullable: false),
                    character_grade_max = table.Column<int>(type: "int", nullable: false),
                    character_grade = table.Column<int>(type: "int", nullable: false),
                    character_critical_rate = table.Column<int>(type: "int", nullable: false),
                    character_critical_damage = table.Column<int>(type: "int", nullable: false),
                    piece_item_id = table.Column<int>(type: "int", nullable: false),
                    piece_amount_duplicate = table.Column<int>(type: "int", nullable: false),
                    character_description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    character_comment_1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    character_comment_2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    character_comment_3 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    character_comment_1_motion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    character_comment_2_motion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    character_comment_3_motion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    start_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    end_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    insert_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_master_character", x => x.character_id);
                });

            migrationBuilder.CreateTable(
                name: "master_daily_login_bonus",
                columns: table => new
                {
                    daily_login_bonus_id = table.Column<int>(type: "int", nullable: false),
                    title_text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    back_image = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    start_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    end_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    insert_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_master_daily_login_bonus", x => x.daily_login_bonus_id);
                });

            migrationBuilder.CreateTable(
                name: "master_gacha_exec_10",
                columns: table => new
                {
                    gacha_exec_10_id = table.Column<int>(type: "int", nullable: false),
                    gacha_lot_group_id_1 = table.Column<int>(type: "int", nullable: false),
                    gacha_lot_count_1 = table.Column<int>(type: "int", nullable: false),
                    gacha_lot_group_id_2 = table.Column<int>(type: "int", nullable: false),
                    gacha_lot_count_2 = table.Column<int>(type: "int", nullable: false),
                    gacha_lot_group_id_3 = table.Column<int>(type: "int", nullable: false),
                    gacha_lot_count_3 = table.Column<int>(type: "int", nullable: false),
                    gacha_consume_value = table.Column<int>(type: "int", nullable: false),
                    gacha_point = table.Column<int>(type: "int", nullable: false),
                    gacha_bonus_reward_group_lot_id = table.Column<int>(type: "int", nullable: false),
                    start_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    end_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    insert_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_master_gacha_exec_10", x => x.gacha_exec_10_id);
                });

            migrationBuilder.CreateTable(
                name: "master_gacha_lot_group",
                columns: table => new
                {
                    gacha_lot_group_id = table.Column<int>(type: "int", nullable: false),
                    show_order = table.Column<int>(type: "int", nullable: false),
                    gacha_lot_group_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    gacha_lot_group_description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    start_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    end_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    insert_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_master_gacha_lot_group", x => x.gacha_lot_group_id);
                });

            migrationBuilder.CreateTable(
                name: "master_item",
                columns: table => new
                {
                    item_id = table.Column<int>(type: "int", nullable: false),
                    show_order = table.Column<int>(type: "int", nullable: false),
                    item_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    item_type = table.Column<int>(type: "int", nullable: false),
                    tab_type = table.Column<int>(type: "int", nullable: false),
                    item_param_1 = table.Column<int>(type: "int", nullable: true),
                    item_param_2 = table.Column<int>(type: "int", nullable: true),
                    item_description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    item_image_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    expire_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    start_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    end_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    insert_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_master_item", x => x.item_id);
                });

            migrationBuilder.CreateTable(
                name: "master_mail",
                columns: table => new
                {
                    mail_id = table.Column<int>(type: "int", nullable: false),
                    sender_type = table.Column<int>(type: "int", nullable: false),
                    mail_subject = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    mail_body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    important_flag = table.Column<int>(type: "int", nullable: false),
                    start_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    end_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    insert_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_master_mail", x => x.mail_id);
                });

            migrationBuilder.CreateTable(
                name: "master_notice",
                columns: table => new
                {
                    notice_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    notice_title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    notice_content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    notice_banner_image = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    notice_link = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    notice_type = table.Column<int>(type: "int", nullable: false),
                    show_order = table.Column<int>(type: "int", nullable: false),
                    start_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    end_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    insert_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_master_notice", x => x.notice_id);
                });

            migrationBuilder.CreateTable(
                name: "master_product",
                columns: table => new
                {
                    product_id = table.Column<int>(type: "int", nullable: false),
                    product_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    prism = table.Column<int>(type: "int", nullable: false),
                    price = table.Column<int>(type: "int", nullable: false),
                    product_type = table.Column<int>(type: "int", nullable: false),
                    product_detail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    start_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    end_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    insert_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_master_product", x => x.product_id);
                });

            migrationBuilder.CreateTable(
                name: "master_product_set",
                columns: table => new
                {
                    product_set_id = table.Column<int>(type: "int", nullable: false),
                    product_id = table.Column<int>(type: "int", nullable: false),
                    show_order = table.Column<int>(type: "int", nullable: false),
                    buy_upper_limit = table.Column<int>(type: "int", nullable: false),
                    image = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    reward_id_1 = table.Column<int>(type: "int", nullable: false),
                    reward_id_2 = table.Column<int>(type: "int", nullable: true),
                    reward_id_3 = table.Column<int>(type: "int", nullable: true),
                    start_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    end_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    insert_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_master_product_set", x => x.product_set_id);
                });

            migrationBuilder.CreateTable(
                name: "master_product_set_piece",
                columns: table => new
                {
                    product_set_piece_id = table.Column<int>(type: "int", nullable: false),
                    show_order = table.Column<int>(type: "int", nullable: false),
                    buy_upper_limit = table.Column<int>(type: "int", nullable: false),
                    cost_item_id = table.Column<int>(type: "int", nullable: false),
                    price_piece = table.Column<int>(type: "int", nullable: false),
                    product_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    product_detail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    image = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    reward_id_1 = table.Column<int>(type: "int", nullable: false),
                    reward_id_2 = table.Column<int>(type: "int", nullable: true),
                    reward_id_3 = table.Column<int>(type: "int", nullable: true),
                    start_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    end_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    insert_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_master_product_set_piece", x => x.product_set_piece_id);
                });

            migrationBuilder.CreateTable(
                name: "master_product_set_prism",
                columns: table => new
                {
                    product_set_prism_id = table.Column<int>(type: "int", nullable: false),
                    show_order = table.Column<int>(type: "int", nullable: false),
                    buy_upper_limit = table.Column<int>(type: "int", nullable: false),
                    price_prism = table.Column<int>(type: "int", nullable: false),
                    product_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    product_detail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    image = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    reward_id_1 = table.Column<int>(type: "int", nullable: false),
                    reward_id_2 = table.Column<int>(type: "int", nullable: true),
                    reward_id_3 = table.Column<int>(type: "int", nullable: true),
                    start_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    end_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    insert_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_master_product_set_prism", x => x.product_set_prism_id);
                });

            migrationBuilder.CreateTable(
                name: "master_product_set_token",
                columns: table => new
                {
                    product_set_token_id = table.Column<int>(type: "int", nullable: false),
                    token_reward_group = table.Column<int>(type: "int", nullable: true),
                    show_order = table.Column<int>(type: "int", nullable: false),
                    buy_upper_limit = table.Column<int>(type: "int", nullable: false),
                    price_token = table.Column<int>(type: "int", nullable: false),
                    product_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    product_detail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    image = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    reward_id_1 = table.Column<int>(type: "int", nullable: false),
                    reward_id_2 = table.Column<int>(type: "int", nullable: true),
                    reward_id_3 = table.Column<int>(type: "int", nullable: true),
                    is_package = table.Column<int>(type: "int", nullable: false),
                    package_display_reward_id = table.Column<int>(type: "int", nullable: false),
                    start_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    end_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    insert_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_master_product_set_token", x => x.product_set_token_id);
                });

            migrationBuilder.CreateTable(
                name: "master_reward",
                columns: table => new
                {
                    reward_id = table.Column<int>(type: "int", nullable: false),
                    object_type = table.Column<int>(type: "int", nullable: false),
                    object_value = table.Column<int>(type: "int", nullable: false),
                    object_amount = table.Column<int>(type: "int", nullable: false),
                    additional_param = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    start_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    end_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    insert_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_master_reward", x => x.reward_id);
                });

            migrationBuilder.CreateTable(
                name: "master_character_grade",
                columns: table => new
                {
                    character_grade_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    character_id = table.Column<int>(type: "int", nullable: false),
                    grade = table.Column<int>(type: "int", nullable: false),
                    critical_rate = table.Column<int>(type: "int", nullable: false),
                    critical_damage = table.Column<int>(type: "int", nullable: false),
                    require_count = table.Column<int>(type: "int", nullable: false),
                    price_token = table.Column<int>(type: "int", nullable: false),
                    start_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    end_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    insert_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_master_character_grade", x => x.character_grade_id);
                    table.ForeignKey(
                        name: "FK_master_character_grade_master_character_character_id",
                        column: x => x.character_id,
                        principalTable: "master_character",
                        principalColumn: "character_id");
                });

            migrationBuilder.CreateTable(
                name: "master_character_level",
                columns: table => new
                {
                    character_level_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    character_id = table.Column<int>(type: "int", nullable: false),
                    level = table.Column<int>(type: "int", nullable: false),
                    atk = table.Column<int>(type: "int", nullable: false),
                    def = table.Column<int>(type: "int", nullable: false),
                    hp = table.Column<int>(type: "int", nullable: false),
                    stance = table.Column<int>(type: "int", nullable: false),
                    resource_item_id_1 = table.Column<int>(type: "int", nullable: false),
                    resource_item_id_2 = table.Column<int>(type: "int", nullable: false),
                    resource_item_id_3 = table.Column<int>(type: "int", nullable: false),
                    item_1_amount = table.Column<int>(type: "int", nullable: false),
                    item_2_amount = table.Column<int>(type: "int", nullable: false),
                    item_3_amount = table.Column<int>(type: "int", nullable: false),
                    start_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    end_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    insert_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_master_character_level", x => x.character_level_id);
                    table.ForeignKey(
                        name: "FK_master_character_level_master_character_character_id",
                        column: x => x.character_id,
                        principalTable: "master_character",
                        principalColumn: "character_id");
                });

            migrationBuilder.CreateTable(
                name: "master_daily_login_bonus_day",
                columns: table => new
                {
                    daily_login_bonus_day_id = table.Column<int>(type: "int", nullable: false),
                    daily_login_bonus_id = table.Column<int>(type: "int", nullable: false),
                    total_login_count = table.Column<int>(type: "int", nullable: false),
                    reward_id_1 = table.Column<int>(type: "int", nullable: false),
                    start_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    end_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    insert_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_master_daily_login_bonus_day", x => x.daily_login_bonus_day_id);
                    table.ForeignKey(
                        name: "FK_master_daily_login_bonus_day_master_daily_login_bonus_daily_login_bonus_id",
                        column: x => x.daily_login_bonus_id,
                        principalTable: "master_daily_login_bonus",
                        principalColumn: "daily_login_bonus_id");
                });

            migrationBuilder.CreateTable(
                name: "master_gacha",
                columns: table => new
                {
                    gacha_id = table.Column<int>(type: "int", nullable: false),
                    gacha_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    show_order = table.Column<int>(type: "int", nullable: false),
                    gacha_type = table.Column<int>(type: "int", nullable: false),
                    gacha_lot_group_id = table.Column<int>(type: "int", nullable: false),
                    gacha_consume_value = table.Column<int>(type: "int", nullable: false),
                    gacha_coupon_item_id = table.Column<int>(type: "int", nullable: false),
                    gacha_image = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    gacha_detail_image = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    gacha_description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    gacha_point = table.Column<int>(type: "int", nullable: false),
                    gacha_bonus_reward_group_lot_id = table.Column<int>(type: "int", nullable: false),
                    gacha_exec_10_id = table.Column<int>(type: "int", nullable: true),
                    start_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    end_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    insert_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_master_gacha", x => x.gacha_id);
                    table.ForeignKey(
                        name: "FK_master_gacha_master_gacha_exec_10_gacha_exec_10_id",
                        column: x => x.gacha_exec_10_id,
                        principalTable: "master_gacha_exec_10",
                        principalColumn: "gacha_exec_10_id");
                    table.ForeignKey(
                        name: "FK_master_gacha_master_gacha_lot_group_gacha_lot_group_id",
                        column: x => x.gacha_lot_group_id,
                        principalTable: "master_gacha_lot_group",
                        principalColumn: "gacha_lot_group_id");
                });

            migrationBuilder.CreateTable(
                name: "master_gacha_lot",
                columns: table => new
                {
                    gacha_lot_id = table.Column<int>(type: "int", nullable: false),
                    gacha_lot_group_id = table.Column<int>(type: "int", nullable: false),
                    gacha_character_id = table.Column<int>(type: "int", nullable: false),
                    character_level = table.Column<int>(type: "int", nullable: false),
                    character_plus_lot_group = table.Column<int>(type: "int", nullable: false),
                    weight = table.Column<int>(type: "int", nullable: false),
                    start_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    end_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    insert_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_master_gacha_lot", x => x.gacha_lot_id);
                    table.ForeignKey(
                        name: "FK_master_gacha_lot_master_character_gacha_character_id",
                        column: x => x.gacha_character_id,
                        principalTable: "master_character",
                        principalColumn: "character_id");
                    table.ForeignKey(
                        name: "FK_master_gacha_lot_master_gacha_lot_group_gacha_lot_group_id",
                        column: x => x.gacha_lot_group_id,
                        principalTable: "master_gacha_lot_group",
                        principalColumn: "gacha_lot_group_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_master_character_grade_character_id",
                table: "master_character_grade",
                column: "character_id");

            migrationBuilder.CreateIndex(
                name: "IX_master_character_level_character_id",
                table: "master_character_level",
                column: "character_id");

            migrationBuilder.CreateIndex(
                name: "IX_master_daily_login_bonus_day_daily_login_bonus_id",
                table: "master_daily_login_bonus_day",
                column: "daily_login_bonus_id");

            migrationBuilder.CreateIndex(
                name: "IX_master_gacha_gacha_exec_10_id",
                table: "master_gacha",
                column: "gacha_exec_10_id");

            migrationBuilder.CreateIndex(
                name: "IX_master_gacha_gacha_lot_group_id",
                table: "master_gacha",
                column: "gacha_lot_group_id");

            migrationBuilder.CreateIndex(
                name: "IX_master_gacha_lot_gacha_character_id",
                table: "master_gacha_lot",
                column: "gacha_character_id");

            migrationBuilder.CreateIndex(
                name: "IX_master_gacha_lot_gacha_lot_group_id",
                table: "master_gacha_lot",
                column: "gacha_lot_group_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "master_achievement");

            migrationBuilder.DropTable(
                name: "master_achievement_category");

            migrationBuilder.DropTable(
                name: "master_banner");

            migrationBuilder.DropTable(
                name: "master_character_grade");

            migrationBuilder.DropTable(
                name: "master_character_level");

            migrationBuilder.DropTable(
                name: "master_daily_login_bonus_day");

            migrationBuilder.DropTable(
                name: "master_gacha");

            migrationBuilder.DropTable(
                name: "master_gacha_lot");

            migrationBuilder.DropTable(
                name: "master_item");

            migrationBuilder.DropTable(
                name: "master_mail");

            migrationBuilder.DropTable(
                name: "master_notice");

            migrationBuilder.DropTable(
                name: "master_product");

            migrationBuilder.DropTable(
                name: "master_product_set");

            migrationBuilder.DropTable(
                name: "master_product_set_piece");

            migrationBuilder.DropTable(
                name: "master_product_set_prism");

            migrationBuilder.DropTable(
                name: "master_product_set_token");

            migrationBuilder.DropTable(
                name: "master_reward");

            migrationBuilder.DropTable(
                name: "master_daily_login_bonus");

            migrationBuilder.DropTable(
                name: "master_gacha_exec_10");

            migrationBuilder.DropTable(
                name: "master_character");

            migrationBuilder.DropTable(
                name: "master_gacha_lot_group");
        }
    }
}
