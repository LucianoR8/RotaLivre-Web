// src/pages/LoginPage.tsx
import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { Mail, Lock, LogIn, Sparkles } from 'lucide-react';
import { authService } from '../services/authService'; // <-- Importe o serviço que criamos

export const LoginPage: React.FC = () => {
  const { login } = useAuth();
  const navigate = useNavigate();

  const [email, setEmail] = useState('');
  const [senha, setSenha] = useState('');
  const [loading, setLoading] = useState(false);
  const [errorMsg, setErrorMsg] = useState('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setErrorMsg('');

    try {
      // O authService faz o POST para /api/AuthApi/login e já salva o token no localStorage
      const data = await authService.login({ email, senha });

      if (data && data.token) {
        login(data.usuario, data.token); // Atualiza o estado global da sua aplicação (Context)
        navigate('/'); // Envia para a HomeBase
      }
    } catch (err: any) {
      console.error('Erro ao efetuar login:', err);
      
      // O seu C# retorna 401 (Unauthorized) quando a senha/email não bate
      if (err.response && err.response.status === 401) {
        setErrorMsg(err.response.data.mensagem || 'E-mail ou senha inválidos.');
      } else {
        setErrorMsg('Erro de conexão ao servidor. Tente novamente mais tarde.');
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="pt-20 pb-24 px-4 flex items-center justify-center max-w-md mx-auto">
      <div className="w-full max-w-md bg-white rounded-3xl p-8 shadow-2xl border border-slate-100">
        <div className="text-center mb-8">
          <div className="inline-flex p-3 bg-[#4ecdc4]/15 rounded-2xl text-[#1a535c] mb-3">
            <Sparkles className="w-8 h-8 text-[#ff6b6b]" />
          </div>
          <h1 className="text-2xl font-extrabold text-[#1a535c]">Bem-vindo de volta!</h1>
          <p className="text-xs text-slate-500 mt-1">Acesse sua conta do Rota Livre</p>
        </div>

        {errorMsg && (
          <div className="mb-6 p-4 bg-rose-100 text-rose-800 rounded-2xl text-xs font-semibold">
            {errorMsg}
          </div>
        )}

        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-xs font-bold text-slate-500 uppercase mb-1">E-mail</label>
            <div className="relative">
              <Mail className="w-5 h-5 text-slate-400 absolute left-4 top-3.5" />
              <input
                type="email"
                value={email}
                onChange={e => setEmail(e.target.value)}
                placeholder="seu.email@exemplo.com"
                required
                className="w-full pl-12 pr-4 py-3 bg-[#f5f7fa] rounded-2xl border border-slate-200 text-sm focus:outline-none focus:border-[#4ecdc4]"
              />
            </div>
          </div>

          <div>
            <div className="flex items-center justify-between mb-1">
              <label className="block text-xs font-bold text-slate-500 uppercase">Senha</label>
              <Link to="/recuperar-senha" className="text-xs text-[#4ecdc4] hover:underline font-semibold">
                Esqueceu a senha?
              </Link>
            </div>
            <div className="relative">
              <Lock className="w-5 h-5 text-slate-400 absolute left-4 top-3.5" />
              <input
                type="password"
                value={senha}
                onChange={e => setSenha(e.target.value)}
                placeholder="••••••••"
                required
                className="w-full pl-12 pr-4 py-3 bg-[#f5f7fa] rounded-2xl border border-slate-200 text-sm focus:outline-none focus:border-[#4ecdc4]"
              />
            </div>
          </div>

          <button
            type="submit"
            disabled={loading}
            className="w-full bg-gradient-to-r from-[#1a535c] to-[#236c78] hover:opacity-95 text-white font-bold py-3.5 rounded-2xl text-sm transition shadow-lg flex items-center justify-center gap-2 mt-6"
          >
            <LogIn className="w-5 h-5" />
            <span>{loading ? 'Entrando...' : 'Entrar na Conta'}</span>
          </button>
        </form>

        <div className="mt-8 text-center text-xs text-slate-500 pt-6 border-t border-slate-100">
          Não tem uma conta ainda?{' '}
          <Link to="/cadastro" className="text-[#1a535c] font-bold hover:underline">
            Cadastre-se gratuitamente
          </Link>
        </div>
      </div>
    </div>
  );
};