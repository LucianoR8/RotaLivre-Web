using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Rota_LivreWEB_API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categoria",
                columns: table => new
                {
                    id_categoria = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tipo_categoria = table.Column<string>(type: "text", nullable: false),
                    img = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categoria", x => x.id_categoria);
                });

            migrationBuilder.CreateTable(
                name: "Endereco",
                columns: table => new
                {
                    id_endereco = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nome_rua = table.Column<string>(type: "text", nullable: false),
                    numero_rua = table.Column<int>(type: "integer", nullable: false),
                    complemento = table.Column<string>(type: "text", nullable: false),
                    bairro = table.Column<string>(type: "text", nullable: false),
                    cep = table.Column<int>(type: "integer", nullable: false),
                    id_passeio = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Endereco", x => x.id_endereco);
                });

            migrationBuilder.CreateTable(
                name: "Funcionario",
                columns: table => new
                {
                    id_funcionario = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nome_funcionario = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Funcionario", x => x.id_funcionario);
                });

            migrationBuilder.CreateTable(
                name: "Grupo",
                columns: table => new
                {
                    id_grupo = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    link_grupo = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grupo", x => x.id_grupo);
                });

            migrationBuilder.CreateTable(
                name: "Localizacao",
                columns: table => new
                {
                    id_localizacao = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    latitude = table.Column<double>(type: "double precision", nullable: false),
                    longitude = table.Column<double>(type: "double precision", nullable: false),
                    velocidade = table.Column<double>(type: "double precision", nullable: true),
                    data_registro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Localizacao", x => x.id_localizacao);
                });

            migrationBuilder.CreateTable(
                name: "PerguntaSeguranca",
                columns: table => new
                {
                    id_pergunta = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    pergunta_seg = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerguntaSeguranca", x => x.id_pergunta);
                });

            migrationBuilder.CreateTable(
                name: "Passeio",
                columns: table => new
                {
                    id_passeio = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_categoria = table.Column<int>(type: "integer", nullable: false),
                    nome_passeio = table.Column<string>(type: "text", nullable: false),
                    funcionamento = table.Column<string>(type: "text", nullable: false),
                    descricao = table.Column<string>(type: "text", nullable: false),
                    img_url = table.Column<string>(type: "text", nullable: false),
                    Enderecoid_endereco = table.Column<int>(type: "integer", nullable: true),
                    QuantidadeCurtidas = table.Column<int>(type: "integer", nullable: false),
                    AlternarCurtida = table.Column<bool>(type: "boolean", nullable: false),
                    UsuarioJaCurtiu = table.Column<bool>(type: "boolean", nullable: false),
                    UsuarioJaPendente = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Passeio", x => x.id_passeio);
                    table.ForeignKey(
                        name: "FK_Passeio_Endereco_Enderecoid_endereco",
                        column: x => x.Enderecoid_endereco,
                        principalTable: "Endereco",
                        principalColumn: "id_endereco",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    id_usuario = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nome_completo = table.Column<string>(type: "text", nullable: false),
                    data_nasc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    senha = table.Column<string>(type: "text", nullable: false),
                    ConfirmarSenha = table.Column<string>(type: "text", nullable: false),
                    resposta_seg = table.Column<string>(type: "text", nullable: false),
                    id_pergunta = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.id_usuario);
                    table.ForeignKey(
                        name: "FK_Usuario_PerguntaSeguranca_id_pergunta",
                        column: x => x.id_pergunta,
                        principalTable: "PerguntaSeguranca",
                        principalColumn: "id_pergunta",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GrupoPasseio",
                columns: table => new
                {
                    id_grupo_passeio = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_grupo = table.Column<int>(type: "integer", nullable: false),
                    id_passeio = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrupoPasseio", x => x.id_grupo_passeio);
                    table.ForeignKey(
                        name: "FK_GrupoPasseio_Grupo_id_grupo",
                        column: x => x.id_grupo,
                        principalTable: "Grupo",
                        principalColumn: "id_grupo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GrupoPasseio_Passeio_id_passeio",
                        column: x => x.id_passeio,
                        principalTable: "Passeio",
                        principalColumn: "id_passeio",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PasseioFuncionario",
                columns: table => new
                {
                    id_passeio_funcionario = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_passeio = table.Column<int>(type: "integer", nullable: false),
                    id_funcionario = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasseioFuncionario", x => x.id_passeio_funcionario);
                    table.ForeignKey(
                        name: "FK_PasseioFuncionario_Funcionario_id_funcionario",
                        column: x => x.id_funcionario,
                        principalTable: "Funcionario",
                        principalColumn: "id_funcionario",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PasseioFuncionario_Passeio_id_passeio",
                        column: x => x.id_passeio,
                        principalTable: "Passeio",
                        principalColumn: "id_passeio",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Avaliacao",
                columns: table => new
                {
                    id_avaliacao = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_passeio = table.Column<int>(type: "integer", nullable: false),
                    id_usuario = table.Column<int>(type: "integer", nullable: false),
                    feedback = table.Column<string>(type: "text", nullable: false),
                    nota = table.Column<int>(type: "integer", nullable: false),
                    data_feedback = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    nome_completo = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Avaliacao", x => x.id_avaliacao);
                    table.ForeignKey(
                        name: "FK_Avaliacao_Passeio_id_passeio",
                        column: x => x.id_passeio,
                        principalTable: "Passeio",
                        principalColumn: "id_passeio",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Avaliacao_Usuario_id_usuario",
                        column: x => x.id_usuario,
                        principalTable: "Usuario",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CurtidaPasseio",
                columns: table => new
                {
                    id_curtida = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_usuario = table.Column<int>(type: "integer", nullable: false),
                    id_passeio = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurtidaPasseio", x => x.id_curtida);
                    table.ForeignKey(
                        name: "FK_CurtidaPasseio_Passeio_id_passeio",
                        column: x => x.id_passeio,
                        principalTable: "Passeio",
                        principalColumn: "id_passeio",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CurtidaPasseio_Usuario_id_usuario",
                        column: x => x.id_usuario,
                        principalTable: "Usuario",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GrupoUsuario",
                columns: table => new
                {
                    id_grupo_usuario = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_grupo = table.Column<int>(type: "integer", nullable: false),
                    id_usuario = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrupoUsuario", x => x.id_grupo_usuario);
                    table.ForeignKey(
                        name: "FK_GrupoUsuario_Grupo_id_grupo",
                        column: x => x.id_grupo,
                        principalTable: "Grupo",
                        principalColumn: "id_grupo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GrupoUsuario_Usuario_id_usuario",
                        column: x => x.id_usuario,
                        principalTable: "Usuario",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PasseioFavorito",
                columns: table => new
                {
                    id_favorito = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_usuario = table.Column<int>(type: "integer", nullable: false),
                    id_passeio = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasseioFavorito", x => x.id_favorito);
                    table.ForeignKey(
                        name: "FK_PasseioFavorito_Passeio_id_passeio",
                        column: x => x.id_passeio,
                        principalTable: "Passeio",
                        principalColumn: "id_passeio",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PasseioFavorito_Usuario_id_usuario",
                        column: x => x.id_usuario,
                        principalTable: "Usuario",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "passeiospendentes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_usuario = table.Column<int>(type: "integer", nullable: false),
                    id_passeio = table.Column<int>(type: "integer", nullable: false),
                    data_adicao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_passeiospendentes", x => x.id);
                    table.ForeignKey(
                        name: "FK_passeiospendentes_Passeio_id_passeio",
                        column: x => x.id_passeio,
                        principalTable: "Passeio",
                        principalColumn: "id_passeio",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_passeiospendentes_Usuario_id_usuario",
                        column: x => x.id_usuario,
                        principalTable: "Usuario",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioLocalizacao",
                columns: table => new
                {
                    id_usuario_localizacao = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_usuario = table.Column<int>(type: "integer", nullable: false),
                    id_localizacao = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioLocalizacao", x => x.id_usuario_localizacao);
                    table.ForeignKey(
                        name: "FK_UsuarioLocalizacao_Localizacao_id_localizacao",
                        column: x => x.id_localizacao,
                        principalTable: "Localizacao",
                        principalColumn: "id_localizacao",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuarioLocalizacao_Usuario_id_usuario",
                        column: x => x.id_usuario,
                        principalTable: "Usuario",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Avaliacao_id_passeio",
                table: "Avaliacao",
                column: "id_passeio");

            migrationBuilder.CreateIndex(
                name: "IX_Avaliacao_id_usuario",
                table: "Avaliacao",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_CurtidaPasseio_id_passeio",
                table: "CurtidaPasseio",
                column: "id_passeio");

            migrationBuilder.CreateIndex(
                name: "IX_CurtidaPasseio_id_usuario",
                table: "CurtidaPasseio",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_GrupoPasseio_id_grupo",
                table: "GrupoPasseio",
                column: "id_grupo");

            migrationBuilder.CreateIndex(
                name: "IX_GrupoPasseio_id_passeio",
                table: "GrupoPasseio",
                column: "id_passeio");

            migrationBuilder.CreateIndex(
                name: "IX_GrupoUsuario_id_grupo",
                table: "GrupoUsuario",
                column: "id_grupo");

            migrationBuilder.CreateIndex(
                name: "IX_GrupoUsuario_id_usuario",
                table: "GrupoUsuario",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_Passeio_Enderecoid_endereco",
                table: "Passeio",
                column: "Enderecoid_endereco");

            migrationBuilder.CreateIndex(
                name: "IX_PasseioFavorito_id_passeio",
                table: "PasseioFavorito",
                column: "id_passeio");

            migrationBuilder.CreateIndex(
                name: "IX_PasseioFavorito_id_usuario",
                table: "PasseioFavorito",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_PasseioFuncionario_id_funcionario",
                table: "PasseioFuncionario",
                column: "id_funcionario");

            migrationBuilder.CreateIndex(
                name: "IX_PasseioFuncionario_id_passeio",
                table: "PasseioFuncionario",
                column: "id_passeio");

            migrationBuilder.CreateIndex(
                name: "IX_passeiospendentes_id_passeio",
                table: "passeiospendentes",
                column: "id_passeio");

            migrationBuilder.CreateIndex(
                name: "IX_passeiospendentes_id_usuario",
                table: "passeiospendentes",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_id_pergunta",
                table: "Usuario",
                column: "id_pergunta");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioLocalizacao_id_localizacao",
                table: "UsuarioLocalizacao",
                column: "id_localizacao");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioLocalizacao_id_usuario",
                table: "UsuarioLocalizacao",
                column: "id_usuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Avaliacao");

            migrationBuilder.DropTable(
                name: "Categoria");

            migrationBuilder.DropTable(
                name: "CurtidaPasseio");

            migrationBuilder.DropTable(
                name: "GrupoPasseio");

            migrationBuilder.DropTable(
                name: "GrupoUsuario");

            migrationBuilder.DropTable(
                name: "PasseioFavorito");

            migrationBuilder.DropTable(
                name: "PasseioFuncionario");

            migrationBuilder.DropTable(
                name: "passeiospendentes");

            migrationBuilder.DropTable(
                name: "UsuarioLocalizacao");

            migrationBuilder.DropTable(
                name: "Grupo");

            migrationBuilder.DropTable(
                name: "Funcionario");

            migrationBuilder.DropTable(
                name: "Passeio");

            migrationBuilder.DropTable(
                name: "Localizacao");

            migrationBuilder.DropTable(
                name: "Usuario");

            migrationBuilder.DropTable(
                name: "Endereco");

            migrationBuilder.DropTable(
                name: "PerguntaSeguranca");
        }
    }
}
