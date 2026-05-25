using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;

namespace RotaLivreMobile.Services
{
    internal class AuthService
    {
        private const string TokenKey = "auth_token";

        public async Task SalvarToken(string token)
        {
            await SecureStorage.SetAsync(TokenKey, token);
        }

        public async Task<string?> ObterToken()
        {
            return await SecureStorage.GetAsync(TokenKey);
        }

        public void RemoverToken()
        {
            SecureStorage.Remove(TokenKey);
        }
    }
}
