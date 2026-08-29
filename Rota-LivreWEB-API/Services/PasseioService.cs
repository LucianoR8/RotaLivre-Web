using Microsoft.EntityFrameworkCore;
using Rota_LivreWEB_API.Data;
using Rota_LivreWEB_API.DTOs;
using Rota_LivreWEB_API.Interfaces;
using Rota_LivreWEB_API.Models;

namespace Rota_LivreWEB_API.Services
{
    public class PasseioService : IPasseioService
    {
        private readonly AppDbContext _context;

        public PasseioService(AppDbContext context)
        {
            _context = context;
        }

        // =========================================================
        // MONTAR URL DA IMAGEM
        // =========================================================

        private string MontarImagemUrl(string imgUrl)
        {
            if (string.IsNullOrWhiteSpace(imgUrl))
                return "";

            // Imagem armazenada no Supabase Storage
            if (imgUrl.StartsWith(
                    "http://",
                    StringComparison.OrdinalIgnoreCase) ||
                imgUrl.StartsWith(
                    "https://",
                    StringComparison.OrdinalIgnoreCase))
            {
                return imgUrl;
            }

            // Compatibilidade com imagens antigas
            // armazenadas no Render
            return $"https://rotalivre-web.onrender.com/img/passeios/{imgUrl}";
        }

        // =========================================================
        // LISTAR TODOS OS PASSEIOS
        // =========================================================

        public async Task<IEnumerable<PasseioDto>> GetAllAsync()
        {
            return await _context.Passeio
                .Select(p => new PasseioDto
                {
                    Id = p.id_passeio,

                    Nome = p.nome_passeio,

                    Descricao = p.descricao,

                    Funcionamento = p.funcionamento,

                    CategoriaId = p.id_categoria,

                    CategoriaNome = p.Categoria != null
                        ? p.Categoria.tipo_categoria
                        : null,

                    ImagemUrl = p.img_url,

                    QuantidadeCurtidas =
                        _context.CurtidaPasseio
                            .Count(c =>
                                c.id_passeio ==
                                p.id_passeio),

                    Endereco = p.Endereco != null
                        ? new EnderecoDto
                        {
                            NomeRua =
                                p.Endereco.nome_rua,

                            NumeroRua =
                                p.Endereco.numero_rua,

                            Complemento =
                                p.Endereco.complemento,

                            Bairro =
                                p.Endereco.bairro,

                            Cep =
                                p.Endereco.cep,

                            Latitude =
                                p.Endereco.Latitude,

                            Longitude =
                                p.Endereco.Longitude,

                            RaioMetros =
                                p.Endereco.RaioMetros
                        }
                        : null
                })
                .ToListAsync();
        }

        // =========================================================
        // BUSCAR PASSEIO POR ID
        // =========================================================

        public async Task<PasseioDto> GetByIdAsync(int id)
        {
            var passeio = await _context.Passeio
                .Include(p => p.Endereco)
                .FirstOrDefaultAsync(
                    p => p.id_passeio == id);

            if (passeio == null)
                return null;

            return new PasseioDto
            {
                Id = passeio.id_passeio,

                Nome = passeio.nome_passeio,

                Descricao = passeio.descricao,

                Funcionamento =
                    passeio.funcionamento,

                ImagemUrl =
                    MontarImagemUrl(
                        passeio.img_url),

                QuantidadeCurtidas =
                    await _context.CurtidaPasseio
                        .CountAsync(
                            c =>
                                c.id_passeio ==
                                passeio.id_passeio),

                UsuarioJaCurtiu = false,

                Endereco =
                    passeio.Endereco != null
                        ? new EnderecoDto
                        {
                            NomeRua =
                                passeio.Endereco.nome_rua,

                            NumeroRua =
                                passeio.Endereco.numero_rua,

                            Complemento =
                                passeio.Endereco.complemento,

                            Bairro =
                                passeio.Endereco.bairro,

                            Cep =
                                passeio.Endereco.cep,

                            Latitude =
                                passeio.Endereco.Latitude,

                            Longitude =
                                passeio.Endereco.Longitude,

                            RaioMetros =
                                passeio.Endereco.RaioMetros
                        }
                        : null
            };
        }

        // =========================================================
        // CRIAR PASSEIO
        // =========================================================

