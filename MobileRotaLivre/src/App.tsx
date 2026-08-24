import React from 'react';

import {
  BrowserRouter,
  Routes,
  Route,
  Navigate
} from 'react-router-dom';

import { AuthProvider, useAuth } from './context/AuthContext';

import { Header } from './components/Header';
import { Footer } from './components/Footer';


// =========================================================
// PAGES
// =========================================================

import { HomePage } from './pages/HomePage';
import { CategoriaPage } from './pages/CategoriaPage';
import { PasseioDetalhesPage } from './pages/PasseioDetalhesPage';
import { PerfilPage } from './pages/PerfilPage';
import { EditarPerfilPage } from './pages/EditarPerfilPage';

import { LoginPage } from './pages/LoginPage';
import { CadastroPage } from './pages/CadastroPage';
import { RecuperarSenhaPage } from './pages/RecuperarSenhaPage';

import { GrupoPage } from './pages/GrupoPage';
import { MapaGrupoPage } from './pages/MapaGrupoPage';

import { CadastrarPasseioPage } from './pages/CadastrarPasseioPage';
import { CriarGrupoPasseioPage } from './pages/CriarGrupoPasseioPage';


// =========================================================
// ROTA PROTEGIDA
// =========================================================

const RotaProtegida: React.FC<{
  children: React.ReactNode;
}> = ({ children }) => {

  const { isLoggedIn } =
    useAuth();


  // -----------------------------------------------
  // Usuário não está logado
  // -----------------------------------------------

  if (!isLoggedIn) {

    return (
      <Navigate
        to="/login"
        replace
      />
    );

  }


  return <>{children}</>;

};


// =========================================================
// ROTAS PÚBLICAS
// =========================================================

const RotaPublica: React.FC<{
  children: React.ReactNode;
}> = ({ children }) => {

  const { isLoggedIn } =
    useAuth();


  // -----------------------------------------------
  // Se já estiver logado, não precisa acessar
  // login/cadastro novamente
  // -----------------------------------------------

  if (isLoggedIn) {

    return (
      <Navigate
        to="/"
        replace
      />
    );

  }


  return <>{children}</>;

};


// =========================================================
// APLICAÇÃO
// =========================================================

export const App: React.FC = () => {

  return (

    <AuthProvider>

      <BrowserRouter>

        {/* =================================================
            FUNDO ANIMADO
        ================================================== */}

        <div className="bg-bubbles">

          <div className="bubble-1" />

          <div className="bubble-2" />

          <div className="bubble-3" />

        </div>


        {/* =================================================
            CONTAINER PRINCIPAL
        ================================================== */}

        <div
          className="
            max-w-md
            mx-auto
            min-h-screen
            bg-[#f5f7fa]
            relative
            sm:my-3
            sm:rounded-[36px]
            shadow-2xl
            overflow-hidden
            border
            border-slate-200/60
            flex
            flex-col
          "
        >

          {/* =================================================
              HEADER
          ================================================== */}

          <Header />


          {/* =================================================
              CONTEÚDO
          ================================================== */}

          <main
            className="
              flex-1
              w-full
              min-h-screen
            "
          >

            <Routes>


              {/* =================================================
                  ROTAS PÚBLICAS
              ================================================== */}


              <Route
                path="/login"
                element={
                  <RotaPublica>
                    <LoginPage />
                  </RotaPublica>
                }
              />


              <Route
                path="/cadastro"
                element={
                  <RotaPublica>
                    <CadastroPage />
                  </RotaPublica>
                }
              />


              <Route
                path="/recuperar-senha"
                element={
                  <RotaPublica>
                    <RecuperarSenhaPage />
                  </RotaPublica>
                }
              />


              {/* =================================================
                  ROTAS PROTEGIDAS
              ================================================== */}


              <Route
                path="/"
                element={
                  <RotaProtegida>
                    <HomePage />
                  </RotaProtegida>
                }
              />


              <Route
                path="/categoria/:id"
                element={
                  <RotaProtegida>
                    <CategoriaPage />
                  </RotaProtegida>
                }
              />


              <Route
                path="/passeio/:id"
                element={
                  <RotaProtegida>
                    <PasseioDetalhesPage />
                  </RotaProtegida>
                }
              />


              <Route
                path="/perfil"
                element={
                  <RotaProtegida>
                    <PerfilPage />
                  </RotaProtegida>
                }
              />


              <Route
                path="/perfil/editar"
                element={
                  <RotaProtegida>
                    <EditarPerfilPage />
                  </RotaProtegida>
                }
              />


              <Route
                path="/grupos"
                element={
                  <RotaProtegida>
                    <GrupoPage />
                  </RotaProtegida>
                }
              />


              <Route
                path="/grupo/:id"
                element={
                  <RotaProtegida>
                    <MapaGrupoPage />
                  </RotaProtegida>
                }
              />


              <Route
                path="/ao-vivo"
                element={
                  <RotaProtegida>
                    <MapaGrupoPage />
                  </RotaProtegida>
                }
              />


              <Route
                path="/criar-grupo-passeio/:id"
                element={
                  <RotaProtegida>
                    <CriarGrupoPasseioPage />
                  </RotaProtegida>
                }
              />


              <Route
                path="/cadastrar-passeio"
                element={
                  <RotaProtegida>
                    <CadastrarPasseioPage />
                  </RotaProtegida>
                }
              />


              {/* =================================================
                  QUALQUER ROTA DESCONHECIDA
              ================================================== */}

              <Route
                path="*"
                element={
                  <Navigate
                    to="/"
                    replace
                  />
                }
              />

            </Routes>

          </main>


          {/* =================================================
              FOOTER
          ================================================== */}

          <Footer />

        </div>

      </BrowserRouter>

    </AuthProvider>

  );

};