using Microsoft.EntityFrameworkCore;
using Rota_LivreWEB_API.Data;
using Rota_LivreWEB_API.DTOs;
using Rota_LivreWEB_API.Interfaces;
using Rota_LivreWEB_API.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Rota_LivreWEB_API.Services
{
    public class PasseioService : IPasseioService
    {
        private readonly AppDbContext _context;

        public PasseioService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PasseioDto>> GetAllAsync()
        {
            return await _context.Passeio
                .Select(p => new PasseioDto
                {
                    Id = p.id_passeio,
                    Nome = p.nome_passeio,
                    Descricao = p.descricao,
                    Funcionamento = p.funcionamento,
                    ImagemUrl = $"https://rotalivre-web.onrender.com/img/passeios/{p.img_url}",
                    QuantidadeCurtidas = p.QuantidadeCurtidas
                })
                .ToListAsync();
        }

        public async Task<PasseioDto> GetByIdAsync(int id)
        {
            var passeio = await _context.Passeio
                .Include(p => p.Endereco)
                .FirstOrDefaultAsync(p => p.id_passeio == id);
            
            if (passeio == null)
                return null;

            return new PasseioDto
            {
                Id = passeio.id_passeio,
                Nome = passeio.nome_passeio,
                Descricao = passeio.descricao,
                Funcionamento = passeio.funcionamento,
                ImagemUrl = $"https://rotalivre-web.onrender.com/img/passeios/{passeio.img_url}",
                QuantidadeCurtidas = await _context.CurtidaPasseio
                    .CountAsync(c => c.id_passeio == passeio.id_passeio),
                UsuarioJaCurtiu = false,
                Endereco = passeio.Endereco != null ? new EnderecoDto
                {
                    NomeRua = passeio.Endereco.nome_rua,
                    NumeroRua = passeio.Endereco.numero_rua,
                    Complemento = passeio.Endereco.complemento,
                    Bairro = passeio.Endereco.bairro,
                    Cep = passeio.Endereco.cep
                } : null
            };
        }


        public async Task<PasseioDto> CreateAsync(PasseioDto dto)
        {
            var passeio = new Passeio
            {
                nome_passeio = dto.Nome,
                descricao = dto.Descricao,
                funcionamento = dto.Funcionamento,
                img_url = dto.ImagemUrl,
                QuantidadeCurtidas = dto.QuantidadeCurtidas
            };

            _context.Passeio.Add(passeio);
            await _context.SaveChangesAsync();

            dto.Id = passeio.id_passeio;
            return dto;
        }

        public async Task<IEnumerable<PasseioDto>> GetByCategoriaAsync(int categoriaId)
        {
            return await _context.Passeio
                .Where(p => p.id_categoria == categoriaId)
                .Select(p => new PasseioDto
                {
                    Id = p.id_passeio,
                    Nome = p.nome_passeio,
                    Descricao = p.descricao,
                    Funcionamento = p.funcionamento,
                    ImagemUrl = $"https://rotalivre-web.onrender.com/img/passeios/{p.img_url}",
                    QuantidadeCurtidas = _context.CurtidaPasseio
                        .Count(c => c.id_passeio == p.id_passeio)
                })
                .ToListAsync();
        }

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

        public async Task<PasseioDto> GetByIdComUsuarioAsync(int id, int usuarioId)
        {
            var passeio = await _context.Passeio
                .Include(p => p.Endereco)
                .FirstOrDefaultAsync(p => p.id_passeio == id);

            if (passeio == null)
                return null;

            var jaCurtiu = await _context.CurtidaPasseio
                .AnyAsync(c => c.id_usuario == usuarioId && c.id_passeio == id);

            return new PasseioDto
            {
                Id = passeio.id_passeio,
                Nome = passeio.nome_passeio,
                Descricao = passeio.descricao,
                Funcionamento = passeio.funcionamento,
                ImagemUrl = $"https://rotalivre-web.onrender.com/img/passeios/{passeio.img_url}",
                QuantidadeCurtidas = await _context.CurtidaPasseio
                    .CountAsync(c => c.id_passeio == passeio.id_passeio),
                UsuarioJaCurtiu = jaCurtiu
            };
        }

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

        public async Task<(List<PasseioDto>, List<PasseioDto>)> GetMeusPasseiosAsync(int userId)
        {
            var curtidos = await _context.CurtidaPasseio
                .Where(c => c.id_usuario == userId)
                .Select(c => new PasseioDto
                {
                    Id = c.Passeio.id_passeio,
                    Nome = c.Passeio.nome_passeio,
                    ImagemUrl = $"https://rotalivre-web.onrender.com/img/passeios/{c.Passeio.img_url}"
                })
                .ToListAsync();

            var pendentes = await _context.PasseioPendente
                .Where(p => p.id_usuario == userId)
                .Select(p => new PasseioDto
                {
                    Id = p.Passeio.id_passeio,
                    Nome = p.Passeio.nome_passeio,
                    ImagemUrl = $"https://rotalivre-web.onrender.com/img/passeios/{p.Passeio.img_url}"
                })
                .ToListAsync();

            return (curtidos, pendentes);
        }

    }
}