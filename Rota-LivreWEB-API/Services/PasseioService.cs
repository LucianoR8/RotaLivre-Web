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
                    ImagemUrl = $"http://192.168.15.121:7015/img/passeios/{p.img_url}",
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
                ImagemUrl = $"http://192.168.15.121:7015/img/passeios/{passeio.img_url}",
                QuantidadeCurtidas = passeio.QuantidadeCurtidas,
                Endereco = passeio.Endereco == null ? null : new EnderecoDto
                {
                    NomeRua = passeio.Endereco.nome_rua,
                    NumeroRua = passeio.Endereco.numero_rua,
                    Complemento = passeio.Endereco.complemento,
                    Bairro = passeio.Endereco.bairro,
                    Cep = passeio.Endereco.cep
                }
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

       
    }
}