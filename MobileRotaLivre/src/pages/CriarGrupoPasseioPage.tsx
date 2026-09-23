import React, { useEffect, useState } from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { PasseioDto } from '../types';
import { passeioService } from '../services/passeioService';
import {
  Users,
  Calendar,
  Clock,
  MapPin,
  ArrowLeft,
  ArrowRight,
  Hash
} from 'lucide-react';

const API_BASE_URL = 'https://rotalivre-web.onrender.com';

export const CriarGrupoPasseioPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const { getAuthHeader } = useAuth();
  const navigate = useNavigate();

  const [passeio, setPasseio] = useState<PasseioDto | null>(null);

  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);

  const [nomeGrupo, setNomeGrupo] = useState('');
  const [dataPasseio, setDataPasseio] = useState('');
  const [horarioPasseio, setHorarioPasseio] = useState('');
  
  const [dataFim, setDataFim] = useState('');
  const [horarioFim, setHorarioFim] = useState('');

  const [errorMsg, setErrorMsg] = useState('');

  // =========================================================
  // CARREGAR PASSEIO
  // =========================================================

  useEffect(() => {
    if (!id) {
      setErrorMsg('Passeio não informado.');
      setLoading(false);
      return;
    }

    const carregarPasseio = async () => {
      try {
        setLoading(true);
        setErrorMsg('');

        const idPasseio = Number(id);

        if (!Number.isInteger(idPasseio) || idPasseio <= 0) {
          throw new Error('ID do passeio inválido.');
        }

        const data = await passeioService.buscarPorId(idPasseio);

        if (!data) {
          throw new Error('Passeio não encontrado.');
        }

        setPasseio(data);

        const nome = (data as any).nome ?? (data as any).nome_passeio ?? 'Passeio';
        setNomeGrupo(`Grupo ${nome}`);

      } catch (error) {
        setPasseio(null);
        setErrorMsg(error instanceof Error ? error.message : 'Erro ao carregar informações do passeio.');
      } finally {
        setLoading(false);
      }
    };

    carregarPasseio();
  }, [id]);

  // =========================================================
  // CRIAR GRUPO
  // =========================================================

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!id) { setErrorMsg('ID do passeio não encontrado.'); return; }
    if (!passeio) { setErrorMsg('O passeio ainda não foi carregado.'); return; }
    if (!nomeGrupo.trim()) { setErrorMsg('Digite um nome para o grupo.'); return; }
    if (!dataPasseio || !horarioPasseio) { setErrorMsg('Selecione o início do passeio.'); return; }
    if (!dataFim || !horarioFim) { setErrorMsg('Selecione o fim do passeio.'); return; }

    const dataInicioStr = `${dataPasseio}T${horarioPasseio}:00`;
    const dataFimStr = `${dataFim}T${horarioFim}:00`;

    if (new Date(dataFimStr) <= new Date(dataInicioStr)) {
      setErrorMsg('O horário de término deve ser depois do horário de início.');
      return;
    }

    const idPasseio = Number(id);

    const body = {
      passeioId: idPasseio,
      nome: nomeGrupo.trim(),
      dataInicio: dataInicioStr,
      dataFim: dataFimStr
    };

    setSubmitting(true);
    setErrorMsg('');

    try {
      const response = await fetch(`${API_BASE_URL}/api/grupo/criar`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          ...getAuthHeader()
        },
        body: JSON.stringify(body)
      });

      const text = await response.text();
      let data: any = null;

      try {
        data = text ? JSON.parse(text) : null;
      } catch (parseError) {
        setErrorMsg('O servidor retornou uma resposta inválida.');
        return;
      }

      if (!response.ok) {
        const mensagem = data?.mensagem ?? data?.message ?? data?.erro ?? 'Não foi possível criar o grupo.';
        setErrorMsg(mensagem);
        return;
      }

      if (data?.idGrupo) {
        sessionStorage.setItem('activeLiveGrupoId', String(data.idGrupo));
      }

      navigate('/grupos');
    } catch (error) {
      setErrorMsg(error instanceof Error ? error.message : 'Erro de conexão ao servidor.');
    } finally {
      setSubmitting(false);
    }
  };

  // =========================================================
  // IMAGEM
  // =========================================================

  const getImageUrl = (url?: string) => {
    if (!url) return 'https://images.unsplash.com/photo-1519331379826-f10be5486c6f?w=800';
    if (url.startsWith('http')) return url;
    return `${API_BASE_URL}/img/passeios/${url}`;
  };

  const nomePasseio = (passeio as any)?.nome ?? (passeio as any)?.nome_passeio ?? 'Passeio';
  const imagemPasseio = (passeio as any)?.imagemUrl ?? (passeio as any)?.img_url;
  const bairroPasseio = (passeio as any)?.endereco?.bairro ?? 'São Paulo';
  const idPasseio = (passeio as any)?.id ?? (passeio as any)?.id_passeio ?? Number(id);

  if (loading) {
    return (
      <div className="pt-32 pb-28 flex justify-center items-center min-h-[60vh]">
        <div className="w-12 h-12 border-4 border-[#4ecdc4] border-t-transparent rounded-full animate-spin" />
      </div>
    );
  }

  if (!passeio) {
    return (
      <div className="pt-32 pb-28 max-w-xl mx-auto text-center px-4">
        <h2 className="text-2xl font-bold text-[#1a535c] mb-4">Passeio não encontrado</h2>
        <p className="text-sm text-slate-500 mb-6">Não foi possível carregar o passeio selecionado.</p>
        {errorMsg && <div className="mb-5 p-4 bg-rose-100 text-rose-700 rounded-2xl text-xs font-semibold">{errorMsg}</div>}
        <Link to="/" className="inline-flex items-center gap-2 bg-[#1a535c] text-white px-6 py-2.5 rounded-full font-bold text-sm">
          <ArrowLeft className="w-4 h-4" /> Voltar para Home
        </Link>
      </div>
    );
  }

  return (
    <div className="pt-20 pb-24 max-w-md mx-auto px-4">
      <Link to={`/passeio/${idPasseio}`} className="inline-flex items-center gap-2 text-sm font-semibold text-[#1a535c] hover:text-[#4ecdc4] mb-6 transition">
        <ArrowLeft className="w-4 h-4" />
        <span>Voltar para detalhes do passeio</span>
      </Link>

      <div className="bg-white rounded-3xl shadow-xl border border-slate-100 overflow-hidden">
        <div className="bg-gradient-to-r from-[#1a535c] to-[#236c78] p-8 text-white">
          <div className="flex items-center gap-3 mb-2">
            <div className="p-2.5 bg-[#4ecdc4] rounded-2xl">
              <Users className="w-6 h-6" />
            </div>
            <span className="text-xs font-bold uppercase tracking-wider text-[#4ecdc4]">Novo Grupo de Visita</span>
          </div>
          <h1 className="text-2xl sm:text-3xl font-extrabold mb-1">Criar Grupo</h1>
          <p className="text-xs text-slate-200">Agende o passeio e defina o tempo limite.</p>
        </div>

        <div className="p-8 space-y-6">
          <div className="bg-[#f5f7fa] rounded-2xl p-5 border border-slate-200">
            <div className="flex items-center gap-4">
              <img src={getImageUrl(imagemPasseio)} alt={nomePasseio} className="w-16 h-16 rounded-2xl object-cover shadow-sm shrink-0" />
              <div className="min-w-0">
                <h3 className="font-extrabold text-[#1a535c] text-base">{nomePasseio}</h3>
                <p className="text-xs text-slate-500 flex items-center gap-1 mt-1"><MapPin className="w-3.5 h-3.5 text-[#ff6b6b]" /><span className="truncate">{bairroPasseio}</span></p>
              </div>
            </div>
          </div>

          {errorMsg && <div className="p-4 bg-rose-100 text-rose-800 rounded-2xl text-xs font-semibold text-center">{errorMsg}</div>}

          <form onSubmit={handleSubmit} className="space-y-5">
            <div>
              <label className="block text-xs font-bold text-slate-500 uppercase mb-2">Nome do Grupo</label>
              <input type="text" value={nomeGrupo} onChange={e => setNomeGrupo(e.target.value)} placeholder="Ex: Amigos no Ibirapuera" required className="w-full p-3.5 bg-[#f5f7fa] rounded-2xl border border-slate-200 text-sm font-semibold text-[#1a535c] focus:outline-none focus:border-[#4ecdc4]" />
            </div>

            <div className="grid grid-cols-2 gap-4">
              <div>
                <label className="block text-xs font-bold text-slate-500 uppercase mb-2 flex items-center gap-1.5"><Calendar className="w-4 h-4 text-[#4ecdc4]" /> Início (Data)</label>
                <input type="date" value={dataPasseio} onChange={e => setDataPasseio(e.target.value)} min={new Date().toISOString().split('T')[0]} required className="w-full p-3.5 bg-[#f5f7fa] rounded-2xl border border-slate-200 text-sm font-semibold text-[#1a535c] focus:outline-none focus:border-[#4ecdc4]" />
              </div>
              <div>
                <label className="block text-xs font-bold text-slate-500 uppercase mb-2 flex items-center gap-1.5"><Clock className="w-4 h-4 text-[#4ecdc4]" /> Horário</label>
                <input type="time" value={horarioPasseio} onChange={e => setHorarioPasseio(e.target.value)} required className="w-full p-3.5 bg-[#f5f7fa] rounded-2xl border border-slate-200 text-sm font-semibold text-[#1a535c] focus:outline-none focus:border-[#4ecdc4]" />
              </div>
            </div>

            <div className="grid grid-cols-2 gap-4">
              <div>
                <label className="block text-xs font-bold text-slate-500 uppercase mb-2 flex items-center gap-1.5"><Calendar className="w-4 h-4 text-[#ff6b6b]" /> Fim Previsto</label>
                <input type="date" value={dataFim} onChange={e => setDataFim(e.target.value)} min={dataPasseio || new Date().toISOString().split('T')[0]} required className="w-full p-3.5 bg-[#f5f7fa] rounded-2xl border border-slate-200 text-sm font-semibold text-[#1a535c] focus:outline-none focus:border-[#4ecdc4]" />
              </div>
              <div>
                <label className="block text-xs font-bold text-slate-500 uppercase mb-2 flex items-center gap-1.5"><Clock className="w-4 h-4 text-[#ff6b6b]" /> Horário</label>
                <input type="time" value={horarioFim} onChange={e => setHorarioFim(e.target.value)} required className="w-full p-3.5 bg-[#f5f7fa] rounded-2xl border border-slate-200 text-sm font-semibold text-[#1a535c] focus:outline-none focus:border-[#4ecdc4]" />
              </div>
            </div>
            <p className="text-[10px] text-slate-400 mt-1 text-center">Ao atingir o fim previsto, o grupo será finalizado automaticamente.</p>

            <button type="submit" disabled={submitting} className="w-full bg-[#4ecdc4] hover:bg-[#4ecdc4]/90 disabled:opacity-60 text-white font-extrabold py-4 rounded-2xl text-sm transition shadow-lg flex items-center justify-center gap-2 mt-6">
              {submitting ? (
                <><div className="w-5 h-5 border-2 border-white border-t-transparent rounded-full animate-spin" /><span>Criando Grupo...</span></>
              ) : (
                <><span>Confirmar e Criar Grupo</span><ArrowRight className="w-4 h-4" /></>
              )}
            </button>
          </form>
        </div>
      </div>
    </div>
  );
};