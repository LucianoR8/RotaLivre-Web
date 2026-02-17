using System.Threading.Tasks;
using Rota_LivreWEB_API.DTOs;

namespace Rota_LivreWEB_API.Interfaces
{
    public interface IHomeService
    {
        Task<HomeDto> GetHomeAsync(int usuarioId);
    }
}