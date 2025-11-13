using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillUp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SKILLUP_CURSO",
                columns: table => new
                {
                    ID_CURSO = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NOME = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    CATEGORIA = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    CARGA_HORARIA = table.Column<decimal>(type: "NUMBER", nullable: false),
                    DIFICULDADE = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    DESCRICAO = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SKILLUP_CURSO", x => x.ID_CURSO);
                });

            migrationBuilder.CreateTable(
                name: "SKILLUP_USUARIO",
                columns: table => new
                {
                    ID_USUARIO = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NOME = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    EMAIL = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    SENHA = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    AREA_INTERESSE = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    DATA_CADASTRO = table.Column<DateTime>(type: "DATE", nullable: false, defaultValueSql: "SYSDATE")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SKILLUP_USUARIO", x => x.ID_USUARIO);
                });

            migrationBuilder.CreateTable(
                name: "SKILLUP_PROGRESSO",
                columns: table => new
                {
                    ID_PROGRESSO = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ID_USUARIO = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    ID_CURSO = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    STATUS = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false, defaultValue: "NaoIniciado"),
                    DATA_INICIO = table.Column<DateTime>(type: "DATE", nullable: true),
                    DATA_FIM = table.Column<DateTime>(type: "DATE", nullable: true),
                    PORCENTAGEM = table.Column<decimal>(type: "NUMBER(5,2)", nullable: false, defaultValue: 0m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SKILLUP_PROGRESSO", x => x.ID_PROGRESSO);
                    table.ForeignKey(
                        name: "FK_PROGRESSO_CURSO",
                        column: x => x.ID_CURSO,
                        principalTable: "SKILLUP_CURSO",
                        principalColumn: "ID_CURSO",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PROGRESSO_USUARIO",
                        column: x => x.ID_USUARIO,
                        principalTable: "SKILLUP_USUARIO",
                        principalColumn: "ID_USUARIO",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SKILLUP_PROGRESSO_ID_CURSO",
                table: "SKILLUP_PROGRESSO",
                column: "ID_CURSO");

            migrationBuilder.CreateIndex(
                name: "UX_PROGRESSO_USUARIO_CURSO",
                table: "SKILLUP_PROGRESSO",
                columns: new[] { "ID_USUARIO", "ID_CURSO" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_SKILLUP_USUARIO_EMAIL",
                table: "SKILLUP_USUARIO",
                column: "EMAIL",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SKILLUP_PROGRESSO");

            migrationBuilder.DropTable(
                name: "SKILLUP_CURSO");

            migrationBuilder.DropTable(
                name: "SKILLUP_USUARIO");
        }
    }
}
