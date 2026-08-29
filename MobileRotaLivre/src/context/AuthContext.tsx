import React, {
  createContext,
  useContext,
  useState
} from 'react';

import { Usuario } from '../types';

interface AuthContextType {
  usuario: Usuario | null;
  token: string | null;

  setUsuario: (u: Usuario | null) => void;

  login: (
    userData: Usuario,
    token: string
  ) => void;

  logout: () => void;

  isLoggedIn: boolean;

  getAuthHeader: () => Record<string, string>;
}

const AuthContext =
  createContext<AuthContextType | undefined>(
    undefined
  );


// =========================================================
// CONFIGURAÇÃO DA SESSÃO
// =========================================================

// 7 dias em milissegundos
const TEMPO_SESSAO =
  7 * 24 * 60 * 60 * 1000;


// =========================================================
// AUTH PROVIDER
// =========================================================

export const AuthProvider: React.FC<{
  children: React.ReactNode;
}> = ({ children }) => {

  // =======================================================
  // USUÁRIO
  // =======================================================

  const [usuario, setUsuario] =
    useState<Usuario | null>(() => {

      const savedUser =
        localStorage.getItem(
          'rotalivre_user'
        );

      const savedToken =
        localStorage.getItem(
          'rotalivre_token'
        );

      const savedExpiration =
        localStorage.getItem(
          'rotalivre_login_expira'
        );


      // -----------------------------------------------
      // Não existe usuário ou token
      // -----------------------------------------------

      if (
        !savedUser ||
        !savedToken
      ) {
        return null;
      }


      // -----------------------------------------------
      // Verifica expiração
      // -----------------------------------------------

      if (savedExpiration) {

        const expiration =
          Number(savedExpiration);

        if (
          Number.isNaN(expiration) ||
          Date.now() >= expiration
        ) {

          console.log(
            '[Auth] Sessão expirada.'
          );

          localStorage.removeItem(
            'rotalivre_user'
          );

          localStorage.removeItem(
            'rotalivre_token'
          );

          localStorage.removeItem(
            'rotalivre_login_expira'
          );

          return null;
        }
      }


      // -----------------------------------------------
      // Recupera usuário
      // -----------------------------------------------

      try {

        return JSON.parse(
          savedUser
        );

      } catch {

        localStorage.removeItem(
          'rotalivre_user'
        );

        localStorage.removeItem(
          'rotalivre_token'
        );

        localStorage.removeItem(
          'rotalivre_login_expira'
        );

        return null;
      }

    });


  // =======================================================
  // TOKEN
  // =======================================================

  const [token, setToken] =
    useState<string | null>(() => {

      const savedToken =
        localStorage.getItem(
          'rotalivre_token'
        );

      const savedExpiration =
        localStorage.getItem(
          'rotalivre_login_expira'
        );


      // -----------------------------------------------
      // Não existe token
      // -----------------------------------------------

      if (!savedToken) {
        return null;
      }


      // -----------------------------------------------
      // Verifica expiração
      // -----------------------------------------------

      if (savedExpiration) {

        const expiration =
          Number(savedExpiration);

        if (
          Number.isNaN(expiration) ||
          Date.now() >= expiration
        ) {

          localStorage.removeItem(
            'rotalivre_user'
          );

          localStorage.removeItem(
            'rotalivre_token'
          );

          localStorage.removeItem(
            'rotalivre_login_expira'
          );

          return null;
        }
      }


      return savedToken;

    });


  // =======================================================
  // LOGIN
  // =======================================================

  const login = (
    userData: Usuario,
    tokenRecebido: string
  ) => {

    console.log(
      '[Auth] Realizando login.'
    );


    // -----------------------------------------------
    // Calcula expiração
    // -----------------------------------------------

    const dataExpiracao =
      Date.now() + TEMPO_SESSAO;


    // -----------------------------------------------
    // Estado React
    // -----------------------------------------------

    setUsuario(
      userData
    );

    setToken(
      tokenRecebido
    );


    // -----------------------------------------------
    // Persistência local
    // -----------------------------------------------

    localStorage.setItem(
      'rotalivre_user',
      JSON.stringify(userData)
    );

    localStorage.setItem(
      'rotalivre_token',
      tokenRecebido
    );

    localStorage.setItem(
      'rotalivre_login_expira',
      String(dataExpiracao)
    );


    console.log(
      '[Auth] Sessão válida até:',
      new Date(
        dataExpiracao
      ).toLocaleString(
        'pt-BR'
      )
    );

  };


  // =======================================================
  // LOGOUT
  // =======================================================

  const logout = () => {

    console.log(
      '[Auth] Realizando logout.'
    );


    // -----------------------------------------------
    // Estado React
    // -----------------------------------------------

    setUsuario(null);

    setToken(null);


    // -----------------------------------------------
    // Limpa persistência
    // -----------------------------------------------

    localStorage.removeItem(
      'rotalivre_user'
    );

    localStorage.removeItem(
      'rotalivre_token'
    );

    localStorage.removeItem(
      'rotalivre_login_expira'
    );

  };


  // =======================================================
  // AUTH HEADER
  // =======================================================

  const getAuthHeader =
    (): Record<string, string> => {

      if (!token) {
        return {};
      }

      return {
        Authorization:
          `Bearer ${token}`
      };

    };


  // =======================================================
  // ESTADO DE LOGIN
  // =======================================================

  const isLoggedIn =
    !!usuario &&
    !!token;


  // =======================================================
  // PROVIDER
  // =======================================================

  return (

    <AuthContext.Provider
      value={{
        usuario,
        token,

        setUsuario,

        login,

        logout,

        isLoggedIn,

        getAuthHeader
      }}
    >

      {children}

    </AuthContext.Provider>

  );

};


// =========================================================
// HOOK
// =========================================================

export const useAuth = () => {

  const context =
    useContext(
      AuthContext
    );

  if (!context) {

    throw new Error(
      'useAuth must be used within an AuthProvider'
    );

  }

  return context;

};