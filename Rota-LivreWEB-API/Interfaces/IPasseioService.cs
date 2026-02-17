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
    }
}