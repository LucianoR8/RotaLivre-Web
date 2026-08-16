import { api } from './api';
import { PasseioDto } from '../types';

export const passeioService = {

  listarTodos: async (): Promise<PasseioDto[]> => {
    const response = await api.get<PasseioDto[]>(
      '/PasseiosApi'
    );

    return response.data;
  },

  buscarPorId: async (
    id: number
  ): Promise<PasseioDto> => {
    const response = await api.get<PasseioDto>(
      `/PasseiosApi/${id}`
    );

    return response.data;
  },

  buscarPorNome: async (
    termo: string
  ): Promise<PasseioDto[]> => {
    const response = await api.get<PasseioDto[]>(
      `/PasseiosApi/buscar?termo=${encodeURIComponent(termo)}`
    );

    return response.data;
  },

  /**
   * Busca somente os passeios pertencentes a uma categoria.
   */
  buscarPorCategoria: async (
    categoriaId: number
  ): Promise<PasseioDto[]> => {
    console.log(
      '[passeioService] Buscando passeios da categoria:',
      categoriaId
    );

    const response = await api.get<PasseioDto[]>(
      `/PasseiosApi/categoria/${categoriaId}`
    );

    console.log(
      '[passeioService] Passeios recebidos:',
      response.data
    );

    return response.data;
  },

  /**
   * Alterna a curtida do usuário no passeio.
   */
  alternarCurtida: async (
    id: number
  ): Promise<{
    curtiu: boolean;
    totalCurtidas: number;
  }> => {
    console.log(
      '[passeioService] Alternando curtida do passeio:',
      id
    );

    const response = await api.post(
      `/PasseiosApi/${id}/curtir`
    );

    console.log(
      '[passeioService] Resultado da curtida:',
      response.data
    );

    return response.data;
  },

  /**
   * Mantido caso outras telas ainda utilizem esse nome.
   */
  curtirPasseio: async (
    id: number
  ): Promise<{
    curtiu: boolean;
    totalCurtidas: number;
  }> => {
    const response = await api.post(
      `/PasseiosApi/${id}/curtir`
    );

    return response.data;
  }
};