        public async Task<PasseioDto> CreateAsync(
            PasseioDto dto)
        {
            var passeio = new Passeio
            {
                id_categoria =
                    dto.CategoriaId,

                nome_passeio =
                    dto.Nome,

                descricao =
                    dto.Descricao,

                funcionamento =
                    dto.Funcionamento,

                img_url =
                    dto.ImagemUrl,

                status =
                    "ativo"
            };

            _context.Passeio.Add(passeio);

            await _context.SaveChangesAsync();

            dto.Id =
                passeio.id_passeio;

            return dto;
        }

        // =========================================================
        // PASSEIOS POR CATEGORIA
        // =========================================================

        public async Task<IEnumerable<PasseioDto>>
            GetByCategoriaAsync(int categoriaId)
        {
            var passeios = await _context.Passeio
                .Where(p =>
                    p.id_categoria ==
                    categoriaId)
                .Select(p => new
                {
                    Passeio = p,

                    QuantidadeCurtidas =
                        _context.CurtidaPasseio
                            .Count(c =>
                                c.id_passeio ==
                                p.id_passeio)
                })
                .ToListAsync();

            return passeios.Select(x =>
                new PasseioDto
                {
                    Id =
                        x.Passeio.id_passeio,

                    Nome =
                        x.Passeio.nome_passeio,

                    Descricao =
                        x.Passeio.descricao,

                    Funcionamento =
                        x.Passeio.funcionamento,

                    ImagemUrl =
                        MontarImagemUrl(
                            x.Passeio.img_url),

                    QuantidadeCurtidas =
                        x.QuantidadeCurtidas,

                    CategoriaNome =
                        x.Passeio.Categoria
                            ?.tipo_categoria
                });
        }

        // =========================================================
        // CURTIDA
        // =========================================================

        public async Task<bool> AlternarCurtidaAsync(
            int usuarioId,
            int passeioId)
        {
            var curtida =
                await _context.CurtidaPasseio
                    .FirstOrDefaultAsync(c =>
                        c.id_usuario ==
                            usuarioId &&
                        c.id_passeio ==
                            passeioId);

            if (curtida != null)
            {
                _context.CurtidaPasseio
                    .Remove(curtida);

                await _context.SaveChangesAsync();

                return false;
            }

            var novaCurtida =
                new CurtidaPasseio
                {
                    id_usuario =
                        usuarioId,

                    id_passeio =
                        passeioId
                };

            await _context.CurtidaPasseio
                .AddAsync(novaCurtida);

            await _context.SaveChangesAsync();

            return true;
        }

        // =========================================================
        // BUSCAR PASSEIO COM DADOS DO USUÁRIO
        // =========================================================

        public async Task<PasseioDto>
            GetByIdComUsuarioAsync(
                int id,
                int usuarioId)
        {
            var passeio =
                await _context.Passeio
                    .Include(p =>
                        p.Endereco)
                    .FirstOrDefaultAsync(
                        p =>
                            p.id_passeio ==
                            id);

            if (passeio == null)
                return null;

            var jaCurtiu =
                await _context.CurtidaPasseio
                    .AnyAsync(c =>
                        c.id_usuario ==
                            usuarioId &&
                        c.id_passeio ==
                            id);

            var jaPendente =
                await _context.PasseioPendente
                    .AnyAsync(p =>
                        p.id_usuario ==
                            usuarioId &&
                        p.id_passeio ==
                            id);

            var quantidadeCurtidas =
                await _context.CurtidaPasseio
                    .CountAsync(c =>
                        c.id_passeio ==
                        passeio.id_passeio);

            return new PasseioDto
            {
                Id =
                    passeio.id_passeio,

                Nome =
                    passeio.nome_passeio,

                Descricao =
                    passeio.descricao,

                Funcionamento =
                    passeio.funcionamento,

                ImagemUrl =
                    MontarImagemUrl(
                        passeio.img_url),

                QuantidadeCurtidas =
                    quantidadeCurtidas,

                UsuarioJaCurtiu =
                    jaCurtiu,

                UsuarioJaPendente =
                    jaPendente,

                Endereco =
                    passeio.Endereco != null
                        ? new EnderecoDto
                        {
                            NomeRua =
                                passeio.Endereco.nome_rua,

                            NumeroRua =
                                passeio.Endereco.numero_rua,

                            Complemento =
                                passeio.Endereco.complemento,

                            Bairro =
                                passeio.Endereco.bairro,

                            Cep =
                                passeio.Endereco.cep,

                            Latitude =
                                passeio.Endereco.Latitude,

                            Longitude =
                                passeio.Endereco.Longitude,

                            RaioMetros =
                                passeio.Endereco.RaioMetros
                        }
                        : null
            };
        }

