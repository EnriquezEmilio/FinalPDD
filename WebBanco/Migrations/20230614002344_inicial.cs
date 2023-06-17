using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WebBanco.Migrations
{
    /// <inheritdoc />
    public partial class inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cajas",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Cbu = table.Column<string>(type: "varchar(50)", nullable: false),
                    saldo = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cajas", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    numusr = table.Column<int>(name: "num_usr", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    dni = table.Column<int>(type: "int", nullable: false),
                    nombre = table.Column<string>(type: "varchar(50)", nullable: false),
                    apellido = table.Column<string>(type: "varchar(50)", nullable: false),
                    email = table.Column<string>(type: "varchar(250)", nullable: false),
                    password = table.Column<string>(type: "varchar(50)", nullable: false),
                    intentosFallidos = table.Column<int>(type: "int", nullable: false),
                    bloqueado = table.Column<bool>(type: "bit", nullable: false),
                    esADM = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.numusr);
                });

            migrationBuilder.CreateTable(
                name: "Movimientos",
                columns: table => new
                {
                    idm = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    detalle = table.Column<string>(type: "varchar(250)", nullable: false),
                    monto = table.Column<double>(type: "float", nullable: false),
                    fecha = table.Column<DateTime>(type: "date", nullable: false),
                    id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movimientos", x => x.idm);
                    table.ForeignKey(
                        name: "FK_Movimientos_Cajas_id",
                        column: x => x.id,
                        principalTable: "Cajas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pagos",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "varchar(50)", nullable: false),
                    monto = table.Column<double>(type: "float", nullable: false),
                    pagado = table.Column<bool>(type: "bit", nullable: false),
                    metodo = table.Column<string>(type: "varchar(50)", nullable: false),
                    numusr = table.Column<int>(name: "num_usr", type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pagos", x => x.id);
                    table.ForeignKey(
                        name: "FK_Pagos_Usuarios_num_usr",
                        column: x => x.numusr,
                        principalTable: "Usuarios",
                        principalColumn: "num_usr",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Plazos",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    monto = table.Column<double>(type: "float", nullable: false),
                    fechaIni = table.Column<DateTime>(type: "date", nullable: false),
                    fechaFin = table.Column<DateTime>(type: "date", nullable: false),
                    tasa = table.Column<double>(type: "float", nullable: false),
                    pagado = table.Column<bool>(type: "bit", nullable: false),
                    numusr = table.Column<int>(name: "num_usr", type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plazos", x => x.id);
                    table.ForeignKey(
                        name: "FK_Plazos_Usuarios_num_usr",
                        column: x => x.numusr,
                        principalTable: "Usuarios",
                        principalColumn: "num_usr",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tarjetas",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    numero = table.Column<string>(type: "varchar(50)", nullable: false),
                    codigoV = table.Column<string>(type: "varchar(50)", nullable: false),
                    limite = table.Column<double>(type: "float", nullable: false),
                    consumos = table.Column<double>(type: "float", nullable: false),
                    numusr = table.Column<int>(name: "num_usr", type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tarjetas", x => x.id);
                    table.ForeignKey(
                        name: "FK_Tarjetas_Usuarios_num_usr",
                        column: x => x.numusr,
                        principalTable: "Usuarios",
                        principalColumn: "num_usr",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioCaja",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false),
                    numusr = table.Column<int>(name: "num_usr", type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioCaja", x => new { x.numusr, x.id });
                    table.ForeignKey(
                        name: "FK_UsuarioCaja_Cajas_id",
                        column: x => x.id,
                        principalTable: "Cajas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuarioCaja_Usuarios_num_usr",
                        column: x => x.numusr,
                        principalTable: "Usuarios",
                        principalColumn: "num_usr",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "num_usr", "apellido", "bloqueado", "dni", "email", "esADM", "intentosFallidos", "nombre", "password" },
                values: new object[,]
                {
                    { 1, "222", false, 1234, "111@111", true, 0, "111", "111" },
                    { 2, "sda", false, 1235, "222@111", false, 0, "asd", "111" },
                    { 3, "dsa", false, 1236, "333@111", true, 0, "asd", "111" },
                    { 4, "ads", true, 1237, "444@111", false, 3, "asd", "111" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Movimientos_id",
                table: "Movimientos",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_num_usr",
                table: "Pagos",
                column: "num_usr");

            migrationBuilder.CreateIndex(
                name: "IX_Plazos_num_usr",
                table: "Plazos",
                column: "num_usr");

            migrationBuilder.CreateIndex(
                name: "IX_Tarjetas_num_usr",
                table: "Tarjetas",
                column: "num_usr");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioCaja_id",
                table: "UsuarioCaja",
                column: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Movimientos");

            migrationBuilder.DropTable(
                name: "Pagos");

            migrationBuilder.DropTable(
                name: "Plazos");

            migrationBuilder.DropTable(
                name: "Tarjetas");

            migrationBuilder.DropTable(
                name: "UsuarioCaja");

            migrationBuilder.DropTable(
                name: "Cajas");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
