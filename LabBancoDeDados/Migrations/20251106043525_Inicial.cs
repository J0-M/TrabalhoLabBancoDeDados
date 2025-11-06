using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LabBancoDeDados.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ARTISTA",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Nacionalidade = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ARTISTA", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "USUARIO",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USUARIO", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MUSICA",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Titulo = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    DuracaoSegundos = table.Column<int>(type: "integer", nullable: false),
                    ArtistaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MUSICA", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MUSICA_ARTISTA_ArtistaId",
                        column: x => x.ArtistaId,
                        principalTable: "ARTISTA",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PLAYLIST",
                columns: table => new
                {
                    PlaylistId = table.Column<int>(type: "integer", nullable: false),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    Nome = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    data_criacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PLAYLIST", x => new { x.PlaylistId, x.UsuarioId });
                    table.ForeignKey(
                        name: "FK_PLAYLIST_USUARIO_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "USUARIO",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MUSICA_PLAYLIST",
                columns: table => new
                {
                    MusicaId = table.Column<int>(type: "integer", nullable: false),
                    PlaylistId = table.Column<int>(type: "integer", nullable: false),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    OrdemNaPlaylist = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MUSICA_PLAYLIST", x => new { x.MusicaId, x.PlaylistId, x.UsuarioId });
                    table.ForeignKey(
                        name: "FK_MUSICA_PLAYLIST_MUSICA_MusicaId",
                        column: x => x.MusicaId,
                        principalTable: "MUSICA",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MUSICA_PLAYLIST_PLAYLIST_PlaylistId_UsuarioId",
                        columns: x => new { x.PlaylistId, x.UsuarioId },
                        principalTable: "PLAYLIST",
                        principalColumns: new[] { "PlaylistId", "UsuarioId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MUSICA_ArtistaId",
                table: "MUSICA",
                column: "ArtistaId");

            migrationBuilder.CreateIndex(
                name: "IX_MUSICA_PLAYLIST_PlaylistId_UsuarioId",
                table: "MUSICA_PLAYLIST",
                columns: new[] { "PlaylistId", "UsuarioId" });

            migrationBuilder.CreateIndex(
                name: "IX_PLAYLIST_UsuarioId",
                table: "PLAYLIST",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MUSICA_PLAYLIST");

            migrationBuilder.DropTable(
                name: "MUSICA");

            migrationBuilder.DropTable(
                name: "PLAYLIST");

            migrationBuilder.DropTable(
                name: "ARTISTA");

            migrationBuilder.DropTable(
                name: "USUARIO");
        }
    }
}
