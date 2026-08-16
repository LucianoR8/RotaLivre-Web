import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { Mail, Shield, Lock, CheckCircle, ArrowRight } from 'lucide-react';

export const RecuperarSenhaPage: React.FC = () => {
  const navigate = useNavigate();

  const [step, setStep] = useState<1 | 2 | 3 | 4>(1);
  const [email, setEmail] = useState('');
  const [pergunta, setPergunta] = useState('');
  const [resposta, setResposta] = useState('');
  const [novaSenha, setNovaSenha] = useState('');
  const [loading, setLoading] = useState(false);
  const [errorMsg, setErrorMsg] = useState('');

  // Step 1: Request Password Reset
  const handleSolicitar = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setErrorMsg('');

    try {
      const res = await fetch('/api/auth/solicitar-redefinicao', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email })
      });
      const data = await res.json();
      if (data.sucesso) {
        setPergunta(data.pergunta);
        setStep(2);
      } else {
        setErrorMsg(data.mensagem || 'E-mail não encontrado.');
      }
    } catch (err) {
      setErrorMsg('Erro de comunicação com o servidor.');
    } finally {
      setLoading(false);
    }
  };

  // Step 2: Verify Security Answer
  const handleVerificarResposta = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setErrorMsg('');

    try {
      const res = await fetch('/api/auth/verificar-resposta', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email, resposta_seg: resposta })
      });
      const data = await res.json();
      if (data.sucesso) {
        setStep(3);
      } else {
        setErrorMsg(data.mensagem || 'Resposta incorreta.');
      }
    } catch (err) {
      setErrorMsg('Erro de comunicação com o servidor.');
    } finally {
      setLoading(false);
    }
  };

  // Step 3: Set New Password
  const handleRedefinirSenha = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setErrorMsg('');

    try {
      const res = await fetch('/api/auth/redefinir-senha', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email, novaSenha })
      });
      const data = await res.json();
      if (data.sucesso) {
        setStep(4);
      } else {
        setErrorMsg(data.mensagem || 'Falha ao alterar senha.');
      }
    } catch (err) {
      setErrorMsg('Erro de comunicação com o servidor.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="pt-20 pb-24 px-4 flex items-center justify-center max-w-md mx-auto">
      <div className="w-full max-w-md bg-white rounded-3xl p-8 shadow-2xl border border-slate-100">
        {step === 1 && (
          <div>
            <div className="text-center mb-6">
              <div className="inline-flex p-3 bg-[#4ecdc4]/15 rounded-2xl text-[#1a535c] mb-3">
                <Mail className="w-8 h-8 text-[#ff6b6b]" />
              </div>
              <h1 className="text-2xl font-extrabold text-[#1a535c]">Recuperar Senha</h1>
              <p className="text-xs text-slate-500 mt-1">Informe seu e-mail para iniciar o processo</p>
            </div>

            {errorMsg && (
              <div className="mb-4 p-3 bg-rose-100 text-rose-800 rounded-xl text-xs font-semibold">
                {errorMsg}
              </div>
            )}

            <form onSubmit={handleSolicitar} className="space-y-4">
              <div>
                <label className="block text-xs font-bold text-slate-500 uppercase mb-1">E-mail Cadastrado</label>
                <div className="relative">
                  <Mail className="w-5 h-5 text-slate-400 absolute left-4 top-3.5" />
                  <input
                    type="email"
                    value={email}
                    onChange={e => setEmail(e.target.value)}
                    required
                    placeholder="seu.email@exemplo.com"
                    className="w-full pl-12 pr-4 py-3 bg-[#f5f7fa] rounded-2xl border border-slate-200 text-sm focus:outline-none focus:border-[#4ecdc4]"
                  />
                </div>
              </div>

              <button
                type="submit"
                disabled={loading}
                className="w-full bg-[#1a535c] hover:bg-[#1a535c]/90 text-white font-bold py-3.5 rounded-2xl text-sm transition shadow-lg flex items-center justify-center gap-2 mt-6"
              >
                <span>{loading ? 'Buscando...' : 'Avançar'}</span>
                <ArrowRight className="w-4 h-4" />
              </button>
            </form>
          </div>
        )}

        {step === 2 && (
          <div>
            <div className="text-center mb-6">
              <div className="inline-flex p-3 bg-[#4ecdc4]/15 rounded-2xl text-[#1a535c] mb-3">
                <Shield className="w-8 h-8 text-[#4ecdc4]" />
              </div>
              <h1 className="text-2xl font-extrabold text-[#1a535c]">Pergunta de Segurança</h1>
              <p className="text-xs text-slate-500 mt-2 bg-[#f5f7fa] p-3 rounded-xl border border-slate-200 font-semibold text-[#1a535c]">
                "{pergunta}"
              </p>
            </div>

            {errorMsg && (
              <div className="mb-4 p-3 bg-rose-100 text-rose-800 rounded-xl text-xs font-semibold">
                {errorMsg}
              </div>
            )}

            <form onSubmit={handleVerificarResposta} className="space-y-4">
              <div>
                <label className="block text-xs font-bold text-slate-500 uppercase mb-1">Sua Resposta</label>
                <input
                  type="text"
                  value={resposta}
                  onChange={e => setResposta(e.target.value)}
                  required
                  placeholder="Digite sua resposta"
                  className="w-full p-3 bg-[#f5f7fa] rounded-2xl border border-slate-200 text-sm focus:outline-none focus:border-[#4ecdc4]"
                />
              </div>

              <button
                type="submit"
                disabled={loading}
                className="w-full bg-[#1a535c] hover:bg-[#1a535c]/90 text-white font-bold py-3.5 rounded-2xl text-sm transition shadow-lg flex items-center justify-center gap-2 mt-6"
              >
                <span>{loading ? 'Verificando...' : 'Confirmar Resposta'}</span>
                <ArrowRight className="w-4 h-4" />
              </button>
            </form>
          </div>
        )}

        {step === 3 && (
          <div>
            <div className="text-center mb-6">
              <div className="inline-flex p-3 bg-[#4ecdc4]/15 rounded-2xl text-[#1a535c] mb-3">
                <Lock className="w-8 h-8 text-[#ff6b6b]" />
              </div>
              <h1 className="text-2xl font-extrabold text-[#1a535c]">Nova Senha</h1>
              <p className="text-xs text-slate-500 mt-1">Digite sua nova senha de acesso</p>
            </div>

            {errorMsg && (
              <div className="mb-4 p-3 bg-rose-100 text-rose-800 rounded-xl text-xs font-semibold">
                {errorMsg}
              </div>
            )}

            <form onSubmit={handleRedefinirSenha} className="space-y-4">
              <div>
                <label className="block text-xs font-bold text-slate-500 uppercase mb-1">Nova Senha</label>
                <input
                  type="password"
                  value={novaSenha}
                  onChange={e => setNovaSenha(e.target.value)}
                  required
                  placeholder="••••••••"
                  className="w-full p-3 bg-[#f5f7fa] rounded-2xl border border-slate-200 text-sm focus:outline-none focus:border-[#4ecdc4]"
                />
              </div>

              <button
                type="submit"
                disabled={loading}
                className="w-full bg-[#1a535c] hover:bg-[#1a535c]/90 text-white font-bold py-3.5 rounded-2xl text-sm transition shadow-lg flex items-center justify-center gap-2 mt-6"
              >
                <span>{loading ? 'Salvando...' : 'Alterar Senha'}</span>
              </button>
            </form>
          </div>
        )}

        {step === 4 && (
          <div className="text-center py-4">
            <div className="inline-flex p-4 bg-emerald-100 rounded-full text-emerald-600 mb-4">
              <CheckCircle className="w-12 h-12" />
            </div>
            <h2 className="text-2xl font-extrabold text-[#1a535c] mb-2">Senha Alterada!</h2>
            <p className="text-xs text-slate-500 mb-6">Sua senha foi atualizada com sucesso. Você já pode fazer login.</p>
            <Link
              to="/login"
              className="inline-block bg-[#1a535c] text-white font-bold py-3 px-8 rounded-full text-sm shadow-md transition hover:bg-[#1a535c]/90"
            >
              Ir para o Login
            </Link>
          </div>
        )}

        {step < 4 && (
          <div className="mt-6 text-center text-xs text-slate-500 pt-4 border-t border-slate-100">
            Lembrou da senha?{' '}
            <Link to="/login" className="text-[#1a535c] font-bold hover:underline">
              Voltar ao Login
            </Link>
          </div>
        )}
      </div>
    </div>
  );
};
