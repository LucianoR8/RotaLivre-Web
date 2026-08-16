import React, { useEffect, useState } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { Passeio } from '../types';
import { Users, Calendar, Clock, MapPin, ArrowLeft, ArrowRight, Sparkles, Hash } from 'lucide-react';

export const CriarGrupoPasseioPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const { getAuthHeader } = useAuth();
  const navigate = useNavigate();

  const [passeio, setPasseio] = useState<Passeio | null>(null);
  const [loading, setLoading] = useState(true);
  const [nomeGrupo, setNomeGrupo] = useState('');
  const [dataPasseio, setDataPasseio] = useState('');
  const [horarioPasseio, setHorarioPasseio] = useState('');
  const [submitting, setSubmitting] = useState(false);
  const [errorMsg, setErrorMsg] = useState('');

  useEffect(() => {
    if (!id) return;
    setLoading(true);
    fetch(`/api/passeios/${id}`, { headers: getAuthHeader() })
      .then(res => res.json())
      .then(data => {
        if (data && data.id_passeio) {
          setPasseio(data);
          setNomeGrupo(`Grupo ${data.nome_passeio}`);
        } else {
          setErrorMsg('Passeio não encontrado.');
        }
        setLoading(false);
      })
      .catch(err => {
        console.error('Erro ao buscar passeio:', err);
        setErrorMsg('Erro ao carregar informações do passeio.');
        setLoading(false);
      });
  }, [id]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!nomeGrupo.trim() || !dataPasseio || !horarioPasseio) {
      setErrorMsg('Por favor, preencha o nome do grupo, o dia e o horário do passeio.');
      return;
    }

    setSubmitting(true);
    setErrorMsg('');

    try {
      const res = await fetch('/api/grupo/criar', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          ...getAuthHeader()
        },
        body: JSON.stringify({
          nome_grupo: nomeGrupo,
          id_passeio: Number(id),
          data_passeio: dataPasseio,
          horario_passeio: horarioPasseio
        })
      });

      const data = await res.json();
      if (data.sucesso && data.grupo) {
        navigate('/grupos');
      } else {
        setErrorMsg('Não foi possível criar o grupo. Tente novamente.');
      }
    } catch (err) {
      console.error('Erro ao criar grupo:', err);
      setErrorMsg('Erro de conexão ao servidor.');
    } finally {
      setSubmitting(false);
    }
  };

  const getImageUrl = (url?: string) => {
    if (!url) return 'https://images.unsplash.com/photo-1519331379826-f10be5486c6f?w=800';
    if (url.startsWith('http')) return url;
    return `/img/passeios/${url}`;
  };

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
        <Link to="/" className="bg-[#1a535c] text-white px-6 py-2.5 rounded-full font-bold text-sm">
          Voltar para Home
        </Link>
      </div>
    );
  }

  return (
    <div className="pt-20 pb-24 max-w-md mx-auto px-4">
      {/* Back link */}
      <Link
        to={`/passeio/${passeio.id_passeio}`}
        className="inline-flex items-center gap-2 text-sm font-semibold text-[#1a535c] hover:text-[#4ecdc4] mb-6 transition"
      >
        <ArrowLeft className="w-4 h-4" />
        <span>Voltar para detalhes do passeio</span>
      </Link>

      {/* Main Container */}
      <div className="bg-white rounded-3xl shadow-xl border border-slate-100 overflow-hidden">
        {/* Header Banner */}
        <div className="bg-gradient-to-r from-[#1a535c] to-[#236c78] p-8 text-white relative">
          <div className="flex items-center gap-3 mb-2">
            <div className="p-2.5 bg-[#4ecdc4] rounded-2xl text-white">
              <Users className="w-6 h-6" />
            </div>
            <span className="text-xs font-bold uppercase tracking-wider text-[#4ecdc4]">Novo Grupo de Visita</span>
          </div>
          <h1 className="text-2xl sm:text-3xl font-extrabold mb-1">Criar Grupo para Passeio</h1>
          <p className="text-xs text-slate-200">
            Agende o dia e horário do passeio para reunir amigos e compartilhar a localização em tempo real.
          </p>
        </div>

        {/* Info Card do Passeio & Código do Passeio */}
        <div className="p-8 space-y-6">
          <div className="bg-[#f5f7fa] rounded-2xl p-5 border border-slate-200 flex flex-col sm:flex-row items-start sm:items-center justify-between gap-4">
            <div className="flex items-center gap-4">
              <img
                src={getImageUrl(passeio.img_url)}
                alt={passeio.nome_passeio}
                className="w-16 h-16 rounded-2xl object-cover shadow-sm shrink-0"
              />
              <div>
                <h3 className="font-extrabold text-[#1a535c] text-base">{passeio.nome_passeio}</h3>
                <p className="text-xs text-slate-500 flex items-center gap-1 mt-0.5">
                  <MapPin className="w-3.5 h-3.5 text-[#ff6b6b]" />
                  <span>{passeio.endereco ? `${passeio.endereco.bairro}` : 'São Paulo'}</span>
                </p>
              </div>
            </div>

            {/* Código do Passeio Info Badge */}
            <div className="bg-white px-4 py-2.5 rounded-2xl border border-slate-200 shadow-sm flex items-center gap-2 self-stretch sm:self-auto justify-center">
              <Hash className="w-4 h-4 text-[#ff6b6b]" />
              <div className="text-left">
                <span className="block text-[10px] font-bold text-slate-400 uppercase tracking-wider">Código do Passeio</span>
                <span className="text-xs font-extrabold text-[#1a535c]">PASSEIO-#{passeio.id_passeio}</span>
              </div>
            </div>
          </div>

          {errorMsg && (
            <div className="p-4 bg-rose-100 text-rose-800 rounded-2xl text-xs font-semibold text-center">
              {errorMsg}
            </div>
          )}

          {/* Form */}
          <form onSubmit={handleSubmit} className="space-y-5">
            <div>
              <label className="block text-xs font-bold text-slate-500 uppercase mb-2">
                Nome do Grupo
              </label>
              <input
                type="text"
                value={nomeGrupo}
                onChange={e => setNomeGrupo(e.target.value)}
                placeholder="Ex: Amigos no Ibirapuera"
                required
                className="w-full p-3.5 bg-[#f5f7fa] rounded-2xl border border-slate-200 text-sm font-semibold text-[#1a535c] focus:outline-none focus:border-[#4ecdc4]"
              />
            </div>

            <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
              <div>
                <label className="block text-xs font-bold text-slate-500 uppercase mb-2 flex items-center gap-1.5">
                  <Calendar className="w-4 h-4 text-[#4ecdc4]" />
                  Dia do Passeio
                </label>
                <input
                  type="date"
                  value={dataPasseio}
                  onChange={e => setDataPasseio(e.target.value)}
                  required
                  className="w-full p-3.5 bg-[#f5f7fa] rounded-2xl border border-slate-200 text-sm font-semibold text-[#1a535c] focus:outline-none focus:border-[#4ecdc4]"
                />
              </div>

              <div>
                <label className="block text-xs font-bold text-slate-500 uppercase mb-2 flex items-center gap-1.5">
                  <Clock className="w-4 h-4 text-[#ff6b6b]" />
                  Horário do Passeio
                </label>
                <input
                  type="time"
                  value={horarioPasseio}
                  onChange={e => setHorarioPasseio(e.target.value)}
                  required
                  className="w-full p-3.5 bg-[#f5f7fa] rounded-2xl border border-slate-200 text-sm font-semibold text-[#1a535c] focus:outline-none focus:border-[#4ecdc4]"
                />
              </div>
            </div>

            <button
              type="submit"
              disabled={submitting}
              className="w-full bg-[#4ecdc4] hover:bg-[#4ecdc4]/90 text-white font-extrabold py-4 rounded-2xl text-sm transition shadow-lg flex items-center justify-center gap-2 mt-6 hover:-translate-y-0.5"
            >
              <span>{submitting ? 'Criando Grupo...' : 'Confirmar e Criar Grupo'}</span>
              <ArrowRight className="w-4 h-4" />
            </button>
          </form>
        </div>
      </div>
    </div>
  );
};
