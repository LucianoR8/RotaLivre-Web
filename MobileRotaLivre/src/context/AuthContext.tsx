import React, { createContext, useContext, useState } from 'react';
import { Usuario } from '../types';

interface AuthContextType {
  usuario: Usuario | null;
  token: string | null;

  setUsuario: (u: Usuario | null) => void;

  login: (userData: Usuario, token: string) => void;

  logout: () => void;

  isLoggedIn: boolean;

  getAuthHeader: () => Record<string, string>;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({
  children
}) => {

  const [usuario, setUsuario] = useState<Usuario | null>(() => {
    const saved = localStorage.getItem('rotalivre_user');

    if (saved) {
      try {
        return JSON.parse(saved);
      } catch {
        return null;
      }
    }

    return null;
  });

  const [token, setToken] = useState<string | null>(() => {
    return localStorage.getItem('rotalivre_token');
  });

  const login = (userData: Usuario, tokenRecebido: string) => {

    setUsuario(userData);
    setToken(tokenRecebido);

    localStorage.setItem(
      'rotalivre_user',
      JSON.stringify(userData)
    );

    localStorage.setItem(
      'rotalivre_token',
      tokenRecebido
    );
  };

  const logout = () => {

    setUsuario(null);
    setToken(null);

    localStorage.removeItem('rotalivre_user');
    localStorage.removeItem('rotalivre_token');
  };

  const getAuthHeader = (): Record<string, string> => {

    if (!token) {
      return {};
    }

    return {
      Authorization: `Bearer ${token}`
    };
  };

  return (
    <AuthContext.Provider
      value={{
        usuario,
        token,
        setUsuario,
        login,
        logout,
        isLoggedIn: !!usuario && !!token,
        getAuthHeader
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {

  const context = useContext(AuthContext);

  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }

  return context;
};