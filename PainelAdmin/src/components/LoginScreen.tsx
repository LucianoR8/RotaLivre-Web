import { useState, FormEvent } from 'react';
import { Compass, Lock, Mail, Eye, EyeOff, ArrowRight, ShieldCheck } from 'lucide-react';
import { AdminUser } from '../types';
import { api } from '../services/api'; // Certifique-se de que este arquivo foi criado!

interface LoginScreenProps {
  onLoginSuccess: (user: AdminUser) => void;
}

export function LoginScreen({ onLoginSuccess }: LoginScreenProps) {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [showPassword, setShowPassword] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [errorMessage, setErrorMessage] = useState('');

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setErrorMessage('');

    if (!email.trim() || !password.trim()) {
      setErrorMessage('Por favor, informe seu e-mail e senha.');
      return;
    }

    setIsLoading(true);

    try {
      // Faz o POST usando a configuração centralizada do Axios
      const response = await api.post('/AuthApi/login', {
        email: email.trim(),
        senha: password,
      });

      const data = response.data;
      
      // LOG PARA DEBUG: Veja no console do navegador (F12) o que a API devolveu
      console.log("RESPOSTA DA API:", data); 

      if (!data.token) {
        setErrorMessage('Falha ao obter token de acesso.');
        setIsLoading(false);
        return;
      }

      // Validação de Permissão (Abrange todas as variações do C# e Supabase)
      const isUserAdmin = data.usuario?.isAdmin ?? data.usuario?.IsAdmin ?? data.usuario?.is_admin ?? false;
      
      if (!isUserAdmin) {
        setErrorMessage('Acesso Negado: Este usuário não possui privilégios de administrador.');
        setIsLoading(false);
        return;
      }

      // Monta os dados do Admin autenticado garantindo as propriedades
      const adminData: AdminUser = {
        id: data.usuario?.id || data.usuario?.Id || data.usuario?.id_usuario || 1,
        name: data.usuario?.nome || data.usuario?.Nome || data.usuario?.nome_completo || 'Administrador',
        email: data.usuario?.email || data.usuario?.Email || email,
        role: 'Administrador Master',
        avatarUrl: `https://ui-avatars.com/api/?name=${encodeURIComponent(
          data.usuario?.nome || data.usuario?.Nome || data.usuario?.nome_completo || 'Admin'
        )}&background=1a535c&color=fff`,
      };

      // Salva no LocalStorage para persistir sessão
      localStorage.setItem('admin_token', data.token);
      localStorage.setItem('admin_user', JSON.stringify(adminData));

      // Notifica o App.tsx para trocar de tela
      onLoginSuccess(adminData);

    } catch (error: any) {
      console.error('Erro na autenticação:', error);
      // Pega a mensagem de erro que vem da sua API C# (ex: 401 Unauthorized)
      const apiMsg = error.response?.data?.mensagem;
      setErrorMessage(apiMsg || 'E-mail ou senha inválidos. Verifique suas credenciais.');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div
      id="login-screen-container"
      className="min-h-screen w-full bg-slate-50 flex items-center justify-center p-4 sm:p-6"
    >
      {/* Elementos visuais de fundo */}
      <div className="absolute inset-0 overflow-hidden pointer-events-none opacity-40">
        <div className="absolute top-[-10%] left-[-10%] w-[45vw] h-[45vw] rounded-full bg-[#4ecdc4]/15 blur-3xl" />
        <div className="absolute bottom-[-10%] right-[-10%] w-[45vw] h-[45vw] rounded-full bg-[#1a535c]/10 blur-3xl" />
      </div>

      <div className="relative w-full max-w-md bg-white rounded-2xl shadow-xl shadow-slate-200/60 border border-slate-200/80 p-8 z-10 animate-in fade-in zoom-in-95 duration-300">
        {/* Cabeçalho */}
        <div className="text-center mb-8">
          <div className="inline-flex items-center justify-center w-16 h-16 rounded-2xl bg-[#1a535c] text-[#4ecdc4] shadow-lg shadow-[#1a535c]/20 mb-4">
            <Compass className="w-9 h-9" />
          </div>
          <h1 className="text-2xl font-extrabold text-[#1a535c] tracking-tight">
            Rota Livre
          </h1>
          <p className="text-sm text-slate-500 font-medium mt-1">
            Painel Administrativo do Sistema
          </p>
        </div>

        {/* Mensagem de Erro */}
        {errorMessage && (
          <div className="mb-5 p-3.5 rounded-xl bg-[#ff6b6b]/10 border border-[#ff6b6b]/20 text-xs font-semibold text-[#ff6b6b] flex items-center gap-2">
            <span>{errorMessage}</span>
          </div>
        )}

        {/* Formulário */}
        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label
              htmlFor="input-email"
              className="block text-xs font-bold text-slate-700 uppercase tracking-wider mb-1.5"
            >
              E-mail Administrativo
            </label>
            <div className="relative">
              <div className="absolute inset-y-0 left-0 pl-3.5 flex items-center pointer-events-none text-slate-400">
                <Mail className="w-4 h-4" />
              </div>
              <input
                id="input-email"
                type="email"
                required
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                placeholder="seu.email@rotalivre.com"
                className="w-full pl-10 pr-4 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-sm text-slate-800 placeholder-slate-400 focus:outline-none focus:ring-2 focus:ring-[#4ecdc4] focus:border-[#1a535c] transition-all"
              />
            </div>
          </div>

          <div>
            <div className="flex items-center justify-between mb-1.5">
              <label
                htmlFor="input-password"
                className="block text-xs font-bold text-slate-700 uppercase tracking-wider"
              >
                Senha de Acesso
              </label>
            </div>
            <div className="relative">
              <div className="absolute inset-y-0 left-0 pl-3.5 flex items-center pointer-events-none text-slate-400">
                <Lock className="w-4 h-4" />
              </div>
              <input
                id="input-password"
                type={showPassword ? 'text' : 'password'}
                required
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                placeholder="••••••••"
                className="w-full pl-10 pr-10 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-sm text-slate-800 placeholder-slate-400 focus:outline-none focus:ring-2 focus:ring-[#4ecdc4] focus:border-[#1a535c] transition-all"
              />
              <button
                type="button"
                onClick={() => setShowPassword(!showPassword)}
                className="absolute inset-y-0 right-0 pr-3.5 flex items-center text-slate-400 hover:text-slate-600"
                aria-label="Alternar visibilidade de senha"
              >
                {showPassword ? <EyeOff className="w-4 h-4" /> : <Eye className="w-4 h-4" />}
              </button>
            </div>
          </div>

          <button
            type="submit"
            id="btn-submit-login"
            disabled={isLoading}
            className="w-full mt-2 py-3 px-4 bg-[#1a535c] hover:bg-[#154249] text-white font-bold text-sm rounded-xl shadow-lg shadow-[#1a535c]/20 hover:shadow-xl hover:shadow-[#1a535c]/30 flex items-center justify-center gap-2 transition-all active:scale-98 disabled:opacity-70 cursor-pointer"
          >
            {isLoading ? (
              <div className="w-5 h-5 border-2 border-white/30 border-t-white rounded-full animate-spin" />
            ) : (
              <>
                <span>Entrar no Painel</span>
                <ArrowRight className="w-4 h-4 text-[#4ecdc4]" />
              </>
            )}
          </button>
        </form>

        <div className="mt-6 pt-5 border-t border-slate-100 text-center">
          <div className="flex items-center justify-center gap-1 text-xs text-slate-500">
            <ShieldCheck className="w-4 h-4 text-[#4ecdc4]" />
            <span>Acesso Restrito com Auditoria de Operações</span>
          </div>
        </div>
      </div>
    </div>
  );
}