// src/pages/CadastroPage.tsx
import React, { useState, useEffect } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { User, Mail, Lock, Shield, UserPlus, Phone, Calendar, CreditCard } from 'lucide-react';
import { usuarioService } from '../services/usuarioService';
import { PerguntaSeguranca } from '../types';

export const CadastroPage: React.FC = () => {
  const navigate = useNavigate();

  // Estados dos inputs
  const [nomeCompleto, setNomeCompleto] = useState('');
  const [email, setEmail] = useState('');
  const [dataNascimento, setDataNascimento] = useState('');
  const [senha, setSenha] = useState('');
  const [idPergunta, setIdPergunta] = useState(1);
  const [respostaSeg, setRespostaSeg] = useState('');

  // Estados de controle da página
  const [perguntas, setPerguntas] = useState<PerguntaSeguranca[]>([]);
  const [loading, setLoading] = useState(false);
  const [errorMsg, setErrorMsg] = useState('');
  const [sucessoMsg, setSucessoMsg] = useState('');

  // Busca as perguntas de segurança pela API via Axios
  useEffect(() => {
    const carregarPerguntas = async () => {
      try {
        const dados = await usuarioService.buscarPerguntas();
        setPerguntas(dados || []);
        if (dados && dados.length > 0) {
          setIdPergunta(dados[0].id_pergunta);
        }
      } catch (err) {
        console.error('Erro ao carregar perguntas:', err);
        setErrorMsg('Erro ao carregar as perguntas de segurança.');
      }
    };
    carregarPerguntas();
  }, []);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setErrorMsg('');
    setSucessoMsg('');

    try {
      // Fazendo a chamada para o C# usando o Serviço que mapeia o UsuarioCadastroDto
      await usuarioService.cadastrar({
        nome: nomeCompleto,
        email: email,
        dataNasc: dataNascimento,
        senha: senha,
        idPergunta: Number(idPergunta),
        respostaSeg: respostaSeg
      });

      setSucessoMsg('Cadastro realizado com sucesso! Redirecionando...');
      
      // Redireciona para o login após 2 segundos
      setTimeout(() => {
        navigate('/login');
      }, 2000);

    } catch (error: any) {
      console.error('Erro ao cadastrar:', error);
      
      // Tratamento das mensagens que a sua API C# devolve
      if (error.response?.data === 'EMAIL_JA_EXISTE') {
        setErrorMsg('Este e-mail já está cadastrado!');
      } else if (error.response?.data === 'ERRO_AO_CADASTRAR') {
        setErrorMsg('Ocorreu um erro ao salvar o usuário. Tente novamente.');
      } else {
        setErrorMsg('Erro ao conectar ao servidor. Tente novamente mais tarde.');
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="pt-20 pb-24 px-4 flex items-center justify-center max-w-md mx-auto">
      <div className="w-full max-w-lg bg-white rounded-3xl p-8 shadow-2xl border border-slate-100">
        <div className="text-center mb-6">
          <h1 className="text-2xl font-extrabold text-[#1a535c]">Criar Nova Conta</h1>
          <p className="text-xs text-slate-500 mt-1">Preencha os dados abaixo para começar a usar o Rota Livre</p>
        </div>

        {/* Mensagem de Erro */}
        {errorMsg && (
          <div className="mb-6 p-4 bg-rose-100 text-rose-800 rounded-2xl text-xs font-semibold">
            {errorMsg}
          </div>
        )}

        {/* Mensagem de Sucesso */}
        {sucessoMsg && (
          <div className="mb-6 p-4 bg-green-100 text-green-800 rounded-2xl text-xs font-semibold">
            {sucessoMsg}
          </div>
        )}

        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-xs font-bold text-slate-500 uppercase mb-1">Nome Completo</label>
            <div className="relative">
              <User className="w-5 h-5 text-slate-400 absolute left-4 top-3.5" />
              <input
                type="text"
                value={nomeCompleto}
                onChange={e => setNomeCompleto(e.target.value)}
                placeholder="Seu nome completo"
                required
                className="w-full pl-12 pr-4 py-3 bg-[#f5f7fa] rounded-2xl border border-slate-200 text-sm focus:outline-none focus:border-[#4ecdc4]"
              />
            </div>
          </div>

          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <div>
              <label className="block text-xs font-bold text-slate-500 uppercase mb-1">E-mail</label>
              <div className="relative">
                <Mail className="w-5 h-5 text-slate-400 absolute left-4 top-3.5" />
                <input
                  type="email"
                  value={email}
                  onChange={e => setEmail(e.target.value)}
                  placeholder="seu@email.com"
                  required
                  className="w-full pl-12 pr-4 py-3 bg-[#f5f7fa] rounded-2xl border border-slate-200 text-sm focus:outline-none focus:border-[#4ecdc4]"
                />
              </div>
            </div>

            <div>
              <label className="block text-xs font-bold text-slate-500 uppercase mb-1">Senha</label>
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
          </div>
          <div>
            <label className="block text-xs font-bold text-slate-500 uppercase mb-1">Data de Nascimento</label>
            <div className="relative">
              <Calendar className="w-5 h-5 text-slate-400 absolute left-4 top-3.5" />
              <input
                type="date"
                value={dataNascimento}
                onChange={e => setDataNascimento(e.target.value)}
                className="w-full pl-12 pr-4 py-3 bg-[#f5f7fa] rounded-2xl border border-slate-200 text-sm focus:outline-none focus:border-[#4ecdc4]"
                required
              />
            </div>
          </div>

          {/* Security Question */}
          <div className="pt-2 border-t border-slate-100">
            <label className="block text-xs font-bold text-slate-500 uppercase mb-1">Pergunta de Segurança</label>
            <select
              value={idPergunta}
              onChange={e => setIdPergunta(Number(e.target.value))}
              className="w-full p-3 bg-[#f5f7fa] rounded-2xl border border-slate-200 text-sm focus:outline-none focus:border-[#4ecdc4] mb-3"
              required
            >
              {perguntas.map(p => (
                <option key={p.id_pergunta} value={p.id_pergunta}>
                  {p.pergunta_seg}
                </option>
              ))}
            </select>

            <label className="block text-xs font-bold text-slate-500 uppercase mb-1">Resposta de Segurança</label>
            <div className="relative">
              <Shield className="w-5 h-5 text-slate-400 absolute left-4 top-3.5" />
              <input
                type="text"
                value={respostaSeg}
                onChange={e => setRespostaSeg(e.target.value)}
                placeholder="Sua resposta secreta"
                required
                className="w-full pl-12 pr-4 py-3 bg-[#f5f7fa] rounded-2xl border border-slate-200 text-sm focus:outline-none focus:border-[#4ecdc4]"
              />
            </div>
          </div>

          <button
            type="submit"
            disabled={loading}
            className="w-full bg-[#1a535c] hover:bg-[#1a535c]/90 text-white font-bold py-3.5 rounded-2xl text-sm transition shadow-lg flex items-center justify-center gap-2 mt-6"
          >
            <UserPlus className="w-5 h-5" />
            <span>{loading ? 'Cadastrando...' : 'Concluir Cadastro'}</span>
          </button>
        </form>

        <div className="mt-6 text-center text-xs text-slate-500 pt-4 border-t border-slate-100">
          Já tem uma conta?{' '}
          <Link to="/login" className="text-[#1a535c] font-bold hover:underline">
            Faça login aqui
          </Link>
        </div>
      </div>
    </div>
  );
};