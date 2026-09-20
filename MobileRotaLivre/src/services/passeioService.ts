import { api } from './api';
import { PasseioDto } from '../types';

export const passeioService = {

  listarTodos: async (): Promise<PasseioDto[]> => {
    const response = await api.get<PasseioDto[]>('/PasseiosApi');
    return response.data;
  },

  buscarPorId: async (id: number): Promise<PasseioDto> => {
    const response = await api.get<PasseioDto>(`/PasseiosApi/${id}`);
    return response.data;
  },

  buscarPorNome: async (termo: string): Promise<PasseioDto[]> => {
    const response = await api.get<PasseioDto[]>(
      `/PasseiosApi/buscar?termo=${encodeURIComponent(termo)}`
    );
    return response.data;
  },

  buscarPorCategoria: async (categoriaId: number): Promise<PasseioDto[]> => {
    const response = await api.get<PasseioDto[]>(`/PasseiosApi/categoria/${categoriaId}`);
    return response.data;
  },

  // ==========================================
  // BUSCA CRUZADA (CIDADE + TEMA)
  // ==========================================
  buscarPorCidadeECategoria: async (cidadeId: number, categoriaId: number): Promise<PasseioDto[]> => {
    const response = await api.get<PasseioDto[]>(
      `/PasseiosApi/cidade/${cidadeId}/categoria/${categoriaId}`
    );
    return response.data;
  },

  alternarCurtida: async (id: number): Promise<{ curtiu: boolean; totalCurtidas: number; }> => {
    const response = await api.post(`/PasseiosApi/${id}/curtir`);
    return response.data;
  },

  curtirPasseio: async (id: number): Promise<{ curtiu: boolean; totalCurtidas: number; }> => {
    const response = await api.post(`/PasseiosApi/${id}/curtir`);
    return response.data;
  }
};