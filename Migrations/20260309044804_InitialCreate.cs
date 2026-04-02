using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SNSCakeBakery_Service.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ADDRESSES",
                columns: table => new
                {
                    ADDRESSID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    USERID = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    STREET = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    UNIT = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    CITY = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    STATE = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    POSTALCODE = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    COUNTRY = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ADDRESSES", x => x.ADDRESSID);
                });

            migrationBuilder.CreateTable(
                name: "GOOGLESYNCLOGS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    LASTSYNCED = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    TOTALIMPORTED = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    STATUS = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GOOGLESYNCLOGS", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "IMAGETYPES",
                columns: table => new
                {
                    IMAGETYPEID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    IMAGETYPENAME = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IMAGETYPES", x => x.IMAGETYPEID);
                });

            migrationBuilder.CreateTable(
                name: "PRODUCTTYPES",
                columns: table => new
                {
                    TYPEID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    TYPENAME = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PRODUCTTYPES", x => x.TYPEID);
                });

            migrationBuilder.CreateTable(
                name: "USERS",
                columns: table => new
                {
                    ID = table.Column<string>(type: "NVARCHAR2(36)", maxLength: 36, nullable: false),
                    EMAIL = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    FIREBASEUID = table.Column<string>(type: "NVARCHAR2(128)", maxLength: 128, nullable: false),
                    FIRSTNAME = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    LASTNAME = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    CREATEDDATE = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USERS", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PRODUCTS",
                columns: table => new
                {
                    PRODUCTID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    PRODUCTTYPEID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    DESCRIPTION = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    BASEPRICE = table.Column<decimal>(type: "NUMBER(18,2)", nullable: false),
                    ISACTIVE = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    CREATEDAT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PRODUCTS", x => x.PRODUCTID);
                    table.ForeignKey(
                        name: "FK_PRODUCTS_PRODUCTTYPES_PRODUCTTYPEID",
                        column: x => x.PRODUCTTYPEID,
                        principalTable: "PRODUCTTYPES",
                        principalColumn: "TYPEID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ORDERS",
                columns: table => new
                {
                    ID = table.Column<string>(type: "NVARCHAR2(36)", maxLength: 36, nullable: false),
                    USERID = table.Column<string>(type: "NVARCHAR2(36)", nullable: false),
                    CAKETYPE = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    SIZE = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    QUANTITY = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    NOTES = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    SOURCE = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    CREATEDDATE = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DELIVERYDATE = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DELIVERYADDRESSID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    DELIVERYREQUIRED = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    ORDERDATE = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ORDERS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ORDERS_USERS_USERID",
                        column: x => x.USERID,
                        principalTable: "USERS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PRODUCTIMAGES",
                columns: table => new
                {
                    IMAGEID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    PRODUCTID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    IMAGETYPEID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    STORAGEKEY = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    ISPRIMARY = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    DISPLAYORDER = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    UPLOADEDAT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PRODUCTIMAGES", x => x.IMAGEID);
                    table.ForeignKey(
                        name: "FK_PRODUCTIMAGES_PRODUCTS_PRODUCTID",
                        column: x => x.PRODUCTID,
                        principalTable: "PRODUCTS",
                        principalColumn: "PRODUCTID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ORDERS_USERID",
                table: "ORDERS",
                column: "USERID");

            migrationBuilder.CreateIndex(
                name: "IX_PRODUCTIMAGES_PRODUCTID",
                table: "PRODUCTIMAGES",
                column: "PRODUCTID");

            migrationBuilder.CreateIndex(
                name: "IX_PRODUCTS_PRODUCTTYPEID",
                table: "PRODUCTS",
                column: "PRODUCTTYPEID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ADDRESSES");

            migrationBuilder.DropTable(
                name: "GOOGLESYNCLOGS");

            migrationBuilder.DropTable(
                name: "IMAGETYPES");

            migrationBuilder.DropTable(
                name: "ORDERS");

            migrationBuilder.DropTable(
                name: "PRODUCTIMAGES");

            migrationBuilder.DropTable(
                name: "USERS");

            migrationBuilder.DropTable(
                name: "PRODUCTS");

            migrationBuilder.DropTable(
                name: "PRODUCTTYPES");
        }
    }
}
