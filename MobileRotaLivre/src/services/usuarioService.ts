import { api } from './api';
import {
  UsuarioCadastroDto,
  PerguntaSeguranca,
  UsuarioPerfilDto
} from '../types';

export const usuarioService = {

  buscarPerguntas: async (): Promise<PerguntaSeguranca[]> => {
    const response = await api.get<PerguntaSeguranca[]>(
      '/UsuarioApi/perguntas'
    );

    return response.data;
  },

  cadastrar: async (
    dados: UsuarioCadastroDto
  ): Promise<void> => {
    await api.post('/UsuarioApi/cadastrar', dados);
  },

  buscarPerfil: async (
    id: number
  ): Promise<UsuarioPerfilDto> => {
    const response = await api.get<UsuarioPerfilDto>(
      `/UsuarioApi/perfil/${id}`
    );

    return response.data;
  },

  atualizarPerfil: async (
    dados: UsuarioPerfilDto
  ): Promise<void> => {
    await api.put('/UsuarioApi/editar', dados);
  },

  uploadFoto: async (
    id: number,
    arquivo: File
  ): Promise<string> => {

    const formData = new FormData();

    formData.append('foto', arquivo);

    console.log('Arquivo enviado:', {
      nome: arquivo.name,
      tipo: arquivo.type,
      tamanho: arquivo.size
    });

    const response = await api.post<{ fotoUrl: string }>(
      `/UsuarioApi/upload-foto/${id}`,
      formData
    );

    return response.data.fotoUrl;
  },

  buscarPerguntaRecuperacao: async (
    email: string
  ): Promise<string> => {

    const response = await api.get<{ pergunta: string }>(
      '/UsuarioApi/pergunta',
      {
        params: {
          email
        }
      }
    );

    return response.data.pergunta;
  },

  verificarRespostaRecuperacao: async (
    email: string,
    resposta: string
  ): Promise<void> => {

    await api.post(
      '/UsuarioApi/verificar-resposta',
      {
        email,
        resposta
      }
    );
  },

  redefinirSenha: async (
    email: string,
    novaSenha: string
  ): Promise<void> => {

    await api.post(
      '/UsuarioApi/redefinir',
      {
        email,
        novaSenha
      }
    );
  },

  removerFoto: async (
    id: number
  ): Promise<void> => {

    await api.delete(
      `/UsuarioApi/remover-foto/${id}`
    );
  }
};