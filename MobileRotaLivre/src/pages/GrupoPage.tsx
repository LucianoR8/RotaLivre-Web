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

  // =========================================================
  // ESTADOS
  // =========================================================

  const [codigoEntrar, setCodigoEntrar] = useState('');

  const [loading, setLoading] = useState(false);

  const [msgError, setMsgError] = useState('');

  const [meusGrupos, setMeusGrupos] =
    useState<PasseioPendenteDto[]>([]);

  const [loadingGrupos, setLoadingGrupos] =
    useState(true);

  const [selectedGrupo, setSelectedGrupo] =
    useState<PasseioPendenteDto | null>(null);

  const [grupoDetalhes, setGrupoDetalhes] =
    useState<GrupoDetalhesDto | null>(null);

  const [loadingDetalhes, setLoadingDetalhes] =
    useState(false);

  const [copiedCode, setCopiedCode] =
    useState(false);

  const [confirmLeave, setConfirmLeave] =
    useState(false);

  const [confirmCancel, setConfirmCancel] =
    useState(false);

  const [leaving, setLeaving] =
    useState(false);

  const [canceling, setCanceling] =
    useState(false);

  const [starting, setStarting] =
    useState(false);

  const [editandoData, setEditandoData] =
    useState(false);

  const [novaData, setNovaData] =
    useState('');

  const [novoHorario, setNovoHorario] =
    useState('');

  const [alterandoData, setAlterandoData] =
    useState(false);

  const [errorData, setErrorData] =
    useState('');


  // =========================================================
  // IDENTIFICAÇÃO DO USUÁRIO
  // =========================================================

  const getUsuarioId = (): number | null => {
    if (!usuario) return null;

    const id =
      (usuario as any).id_usuario ??
      (usuario as any).idUsuario ??
      (usuario as any).id;

    if (id === undefined || id === null) {
      return null;
    }

    const numero = Number(id);

    return Number.isNaN(numero)
      ? null
      : numero;
  };


  // =========================================================
  // VERIFICA SE USUÁRIO É CRIADOR
  // =========================================================

  const usuarioEhCriador = (): boolean => {
    const usuarioId = getUsuarioId();

    if (
      usuarioId === null ||
      !grupoDetalhes
    ) {
      return false;
    }

    return (
      Number(grupoDetalhes.criadorId) ===
      Number(usuarioId)
    );
  };


  // =========================================================
  // CARREGAR MEUS GRUPOS
  // =========================================================

  const carregarMeusGrupos = async () => {
    console.log('====================================');
    console.log(
      '[GrupoPage] INICIANDO carregarMeusGrupos'
    );
    console.log('====================================');

    setLoadingGrupos(true);

    try {
      const data =
        await buscarMeusPendentes(
          getAuthHeader
        );

      console.log(
        '[GrupoPage] Grupos recebidos:',
        data
      );

      console.log(
        '[GrupoPage] Quantidade:',
        data.length
      );

      setMeusGrupos(data);

    } catch (err) {
      console.error(
        '[GrupoPage] ERRO AO CARREGAR GRUPOS:',
        err
      );

      setMeusGrupos([]);

    } finally {
      setLoadingGrupos(false);

      console.log(
        '[GrupoPage] FINALIZOU carregarMeusGrupos'
      );

      console.log(
        '===================================='
      );
    }
  };


  // =========================================================
  // CARREGAMENTO INICIAL
  // =========================================================

  useEffect(() => {
    carregarMeusGrupos();
  }, []);


  // =========================================================
  // ABRIR DETALHES DO GRUPO
  // =========================================================

  const abrirDetalhesGrupo = async (
    grupo: PasseioPendenteDto
  ) => {
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
      console.log(
        '[GrupoPage] Buscando detalhes do grupo:',
        grupo.idGrupo
      );

      const detalhes =
        await buscarGrupo(
          grupo.idGrupo,
          getAuthHeader
        );

      console.log(
        '[GrupoPage] Detalhes recebidos:',
        detalhes
      );

      setGrupoDetalhes(detalhes);

    } catch (err) {
      console.error(
        '[GrupoPage] Erro ao carregar detalhes:',
        err
      );

      setMsgError(
        'Não foi possível carregar os detalhes do grupo.'
      );

    } finally {
      setLoadingDetalhes(false);
    }
  };


  // =========================================================
  // ENTRAR NO GRUPO
  // =========================================================

  const handleEntrar = async (e: React.FormEvent) => {
    e.preventDefault();

    const codigo = codigoEntrar.trim().toUpperCase();

    if (!codigo) return;

    console.log('====================================');
    console.log('[GrupoPage] TENTANDO ENTRAR NO GRUPO');
    console.log('[GrupoPage] Código:', codigo);
    console.log('====================================');

    setLoading(true);
    setMsgError('');

    try {
      const headers = {
        'Content-Type': 'application/json',
        ...getAuthHeader()
      };

      const body = {
        codigoConvite: codigo
      };

      console.log('[GrupoPage] POST /api/grupo/entrar');
      console.log('[GrupoPage] Body:', body);
      console.log('[GrupoPage] Headers:', headers);

      const res = await fetch('/api/grupo/entrar', {
        method: 'POST',
        headers,
        body: JSON.stringify(body)
      });

      console.log(
        '[GrupoPage] Status entrar:',
        res.status
      );

      console.log(
        '[GrupoPage] OK entrar:',
        res.ok
      );

      const text = await res.text();

      console.log(
        '[GrupoPage] Resposta entrar:',
        text
      );

      let data: any = null;

      if (text) {
        try {
          data = JSON.parse(text);
        } catch {
          console.warn(
            '[GrupoPage] Resposta não é JSON:',
            text
          );
        }
      }

      console.log(
        '[GrupoPage] Dados entrar:',
        data
      );

      if (!res.ok) {
        const mensagem =
          data?.mensagem ||
          data?.title ||
          text ||
          'Não foi possível entrar no grupo.';

        console.error(
          '[GrupoPage] API retornou erro:',
          res.status,
          mensagem
        );

        setMsgError(mensagem);
        return;
      }

      console.log(
        '[GrupoPage] ENTRADA NO GRUPO REALIZADA COM SUCESSO'
      );

      setCodigoEntrar('');

      if (data?.idGrupo) {
        try {
          const grupo = await buscarGrupo(
            Number(data.idGrupo),
            getAuthHeader
          );

          console.log(
            '[GrupoPage] Detalhes do grupo:',
            grupo
          );

          const grupoConvertido: PasseioPendenteDto = {
            idGrupo: grupo.idGrupo,

            idPasseio: grupo.idPasseio,

            nomeGrupo: grupo.nome,

            codigoConvite: grupo.codigoConvite,

            status: grupo.status,

            dataInicio: grupo.dataInicio,

            criadorId: grupo.criadorId,

            passeio: grupo.passeio
              ? {
                  id: grupo.passeio.id,
                  nome: grupo.passeio.nome,
                  descricao: grupo.passeio.descricao,
                  imagemUrl: grupo.passeio.imagemUrl
                }
              : {
                  id: grupo.idPasseio,
                  nome: 'Passeio',
                  descricao: '',
                  imagemUrl: ''
                }
          };

          console.log(
            '[GrupoPage] Grupo convertido:',
            grupoConvertido
          );

          setSelectedGrupo(
            grupoConvertido
          );

          setGrupoDetalhes(
            grupo
          );

          await carregarMeusGrupos();

        } catch (erro) {
          console.error(
            '[GrupoPage] Erro ao carregar detalhes do grupo:',
            erro
          );

          await carregarMeusGrupos();
        }

      } else {
        await carregarMeusGrupos();
      }

    } catch (err) {
      console.error(
        '[GrupoPage] Erro de conexão ao entrar:',
        err
      );

      setMsgError(
        'Não foi possível conectar ao servidor.'
      );

    } finally {
      setLoading(false);
    }
  };


  // =========================================================
  // COPIAR CÓDIGO
  // =========================================================

  const handleCopyCode = async (
    codigo: string
  ) => {
    try {
      await navigator.clipboard.writeText(
        codigo
      );

      setCopiedCode(true);

      setTimeout(() => {
        setCopiedCode(false);
      }, 2000);

    } catch (err) {
      console.error(
        'Erro ao copiar código:',
        err
      );
    }
  };


  // =========================================================
  // INICIAR PASSEIO
  // =========================================================

  const handleIniciarPasseio = async (
    grupoId: number
  ) => {
    if (starting) return;

    setStarting(true);

    setMsgError('');

    try {
      const resposta =
        await iniciarPasseio(
          grupoId,
          getAuthHeader
        );

      console.log(
        'Passeio iniciado:',
        resposta
      );

      sessionStorage.setItem(
        'activeLiveGrupoId',
        String(grupoId)
      );

      setSelectedGrupo(null);

      setGrupoDetalhes(null);

      setConfirmCancel(false);

      setConfirmLeave(false);

      navigate(
        `/ao-vivo?grupoId=${grupoId}`
      );

    } catch (err) {
      console.error(
        'Erro ao iniciar passeio:',
        err
      );

      setMsgError(
        err instanceof Error
          ? err.message
          : 'Não foi possível iniciar o passeio.'
      );

    } finally {
      setStarting(false);
    }
  };


  // =========================================================
  // SAIR DO GRUPO
  // =========================================================
  //
  // IMPORTANTE:
  // grupoService usa POST /api/grupo/{id}/sair
  // =========================================================

  const handleSairGrupo = async (
    grupoId: number
  ) => {
    if (leaving) return;

    setLeaving(true);

    setMsgError('');

    try {
      console.log(
        '[GrupoPage] Saindo do grupo:',
        grupoId
      );

      const data = await sairDoGrupo(
        grupoId,
        getAuthHeader
      );

      console.log(
        '[GrupoPage] Resposta sair:',
        data
      );

      console.log(
        '[GrupoPage] Saiu do grupo com sucesso.'
      );

      setSelectedGrupo(null);

      setGrupoDetalhes(null);

      setConfirmLeave(false);

      setConfirmCancel(false);

      await carregarMeusGrupos();

    } catch (err) {
      console.error(
        '[GrupoPage] Erro ao sair do grupo:',
        err
      );

      setMsgError(
        err instanceof Error
          ? err.message
          : 'Não foi possível sair do grupo.'
      );

    } finally {
      setLeaving(false);
    }
  };


  // =========================================================
  // CANCELAR PASSEIO
  // =========================================================
  //
  // IMPORTANTE:
  // grupoService usa POST /api/grupo/{id}/cancelar
  // =========================================================

  const handleCancelarPasseio = async (
    grupoId: number
  ) => {
    if (canceling) return;

    setCanceling(true);

    setMsgError('');

    try {
      console.log(
        '[GrupoPage] Cancelando grupo:',
        grupoId
      );

      const data = await cancelarGrupo(
        grupoId,
        getAuthHeader
      );

      console.log(
        '[GrupoPage] Resposta cancelar:',
        data
      );

      console.log(
        '[GrupoPage] Grupo cancelado com sucesso.'
      );

      setSelectedGrupo(null);

      setGrupoDetalhes(null);

      setConfirmCancel(false);

      setConfirmLeave(false);

      await carregarMeusGrupos();

    } catch (err) {
      console.error(
        '[GrupoPage] Erro ao cancelar passeio:',
        err
      );

      setMsgError(
        err instanceof Error
          ? err.message
          : 'Erro ao cancelar passeio.'
      );

    } finally {
      setCanceling(false);
    }
  };


  // =========================================================
  // INICIAR EDIÇÃO DA DATA
  // =========================================================

  const iniciarEdicaoData = () => {
    setErrorData('');

    if (selectedGrupo?.dataInicio) {
      const d = new Date(selectedGrupo.dataInicio);

      setNovaData(
        d.toISOString().split('T')[0]
      );

      setNovoHorario(
        `${String(d.getHours()).padStart(2, '0')}:${String(
          d.getMinutes()
        ).padStart(2, '0')}`
      );
    } else {
      setNovaData('');
      setNovoHorario('');
    }

    setEditandoData(true);
  };


  // =========================================================
  // ALTERAR DATA DO PASSEIO
  // =========================================================

  const handleAlterarData = async (
    grupoId: number
  ) => {
    if (alterandoData) return;

    if (!novaData || !novoHorario) {
      setErrorData(
        'Selecione a data e o horário do passeio.'
      );
      return;
    }

    const dataInicio = `${novaData}T${novoHorario}:00`;

    setAlterandoData(true);

    setErrorData('');

    try {
      console.log(
        '[GrupoPage] Alterando data do grupo:',
        grupoId,
        dataInicio
      );

      const data = await alterarDataGrupo(
        grupoId,
        dataInicio,
        getAuthHeader
      );

      console.log(
        '[GrupoPage] Resposta alterar data:',
        data
      );

      // Atualiza a data na lista e nos detalhes exibidos.
      setSelectedGrupo(prev =>
        prev
          ? {
              ...prev,
              dataInicio: data?.dataInicio ?? dataInicio
            }
          : prev
      );

      setGrupoDetalhes(prev =>
        prev
          ? {
              ...prev,
              dataInicio: data?.dataInicio ?? dataInicio
            }
          : prev
      );

      await carregarMeusGrupos();

      setEditandoData(false);

    } catch (err) {
      console.error(
        '[GrupoPage] Erro ao alterar data:',
        err
      );

      setErrorData(
        err instanceof Error
          ? err.message
          : 'Erro ao alterar a data do passeio.'
      );

    } finally {
      setAlterandoData(false);
    }
  };


  // =========================================================
  // IMAGEM DO PASSEIO
  // =========================================================

  const getPasseioImageUrl = (
    url?: string
  ) => {
    if (!url) {
      return 'https://images.unsplash.com/photo-1519331379826-f10be5486c6f?w=800';
    }

    if (url.startsWith('http')) {
      return url;
    }

    return `/img/passeios/${url}`;
  };


  // =========================================================
  // FORMATADORES
  // =========================================================

  const formatarData = (
    data?: string
  ) => {
    if (!data) {
      return 'Data a definir';
    }

    return new Date(
      data
    ).toLocaleDateString(
      'pt-BR'
    );
  };


  const formatarHorario = (
    data?: string
  ) => {
    if (!data) return '';

    return new Date(
      data
    ).toLocaleTimeString(
      'pt-BR',
      {
        hour: '2-digit',
        minute: '2-digit'
      }
    );
  };


  // =========================================================
  // RENDER
  // =========================================================

  return (
    <div className="pt-20 pb-24 max-w-md mx-auto px-4">

      {/* =====================================================
          CABEÇALHO
      ====================================================== */}

      <div className="text-center mb-10">

        <div className="inline-flex p-3 bg-[#4ecdc4]/15 rounded-2xl text-[#1a535c] mb-3">

          <Users className="w-8 h-8 text-[#4ecdc4]" />

        </div>

        <h1 className="text-3xl font-extrabold text-[#1a535c]">
          Meus Grupos e Passeios
        </h1>

        <p className="text-sm text-slate-500 mt-1">
          Acompanhe os passeios que você criou ou participa,
          ou entre em um novo grupo usando o código de convite.
        </p>

      </div>


      {/* =====================================================
          CONTEÚDO
      ====================================================== */}

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-8 items-start">


        {/* ===================================================
            ENTRAR NO GRUPO
        ==================================================== */}

        <div className="lg:col-span-1 bg-white rounded-3xl p-6 shadow-xl border border-slate-100">

          <div className="flex items-center gap-3 mb-3">

            <div className="p-2.5 bg-[#4ecdc4] text-white rounded-xl">

              <KeyRound className="w-5 h-5" />

            </div>

            <h2 className="text-lg font-bold text-[#1a535c]">
              Entrar com Código
            </h2>

          </div>


          <p className="text-xs text-slate-500 mb-5 leading-relaxed">
            Digite o código do grupo para participar do passeio.
          </p>


          {msgError && !selectedGrupo && (

            <div className="mb-4 p-3 bg-rose-100 text-rose-800 rounded-xl text-xs font-semibold text-center">

              {msgError}

            </div>

          )}


          <form
            onSubmit={handleEntrar}
            className="space-y-3"
          >

            <div>

              <label className="block text-[11px] font-bold text-slate-500 uppercase mb-1">
                Código do Grupo
              </label>

              <input
                type="text"
                value={codigoEntrar}
                onChange={e =>
                  setCodigoEntrar(
                    e.target.value.toUpperCase()
                  )
                }
                placeholder="01BEB3"
                required
                className="w-full p-3 bg-[#f5f7fa] rounded-xl border border-slate-200 text-sm font-bold tracking-widest text-[#1a535c] uppercase focus:outline-none focus:border-[#4ecdc4]"
              />

            </div>


            <button
              type="submit"
              disabled={loading}
              className="w-full bg-[#4ecdc4] hover:bg-[#4ecdc4]/90 disabled:opacity-60 text-white font-bold py-3 rounded-xl text-xs transition shadow-md flex items-center justify-center gap-2"
            >

              <span>
                {loading
                  ? 'Acessando...'
                  : 'Entrar no Grupo'}
              </span>

              <ArrowRight className="w-4 h-4" />

            </button>

          </form>

        </div>


        {/* ===================================================
            LISTA DE GRUPOS
        ==================================================== */}

        <div className="lg:col-span-2 space-y-4">

          <h2 className="text-xl font-extrabold text-[#1a535c] flex items-center gap-2">

            <Sparkles className="w-5 h-5 text-[#ff6b6b]" />

            Seus Passeios Agendados

          </h2>


          {loadingGrupos ? (

            <div className="bg-white rounded-3xl p-8 text-center shadow-md border border-slate-100">

              <div className="w-8 h-8 border-3 border-[#4ecdc4] border-t-transparent rounded-full animate-spin mx-auto mb-2" />

              <p className="text-xs text-slate-400">
                Carregando seus grupos...
              </p>

            </div>

          ) : meusGrupos.length > 0 ? (

            <div className="space-y-4">

              {meusGrupos.map(g => (

                <div
                  key={g.idGrupo}
                  onClick={() =>
                    abrirDetalhesGrupo(g)
                  }
                  className="bg-white rounded-2xl p-5 shadow-md border border-slate-100 hover:shadow-xl hover:border-[#4ecdc4]/40 transition-all cursor-pointer group flex flex-col sm:flex-row items-start sm:items-center justify-between gap-4"
                >

                  <div className="flex items-center gap-4">

                    <img
                      src={getPasseioImageUrl(
                        g.passeio?.imagemUrl
                      )}
                      alt={g.nomeGrupo}
                      className="w-16 h-16 rounded-2xl object-cover shrink-0 group-hover:scale-105 transition-transform"
                    />


                    <div>

                      <div className="flex items-center gap-2">

                        <span className="bg-[#4ecdc4]/15 text-[#1a535c] font-bold text-[10px] px-2.5 py-0.5 rounded-full uppercase tracking-wider">

                          Código: {g.codigoConvite}

                        </span>

                      </div>


                      <h3 className="font-extrabold text-[#1a535c] text-base group-hover:text-[#4ecdc4] transition mt-1">

                        {g.nomeGrupo}

                      </h3>


                      {g.passeio && (

                        <p className="text-xs text-slate-500 font-medium flex items-center gap-1 mt-0.5">

                          <MapPin className="w-3.5 h-3.5 text-[#ff6b6b]" />

                          <span>
                            {g.passeio.nome}
                          </span>

                        </p>

                      )}


                      <div className="flex flex-wrap items-center gap-3 text-xs text-slate-400 mt-2 font-semibold">

                        {g.dataInicio && (

                          <span className="flex items-center gap-1 text-[#1a535c]">

                            <Calendar className="w-3.5 h-3.5 text-[#4ecdc4]" />

                            {formatarData(
                              g.dataInicio
                            )}

                          </span>

                        )}


                        {g.dataInicio && (

                          <span className="flex items-center gap-1 text-[#1a535c]">

                            <Clock className="w-3.5 h-3.5 text-[#ff6b6b]" />

                            {formatarHorario(
                              g.dataInicio
                            )}

                          </span>

                        )}

                      </div>

                    </div>

                  </div>


                  <button
                    onClick={e => {
                      e.stopPropagation();

                      abrirDetalhesGrupo(g);
                    }}
                    className="bg-[#1a535c] group-hover:bg-[#4ecdc4] text-white font-bold px-4 py-2.5 rounded-xl text-xs transition flex items-center gap-1.5 self-end sm:self-auto shrink-0"
                  >

                    <Play className="w-3.5 h-3.5 fill-current" />

                    <span>
                      Ver Detalhes
                    </span>

                  </button>

                </div>

              ))}

            </div>

          ) : (

            <div className="bg-white rounded-3xl p-8 text-center shadow-md border border-slate-100">

              <Users className="w-10 h-10 text-slate-300 mx-auto mb-3" />

              <p className="text-sm font-semibold text-slate-600 mb-1">
                Nenhum passeio em grupo cadastrado
              </p>

              <p className="text-xs text-slate-400">
                Crie um grupo para um passeio ou entre usando um código de convite.
              </p>

            </div>

          )}

        </div>

      </div>


      {/* =====================================================
          MODAL
          
          CORREÇÃO:
          - z-[100] para ficar acima da tabbar
          - overflow-y-auto no overlay
          - max-h-[calc(100vh-2rem)] no card
      ====================================================== */}

      {selectedGrupo && (

        <div
          className="
            fixed
            inset-0
            z-[100]
            bg-slate-900/60
            backdrop-blur-sm
            flex
            items-center
            justify-center
            p-4
            overflow-y-auto
          "
        >

          <div
            className="
              bg-white
              rounded-3xl
              max-w-lg
              w-full
              shadow-2xl
              border
              border-slate-100
              relative
              overflow-hidden
              max-h-[calc(100vh-2rem)]
              flex
              flex-col
              my-auto
            "
          >


            {/* FECHAR */}

            <button
              onClick={() => {

                if (
                  leaving ||
                  canceling ||
                  starting
                ) {
                  return;
                }

                setSelectedGrupo(null);

                setGrupoDetalhes(null);

                setConfirmCancel(false);

                setConfirmLeave(false);

                setMsgError('');

                setCopiedCode(false);

              }}
              className="absolute top-4 right-4 z-20 p-2 bg-black/40 hover:bg-black/60 text-white rounded-full transition"
            >

              <X className="w-5 h-5" />

            </button>


            {/* =================================================
                IMAGEM
            ================================================== */}

            <div className="relative h-48 sm:h-56 overflow-hidden shrink-0">

              <img
                src={getPasseioImageUrl(
                  selectedGrupo.passeio?.imagemUrl
                )}
                alt={selectedGrupo.nomeGrupo}
                className="w-full h-full object-cover"
              />


              <div className="absolute inset-0 bg-gradient-to-t from-slate-950/80 via-slate-950/20 to-transparent" />


              <div className="absolute bottom-4 left-6 right-6 text-white">

                <span className="bg-[#4ecdc4] text-white text-[10px] font-extrabold px-3 py-1 rounded-full uppercase tracking-wider mb-2 inline-block">

                  {grupoDetalhes &&
                  usuarioEhCriador()
                    ? 'Você é o criador'
                    : 'Passeio Agendado'}

                </span>


                <h2 className="text-2xl font-extrabold leading-tight">

                  {selectedGrupo.nomeGrupo}

                </h2>


                {selectedGrupo.passeio && (

                  <p className="text-xs text-slate-200 mt-1 flex items-center gap-1">

                    <MapPin className="w-3.5 h-3.5 text-[#ff6b6b]" />

                    {selectedGrupo.passeio.nome}

                  </p>

                )}

              </div>

            </div>


            {/* =================================================
                CONTEÚDO COM ROLAGEM
            ================================================== */}

            <div className="p-6 space-y-5 overflow-y-auto flex-1 min-h-0">


              {loadingDetalhes ? (

                <div className="py-8 text-center">

                  <div className="w-8 h-8 border-3 border-[#4ecdc4] border-t-transparent rounded-full animate-spin mx-auto mb-3" />

                  <p className="text-xs text-slate-400">
                    Carregando detalhes...
                  </p>

                </div>

              ) : (

                <>


                  {/* =================================================
                      ERRO
                  ================================================== */}

                  {msgError && (

                    <div className="p-3 bg-rose-100 text-rose-800 rounded-xl text-xs font-semibold text-center">

                      {msgError}

                    </div>

                  )}


                  {/* =================================================
                      CÓDIGO E DATA
                  ================================================== */}

                  <div className="grid grid-cols-2 gap-3 bg-[#f5f7fa] p-3.5 rounded-2xl border border-slate-200">


                    <div>

                      <span className="text-[10px] font-bold text-slate-400 uppercase">
                        Código do Grupo
                      </span>


                      <div className="flex items-center gap-2 mt-0.5">

                        <span className="text-sm font-extrabold text-[#1a535c] tracking-widest">

                          {selectedGrupo.codigoConvite}

                        </span>


                        <button
                          onClick={() =>
                            handleCopyCode(
                              selectedGrupo.codigoConvite
                            )
                          }
                          className="p-1 text-slate-400 hover:text-[#4ecdc4]"
                        >

                          {copiedCode ? (

                            <Check className="w-4 h-4 text-emerald-500" />

                          ) : (

                            <Copy className="w-4 h-4" />

                          )}

                        </button>

                      </div>

                    </div>


                    <div className="border-l border-slate-200 pl-3">

                      <span className="text-[10px] font-bold text-slate-400 uppercase">
                        Data e Horário
                      </span>


                      {editandoData ? (

                        <>
                          <div className="mt-1.5 space-y-1.5">

                            <input
                              type="date"
                              value={novaData}
                              onChange={e =>
                                setNovaData(e.target.value)
                              }
                              min={new Date().toISOString().split('T')[0]}
                              className="w-full p-2 bg-[#f5f7fa] rounded-xl border border-slate-200 text-xs font-semibold text-[#1a535c] focus:outline-none focus:border-[#4ecdc4]"
                            />

                            <input
                              type="time"
                              value={novoHorario}
                              onChange={e =>
                                setNovoHorario(e.target.value)
                              }
                              className="w-full p-2 bg-[#f5f7fa] rounded-xl border border-slate-200 text-xs font-semibold text-[#1a535c] focus:outline-none focus:border-[#4ecdc4]"
                            />

                            {errorData && (
                              <div className="text-[10px] font-semibold text-rose-600">
                                {errorData}
                              </div>
                            )}

                            <div className="grid grid-cols-2 gap-1.5">

                              <button
                                type="button"
                                onClick={() => {
                                  setEditandoData(false);
                                  setErrorData('');
                                }}
                                disabled={alterandoData}
                                className="bg-slate-200 hover:bg-slate-300 text-slate-700 font-bold py-2 rounded-xl text-[11px] transition"
                              >
                                Cancelar
                              </button>

                              <button
                                type="button"
                                onClick={() =>
                                  handleAlterarData(
                                    selectedGrupo.idGrupo
                                  )
                                }
                                disabled={alterandoData}
                                className="bg-[#4ecdc4] hover:bg-[#4ecdc4]/90 disabled:opacity-60 text-white font-bold py-2 rounded-xl text-[11px] flex items-center justify-center gap-1 transition"
                              >
                                {alterandoData
                                  ? 'Salvando...'
                                  : 'Salvar Data'}
                              </button>

                            </div>

                          </div>
                        </>

                      ) : (

                        <>
                          <span className="text-xs font-bold text-[#1a535c] mt-0.5 block">

                            {selectedGrupo.dataInicio
                              ? new Date(
                                  selectedGrupo.dataInicio
                                ).toLocaleDateString(
                                  'pt-BR'
                                )
                              : 'Data a definir'}


                            {selectedGrupo.dataInicio &&
                              ` às ${formatarHorario(
                                selectedGrupo.dataInicio
                              )}`}

                          </span>


                          {grupoDetalhes &&
                          usuarioEhCriador() && (
                            <button
                              type="button"
                              onClick={iniciarEdicaoData}
                              className="mt-2 inline-flex items-center gap-1 text-[11px] font-bold text-[#4ecdc4] hover:text-[#1a535c] transition"
                            >
                              <Clock className="w-3.5 h-3.5" />
                              Alterar Data
                            </button>
                          )}

                        </>

                      )}

                    </div>

                  </div>


                  {/* =================================================
                      STATUS
                  ================================================== */}

                  <div className="bg-slate-50 rounded-2xl p-3 border border-slate-200">

                    <span className="text-[10px] font-bold text-slate-400 uppercase">
                      Status
                    </span>

                    <div className="mt-1 font-extrabold text-[#1a535c] text-sm">
                      {selectedGrupo.status}
                    </div>

                  </div>


                  {/* =================================================
                      QUEM É O CRIADOR
                  ================================================== */}

                  {grupoDetalhes && (

                    <div className="bg-[#4ecdc4]/10 rounded-2xl p-3 border border-[#4ecdc4]/20">

                      <div className="flex items-center justify-between gap-3">

                        <div>

                          <span className="text-[10px] font-bold text-slate-400 uppercase">
                            Você
                          </span>

                          <div className="mt-1 font-extrabold text-[#1a535c] text-sm">

                            {usuarioEhCriador()
                              ? 'Criador do grupo'
                              : 'Participante'}

                          </div>

                        </div>


                        <Users className="w-5 h-5 text-[#4ecdc4]" />

                      </div>

                    </div>

                  )}


                  {/* =================================================
                      CONFIRMAÇÃO DE SAIR
                  ================================================== */}

                  {confirmLeave ? (

                    <div className="bg-amber-50 border border-amber-200 rounded-2xl p-4 text-center space-y-3">

                      <div className="flex items-center justify-center gap-2 text-amber-600 font-bold text-sm">

                        <AlertTriangle className="w-5 h-5" />

                        <span>
                          Deseja sair deste grupo?
                        </span>

                      </div>


                      <p className="text-xs text-slate-600">

                        Você deixará de participar deste passeio.
                        O grupo continuará ativo para os outros integrantes.

                      </p>


                      <div className="grid grid-cols-2 gap-2">

                        <button
                          onClick={() =>
                            setConfirmLeave(false)
                          }
                          disabled={leaving}
                          className="bg-slate-200 hover:bg-slate-300 text-slate-700 font-bold py-2.5 rounded-xl text-xs transition"
                        >

                          Voltar

                        </button>


                        <button
                          onClick={() =>
                            handleSairGrupo(
                              selectedGrupo.idGrupo
                            )
                          }
                          disabled={leaving}
                          className="bg-amber-500 hover:bg-amber-600 disabled:opacity-60 text-white font-bold py-2.5 rounded-xl text-xs flex items-center justify-center gap-1.5 transition"
                        >

                          <LogOut className="w-4 h-4" />

                          {leaving
                            ? 'Saindo...'
                            : 'Sim, Sair'}

                        </button>

                      </div>

                    </div>


                  ) : confirmCancel ? (


                    /* =================================================
                       CONFIRMAÇÃO DE CANCELAMENTO
                    ================================================== */

                    <div className="bg-rose-50 border border-rose-200 rounded-2xl p-4 text-center space-y-3">

                      <div className="flex items-center justify-center gap-2 text-rose-600 font-bold text-sm">

                        <AlertTriangle className="w-5 h-5" />

                        <span>
                          Cancelar este passeio?
                        </span>

                      </div>


                      <p className="text-xs text-slate-600">

                        Esta ação cancelará o grupo inteiro.
                        Os demais integrantes não poderão continuar neste passeio.

                      </p>


                      <div className="grid grid-cols-2 gap-2">

                        <button
                          onClick={() =>
                            setConfirmCancel(false)
                          }
                          disabled={canceling}
                          className="bg-slate-200 hover:bg-slate-300 text-slate-700 font-bold py-2.5 rounded-xl text-xs transition"
                        >

                          Voltar

                        </button>


                        <button
                          onClick={() =>
                            handleCancelarPasseio(
                              selectedGrupo.idGrupo
                            )
                          }
                          disabled={canceling}
                          className="bg-rose-600 hover:bg-rose-700 disabled:opacity-60 text-white font-bold py-2.5 rounded-xl text-xs flex items-center justify-center gap-1.5 transition"
                        >

                          <Trash2 className="w-4 h-4" />

                          {canceling
                            ? 'Cancelando...'
                            : 'Sim, Cancelar'}

                        </button>

                      </div>

                    </div>


                  ) : (


                    /* =================================================
                       AÇÕES NORMAIS
                    ================================================== */

                    <div className="space-y-2">


                      {/* INICIAR */}

                      <button
                        onClick={() =>
                          handleIniciarPasseio(
                            selectedGrupo.idGrupo
                          )
                        }
                        disabled={
                          starting ||
                          selectedGrupo.status ===
                            'FINALIZADO'
                        }
                        className="w-full bg-[#4ecdc4] hover:bg-[#4ecdc4]/90 disabled:opacity-60 text-white font-extrabold py-3.5 rounded-2xl text-sm transition shadow-lg flex items-center justify-center gap-2"
                      >

                        <Play className="w-5 h-5 fill-current" />

                        <span>

                          {starting
                            ? 'Iniciando passeio...'
                            : 'Iniciar Passeio'}

                        </span>

                      </button>


                      {/* =================================================
                          PARTICIPANTE -> SAIR
                      ================================================== */}

                      {grupoDetalhes &&
                      !usuarioEhCriador() && (

                        <button
                          onClick={() =>
                            setConfirmLeave(true)
                          }
                          disabled={
                            leaving ||
                            starting
                          }
                          className="w-full bg-amber-50 hover:bg-amber-100 disabled:opacity-60 text-amber-600 border border-amber-200 font-bold py-3 rounded-2xl text-xs transition flex items-center justify-center gap-2"
                        >

                          <LogOut className="w-4 h-4" />

                          <span>
                            Sair do Grupo
                          </span>

                        </button>

                      )}


                      {/* =================================================
                          CRIADOR -> CANCELAR
                      ================================================== */}

                      {grupoDetalhes &&
                      usuarioEhCriador() && (

                        <button
                          onClick={() =>
                            setConfirmCancel(true)
                          }
                          disabled={
                            canceling ||
                            starting
                          }
                          className="w-full bg-rose-50 hover:bg-rose-100 disabled:opacity-60 text-rose-600 border border-rose-200 font-bold py-3 rounded-2xl text-xs transition flex items-center justify-center gap-2"
                        >

                          <Trash2 className="w-4 h-4" />

                          <span>
                            Cancelar Passeio
                          </span>

                        </button>

                      )}

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

