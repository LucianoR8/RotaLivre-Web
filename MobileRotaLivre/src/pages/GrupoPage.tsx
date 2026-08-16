import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { Grupo, Passeio } from '../types';
import { KeyRound, ArrowRight, Users, Calendar, Clock, MapPin, Hash, X, Play, Copy, Check, Sparkles, Trash2, AlertTriangle } from 'lucide-react';

interface GrupoComPasseio extends Grupo {
  passeio?: Passeio | null;
}

export const GrupoPage: React.FC = () => {
  const { getAuthHeader } = useAuth();
  const navigate = useNavigate();

  const [codigoEntrar, setCodigoEntrar] = useState('');
  const [loading, setLoading] = useState(false);
  const [msgError, setMsgError] = useState('');

  const [meusGrupos, setMeusGrupos] = useState<GrupoComPasseio[]>([]);
  const [loadingGrupos, setLoadingGrupos] = useState(true);

  // Selected group for modal pop-up
  const [selectedGrupo, setSelectedGrupo] = useState<GrupoComPasseio | null>(null);
  const [copiedCode, setCopiedCode] = useState(false);
  const [confirmCancel, setConfirmCancel] = useState(false);
  const [canceling, setCanceling] = useState(false);

  const carregarMeusGrupos = () => {
    setLoadingGrupos(true);
    fetch('/api/grupo/meus', { headers: getAuthHeader() })
      .then(res => res.json())
      .then(data => {
        if (Array.isArray(data)) {
          setMeusGrupos(data);
        }
        setLoadingGrupos(false);
      })
      .catch(err => {
        console.error('Erro ao carregar meus grupos:', err);
        setLoadingGrupos(false);
      });
  };

  useEffect(() => {
    carregarMeusGrupos();
  }, []);

  const handleEntrar = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!codigoEntrar.trim()) return;

    setLoading(true);
    setMsgError('');

    try {
      const res = await fetch('/api/grupo/entrar', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json', ...getAuthHeader() },
        body: JSON.stringify({ codigo_grupo: codigoEntrar })
      });
      const data = await res.json();
      if (data.sucesso && data.grupo) {
        carregarMeusGrupos();
        setSelectedGrupo(data.grupo);
        setCodigoEntrar('');
      } else {
        setMsgError(data.mensagem || 'Código de grupo inválido.');
      }
    } catch (err) {
      setMsgError('Erro de conexão.');
    } finally {
      setLoading(false);
    }
  };

  const handleCopyCode = (codigo: string) => {
    navigator.clipboard.writeText(codigo);
    setCopiedCode(true);
    setTimeout(() => setCopiedCode(false), 2000);
  };

  const handleIniciarPasseio = (grupoId: number) => {
    setSelectedGrupo(null);
    setConfirmCancel(false);
    sessionStorage.setItem('activeLiveGrupoId', String(grupoId));
    navigate(`/ao-vivo?grupoId=${grupoId}`);
  };

  const handleCancelarPasseio = async (grupoId: number) => {
    setCanceling(true);
    try {
      await fetch(`/api/grupo/${grupoId}`, {
        method: 'DELETE',
        headers: getAuthHeader()
      });
      setSelectedGrupo(null);
      setConfirmCancel(false);
      carregarMeusGrupos();
    } catch (err) {
      console.error('Erro ao cancelar passeio:', err);
    } finally {
      setCanceling(false);
    }
  };

  const getPasseioImageUrl = (url?: string) => {
    if (!url) return 'https://images.unsplash.com/photo-1519331379826-f10be5486c6f?w=800';
    if (url.startsWith('http')) return url;
    return `/img/passeios/${url}`;
  };

  return (
    <div className="pt-20 pb-24 max-w-md mx-auto px-4">
      {/* Page Title Header */}
      <div className="text-center mb-10">
        <div className="inline-flex p-3 bg-[#4ecdc4]/15 rounded-2xl text-[#1a535c] mb-3">
          <Users className="w-8 h-8 text-[#4ecdc4]" />
        </div>
        <h1 className="text-3xl font-extrabold text-[#1a535c]">Meus Grupos e Passeios</h1>
        <p className="text-sm text-slate-500 mt-1 max-w-lg mx-auto">
          Acompanhe os passeios que você criou ou participa, ou entre em um novo grupo digitando o código de convite!
        </p>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-8 items-start">
        {/* Join Group Box (Left Column on large screen, top on mobile) */}
        <div className="lg:col-span-1 bg-white rounded-3xl p-6 shadow-xl border border-slate-100">
          <div className="flex items-center gap-3 mb-3">
            <div className="p-2.5 bg-[#4ecdc4] text-white rounded-xl">
              <KeyRound className="w-5 h-5" />
            </div>
            <h2 className="text-lg font-bold text-[#1a535c]">Entrar com Código</h2>
          </div>
          <p className="text-xs text-slate-500 mb-5 leading-relaxed">
            Digite o código do grupo (ex: ROTA123) para se juntar ao passeio.
          </p>

          {msgError && (
            <div className="mb-4 p-3 bg-rose-100 text-rose-800 rounded-xl text-xs font-semibold text-center">
              {msgError}
            </div>
          )}

          <form onSubmit={handleEntrar} className="space-y-3">
            <div>
              <label className="block text-[11px] font-bold text-slate-500 uppercase mb-1">Código do Grupo</label>
              <input
                type="text"
                value={codigoEntrar}
                onChange={e => setCodigoEntrar(e.target.value.toUpperCase())}
                placeholder="ROTA123"
                required
                className="w-full p-3 bg-[#f5f7fa] rounded-xl border border-slate-200 text-sm font-bold tracking-widest text-[#1a535c] uppercase focus:outline-none focus:border-[#4ecdc4]"
              />
            </div>

            <button
              type="submit"
              disabled={loading}
              className="w-full bg-[#4ecdc4] hover:bg-[#4ecdc4]/90 text-white font-bold py-3 rounded-xl text-xs transition shadow-md flex items-center justify-center gap-2 mt-2 hover:-translate-y-0.5"
            >
              <span>{loading ? 'Acessando...' : 'Entrar no Grupo'}</span>
              <ArrowRight className="w-4 h-4" />
            </button>
          </form>
        </div>

        {/* User Groups List (Right Column) */}
        <div className="lg:col-span-2 space-y-4">
          <h2 className="text-xl font-extrabold text-[#1a535c] flex items-center gap-2">
            <Sparkles className="w-5 h-5 text-[#ff6b6b]" />
            Seus Passeios Agendados
          </h2>

          {loadingGrupos ? (
            <div className="bg-white rounded-3xl p-8 text-center shadow-md border border-slate-100">
              <div className="w-8 h-8 border-3 border-[#4ecdc4] border-t-transparent rounded-full animate-spin mx-auto mb-2" />
              <p className="text-xs text-slate-400">Carregando seus grupos...</p>
            </div>
          ) : meusGrupos.length > 0 ? (
            <div className="space-y-4">
              {meusGrupos.map(g => (
                <div
                  key={g.id_grupo}
                  onClick={() => setSelectedGrupo(g)}
                  className="bg-white rounded-2xl p-5 shadow-md border border-slate-100 hover:shadow-xl hover:border-[#4ecdc4]/40 transition-all cursor-pointer group flex flex-col sm:flex-row items-start sm:items-center justify-between gap-4"
                >
                  <div className="flex items-center gap-4">
                    <img
                      src={getPasseioImageUrl(g.passeio?.img_url)}
                      alt={g.nome_grupo}
                      className="w-16 h-16 rounded-2xl object-cover shrink-0 group-hover:scale-105 transition-transform"
                    />
                    <div>
                      <div className="flex items-center gap-2">
                        <span className="bg-[#4ecdc4]/15 text-[#1a535c] font-bold text-[10px] px-2.5 py-0.5 rounded-full uppercase tracking-wider">
                          Código: {g.codigo_grupo}
                        </span>
                      </div>
                      <h3 className="font-extrabold text-[#1a535c] text-base group-hover:text-[#4ecdc4] transition mt-1">
                        {g.nome_grupo}
                      </h3>
                      {g.passeio && (
                        <p className="text-xs text-slate-500 font-medium flex items-center gap-1 mt-0.5">
                          <MapPin className="w-3.5 h-3.5 text-[#ff6b6b]" />
                          <span>{g.passeio.nome_passeio}</span>
                        </p>
                      )}
                      <div className="flex flex-wrap items-center gap-3 text-xs text-slate-400 mt-2 font-semibold">
                        {g.data_passeio && (
                          <span className="flex items-center gap-1 text-[#1a535c]">
                            <Calendar className="w-3.5 h-3.5 text-[#4ecdc4]" />
                            {new Date(g.data_passeio + 'T00:00:00').toLocaleDateString('pt-BR')}
                          </span>
                        )}
                        {g.horario_passeio && (
                          <span className="flex items-center gap-1 text-[#1a535c]">
                            <Clock className="w-3.5 h-3.5 text-[#ff6b6b]" />
                            {g.horario_passeio}
                          </span>
                        )}
                        <span className="flex items-center gap-1 text-slate-500">
                          <Users className="w-3.5 h-3.5" />
                          {g.membros.length} integrante(s)
                        </span>
                      </div>
                    </div>
                  </div>

                  <button className="bg-[#1a535c] group-hover:bg-[#4ecdc4] text-white font-bold px-4 py-2.5 rounded-xl text-xs transition flex items-center gap-1.5 self-end sm:self-auto shrink-0">
                    <Play className="w-3.5 h-3.5 fill-current" />
                    <span>Ver Detalhes</span>
                  </button>
                </div>
              ))}
            </div>
          ) : (
            <div className="bg-white rounded-3xl p-8 text-center shadow-md border border-slate-100">
              <Users className="w-10 h-10 text-slate-300 mx-auto mb-3" />
              <p className="text-sm font-semibold text-slate-600 mb-1">Nenhum passeio em grupo cadastrado</p>
              <p className="text-xs text-slate-400 max-w-sm mx-auto">
                Visite a página de um passeio e clique em "Criar Grupo para este Passeio" ou entre com um código ao lado!
              </p>
            </div>
          )}
        </div>
      </div>

      {/* MODAL POP-UP DO PASSEIO / GRUPO SELECIONADO */}
      {selectedGrupo && (
        <div className="fixed inset-0 z-50 bg-slate-900/60 backdrop-blur-sm flex items-center justify-center p-4 animate-fadeIn">
          <div className="bg-white rounded-3xl max-w-lg w-full shadow-2xl overflow-hidden border border-slate-100 relative animate-scaleUp">
            {/* Close Button */}
            <button
              onClick={() => {
                setSelectedGrupo(null);
                setConfirmCancel(false);
              }}
              className="absolute top-4 right-4 z-10 p-2 bg-black/40 hover:bg-black/60 text-white rounded-full transition"
            >
              <X className="w-5 h-5" />
            </button>

            {/* Banner Image */}
            <div className="relative h-48 sm:h-56 overflow-hidden">
              <img
                src={getPasseioImageUrl(selectedGrupo.passeio?.img_url)}
                alt={selectedGrupo.nome_grupo}
                className="w-full h-full object-cover"
              />
              <div className="absolute inset-0 bg-gradient-to-t from-slate-950/80 via-slate-950/20 to-transparent" />
              <div className="absolute bottom-4 left-6 right-6 text-white">
                <span className="bg-[#4ecdc4] text-white text-[10px] font-extrabold px-3 py-1 rounded-full uppercase tracking-wider mb-2 inline-block">
                  Passeio Agendado
                </span>
                <h2 className="text-2xl font-extrabold leading-tight">{selectedGrupo.nome_grupo}</h2>
                {selectedGrupo.passeio && (
                  <p className="text-xs text-slate-200 mt-1 flex items-center gap-1">
                    <MapPin className="w-3.5 h-3.5 text-[#ff6b6b]" />
                    <span>{selectedGrupo.passeio.nome_passeio}</span>
                  </p>
                )}
              </div>
            </div>

            {/* Modal Details Content */}
            <div className="p-6 space-y-5">
              {/* Code & Schedule Bar */}
              <div className="grid grid-cols-2 gap-3 bg-[#f5f7fa] p-3.5 rounded-2xl border border-slate-200">
                <div className="flex flex-col justify-center">
                  <span className="text-[10px] font-bold text-slate-400 uppercase">Código do Grupo</span>
                  <div className="flex items-center gap-2 mt-0.5">
                    <span className="text-sm font-extrabold text-[#1a535c] tracking-widest">
                      {selectedGrupo.codigo_grupo}
                    </span>
                    <button
                      onClick={() => handleCopyCode(selectedGrupo.codigo_grupo)}
                      className="p-1 text-slate-400 hover:text-[#4ecdc4] transition"
                      title="Copiar código"
                    >
                      {copiedCode ? <Check className="w-4 h-4 text-emerald-500" /> : <Copy className="w-4 h-4" />}
                    </button>
                  </div>
                </div>

                <div className="flex flex-col justify-center border-l border-slate-200 pl-3">
                  <span className="text-[10px] font-bold text-slate-400 uppercase">Data e Horário</span>
                  <span className="text-xs font-bold text-[#1a535c] mt-0.5">
                    {selectedGrupo.data_passeio ? new Date(selectedGrupo.data_passeio + 'T00:00:00').toLocaleDateString('pt-BR') : 'Data a definir'}
                    {selectedGrupo.horario_passeio ? ` às ${selectedGrupo.horario_passeio}` : ''}
                  </span>
                </div>
              </div>

              {/* Members List */}
              <div>
                <h4 className="text-xs font-bold text-slate-500 uppercase mb-2 flex items-center gap-1.5">
                  <Users className="w-4 h-4 text-[#4ecdc4]" />
                  Integrantes do Grupo ({selectedGrupo.membros.length})
                </h4>
                <div className="flex flex-wrap gap-2 max-h-24 overflow-y-auto">
                  {selectedGrupo.membros.map(m => (
                    <span
                      key={m.id_usuario}
                      className="bg-slate-100 text-slate-700 text-xs font-semibold px-3 py-1 rounded-full border border-slate-200"
                    >
                      {m.nome}
                    </span>
                  ))}
                </div>
              </div>

              {/* Action Buttons: Iniciar ou Cancelar Passeio */}
              {confirmCancel ? (
                <div className="bg-rose-50 border border-rose-200 rounded-2xl p-4 text-center space-y-3 animate-fadeIn">
                  <div className="flex items-center justify-center gap-2 text-rose-600 font-bold text-sm">
                    <AlertTriangle className="w-5 h-5 shrink-0" />
                    <span>Deseja cancelar este passeio?</span>
                  </div>
                  <p className="text-xs text-slate-600">
                    Esta ação removerá o agendamento do seu grupo. Não será possível recuperar.
                  </p>
                  <div className="grid grid-cols-2 gap-2 pt-1">
                    <button
                      onClick={() => setConfirmCancel(false)}
                      disabled={canceling}
                      className="w-full bg-slate-200 hover:bg-slate-300 text-slate-700 font-bold py-2.5 rounded-xl text-xs transition"
                    >
                      Voltar
                    </button>
                    <button
                      onClick={() => handleCancelarPasseio(selectedGrupo.id_grupo)}
                      disabled={canceling}
                      className="w-full bg-rose-600 hover:bg-rose-700 text-white font-bold py-2.5 rounded-xl text-xs transition shadow-sm flex items-center justify-center gap-1.5"
                    >
                      {canceling ? (
                        <span>Cancelando...</span>
                      ) : (
                        <>
                          <Trash2 className="w-4 h-4" />
                          <span>Sim, Cancelar</span>
                        </>
                      )}
                    </button>
                  </div>
                </div>
              ) : (
                <div className="space-y-2">
                  <button
                    onClick={() => handleIniciarPasseio(selectedGrupo.id_grupo)}
                    className="w-full bg-[#4ecdc4] hover:bg-[#4ecdc4]/90 text-white font-extrabold py-3.5 rounded-2xl text-sm transition shadow-lg flex items-center justify-center gap-2 hover:scale-[1.01]"
                  >
                    <Play className="w-5 h-5 fill-current" />
                    <span>Iniciar Passeio</span>
                  </button>

                  <button
                    onClick={() => setConfirmCancel(true)}
                    className="w-full bg-rose-50 hover:bg-rose-100 text-rose-600 border border-rose-200/80 font-bold py-3 rounded-2xl text-xs transition flex items-center justify-center gap-2"
                  >
                    <Trash2 className="w-4 h-4" />
                    <span>Cancelar Passeio</span>
                  </button>
                </div>
              )}
            </div>
          </div>
        </div>
      )}
    </div>
  );
};