        // =========================================================
        // CURTIR / DESCURTIR COM TOTAL
        // =========================================================

        public async Task<(
            bool curtiu,
            int totalCurtidas)>
            AlternarCurtidaComTotalAsync(
                int usuarioId,
                int passeioId)
        {
            var curtida =
                await _context.CurtidaPasseio
                    .FirstOrDefaultAsync(c =>
                        c.id_usuario ==
                            usuarioId &&
                        c.id_passeio ==
                            passeioId);

            bool curtiu;

            if (curtida != null)
            {
                _context.CurtidaPasseio
                    .Remove(curtida);

                curtiu = false;
            }
            else
            {
                var pendente =
                    await _context.PasseioPendente
                        .Where(p =>
                            p.id_usuario ==
                                usuarioId &&
                            p.id_passeio ==
                                passeioId)
                        .ToListAsync();

                if (pendente.Any())
                {
                    _context.PasseioPendente
                        .RemoveRange(pendente);
                }

                var nova =
                    new CurtidaPasseio
                    {
                        id_usuario =
                            usuarioId,

                        id_passeio =
                            passeioId
                    };

                await _context.CurtidaPasseio
                    .AddAsync(nova);

                curtiu = true;
            }

            await _context.SaveChangesAsync();

            var total =
                await _context.CurtidaPasseio
                    .CountAsync(c =>
                        c.id_passeio ==
                        passeioId);

            return (
                curtiu,
                total);
        }

        // =========================================================
        // PENDENTE
        // =========================================================

        public async Task<bool> AlternarPendenteAsync(
            int usuarioId,
            int passeioId)
        {
            var existente =
                await _context.PasseioPendente
                    .FirstOrDefaultAsync(p =>
                        p.id_usuario ==
                            usuarioId &&
                        p.id_passeio ==
                            passeioId);

            if (existente != null)
            {
                _context.PasseioPendente
                    .Remove(existente);

                await _context.SaveChangesAsync();

                return false;
            }

            var curtidas =
                await _context.CurtidaPasseio
                    .Where(c =>
                        c.id_usuario ==
                            usuarioId &&
                        c.id_passeio ==
                            passeioId)
                    .ToListAsync();

            if (curtidas.Any())
            {
                _context.CurtidaPasseio
                    .RemoveRange(curtidas);
            }

            var novo =
                new PasseioPendente
                {
                    id_usuario =
                        usuarioId,

                    id_passeio =
                        passeioId,

                    data_adicao =
                        DateTime.UtcNow
                };

            await _context.PasseioPendente
                .AddAsync(novo);

            await _context.SaveChangesAsync();

            return true;
        }

        // =========================================================
        // MEUS PASSEIOS
        // =========================================================

        public async Task<(
            List<PasseioDto>,
            List<PasseioDto>)>
            GetMeusPasseiosAsync(
                int userId)
        {
            var curtidos =
                await _context.CurtidaPasseio
                    .Where(c =>
                        c.id_usuario ==
                        userId)
                    .Select(c =>
                        new PasseioDto
                        {
                            Id =
                                c.Passeio
                                    .id_passeio,

                            Nome =
                                c.Passeio
                                    .nome_passeio,

                            ImagemUrl =
                                c.Passeio
                                    .img_url
                        })
                    .ToListAsync();

            var pendentes =
                await _context.PasseioPendente
                    .Where(p =>
                        p.id_usuario ==
                        userId)
                    .Select(p =>
                        new PasseioDto
                        {
                            Id =
                                p.Passeio
                                    .id_passeio,

                            Nome =
                                p.Passeio
                                    .nome_passeio,

                            ImagemUrl =
                                p.Passeio
                                    .img_url
                        })
                    .ToListAsync();

            // Corrige URLs das imagens
            // mantendo compatibilidade
            // com imagens antigas.

            foreach (var passeio in curtidos)
            {
                passeio.ImagemUrl =
                    MontarImagemUrl(
                        passeio.ImagemUrl);
            }

            foreach (var passeio in pendentes)
            {
                passeio.ImagemUrl =
                    MontarImagemUrl(
                        passeio.ImagemUrl);
            }

            return (
                curtidos,
                pendentes);
        }
    }
}