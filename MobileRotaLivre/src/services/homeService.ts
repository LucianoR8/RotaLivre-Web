import { api } from './api';
import { HomeDto } from '../types';

export const homeService = {
  carregarHome: async (lat?: number, lng?: number): Promise<HomeDto> => {
    let url = '/HomeApi';
    
    // Se tivermos as coordenadas, mandamos na query string
    if (lat !== undefined && lng !== undefined) {
      url += `?lat=${lat}&lng=${lng}`;
    }

    const response = await api.get<HomeDto>(url);

    return response.data;
  },
};