import React from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './context/AuthContext';
import { Header } from './components/Header';
import { Footer } from './components/Footer';

// Pages
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

export const App: React.FC = () => {
  return (
    <AuthProvider>
      <BrowserRouter>
        {/* Animated Background Canvas */}
        <div className="bg-bubbles">
          <div className="bubble-1" />
          <div className="bubble-2" />
          <div className="bubble-3" />
        </div>

        <div className="max-w-md mx-auto min-h-screen bg-[#f5f7fa] relative sm:my-3 sm:rounded-[36px] shadow-2xl overflow-hidden border border-slate-200/60 flex flex-col">
          <Header />

          <main className="flex-1 w-full min-h-screen">
            <Routes>
              <Route path="/" element={<HomePage />} />
              <Route path="/categoria/:id" element={<CategoriaPage />} />
              <Route path="/passeio/:id" element={<PasseioDetalhesPage />} />
              <Route path="/perfil" element={<PerfilPage />} />
              <Route path="/perfil/editar" element={<EditarPerfilPage />} />
              <Route path="/login" element={<LoginPage />} />
              <Route path="/cadastro" element={<CadastroPage />} />
              <Route path="/recuperar-senha" element={<RecuperarSenhaPage />} />
              <Route path="/grupos" element={<GrupoPage />} />
              <Route path="/grupo/:id" element={<MapaGrupoPage />} />
              <Route path="/ao-vivo" element={<MapaGrupoPage />} />
              <Route path="/criar-grupo-passeio/:id" element={<CriarGrupoPasseioPage />} />
              <Route path="/cadastrar-passeio" element={<CadastrarPasseioPage />} />
              <Route path="*" element={<Navigate to="/" replace />} />
            </Routes>
          </main>

          <Footer />
        </div>
      </BrowserRouter>
    </AuthProvider>
  );
};
