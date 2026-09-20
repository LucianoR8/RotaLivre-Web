using Microsoft.EntityFrameworkCore;
using Rota_LivreWEB_API.Data;
using Rota_LivreWEB_API.DTOs;
using Rota_LivreWEB_API.Interfaces;
using Rota_LivreWEB_API.Models;
using System.Net.Http.Headers;

namespace Rota_LivreWEB_API.Services
{
    public class PasseioService : IPasseioService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public PasseioService(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // =========================================================
        // LISTAR TODOS OS PASSEIOS
        // =========================================================

        public async Task<IEnumerable<PasseioDto>> GetAllAsync()
        {
            return await _context.Passeio
                .Include(p => p.Categoria)
                .Include(p => p.Cidade) // NOVO: Inclui a cidade
                .Include(p => p.Endereco)
                .Select(p => new PasseioDto
                {
                    Id = p.id_passeio,
                    Nome = p.nome_passeio,
                    Descricao = p.descricao,
                    Funcionamento = p.funcionamento,

                    CategoriaId = p.id_categoria,
                    CategoriaNome = p.Categoria != null ? p.Categoria.tipo_categoria : null,

                    // NOVO: Retorna a Cidade para o Front-end
                    CidadeId = p.id_cidade,
                    CidadeNome = p.Cidade != null ? p.Cidade.tipo_categoria : null,

                    ImagemUrl = p.img_url,

                    QuantidadeCurtidas = _context.CurtidaPasseio
                            .Count(c => c.id_passeio == p.id_passeio),

                    Endereco = p.Endereco != null
                        ? new EnderecoDto
                        {
                            NomeRua = p.Endereco.nome_rua,
                            NumeroRua = p.Endereco.numero_rua,
                            Complemento = p.Endereco.complemento,
                            Bairro = p.Endereco.bairro,
                            Cep = p.Endereco.cep,
                            Latitude = p.Endereco.Latitude,
                            Longitude = p.Endereco.Longitude,
                            RaioMetros = p.Endereco.RaioMetros
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
                .Include(p => p.Categoria) // NOVO
                .Include(p => p.Cidade)    // NOVO
                .FirstOrDefaultAsync(p => p.id_passeio == id);

            if (passeio == null)
                return null;

            return new PasseioDto
            {
                Id = passeio.id_passeio,
                Nome = passeio.nome_passeio,
                Descricao = passeio.descricao,
                Funcionamento = passeio.funcionamento,
                ImagemUrl = passeio.img_url,

                CategoriaId = passeio.id_categoria,
                CategoriaNome = passeio.Categoria?.tipo_categoria,

                // NOVO: Retorna a Cidade para o Front-end
                CidadeId = passeio.id_cidade,
                CidadeNome = passeio.Cidade?.tipo_categoria,

                QuantidadeCurtidas = await _context.CurtidaPasseio
                        .CountAsync(c => c.id_passeio == passeio.id_passeio),

                UsuarioJaCurtiu = false,

                Endereco = passeio.Endereco != null
                        ? new EnderecoDto
                        {
                            NomeRua = passeio.Endereco.nome_rua,
                            NumeroRua = passeio.Endereco.numero_rua,
                            Complemento = passeio.Endereco.complemento,
                            Bairro = passeio.Endereco.bairro,
                            Cep = passeio.Endereco.cep,
                            Latitude = passeio.Endereco.Latitude,
                            Longitude = passeio.Endereco.Longitude,
                            RaioMetros = passeio.Endereco.RaioMetros
                        }
                        : null
            };
        }

        // =========================================================
        // CRIAR PASSEIO
        // =========================================================

        public async Task<PasseioDto> CreateAsync(PasseioDto dto)
        {
            var passeio = new Passeio
            {
                id_categoria = dto.CategoriaId,
                id_cidade = dto.CidadeId, // NOVO
                nome_passeio = dto.Nome,
                descricao = dto.Descricao,
                funcionamento = dto.Funcionamento,
                img_url = dto.ImagemUrl,
                status = "ativo"
            };

            _context.Passeio.Add(passeio);
            await _context.SaveChangesAsync();

            dto.Id = passeio.id_passeio;
            return dto;
        }

        // =========================================================
        // PASSEIOS POR CATEGORIA
        // =========================================================

        public async Task<IEnumerable<PasseioDto>> GetByCategoriaAsync(int categoriaId)
        {
            return await _context.Passeio
                .Include(p => p.Categoria)
                .Where(p => p.id_categoria == categoriaId)
                .Select(p => new PasseioDto
                {
                    Id = p.id_passeio,
                    Nome = p.nome_passeio,
                    Descricao = p.descricao,
                    Funcionamento = p.funcionamento,
                    ImagemUrl = p.img_url,
                    QuantidadeCurtidas = _context.CurtidaPasseio
                            .Count(c => c.id_passeio == p.id_passeio),
                    CategoriaNome = p.Categoria != null ? p.Categoria.tipo_categoria : null
                })
                .ToListAsync();
        }

        // =========================================================
        // ALTERNAR CURTIDA
        // =========================================================

        public async Task<bool> AlternarCurtidaAsync(int usuarioId, int passeioId)
        {
            var curtida = await _context.CurtidaPasseio
                    .FirstOrDefaultAsync(c => c.id_usuario == usuarioId && c.id_passeio == passeioId);

            if (curtida != null)
            {
                _context.CurtidaPasseio.Remove(curtida);
                await _context.SaveChangesAsync();
                return false;
            }

            var novaCurtida = new CurtidaPasseio
            {
                id_usuario = usuarioId,
                id_passeio = passeioId
            };

            await _context.CurtidaPasseio.AddAsync(novaCurtida);
            await _context.SaveChangesAsync();
            return true;
        }

        // =========================================================
        // BUSCAR PASSEIO COM DADOS DO USUÁRIO
        // =========================================================

        public async Task<PasseioDto> GetByIdComUsuarioAsync(int id, int usuarioId)
        {
            var passeio = await _context.Passeio
                    .Include(p => p.Endereco)
                    .Include(p => p.Categoria) // NOVO
                    .Include(p => p.Cidade)    // NOVO
                    .FirstOrDefaultAsync(p => p.id_passeio == id);

            if (passeio == null) return null;

            var jaCurtiu = await _context.CurtidaPasseio
                    .AnyAsync(c => c.id_usuario == usuarioId && c.id_passeio == id);

            var jaPendente = await _context.PasseioPendente
                    .AnyAsync(p => p.id_usuario == usuarioId && p.id_passeio == id);

            var quantidadeCurtidas = await _context.CurtidaPasseio
                    .CountAsync(c => c.id_passeio == passeio.id_passeio);

            return new PasseioDto
            {
                Id = passeio.id_passeio,
                Nome = passeio.nome_passeio,
                Descricao = passeio.descricao,
                Funcionamento = passeio.funcionamento,
                ImagemUrl = passeio.img_url,

                CategoriaId = passeio.id_categoria,
                CategoriaNome = passeio.Categoria?.tipo_categoria,

                // NOVO: Retorna a Cidade para o Front-end
                CidadeId = passeio.id_cidade,
                CidadeNome = passeio.Cidade?.tipo_categoria,

                QuantidadeCurtidas = quantidadeCurtidas,
                UsuarioJaCurtiu = jaCurtiu,
                UsuarioJaPendente = jaPendente,

                Endereco = passeio.Endereco != null
                        ? new EnderecoDto
                        {
                            NomeRua = passeio.Endereco.nome_rua,
                            NumeroRua = passeio.Endereco.numero_rua,
                            Complemento = passeio.Endereco.complemento,
                            Bairro = passeio.Endereco.bairro,
                            Cep = passeio.Endereco.cep,
                            Latitude = passeio.Endereco.Latitude,
                            Longitude = passeio.Endereco.Longitude,
                            RaioMetros = passeio.Endereco.RaioMetros
                        }
                        : null
            };
        }

        // =========================================================
        // CURTIR / DESCURTIR + TOTAL
        // =========================================================

        public async Task<(bool curtiu, int totalCurtidas)> AlternarCurtidaComTotalAsync(int usuarioId, int passeioId)
        {
            var curtida = await _context.CurtidaPasseio
                    .FirstOrDefaultAsync(c => c.id_usuario == usuarioId && c.id_passeio == passeioId);

            bool curtiu;

            if (curtida != null)
            {
                _context.CurtidaPasseio.Remove(curtida);
                curtiu = false;
            }
            else
            {
                var pendente = await _context.PasseioPendente
                        .Where(p => p.id_usuario == usuarioId && p.id_passeio == passeioId)
                        .ToListAsync();

                if (pendente.Any())
                {
                    _context.PasseioPendente.RemoveRange(pendente);
                }

                var nova = new CurtidaPasseio
                {
                    id_usuario = usuarioId,
                    id_passeio = passeioId
                };

                await _context.CurtidaPasseio.AddAsync(nova);
                curtiu = true;
            }

            await _context.SaveChangesAsync();

            var total = await _context.CurtidaPasseio
                    .CountAsync(c => c.id_passeio == passeioId);

            return (curtiu, total);
        }

        // =========================================================
        // ALTERNAR PENDENTE
        // =========================================================

        public async Task<bool> AlternarPendenteAsync(int usuarioId, int passeioId)
        {
            var existente = await _context.PasseioPendente
                    .FirstOrDefaultAsync(p => p.id_usuario == usuarioId && p.id_passeio == passeioId);

            if (existente != null)
            {
                _context.PasseioPendente.Remove(existente);
                await _context.SaveChangesAsync();
                return false;
            }

            var curtidas = await _context.CurtidaPasseio
                    .Where(c => c.id_usuario == usuarioId && c.id_passeio == passeioId)
                    .ToListAsync();

            if (curtidas.Any())
            {
                _context.CurtidaPasseio.RemoveRange(curtidas);
            }

            var novo = new PasseioPendente
            {
                id_usuario = usuarioId,
                id_passeio = passeioId,
                data_adicao = DateTime.UtcNow
            };

            await _context.PasseioPendente.AddAsync(novo);
            await _context.SaveChangesAsync();
            return true;
        }

        // =========================================================
        // MEUS PASSEIOS
        // =========================================================

        public async Task<(List<PasseioDto>, List<PasseioDto>)> GetMeusPasseiosAsync(int userId)
        {
            var curtidos = await _context.CurtidaPasseio
                    .Where(c => c.id_usuario == userId)
                    .Select(c => new PasseioDto
                    {
                        Id = c.Passeio.id_passeio,
                        Nome = c.Passeio.nome_passeio,
                        ImagemUrl = c.Passeio.img_url
                    })
                    .ToListAsync();

            var pendentes = await _context.PasseioPendente
                    .Where(p => p.id_usuario == userId)
                    .Select(p => new PasseioDto
                    {
                        Id = p.Passeio.id_passeio,
                        Nome = p.Passeio.nome_passeio,
                        ImagemUrl = p.Passeio.img_url
                    })
                    .ToListAsync();

            return (curtidos, pendentes);
        }

        public async Task<bool> DeletarAsync(int id)
        {
            var passeio = await _context.Passeio
                .Include(p => p.Endereco)
                .FirstOrDefaultAsync(p => p.id_passeio == id);

            if (passeio == null) return false;

            var imagemUrl = passeio.img_url;

            var avaliacoes = await _context.Avaliacao.Where(a => a.id_passeio == id).ToListAsync();
            if (avaliacoes.Any()) _context.Avaliacao.RemoveRange(avaliacoes);

            var curtidas = await _context.CurtidaPasseio.Where(c => c.id_passeio == id).ToListAsync();
            if (curtidas.Any()) _context.CurtidaPasseio.RemoveRange(curtidas);

            var pendentes = await _context.PasseioPendente.Where(p => p.id_passeio == id).ToListAsync();
            if (pendentes.Any()) _context.PasseioPendente.RemoveRange(pendentes);

            var grupos = await _context.Grupo.Where(g => g.id_passeio == id).ToListAsync();
            foreach (var grupo in grupos)
            {
                grupo.status = "ENCERRADO";
                grupo.id_passeio = null;
                grupo.Passeio = null;
            }

            if (passeio.Endereco != null)
            {
                _context.Endereco.Remove(passeio.Endereco);
            }

            _context.Passeio.Remove(passeio);
            await _context.SaveChangesAsync();

            if (!string.IsNullOrWhiteSpace(imagemUrl))
            {
                try { await ExcluirImagemSupabaseAsync(imagemUrl); }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Não foi possível excluir a imagem do Supabase: {ex.Message}");
                }
            }

            return true;
        }

        private async Task ExcluirImagemSupabaseAsync(string imagemUrl)
        {
            var supabaseUrl = _config["Supabase:Url"];
            var supabaseKey = _config["Supabase:Key"];
            var bucket = _config["Supabase:Bucket"];

            if (string.IsNullOrWhiteSpace(supabaseUrl) ||
                string.IsNullOrWhiteSpace(supabaseKey) ||
                string.IsNullOrWhiteSpace(bucket))
            {
                throw new Exception("Configuração do Supabase não encontrada.");
            }

            var marcador = $"/storage/v1/object/public/{bucket}/";
            var indice = imagemUrl.IndexOf(marcador, StringComparison.OrdinalIgnoreCase);

            if (indice < 0) return;

            var caminhoArquivo = imagemUrl.Substring(indice + marcador.Length);
            if (string.IsNullOrWhiteSpace(caminhoArquivo)) return;

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", supabaseKey);
            httpClient.DefaultRequestHeaders.Add("apikey", supabaseKey);

            var url = $"{supabaseUrl}/storage/v1/object/{bucket}/{caminhoArquivo}";
            var response = await httpClient.DeleteAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var erro = await response.Content.ReadAsStringAsync();
                throw new Exception($"Supabase retornou {(int)response.StatusCode}: {erro}");
            }
        }

        public async Task<IEnumerable<PasseioDto>> GetByCidadeECategoriaAsync(int cidadeId, int categoriaId)
        {
            return await _context.Passeio
                .Include(p => p.Categoria)
                .Include(p => p.Cidade)
                .Where(p => p.id_cidade == cidadeId && p.id_categoria == categoriaId && p.status == "ativo")
                .Select(p => new PasseioDto
                {
                    Id = p.id_passeio,
                    Nome = p.nome_passeio,
                    Descricao = p.descricao,
                    ImagemUrl = p.img_url,
                    CategoriaId = p.id_categoria,
                    CategoriaNome = p.Categoria != null ? p.Categoria.tipo_categoria : null,
                    CidadeId = p.id_cidade,
                    CidadeNome = p.Cidade != null ? p.Cidade.tipo_categoria : null,
                    QuantidadeCurtidas = _context.CurtidaPasseio.Count(c => c.id_passeio == p.id_passeio)
                })
                .ToListAsync();
        }
    }
}