import { api } from './api';

export interface AvaliacaoDto {
  nomeUsuario: string;
  feedback: string;
  data: string;
}

export interface CriarAvaliacaoDto {
  idPasseio: number;
  idUsuario: number;
  feedback: string;
}

export const avaliacaoService = {

  listarPorPasseio: async (
    idPasseio: number
  ): Promise<AvaliacaoDto[]> => {

    const response = await api.get<AvaliacaoDto[]>(
      `/AvaliacaoApi/${idPasseio}`
    );

    return response.data;
  },

  comentar: async (
    dados: CriarAvaliacaoDto
  ): Promise<void> => {

    await api.post(
      '/AvaliacaoApi/comentar',
      dados
    );
  }

};