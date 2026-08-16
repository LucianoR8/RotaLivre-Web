import React from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { Sparkles, User } from 'lucide-react';

export const Header: React.FC = () => {
  const { usuario, isLoggedIn } = useAuth();

  // Pega o nome vindo de 'nome_completo' ou 'nome', ou usa 'Usuário' como fallback
  const nomeExibicao = usuario?.nome || usuario?.nome || 'Usuário';
  const primeiroNome = nomeExibicao.split(' ')[0];

  return (
    <header className="fixed top-0 left-0 right-0 z-50 bg-white/95 backdrop-blur-md border-b-2 border-[#4ecdc4] shadow-sm transition-all">
      <div className="max-w-md mx-auto px-4 h-16 flex items-center justify-between">
        {/* Brand Logo */}
        <Link to="/" className="flex items-center gap-1.5 group">
          <span className="font-black text-xl bg-gradient-to-r from-[#1a535c] to-[#4ecdc4] bg-clip-text text-transparent tracking-tight">
            Rota Livre
          </span>
          <Sparkles className="w-4 h-4 text-[#ff6b6b] animate-spin-slow inline" />
        </Link>

        {/* Navigation Actions */}
        <div className="flex items-center gap-2">
          {isLoggedIn ? (
            <Link
              to="/perfil"
              className="flex items-center gap-1.5 bg-[#1a535c] hover:bg-[#1a535c]/90 text-white font-bold px-3 py-1.5 rounded-full text-xs transition shadow-sm"
            >
              <User className="w-3.5 h-3.5 text-[#4ecdc4]" />
              <span className="max-w-[100px] truncate">{primeiroNome}</span>
            </Link>
          ) : (
            <div className="flex items-center gap-1.5">
              <Link
                to="/login"
                className="bg-[#1a535c] text-white font-bold px-3.5 py-1.5 rounded-full text-xs transition shadow-sm"
              >
                Entrar
              </Link>
              <Link
                to="/cadastro"
                className="border border-[#1a535c] text-[#1a535c] font-bold px-3 py-1.5 rounded-full text-xs transition"
              >
                Cadastrar
              </Link>
            </div>
          )}
        </div>
      </div>
    </header>
  );
};