import { api } from './api';
import { HomeDto } from '../types';

export const homeService = {
  carregarHome: async (): Promise<HomeDto> => {
    const response = await api.get<HomeDto>('/HomeApi');

    return response.data;
  },
};