import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  KeyRound,
  ArrowRight,
  Users,
  Calendar,
  Clock,
  MapPin,
  X,
  Play,
  Copy,
  Check,
  Sparkles,
  LogOut,
  Trash2,
  AlertTriangle
} from 'lucide-react';

import { useAuth } from '../context/AuthContext';

import {
  PasseioPendenteDto,
  GrupoDetalhesDto,
  buscarMeusPendentes,
  buscarGrupo,
  iniciarPasseio,
  sairDoGrupo,
  cancelarGrupo,
  alterarDataGrupo
} from '../services/grupoService';

export const GrupoPage: React.FC = () => {
  const { usuario, getAuthHeader } = useAuth();
  const navigate = useNavigate();

  const [codigoEntrar, setCodigoEntrar] = useState('');
  const [loading, setLoading] = useState(false);
  const [msgError, setMsgError] = useState('');
  const [meusGrupos, setMeusGrupos] = useState<any[]>([]);
  const [loadingGrupos, setLoadingGrupos] = useState(true);
  const [selectedGrupo, setSelectedGrupo] = useState<any | null>(null);
  const [grupoDetalhes, setGrupoDetalhes] = useState<any | null>(null);
  const [loadingDetalhes, setLoadingDetalhes] = useState(false);
  const [copiedCode, setCopiedCode] = useState(false);
  const [confirmLeave, setConfirmLeave] = useState(false);
  const [confirmCancel, setConfirmCancel] = useState(false);
  const [leaving, setLeaving] = useState(false);
  const [canceling, setCanceling] = useState(false);
  const [starting, setStarting] = useState(false);
  
  const [editandoData, setEditandoData] = useState(false);
  const [novaData, setNovaData] = useState('');
  const [novoHorario, setNovoHorario] = useState('');
  const [alterandoData, setAlterandoData] = useState(false);
  const [errorData, setErrorData] = useState('');

  const getUsuarioId = (): number | null => {
    if (!usuario) return null;
    const id = (usuario as any).id_usuario ?? (usuario as any).idUsuario ?? (usuario as any).id;
    if (id === undefined || id === null) return null;
    const numero = Number(id);
    return Number.isNaN(numero) ? null : numero;
  };

  const usuarioEhCriador = (): boolean => {
    const usuarioId = getUsuarioId();
    if (usuarioId === null || !grupoDetalhes) return false;
    return Number(grupoDetalhes.criadorId) === Number(usuarioId);
  };

  const carregarMeusGrupos = async () => {
    setLoadingGrupos(true);
    try {
      const data = await buscarMeusPendentes(getAuthHeader);
      setMeusGrupos(data);
    } catch (err) {
      setMeusGrupos([]);
    } finally {
      setLoadingGrupos(false);
    }
  };

  useEffect(() => {
    carregarMeusGrupos();
  }, []);

  const abrirDetalhesGrupo = async (grupo: any) => {
    setMsgError('');
    setSelectedGrupo(grupo);
    setGrupoDetalhes(null);
    setConfirmLeave(false);
    setConfirmCancel(false);
    setCopiedCode(false);
    setEditandoData(false);
    setErrorData('');
    setLoadingDetalhes(true);

    try {
      const detalhes = await buscarGrupo(grupo.idGrupo, getAuthHeader);
      setGrupoDetalhes(detalhes);
    } catch (err) {
      setMsgError('Não foi possível carregar os detalhes do grupo.');
    } finally {
      setLoadingDetalhes(false);
    }
  };

  const handleEntrar = async (e: React.FormEvent) => {
    e.preventDefault();
    const codigo = codigoEntrar.trim().toUpperCase();
    if (!codigo) return;

    setLoading(true);
    setMsgError('');

    try {
      const headers = { 'Content-Type': 'application/json', ...getAuthHeader() };
      const body = { codigoConvite: codigo };
      const res = await fetch('https://rotalivre-web.onrender.com/api/grupo/entrar', { method: 'POST', headers, body: JSON.stringify(body) });
      const text = await res.text();
      let data: any = null;

      if (text) {
        try { data = JSON.parse(text); } catch {}
      }

      if (!res.ok) {
        setMsgError(data?.mensagem || data?.title || text || 'Não foi possível entrar no grupo.');
        return;
      }

      setCodigoEntrar('');
      if (data?.idGrupo) {
        try {
          const grupo = await buscarGrupo(Number(data.idGrupo), getAuthHeader);
          
          setSelectedGrupo({
            idGrupo: grupo.idGrupo,
            idPasseio: grupo.idPasseio,
            nomeGrupo: grupo.nome,
            codigoConvite: grupo.codigoConvite,
            status: grupo.status,
            dataInicio: grupo.dataInicio,
            dataFim: grupo.dataFim,
            criadorId: grupo.criadorId,
            passeio: grupo.passeio ? {
              id: grupo.passeio.id,
              nome: grupo.passeio.nome,
              descricao: grupo.passeio.descricao,
              imagemUrl: grupo.passeio.imagemUrl
            } : { id: grupo.idPasseio, nome: 'Passeio', descricao: '', imagemUrl: '' }
          });
          setGrupoDetalhes(grupo);
          await carregarMeusGrupos();
        } catch (erro) {
          await carregarMeusGrupos();
        }
      } else {
        await carregarMeusGrupos();
      }
    } catch (err) {
      setMsgError('Não foi possível conectar ao servidor.');
    } finally {
      setLoading(false);
    }
  };

  const handleCopyCode = async (codigo: string) => {
    try {
      await navigator.clipboard.writeText(codigo);
      setCopiedCode(true);
      setTimeout(() => setCopiedCode(false), 2000);
    } catch (err) {}
  };

  const handleIniciarPasseio = async (grupoId: number) => {
    if (starting) return;
    setStarting(true);
    setMsgError('');

    try {
      await iniciarPasseio(grupoId, getAuthHeader);
      sessionStorage.setItem('activeLiveGrupoId', String(grupoId));
      setSelectedGrupo(null);
      setGrupoDetalhes(null);
      navigate(`/ao-vivo?grupoId=${grupoId}`);
    } catch (err) {
      setMsgError(err instanceof Error ? err.message : 'Não foi possível iniciar o passeio.');
    } finally {
      setStarting(false);
    }
  };

  const handleSairGrupo = async (grupoId: number) => {
    if (leaving) return;
    setLeaving(true);
    setMsgError('');

    try {
      await sairDoGrupo(grupoId, getAuthHeader);
      setSelectedGrupo(null);
      setGrupoDetalhes(null);
      setConfirmLeave(false);
      await carregarMeusGrupos();
    } catch (err) {
      setMsgError(err instanceof Error ? err.message : 'Não foi possível sair do grupo.');
    } finally {
      setLeaving(false);
    }
  };

  const handleCancelarPasseio = async (grupoId: number) => {
    if (canceling) return;
    setCanceling(true);
    setMsgError('');

    try {
      await cancelarGrupo(grupoId, getAuthHeader);
      setSelectedGrupo(null);
      setGrupoDetalhes(null);
      setConfirmCancel(false);
      await carregarMeusGrupos();
    } catch (err) {
      setMsgError(err instanceof Error ? err.message : 'Erro ao cancelar passeio.');
    } finally {
      setCanceling(false);
    }
  };

  // Garante que o JS trata a data do banco (UTC) corretamente, convertendo-a para a hora local
  const parseUtcDate = (dateString?: string) => {
    if (!dateString) return null;
    return new Date(dateString.endsWith('Z') ? dateString : `${dateString}Z`);
  };

  const iniciarEdicaoData = () => {
    setErrorData('');
    const parsedDate = parseUtcDate(selectedGrupo?.dataInicio);
    if (parsedDate) {
      setNovaData(parsedDate.toISOString().split('T')[0]);
      setNovoHorario(`${String(parsedDate.getHours()).padStart(2, '0')}:${String(parsedDate.getMinutes()).padStart(2, '0')}`);
    } else {
      setNovaData('');
      setNovoHorario('');
    }
    setEditandoData(true);
  };

  const handleAlterarData = async (grupoId: number) => {
    if (alterandoData) return;
    if (!novaData || !novoHorario) { setErrorData('Selecione a data e o horário.'); return; }

    const dataInicio = `${novaData}T${novoHorario}:00`;
    setAlterandoData(true);
    setErrorData('');

    try {
      const data = await alterarDataGrupo(grupoId, dataInicio, getAuthHeader);
      setSelectedGrupo((prev: any) => prev ? { ...prev, dataInicio: data?.dataInicio ?? dataInicio } : prev);
      setGrupoDetalhes((prev: any) => prev ? { ...prev, dataInicio: data?.dataInicio ?? dataInicio } : prev);
      await carregarMeusGrupos();
      setEditandoData(false);
    } catch (err) {
      setErrorData(err instanceof Error ? err.message : 'Erro ao alterar a data.');
    } finally {
      setAlterandoData(false);
    }
  };

  const getPasseioImageUrl = (url?: string) => {
    if (!url) return 'https://images.unsplash.com/photo-1519331379826-f10be5486c6f?w=800';
    if (url.startsWith('http')) return url;
    return `/img/passeios/${url}`;
  };

  const formatarData = (data?: string) => {
    const d = parseUtcDate(data);
    return d ? d.toLocaleDateString('pt-BR') : 'Data a definir';
  };

  const formatarHorario = (data?: string) => {
    const d = parseUtcDate(data);
    return d ? d.toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' }) : '';
  };

  return (
    <div className="pt-20 pb-24 max-w-md mx-auto px-4">
      <div className="text-center mb-10">
        <div className="inline-flex p-3 bg-[#4ecdc4]/15 rounded-2xl text-[#1a535c] mb-3"><Users className="w-8 h-8 text-[#4ecdc4]" /></div>
        <h1 className="text-3xl font-extrabold text-[#1a535c]">Meus Grupos e Passeios</h1>
        <p className="text-sm text-slate-500 mt-1">Acompanhe os passeios ou entre em um novo grupo.</p>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-8 items-start">
        <div className="lg:col-span-1 bg-white rounded-3xl p-6 shadow-xl border border-slate-100">
          <div className="flex items-center gap-3 mb-3">
            <div className="p-2.5 bg-[#4ecdc4] text-white rounded-xl"><KeyRound className="w-5 h-5" /></div>
            <h2 className="text-lg font-bold text-[#1a535c]">Entrar com Código</h2>
          </div>
          <p className="text-xs text-slate-500 mb-5 leading-relaxed">Digite o código do grupo para participar do passeio.</p>
          {msgError && !selectedGrupo && <div className="mb-4 p-3 bg-rose-100 text-rose-800 rounded-xl text-xs font-semibold text-center">{msgError}</div>}
          
          <form onSubmit={handleEntrar} className="space-y-3">
            <div>
              <label className="block text-[11px] font-bold text-slate-500 uppercase mb-1">Código do Grupo</label>
              <input type="text" value={codigoEntrar} onChange={e => setCodigoEntrar(e.target.value.toUpperCase())} placeholder="01BEB3" required className="w-full p-3 bg-[#f5f7fa] rounded-xl border border-slate-200 text-sm font-bold tracking-widest text-[#1a535c] uppercase focus:outline-none focus:border-[#4ecdc4]" />
            </div>
            <button type="submit" disabled={loading} className="w-full bg-[#4ecdc4] hover:bg-[#4ecdc4]/90 disabled:opacity-60 text-white font-bold py-3 rounded-xl text-xs transition shadow-md flex items-center justify-center gap-2">
              <span>{loading ? 'Acessando...' : 'Entrar no Grupo'}</span><ArrowRight className="w-4 h-4" />
            </button>
          </form>
        </div>

        <div className="lg:col-span-2 space-y-4">
          <h2 className="text-xl font-extrabold text-[#1a535c] flex items-center gap-2"><Sparkles className="w-5 h-5 text-[#ff6b6b]" /> Seus Passeios Agendados</h2>
          {loadingGrupos ? (
            <div className="bg-white rounded-3xl p-8 text-center shadow-md border border-slate-100">
              <div className="w-8 h-8 border-3 border-[#4ecdc4] border-t-transparent rounded-full animate-spin mx-auto mb-2" />
              <p className="text-xs text-slate-400">Carregando seus grupos...</p>
            </div>
          ) : meusGrupos.length > 0 ? (
            <div className="space-y-4">
              {meusGrupos.map(g => (
                <div key={g.idGrupo} onClick={() => abrirDetalhesGrupo(g)} className="bg-white rounded-2xl p-5 shadow-md border border-slate-100 hover:shadow-xl hover:border-[#4ecdc4]/40 transition-all cursor-pointer group flex flex-col sm:flex-row items-start sm:items-center justify-between gap-4">
                  <div className="flex items-center gap-4">
                    <img src={getPasseioImageUrl(g.passeio?.imagemUrl)} alt={g.nomeGrupo} className="w-16 h-16 rounded-2xl object-cover shrink-0 group-hover:scale-105 transition-transform" />
                    <div>
                      <div className="flex items-center gap-2"><span className="bg-[#4ecdc4]/15 text-[#1a535c] font-bold text-[10px] px-2.5 py-0.5 rounded-full uppercase tracking-wider">Código: {g.codigoConvite}</span></div>
                      <h3 className="font-extrabold text-[#1a535c] text-base group-hover:text-[#4ecdc4] transition mt-1">{g.nomeGrupo}</h3>
                      {g.passeio && <p className="text-xs text-slate-500 font-medium flex items-center gap-1 mt-0.5"><MapPin className="w-3.5 h-3.5 text-[#ff6b6b]" /><span>{g.passeio.nome}</span></p>}
                      <div className="flex flex-wrap items-center gap-3 text-xs text-slate-400 mt-2 font-semibold">
                        {g.dataInicio && (
                          <span className="flex items-center gap-1 text-[#1a535c]">
                            <Calendar className="w-3.5 h-3.5 text-[#4ecdc4]" />{formatarData(g.dataInicio)} às {formatarHorario(g.dataInicio)}
                          </span>
                        )}
                        {g.dataFim && (
                          <span className="flex items-center gap-1 text-[#ff6b6b]">
                            Até {formatarHorario(g.dataFim)}
                          </span>
                        )}
                      </div>
                    </div>
                  </div>
                  <button onClick={e => { e.stopPropagation(); abrirDetalhesGrupo(g); }} className="bg-[#1a535c] group-hover:bg-[#4ecdc4] text-white font-bold px-4 py-2.5 rounded-xl text-xs transition flex items-center gap-1.5 self-end sm:self-auto shrink-0"><Play className="w-3.5 h-3.5 fill-current" /><span>Ver Detalhes</span></button>
                </div>
              ))}
            </div>
          ) : (
            <div className="bg-white rounded-3xl p-8 text-center shadow-md border border-slate-100">
              <Users className="w-10 h-10 text-slate-300 mx-auto mb-3" />
              <p className="text-sm font-semibold text-slate-600 mb-1">Nenhum passeio em grupo cadastrado</p>
              <p className="text-xs text-slate-400">Crie um grupo ou entre usando um código de convite.</p>
            </div>
          )}
        </div>
      </div>

      {selectedGrupo && (
        <div className="fixed inset-0 z-[100] bg-slate-900/60 backdrop-blur-sm flex items-center justify-center p-4 overflow-y-auto">
          <div className="bg-white rounded-3xl max-w-lg w-full shadow-2xl border border-slate-100 relative overflow-hidden max-h-[calc(100vh-2rem)] flex flex-col my-auto">
            <button onClick={() => { if (leaving || canceling || starting) return; setSelectedGrupo(null); setGrupoDetalhes(null); setConfirmCancel(false); setConfirmLeave(false); setMsgError(''); setCopiedCode(false); }} className="absolute top-4 right-4 z-20 p-2 bg-black/40 hover:bg-black/60 text-white rounded-full transition"><X className="w-5 h-5" /></button>

            <div className="relative h-48 sm:h-56 overflow-hidden shrink-0">
              <img src={getPasseioImageUrl(selectedGrupo.passeio?.imagemUrl)} alt={selectedGrupo.nomeGrupo} className="w-full h-full object-cover" />
              <div className="absolute inset-0 bg-gradient-to-t from-slate-950/80 via-slate-950/20 to-transparent" />
              <div className="absolute bottom-4 left-6 right-6 text-white">
                <span className="bg-[#4ecdc4] text-white text-[10px] font-extrabold px-3 py-1 rounded-full uppercase tracking-wider mb-2 inline-block">
                  {grupoDetalhes && usuarioEhCriador() ? 'Você é o criador' : 'Passeio Agendado'}
                </span>
                <h2 className="text-2xl font-extrabold leading-tight">{selectedGrupo.nomeGrupo}</h2>
                {selectedGrupo.passeio && <p className="text-xs text-slate-200 mt-1 flex items-center gap-1"><MapPin className="w-3.5 h-3.5 text-[#ff6b6b]" />{selectedGrupo.passeio.nome}</p>}
              </div>
            </div>

            <div className="p-6 space-y-5 overflow-y-auto flex-1 min-h-0">
              {loadingDetalhes ? (
                <div className="py-8 text-center">
                  <div className="w-8 h-8 border-3 border-[#4ecdc4] border-t-transparent rounded-full animate-spin mx-auto mb-3" />
                  <p className="text-xs text-slate-400">Carregando detalhes...</p>
                </div>
              ) : (
                <>
                  {msgError && <div className="p-3 bg-rose-100 text-rose-800 rounded-xl text-xs font-semibold text-center">{msgError}</div>}

                  <div className="grid grid-cols-2 gap-3 bg-[#f5f7fa] p-3.5 rounded-2xl border border-slate-200">
                    <div>
                      <span className="text-[10px] font-bold text-slate-400 uppercase">Código do Grupo</span>
                      <div className="flex items-center gap-2 mt-0.5">
                        <span className="text-sm font-extrabold text-[#1a535c] tracking-widest">{selectedGrupo.codigoConvite}</span>
                        <button onClick={() => handleCopyCode(selectedGrupo.codigoConvite)} className="p-1 text-slate-400 hover:text-[#4ecdc4]">{copiedCode ? <Check className="w-4 h-4 text-emerald-500" /> : <Copy className="w-4 h-4" />}</button>
                      </div>
                    </div>
                    <div className="border-l border-slate-200 pl-3">
                      <span className="text-[10px] font-bold text-slate-400 uppercase">Previsão</span>
                      <span className="text-xs font-bold text-[#1a535c] mt-0.5 block">
                        {selectedGrupo.dataInicio ? `${formatarData(selectedGrupo.dataInicio)} às ${formatarHorario(selectedGrupo.dataInicio)}` : 'A definir'}
                      </span>
                      {selectedGrupo.dataFim && (
                        <span className="text-xs font-bold text-[#ff6b6b] mt-0.5 block">
                          Término: {formatarHorario(selectedGrupo.dataFim)}
                        </span>
                      )}
                      {grupoDetalhes && usuarioEhCriador() && (
                        <button type="button" onClick={iniciarEdicaoData} className="mt-2 inline-flex items-center gap-1 text-[11px] font-bold text-[#4ecdc4] hover:text-[#1a535c] transition"><Clock className="w-3.5 h-3.5" /> Alterar Início</button>
                      )}
                    </div>
                  </div>

                  {editandoData && (
                    <div className="bg-[#f5f7fa] p-4 rounded-2xl border border-slate-200 space-y-3">
                      <span className="text-[10px] font-bold text-slate-400 uppercase flex items-center gap-1.5"><Calendar className="w-4 h-4 text-[#4ecdc4]" /> Remarcar Início</span>
                      <div className="space-y-2">
                        <div><label className="block text-[11px] font-bold text-slate-500 uppercase mb-1">Data</label><input type="date" value={novaData} onChange={e => setNovaData(e.target.value)} min={new Date().toISOString().split('T')[0]} className="w-full p-3 bg-white rounded-xl border border-slate-200 text-sm font-semibold text-[#1a535c] focus:outline-none focus:border-[#4ecdc4]" /></div>
                        <div><label className="block text-[11px] font-bold text-slate-500 uppercase mb-1">Horário</label><input type="time" value={novoHorario} onChange={e => setNovoHorario(e.target.value)} className="w-full p-3 bg-white rounded-xl border border-slate-200 text-sm font-semibold text-[#1a535c] focus:outline-none focus:border-[#4ecdc4]" /></div>
                        {errorData && <div className="text-[11px] font-semibold text-rose-600">{errorData}</div>}
                      </div>
                      <div className="grid grid-cols-2 gap-2">
                        <button type="button" onClick={() => { setEditandoData(false); setErrorData(''); }} disabled={alterandoData} className="bg-slate-200 hover:bg-slate-300 text-slate-700 font-bold py-2.5 rounded-xl text-xs transition">Cancelar</button>
                        <button type="button" onClick={() => handleAlterarData(selectedGrupo.idGrupo)} disabled={alterandoData} className="bg-[#4ecdc4] hover:bg-[#4ecdc4]/90 disabled:opacity-60 text-white font-bold py-2.5 rounded-xl text-xs flex items-center justify-center gap-1.5 transition"><Calendar className="w-4 h-4" />{alterandoData ? 'Salvando...' : 'Salvar Data'}</button>
                      </div>
                    </div>
                  )}

                  <div className="bg-slate-50 rounded-2xl p-3 border border-slate-200">
                    <span className="text-[10px] font-bold text-slate-400 uppercase">Status</span>
                    <div className="mt-1 font-extrabold text-[#1a535c] text-sm">{selectedGrupo.status}</div>
                  </div>

                  {grupoDetalhes && (
                    <div className="bg-[#4ecdc4]/10 rounded-2xl p-3 border border-[#4ecdc4]/20">
                      <div className="flex items-center justify-between gap-3">
                        <div><span className="text-[10px] font-bold text-slate-400 uppercase">Você</span><div className="mt-1 font-extrabold text-[#1a535c] text-sm">{usuarioEhCriador() ? 'Criador do grupo' : 'Participante'}</div></div>
                        <Users className="w-5 h-5 text-[#4ecdc4]" />
                      </div>
                    </div>
                  )}

                  {confirmLeave ? (
                    <div className="bg-amber-50 border border-amber-200 rounded-2xl p-4 text-center space-y-3">
                      <div className="flex items-center justify-center gap-2 text-amber-600 font-bold text-sm"><AlertTriangle className="w-5 h-5" /><span>Deseja sair deste grupo?</span></div>
                      <p className="text-xs text-slate-600">Você deixará de participar deste passeio. O grupo continuará ativo para os outros integrantes.</p>
                      <div className="grid grid-cols-2 gap-2">
                        <button onClick={() => setConfirmLeave(false)} disabled={leaving} className="bg-slate-200 hover:bg-slate-300 text-slate-700 font-bold py-2.5 rounded-xl text-xs transition">Voltar</button>
                        <button onClick={() => handleSairGrupo(selectedGrupo.idGrupo)} disabled={leaving} className="bg-amber-500 hover:bg-amber-600 disabled:opacity-60 text-white font-bold py-2.5 rounded-xl text-xs flex items-center justify-center gap-1.5 transition"><LogOut className="w-4 h-4" />{leaving ? 'Saindo...' : 'Sim, Sair'}</button>
                      </div>
                    </div>
                  ) : confirmCancel ? (
                    <div className="bg-rose-50 border border-rose-200 rounded-2xl p-4 text-center space-y-3">
                      <div className="flex items-center justify-center gap-2 text-rose-600 font-bold text-sm"><AlertTriangle className="w-5 h-5" /><span>Cancelar este passeio?</span></div>
                      <p className="text-xs text-slate-600">Esta ação cancelará o grupo inteiro. Os demais integrantes não poderão continuar neste passeio.</p>
                      <div className="grid grid-cols-2 gap-2">
                        <button onClick={() => setConfirmCancel(false)} disabled={canceling} className="bg-slate-200 hover:bg-slate-300 text-slate-700 font-bold py-2.5 rounded-xl text-xs transition">Voltar</button>
                        <button onClick={() => handleCancelarPasseio(selectedGrupo.idGrupo)} disabled={canceling} className="bg-rose-600 hover:bg-rose-700 disabled:opacity-60 text-white font-bold py-2.5 rounded-xl text-xs flex items-center justify-center gap-1.5 transition"><Trash2 className="w-4 h-4" />{canceling ? 'Cancelando...' : 'Sim, Cancelar'}</button>
                      </div>
                    </div>
                  ) : (
                    <div className="space-y-2">
                      <button onClick={() => handleIniciarPasseio(selectedGrupo.idGrupo)} disabled={starting || selectedGrupo.status === 'FINALIZADO'} className="w-full bg-[#4ecdc4] hover:bg-[#4ecdc4]/90 disabled:opacity-60 text-white font-extrabold py-3.5 rounded-2xl text-sm transition shadow-lg flex items-center justify-center gap-2"><Play className="w-5 h-5 fill-current" /><span>{starting ? 'Iniciando passeio...' : 'Iniciar Passeio'}</span></button>
                      {grupoDetalhes && !usuarioEhCriador() && <button onClick={() => setConfirmLeave(true)} disabled={leaving || starting} className="w-full bg-amber-50 hover:bg-amber-100 disabled:opacity-60 text-amber-600 border border-amber-200 font-bold py-3 rounded-2xl text-xs transition flex items-center justify-center gap-2"><LogOut className="w-4 h-4" /><span>Sair do Grupo</span></button>}
                      {grupoDetalhes && usuarioEhCriador() && <button onClick={() => setConfirmCancel(true)} disabled={canceling || starting} className="w-full bg-rose-50 hover:bg-rose-100 disabled:opacity-60 text-rose-600 border border-rose-200 font-bold py-3 rounded-2xl text-xs transition flex items-center justify-center gap-2"><Trash2 className="w-4 h-4" /><span>Cancelar Passeio</span></button>}
                    </div>
                  )}
                </>
              )}
            </div>
          </div>
        </div>
      )}
    </div>
  );
};