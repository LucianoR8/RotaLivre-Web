using System.Collections.Generic;
using System.Threading.Tasks;
using Rota_LivreWEB_API.DTOs;

namespace Rota_LivreWEB_API.Interfaces
{
    public interface IPasseioService
    {
        Task<IEnumerable<PasseioDto>> GetAllAsync();
        Task<PasseioDto> GetByIdAsync(int id);
        Task<PasseioDto> CreateAsync(PasseioDto dto);
        Task<IEnumerable<PasseioDto>> GetByCategoriaAsync(int categoriaId);
        Task<bool> AlternarCurtidaAsync(int usuarioId, int passeioId);
        Task<PasseioDto> GetByIdComUsuarioAsync(int id, int usuarioId);
        Task<(bool curtiu, int totalCurtidas)> AlternarCurtidaComTotalAsync(int usuarioId, int passeioId);
        Task<bool> AlternarPendenteAsync(int usuarioId, int passeioId);
        Task<(List<PasseioDto> curtidos, List<PasseioDto> pendentes)> GetMeusPasseiosAsync(int userId);
    }
}