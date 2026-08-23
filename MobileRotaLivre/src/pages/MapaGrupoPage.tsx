import React, {
  useEffect,
  useRef,
  useState
} from 'react';

import {
  useNavigate,
  useParams,
  useSearchParams,
  Link
} from 'react-router-dom';

import { useAuth } from '../context/AuthContext';

import {
  GrupoDetalhesDto,
  buscarGrupo,
  iniciarPasseio
} from '../services/grupoService';

import {
  ArrowLeft,
  Users,
  Copy,
  Check,
  MapPin,
  RefreshCw,
  Radio,
  Play,
  Power,
  Clock,
  UserCheck,
  UserX,
  ChevronRight,
  CalendarDays
} from 'lucide-react';

import L from 'leaflet';

const API_BASE_URL = 'https://rotalivre-web.onrender.com';

// =========================================================
// TIPOS
// =========================================================

interface PasseioPendenteDto {
  idGrupo: number;
  idPasseio: number;
  nomeGrupo: string;
  codigoConvite: string;
  status: string;
  dataInicio?: string | null;

  passeio: {
    id: number;
    nome: string;
    descricao?: string | null;
    imagemUrl?: string | null;
  };
}

// =========================================================
// COMPONENTE
// =========================================================

export const MapaGrupoPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();

  const [searchParams] = useSearchParams();

  const navigate = useNavigate();

  const { getAuthHeader } = useAuth();

  // =======================================================
  // ESTADOS
  // =======================================================

  const [gruposAoVivo, setGruposAoVivo] = useState<
    PasseioPendenteDto[]
  >([]);

  const [grupo, setGrupo] =
    useState<GrupoDetalhesDto | null>(null);

  const [loading, setLoading] =
    useState(true);

  const [loadingGrupos, setLoadingGrupos] =
    useState(true);

  const [starting, setStarting] =
    useState(false);

  const [copied, setCopied] =
    useState(false);

  const [errorMsg, setErrorMsg] =
    useState('');

  const mapRef =
    useRef<HTMLDivElement>(null);

  const leafletMap =
    useRef<L.Map | null>(null);

  // =======================================================
  // ID DO GRUPO
  // =======================================================

  const getGrupoId = (): number | null => {
    const activeSessionId =
      sessionStorage.getItem(
        'activeLiveGrupoId'
      );

    const paramId =
      id ||
      searchParams.get('grupoId') ||
      activeSessionId;

    if (!paramId) {
      return null;
    }

    const numero = Number(paramId);

    return Number.isFinite(numero)
      ? numero
      : null;
  };

  // =======================================================
  // BUSCAR GRUPOS AO VIVO DO USUÁRIO
  // =======================================================

  const carregarGruposAoVivo = async () => {
    console.log('====================================');
    console.log('[AoVivo] BUSCANDO GRUPOS DO USUÁRIO');
    console.log('====================================');

    setLoadingGrupos(true);
    setErrorMsg('');

    try {
      const headers = getAuthHeader();

      console.log(
        '[AoVivo] GET:',
        `${API_BASE_URL}/api/grupo/meus-pendentes`
      );

      console.log(
        '[AoVivo] Headers:',
        headers
      );

      const response = await fetch(
        `${API_BASE_URL}/api/grupo/meus-pendentes`,
        {
          method: 'GET',
          headers
        }
      );

      console.log(
        '[AoVivo] Status:',
        response.status
      );

      console.log(
        '[AoVivo] OK:',
        response.ok
      );

      const text =
        await response.text();

      console.log(
        '[AoVivo] Resposta bruta:',
        text
      );

      if (!response.ok) {
        throw new Error(
          `Erro HTTP ${response.status}: ${text}`
        );
      }

      let data: PasseioPendenteDto[];

      try {
        data = text
          ? JSON.parse(text)
          : [];
      } catch (error) {
        console.error(
          '[AoVivo] Erro ao converter JSON:',
          error
        );

        throw new Error(
          'A API retornou uma resposta inválida.'
        );
      }

      console.log(
        '[AoVivo] Todos os grupos/passeios recebidos:',
        data
      );

      const lista =
        Array.isArray(data)
          ? data
          : [];

      const ativos =
        lista.filter(
          item =>
            item.status ===
            'EM_ANDAMENTO'
        );

      console.log(
        '[AoVivo] Grupos EM_ANDAMENTO:',
        ativos
      );

      setGruposAoVivo(ativos);

      return ativos;

    } catch (error) {
      console.error(
        '[AoVivo] ERRO AO BUSCAR GRUPOS:',
        error
      );

      setGruposAoVivo([]);

      setErrorMsg(
        error instanceof Error
          ? error.message
          : 'Não foi possível carregar os grupos ao vivo.'
      );

      return [];

    } finally {
      setLoadingGrupos(false);

      console.log(
        '[AoVivo] FINALIZOU BUSCA DOS GRUPOS'
      );
    }
  };

  // =======================================================
  // BUSCAR DETALHES DE UM GRUPO
  // =======================================================

  const carregarGrupo = async (
    grupoId?: number
  ) => {
    const idFinal =
      grupoId ?? getGrupoId();

    console.log(
      '[AoVivo] ID selecionado:',
      idFinal
    );

    if (!idFinal) {
      setGrupo(null);
      setLoading(false);
      return;
    }

    setLoading(true);
    setErrorMsg('');

    try {
      console.log(
        '[AoVivo] Buscando detalhes do grupo:',
        idFinal
      );

      const data =
        await buscarGrupo(
          idFinal,
          getAuthHeader
        );

      console.log(
        '[AoVivo] Detalhes recebidos:',
        data
      );

      setGrupo(data);

      sessionStorage.setItem(
        'activeLiveGrupoId',
        String(idFinal)
      );

    } catch (error) {
      console.error(
        '[AoVivo] ERRO AO BUSCAR GRUPO:',
        error
      );

      setGrupo(null);

      setErrorMsg(
        error instanceof Error
          ? error.message
          : 'Não foi possível carregar os dados do grupo.'
      );

    } finally {
      setLoading(false);
    }
  };

  // =======================================================
  // CARREGAMENTO INICIAL
  // =======================================================

  useEffect(() => {
    const inicializar = async () => {
      console.log(
        '[AoVivo] INICIANDO TELA'
      );

      const ativos =
        await carregarGruposAoVivo();

      const grupoId =
        getGrupoId();

      /*
       * Se veio um grupo específico pela URL
       * ou pelo sessionStorage, abrimos ele.
       */

      if (grupoId) {
        await carregarGrupo(
          grupoId
        );

        return;
      }

      /*
       * Se não veio grupo específico,
       * selecionamos automaticamente o
       * primeiro grupo EM_ANDAMENTO.
       */

      if (ativos.length > 0) {
        await carregarGrupo(
          ativos[0].idGrupo
        );
      } else {
        setLoading(false);
      }
    };

    inicializar();
  }, [id]);

  // =======================================================
  // ATUALIZAÇÃO AUTOMÁTICA
  // =======================================================

  useEffect(() => {
    const intervalo =
      setInterval(async () => {
        console.log(
          '[AoVivo] Atualizando grupos...'
        );

        const ativos =
          await carregarGruposAoVivo();

        if (!grupo) {
          if (ativos.length > 0) {
            await carregarGrupo(
              ativos[0].idGrupo
            );
          }

          return;
        }

        /*
         * Verifica se o grupo atual
         * ainda está em andamento.
         */

        const grupoAindaAtivo =
          ativos.some(
            item =>
              item.idGrupo ===
              grupo.idGrupo
          );

        if (!grupoAindaAtivo) {
          console.log(
            '[AoVivo] Grupo atual não está mais em andamento.'
          );

          setGrupo(null);

          sessionStorage.removeItem(
            'activeLiveGrupoId'
          );

          return;
        }

        try {
          const atualizado =
            await buscarGrupo(
              grupo.idGrupo,
              getAuthHeader
            );

          setGrupo(atualizado);

        } catch (error) {
          console.error(
            '[AoVivo] Erro ao atualizar detalhes:',
            error
          );
        }

      }, 5000);

    return () =>
      clearInterval(intervalo);

  }, [grupo?.idGrupo]);

  // =======================================================
  // MAPA
  // =======================================================

  useEffect(() => {
    if (
      !grupo ||
      !mapRef.current ||
      leafletMap.current
    ) {
      return;
    }

    console.log(
      '[AoVivo] Inicializando mapa.'
    );

    const map =
      L.map(mapRef.current)
        .setView(
          [-23.5874, -46.6576],
          13
        );

    L.tileLayer(
      'https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png',
      {
        attribution:
          '&copy; OpenStreetMap'
      }
    ).addTo(map);

    leafletMap.current = map;

    setTimeout(() => {
      map.invalidateSize();
    }, 100);

    return () => {
      if (leafletMap.current) {
        leafletMap.current.remove();
        leafletMap.current = null;
      }
    };

  }, [grupo?.idGrupo]);

  // =======================================================
  // INICIAR PASSEIO
  // =======================================================

  const handleIniciarPasseio =
    async () => {
      if (!grupo) {
        return;
      }

      setStarting(true);
      setErrorMsg('');

      console.log(
        '[AoVivo] Iniciando grupo:',
        grupo.idGrupo
      );

      try {
        const resposta =
          await iniciarPasseio(
            grupo.idGrupo,
            getAuthHeader
          );

        console.log(
          '[AoVivo] Resposta iniciar:',
          resposta
        );

        const grupoAtualizado =
          await buscarGrupo(
            grupo.idGrupo,
            getAuthHeader
          );

        console.log(
          '[AoVivo] Grupo após iniciar:',
          grupoAtualizado
        );

        setGrupo(
          grupoAtualizado
        );

        sessionStorage.setItem(
          'activeLiveGrupoId',
          String(grupo.idGrupo)
        );

        await carregarGruposAoVivo();

      } catch (error) {
        console.error(
          '[AoVivo] ERRO AO INICIAR:',
          error
        );

        setErrorMsg(
          error instanceof Error
            ? error.message
            : 'Não foi possível iniciar o passeio.'
        );

      } finally {
        setStarting(false);
      }
    };

  // =======================================================
  // COPIAR CÓDIGO
  // =======================================================

  const handleCopyCode =
    async () => {
      if (!grupo) {
        return;
      }

      try {
        await navigator.clipboard.writeText(
          grupo.codigoConvite
        );

        setCopied(true);

        setTimeout(() => {
          setCopied(false);
        }, 2000);

      } catch (error) {
        console.error(
          '[AoVivo] Erro ao copiar código:',
          error
        );
      }
    };

  // =======================================================
  // VOLTAR
  // =======================================================

  const handleVoltar =
    () => {
      sessionStorage.removeItem(
        'activeLiveGrupoId'
      );

      if (leafletMap.current) {
        leafletMap.current.remove();
        leafletMap.current = null;
      }

      navigate('/grupos');
    };

  // =======================================================
  // FORMATADORES
  // =======================================================

  const formatarData =
    (
      data?: string | null
    ) => {
      if (!data) {
        return 'Não definida';
      }

      const date =
        new Date(data);

      if (
        Number.isNaN(
          date.getTime()
        )
      ) {
        return data;
      }

      return date.toLocaleDateString(
        'pt-BR'
      );
    };

  const formatarDataHora =
    (
      data?: string | null
    ) => {
      if (!data) {
        return 'Não iniciado';
      }

      const date =
        new Date(data);

      if (
        Number.isNaN(
          date.getTime()
        )
      ) {
        return data;
      }

      return date.toLocaleString(
        'pt-BR'
      );
    };

  // =======================================================
  // LOADING
  // =======================================================

  if (
    loading &&
    loadingGrupos
  ) {
    return (
      <div className="pt-32 pb-28 flex justify-center items-center min-h-[60vh]">
        <div className="w-12 h-12 border-4 border-[#4ecdc4] border-t-transparent rounded-full animate-spin" />
      </div>
    );
  }

  // =======================================================
  // NENHUM GRUPO AO VIVO
  // =======================================================

  if (
    !loadingGrupos &&
    gruposAoVivo.length === 0 &&
    !grupo
  ) {
    return (
      <div className="pt-24 pb-28 max-w-lg mx-auto px-4">

        <div className="flex items-center gap-3 mb-8">

          <button
            onClick={() =>
              navigate('/grupos')
            }
            className="p-3 bg-white rounded-full text-[#1a535c] shadow-md hover:bg-[#4ecdc4] hover:text-white transition"
          >
            <ArrowLeft className="w-5 h-5" />
          </button>

          <div>
            <span className="text-xs font-bold uppercase tracking-wider text-[#ff6b6b]">
              Rota Livre
            </span>

            <h1 className="text-3xl font-extrabold text-[#1a535c]">
              Ao Vivo
            </h1>
          </div>

        </div>

        {errorMsg && (
          <div className="mb-6 p-4 bg-rose-100 border border-rose-200 text-rose-700 rounded-2xl text-xs font-semibold">
            {errorMsg}
          </div>
        )}

        <div className="bg-white rounded-3xl p-10 text-center shadow-xl border border-slate-100">

          <div className="w-16 h-16 mx-auto mb-5 rounded-full bg-[#4ecdc4]/15 flex items-center justify-center">
            <Radio className="w-8 h-8 text-[#4ecdc4]" />
          </div>

          <h2 className="text-xl font-extrabold text-[#1a535c] mb-2">
            Nenhum passeio ao vivo
          </h2>

          <p className="text-sm text-slate-500 mb-6">
            Você não possui nenhum grupo com passeio em andamento neste momento.
          </p>

          <button
            onClick={() =>
              navigate('/grupos')
            }
            className="inline-flex items-center gap-2 bg-[#1a535c] text-white px-6 py-3 rounded-full font-bold text-sm hover:bg-[#4ecdc4] transition"
          >
            <Users className="w-4 h-4" />
            Ver meus grupos
          </button>

        </div>

      </div>
    );
  }

  // =======================================================
  // TELA
  // =======================================================

  return (
    <div className="pt-20 pb-24 max-w-2xl mx-auto px-4">

      {/* ===================================================
          CABEÇALHO
      =================================================== */}

      <div className="flex items-center justify-between gap-3 mb-6">

        <button
          onClick={handleVoltar}
          className="p-3 bg-white rounded-full text-[#1a535c] hover:bg-[#4ecdc4] hover:text-white transition shadow-md flex items-center gap-2 font-semibold text-sm"
        >
          <ArrowLeft className="w-5 h-5" />
          <span>Grupos</span>
        </button>

        <button
          onClick={() =>
            carregarGruposAoVivo()
          }
          className="p-3 bg-white rounded-full text-[#1a535c] hover:bg-[#4ecdc4] hover:text-white transition shadow-md"
          title="Atualizar"
        >
          <RefreshCw className="w-5 h-5" />
        </button>

      </div>

      {/* ===================================================
          LISTA DE GRUPOS AO VIVO
      =================================================== */}

      {gruposAoVivo.length > 0 && (
        <div className="mb-6">

          <div className="flex items-center justify-between mb-3 px-1">

            <div>
              <h2 className="font-extrabold text-[#1a535c]">
                Passeios ao vivo
              </h2>

              <p className="text-xs text-slate-400">
                Grupos em andamento dos quais você participa
              </p>
            </div>

            <span className="bg-emerald-100 text-emerald-700 text-xs font-extrabold px-3 py-1 rounded-full">
              {gruposAoVivo.length}
            </span>

          </div>

          <div className="space-y-3">

            {gruposAoVivo.map(item => {

              const selecionado =
                grupo?.idGrupo ===
                item.idGrupo;

              return (
                <button
                  key={item.idGrupo}
                  onClick={() =>
                    carregarGrupo(
                      item.idGrupo
                    )
                  }
                  className={`w-full text-left bg-white rounded-2xl p-4 border shadow-sm transition ${
                    selecionado
                      ? 'border-[#4ecdc4] shadow-md'
                      : 'border-slate-100 hover:border-[#4ecdc4]/50'
                  }`}
                >

                  <div className="flex items-center gap-3">

                    <div className="w-12 h-12 rounded-2xl bg-emerald-50 flex items-center justify-center shrink-0">
                      <Radio className="w-6 h-6 text-emerald-500" />
                    </div>

                    <div className="min-w-0 flex-1">

                      <div className="flex items-center gap-2 mb-1">

                        <h3 className="font-extrabold text-sm text-[#1a535c] truncate">
                          {item.nomeGrupo}
                        </h3>

                        <span className="shrink-0 w-2 h-2 bg-emerald-500 rounded-full animate-pulse" />

                      </div>

                      <p className="text-xs text-slate-500 truncate">
                        {item.passeio?.nome}
                      </p>

                    </div>

                    <ChevronRight className="w-5 h-5 text-slate-300 shrink-0" />

                  </div>

                </button>
              );
            })}

          </div>

        </div>
      )}

      {/* ===================================================
          LOADING GRUPO
      =================================================== */}

      {loading && !grupo && (
        <div className="py-10 flex justify-center">
          <div className="w-10 h-10 border-4 border-[#4ecdc4] border-t-transparent rounded-full animate-spin" />
        </div>
      )}

      {/* ===================================================
          ERRO
      =================================================== */}

      {errorMsg && grupo && (
        <div className="mb-6 p-4 bg-rose-100 border border-rose-200 text-rose-700 rounded-2xl text-xs font-semibold">
          {errorMsg}
        </div>
      )}

      {/* ===================================================
          GRUPO SELECIONADO
      =================================================== */}

      {grupo && (
        <>

          {/* Banner */}
          <div className="bg-gradient-to-r from-[#1a535c] to-[#236c78] rounded-3xl p-6 text-white shadow-xl mb-6">

            <div className="flex items-center justify-between gap-3 mb-3">

              <span className="bg-emerald-500 text-white text-[10px] font-bold px-3 py-1 rounded-full uppercase tracking-wider inline-flex items-center gap-1">
                <Radio className="w-3.5 h-3.5 animate-pulse" />
                Ao Vivo
              </span>

              <span className="text-xs text-slate-200 font-semibold">
                Grupo #{grupo.idGrupo}
              </span>

            </div>

            <h1 className="text-2xl font-extrabold">
              {grupo.nome}
            </h1>

            <p className="text-sm text-slate-200 mt-1">
              {grupo.passeio?.nome}
            </p>

            <div className="flex items-center gap-2 mt-4">

              <div className="bg-white/15 rounded-xl px-3 py-2">

                <span className="block text-[9px] uppercase text-slate-300 font-bold">
                  Convite
                </span>

                <span className="font-extrabold tracking-widest">
                  {grupo.codigoConvite}
                </span>

              </div>

              <button
                onClick={handleCopyCode}
                className="bg-white/15 hover:bg-white/25 p-3 rounded-xl transition"
                title="Copiar código"
              >
                {copied ? (
                  <Check className="w-5 h-5 text-emerald-300" />
                ) : (
                  <Copy className="w-5 h-5" />
                )}
              </button>

            </div>

          </div>

          {/* Informações */}
          <div className="bg-white rounded-3xl p-5 shadow-lg border border-slate-100 mb-6">

            <div className="flex items-center gap-3 mb-4">

              <div className="w-12 h-12 rounded-2xl bg-[#4ecdc4]/15 flex items-center justify-center">
                <MapPin className="w-6 h-6 text-[#4ecdc4]" />
              </div>

              <div>

                <span className="text-[10px] font-bold uppercase text-slate-400">
                  Passeio
                </span>

                <h2 className="font-extrabold text-[#1a535c]">
                  {grupo.passeio?.nome}
                </h2>

              </div>

            </div>

            {grupo.passeio?.descricao && (
              <p className="text-xs text-slate-500 leading-relaxed mb-4">
                {grupo.passeio.descricao}
              </p>
            )}

            <div className="grid grid-cols-2 gap-3">

              <div className="bg-[#f5f7fa] rounded-2xl p-3">

                <CalendarDays className="w-4 h-4 text-[#4ecdc4]" />

                <span className="block text-[9px] uppercase text-slate-400 font-bold mt-1">
                  Data
                </span>

                <span className="text-xs font-bold text-[#1a535c]">
                  {formatarData(
                    grupo.dataInicio
                  )}
                </span>

              </div>

              <div className="bg-[#f5f7fa] rounded-2xl p-3">

                <Radio className="w-4 h-4 text-emerald-500" />

                <span className="block text-[9px] uppercase text-slate-400 font-bold mt-1">
                  Status
                </span>

                <span className="text-xs font-bold text-emerald-600">
                  Em andamento
                </span>

              </div>

            </div>

          </div>

          {/* =================================================
              MAPA
          ================================================= */}

          <div className="bg-white rounded-3xl p-4 shadow-lg border border-slate-100 mb-6">

            <div className="flex items-center justify-between mb-3 px-1">

              <div>

                <h3 className="font-extrabold text-[#1a535c]">
                  Mapa do Grupo
                </h3>

                <p className="text-[10px] text-slate-400">
                  Localização em tempo real
                </p>

              </div>

              <span className="flex items-center gap-1 text-[10px] font-bold text-emerald-600 bg-emerald-50 px-2 py-1 rounded-full">

                <span className="w-2 h-2 rounded-full bg-emerald-500 animate-pulse" />

                AO VIVO

              </span>

            </div>

            <div
              ref={mapRef}
              className="w-full h-80 rounded-2xl overflow-hidden shadow-inner"
            />

            <div className="mt-3 bg-amber-50 border border-amber-100 rounded-2xl p-3">

              <p className="text-[10px] text-amber-700 leading-relaxed">
                O mapa já está conectado ao grupo. A API atual ainda não fornece latitude e longitude individuais dos integrantes, portanto os marcadores dos participantes serão adicionados quando essa informação estiver disponível.
              </p>

            </div>

          </div>

          {/* =================================================
              INTEGRANTES
          ================================================= */}

          <div className="bg-white rounded-3xl p-5 shadow-lg border border-slate-100 mb-6">

            <h3 className="font-extrabold text-[#1a535c] flex items-center gap-2 mb-4">
              <Users className="w-5 h-5 text-[#4ecdc4]" />
              Integrantes ({grupo.integrantes?.length ?? 0})
            </h3>

            <div className="space-y-3">

              {(grupo.integrantes ?? []).map(
                integrante => (

                  <div
                    key={integrante.idUsuario}
                    className="flex items-center justify-between gap-3 p-3 bg-[#f5f7fa] rounded-2xl"
                  >

                    <div className="flex items-center gap-3">

                      <div className="w-10 h-10 rounded-full bg-[#1a535c] text-white flex items-center justify-center font-extrabold">
                        {integrante.nome
                          .charAt(0)
                          .toUpperCase()}
                      </div>

                      <div>

                        <h4 className="text-xs font-extrabold text-[#1a535c]">
                          {integrante.nome}
                        </h4>

                        <div className="flex items-center gap-1 mt-1">

                          {integrante.online ? (
                            <>
                              <span className="w-2 h-2 rounded-full bg-emerald-500" />

                              <span className="text-[9px] text-emerald-600 font-bold">
                                Online
                              </span>
                            </>
                          ) : (
                            <>
                              <span className="w-2 h-2 rounded-full bg-slate-300" />

                              <span className="text-[9px] text-slate-400 font-bold">
                                Offline
                              </span>
                            </>
                          )}

                        </div>

                      </div>

                    </div>

                    <div>

                      {integrante.iniciouPasseio ? (
                        <div className="flex items-center gap-1 bg-emerald-50 text-emerald-600 px-2 py-1 rounded-full">

                          <UserCheck className="w-3.5 h-3.5" />

                          <span className="text-[9px] font-bold">
                            Iniciou
                          </span>

                        </div>
                      ) : (
                        <div className="flex items-center gap-1 bg-slate-100 text-slate-400 px-2 py-1 rounded-full">

                          <UserX className="w-3.5 h-3.5" />

                          <span className="text-[9px] font-bold">
                            Aguardando
                          </span>

                        </div>
                      )}

                    </div>

                  </div>

                )
              )}

            </div>

          </div>

          {/* =================================================
              ATIVIDADE
          ================================================= */}

          <div className="bg-white rounded-3xl p-5 shadow-lg border border-slate-100 mb-6">

            <h3 className="font-extrabold text-[#1a535c] flex items-center gap-2 mb-4">
              <Clock className="w-5 h-5 text-[#ff6b6b]" />
              Atividade
            </h3>

            <div className="space-y-2">

              {(grupo.integrantes ?? []).map(
                integrante => (

                  <div
                    key={integrante.idUsuario}
                    className="flex justify-between items-center text-xs"
                  >

                    <span className="font-semibold text-slate-600">
                      {integrante.nome}
                    </span>

                    <span className="text-slate-400">
                      {integrante.ultimaAtividade
                        ? formatarDataHora(
                            integrante.ultimaAtividade
                          )
                        : 'Sem atividade'}
                    </span>

                  </div>

                )
              )}

            </div>

          </div>

          {/* =================================================
              BOTÕES
          ================================================= */}

          <div className="space-y-3">

            {!grupo.integrantes?.some(
              integrante =>
                integrante.iniciouPasseio
            ) && (
              <button
                onClick={
                  handleIniciarPasseio
                }
                disabled={starting}
                className="w-full bg-[#4ecdc4] hover:bg-[#4ecdc4]/90 disabled:opacity-60 text-white font-extrabold py-4 rounded-2xl text-sm transition shadow-lg flex items-center justify-center gap-2"
              >

                {starting ? (
                  <>
                    <RefreshCw className="w-5 h-5 animate-spin" />
                    <span>
                      Iniciando...
                    </span>
                  </>
                ) : (
                  <>
                    <Play className="w-5 h-5 fill-current" />
                    <span>
                      Iniciar Passeio
                    </span>
                  </>
                )}

              </button>
            )}

            <button
              onClick={handleVoltar}
              className="w-full bg-rose-50 hover:bg-rose-100 text-rose-600 border border-rose-200 font-bold py-3 rounded-2xl text-xs transition flex items-center justify-center gap-2"
            >
              <Power className="w-4 h-4" />
              <span>
                Voltar para Meus Grupos
              </span>
            </button>

          </div>

        </>
      )}

    </div>
  );
};