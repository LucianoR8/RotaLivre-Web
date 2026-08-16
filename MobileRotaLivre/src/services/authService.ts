import { api } from './api';
import { LoginRequest, LoginResponse } from '../types';

export const authService = {
  login: async (credenciais: LoginRequest): Promise<LoginResponse> => {
    // Faz a chamada para a API
    const response = await api.post('/AuthApi/login', credenciais);
    const data = response.data;

    // Monta o objeto de usuário com todos os campos que a interface 'Usuario' exige
    const usuario = {
      id: data.usuario?.id,
      id_usuario: data.usuario?.id, // Mantém compatibilidade com o id_usuario exigido pelo TS
      email: data.usuario?.email || credenciais.email, // Usa o e-mail da API ou o digitado na tela de login
      nome: data.usuario?.nome,
      nome_completo: data.usuario?.nome // Mapeia o 'nome' do C# para o 'nome_completo' do React
    };

    if (data.token) {
      localStorage.setItem('token', data.token);
      localStorage.setItem('usuario', JSON.stringify(usuario));
    }

    return {
      token: data.token,
      usuario
    };
  },

  logout: () => {
    localStorage.removeItem('token');
    localStorage.removeItem('usuario');
  }
};