// src/services/api.ts

import axios from 'axios';

export const api = axios.create({
  baseURL: 'https://rotalivre-web.onrender.com/api',
});

// Interceptor: Injeta o Token JWT em todas as requisições automaticamente
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('admin_token');

    if (token && config.headers) {
      config.headers.Authorization = `Bearer ${token}`;
    }

    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);