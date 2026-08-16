// src/services/api.ts
import axios from 'axios';

export const api = axios.create({
  baseURL: 'https://rotalivre-web.onrender.com/api',
});

// Interceptor para injetar o token JWT
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('rotalivre_token');

  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }

  // IMPORTANTE:
  // Quando for FormData, não definimos Content-Type manualmente.
  // O navegador precisa criar o multipart/form-data com o boundary.
  if (config.data instanceof FormData) {
    delete config.headers['Content-Type'];
  } else {
    config.headers['Content-Type'] = 'application/json';
  }

  return config;
});