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
  iniciarPasseio,
  finalizarPasseio
} from '../services/grupoService';

import { passeioService } from '../services/passeioService';
import { api } from '../services/api';
import { supabase } from '../services/supabaseClient';
import { PasseioDto } from '../types';
import { getDistance } from 'geolib';

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
  CalendarDays,
  AlertTriangle,
  ShieldCheck,
  Maximize,
  Minimize
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

interface PeerLocation {
  idUsuario: number;
  nome: string;
  latitude: number;
  longitude: number;
  timestamp: number;
}

// =========================================================
// ÍCONES CUSTOMIZADOS LEAFLET
// =========================================================

const criarIconeUsuario = (nome: string, isMe: boolean, dentroDaArea: boolean = true) => {
  const inicial = (nome && nome.trim().length > 0) ? nome.charAt(0).toUpperCase() : '?';
  const corFundo = isMe ? 'bg-[#4ecdc4]' : 'bg-[#1a535c]';
  const borda = dentroDaArea ? 'border-white' : 'border-rose-500 animate-pulse';

  return L.divIcon({
    className: 'leaflet-custom-marker',
    html: `
      <div style="display:flex; flex-direction:column; align-items:center; transform: translate(-50%, -50%);">
        <div style="background-color: white; padding: 2px 6px; border-radius: 9999px; font-size: 10px; font-weight: 800; color: #1a535c; box-shadow: 0 2px 4px rgba(0,0,0,0.15); margin-bottom: 2px; white-space: nowrap;">
          ${isMe ? 'Você' : nome.split(' ')[0]}
        </div>
        <div class="${corFundo} ${borda}" style="width: 32px; height: 32px; border-radius: 9999px; border-width: 2px; border-style: solid; display: flex; align-items: center; justify-content: center; color: white; font-weight: 800; font-size: 13px; box-shadow: 0 4px 6px rgba(0,0,0,0.2);">
          ${inicial}
        </div>
      </div>
    `,
    iconSize: [32, 32],
    iconAnchor: [16, 16],
  });
};

// =========================================================
// COMPONENTE
// =========================================================

export const MapaGrupoPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const { usuario, getAuthHeader } = useAuth();

  // =======================================================
  // ESTADOS PRINCIPAIS
  // =======================================================

  const [gruposAoVivo, setGruposAoVivo] = useState<PasseioPendenteDto[]>([]);
  const [grupo, setGrupo] = useState<GrupoDetalhesDto | null>(null);
  const [passeioCompleto, setPasseioCompleto] = useState<PasseioDto | null>(null);

  const [loading, setLoading] = useState(true);
  const [loadingGrupos, setLoadingGrupos] = useState(true);
  const [starting, setStarting] = useState(false);
  const [copied, setCopied] = useState(false);
  const [errorMsg, setErrorMsg] = useState('');

  // =======================================================
  // ESTADOS DE GPS E GEOFENCING E TELA CHEIA
  // =======================================================

  const [minhaPosicao, setMinhaPosicao] = useState<{ lat: number; lng: number } | null>(null);
  const [dentroDoPerimetro, setDentroDoPerimetro] = useState<boolean | null>(null);
  const [distanciaDoCentro, setDistanciaDoCentro] = useState<number | null>(null);
  const [isFullscreen, setIsFullscreen] = useState(false);

  // Referências Leaflet e Marcadores
  const mapWrapperRef = useRef<HTMLDivElement>(null);
  const mapRef = useRef<HTMLDivElement>(null);
  const leafletMap = useRef<L.Map | null>(null);
  const myMarkerRef = useRef<L.Marker | null>(null);
  const friendsMarkersRef = useRef<Map<number, L.Marker>>(new Map());
  const circlePerimetroRef = useRef<L.Circle | null>(null);
  const lastSyncTimeRef = useRef<number>(0);

  // =======================================================
  // IDENTIFICAÇÃO DO USUÁRIO
  // =======================================================

  const getUsuarioId = (): number | null => {
    if (!usuario) return null;
    const uid =
      (usuario as any).id_usuario ??
      (usuario as any).idUsuario ??
      (usuario as any).id;
    const numero = Number(uid);
    return Number.isFinite(numero) ? numero : null;
  };

  const getGrupoId = (): number | null => {
    const activeSessionId = sessionStorage.getItem('activeLiveGrupoId');
    const paramId = id || searchParams.get('grupoId') || activeSessionId;
    if (!paramId) return null;
    const numero = Number(paramId);
    return Number.isFinite(numero) ? numero : null;
  };

  // =======================================================
  // CONTROLE DE TELA CHEIA (FULLSCREEN)
  // =======================================================

  useEffect(() => {
    const handleFullscreenChange = () => {
      const isFull = !!document.fullscreenElement;
      setIsFullscreen(isFull);
      
      // Força o Leaflet a recalcular o tamanho do mapa após entrar ou sair da tela cheia
      setTimeout(() => {
        if (leafletMap.current) {
          leafletMap.current.invalidateSize();
        }
      }, 200);
    };

    document.addEventListener('fullscreenchange', handleFullscreenChange);
    return () => document.removeEventListener('fullscreenchange', handleFullscreenChange);
  }, []);

  const toggleFullscreen = () => {
    if (!document.fullscreenElement) {
      mapWrapperRef.current?.requestFullscreen().catch(err => {
        console.warn(`Erro ao tentar abrir tela cheia: ${err.message}`);
      });
    } else {
      document.exitFullscreen();
    }
  };

  // =======================================================
  // CARREGAR GRUPOS AO VIVO
  // =======================================================

  const carregarGruposAoVivo = async () => {
    setLoadingGrupos(true);
    setErrorMsg('');

    try {
      const response = await fetch(`${API_BASE_URL}/api/grupo/meus-pendentes`, {
        method: 'GET',
        headers: getAuthHeader()
      });

      if (!response.ok) {
        throw new Error(`Erro HTTP ${response.status}`);
      }

      const data: PasseioPendenteDto[] = await response.json();
      const ativos = Array.isArray(data) ? data.filter(item => item.status === 'EM_ANDAMENTO') : [];
      setGruposAoVivo(ativos);
      return ativos;
    } catch (error) {
      console.error('[AoVivo] Erro ao buscar grupos:', error);
      setGruposAoVivo([]);
      setErrorMsg('Não foi possível carregar os grupos ao vivo.');
      return [];
    } finally {
      setLoadingGrupos(false);
    }
  };

  // =======================================================
  // CARREGAR DETALHES DO GRUPO E DO PASSEIO
  // =======================================================

  const carregarGrupo = async (grupoId?: number) => {
    const idFinal = grupoId ?? getGrupoId();
    if (!idFinal) {
      setGrupo(null);
      setLoading(false);
      return;
    }

    setLoading(true);
    setErrorMsg('');

    try {
      const data = await buscarGrupo(idFinal, getAuthHeader);
      setGrupo(data);
      sessionStorage.setItem('activeLiveGrupoId', String(idFinal));

      if (data.idPasseio) {
        try {
          const passeioDados = await passeioService.buscarPorId(data.idPasseio);
          setPasseioCompleto(passeioDados);
        } catch (err) {
          console.warn('[AoVivo] Não foi possível obter detalhes de endereço do passeio:', err);
        }
      }
    } catch (error) {
      console.error('[AoVivo] Erro ao buscar grupo:', error);
      setGrupo(null);
      setErrorMsg('Não foi possível carregar os dados do grupo.');
    } finally {
      setLoading(false);
    }
  };

  // =======================================================
  // INICIALIZAÇÃO
  // =======================================================

  useEffect(() => {
    const inicializar = async () => {
      const ativos = await carregarGruposAoVivo();
      const grupoId = getGrupoId();

      if (grupoId) {
        await carregarGrupo(grupoId);
        return;
      }

      if (ativos.length > 0) {
        await carregarGrupo(ativos[0].idGrupo);
      } else {
        setLoading(false);
      }
    };

    inicializar();
  }, [id]);

  // =======================================================
  // INICIALIZAÇÃO DO MAPA LEAFLET
  // =======================================================

  useEffect(() => {
    if (!mapRef.current || leafletMap.current) return;

    const initialLat = passeioCompleto?.endereco?.latitude ?? -23.5874;
    const initialLng = passeioCompleto?.endereco?.longitude ?? -46.6576;

    const map = L.map(mapRef.current, {
      zoomControl: true,
      attributionControl: false
    }).setView([initialLat, initialLng], 15);

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      maxZoom: 19
    }).addTo(map);

    leafletMap.current = map;

    setTimeout(() => {
      map.invalidateSize();
    }, 200);

    return () => {
      if (leafletMap.current) {
        leafletMap.current.remove();
        leafletMap.current = null;
      }
    };
  }, [mapRef.current]);

  // =======================================================
  // DESENHAR E ATUALIZAR PERÍMETRO DO PASSEIO BLINDADO
  // =======================================================

  useEffect(() => {
    if (!leafletMap.current || !passeioCompleto?.endereco) return;

    const lat = Number(passeioCompleto.endereco.latitude);
    const lng = Number(passeioCompleto.endereco.longitude);
    const raio = Number(passeioCompleto.endereco.raioMetros) || 400;

    if (!isNaN(lat) && !isNaN(lng)) {
      const isFora = dentroDoPerimetro === false;
      const corHex = isFora ? '#ef4444' : '#4ecdc4';

      if (circlePerimetroRef.current) {
        circlePerimetroRef.current.setStyle({
          color: corHex,
          fillColor: corHex
        });
      } else {
        circlePerimetroRef.current = L.circle([lat, lng], {
          radius: raio,
          color: corHex,
          fillColor: corHex,
          fillOpacity: 0.12,
          weight: 2,
          dashArray: '5, 5'
        }).addTo(leafletMap.current);

        leafletMap.current.setView([lat, lng], 15);
      }
    }
  }, [passeioCompleto, dentroDoPerimetro]);

  // =======================================================
  // SUPABASE REALTIME (BROADCAST) + GPS NATIVO
  // =======================================================

  useEffect(() => {
    const grupoId = grupo?.idGrupo;
    const meuId = getUsuarioId();

    if (!grupoId || !meuId) return;

    const channelName = `grupo_localizacao_${grupoId}`;
    const channel = supabase.channel(channelName);

    channel
      .on('broadcast', { event: 'posicao' }, (payload) => {
        const peer: PeerLocation = payload.payload;
        if (!peer || peer.idUsuario === meuId) return;

        if (leafletMap.current) {
          let marker = friendsMarkersRef.current.get(peer.idUsuario);

          if (marker) {
            marker.setLatLng([peer.latitude, peer.longitude]);
          } else {
            marker = L.marker([peer.latitude, peer.longitude], {
              icon: criarIconeUsuario(peer.nome, false)
            }).addTo(leafletMap.current);

            friendsMarkersRef.current.set(peer.idUsuario, marker);
          }
        }
      })
      .subscribe();

    let watchId: number | null = null;

    if ('geolocation' in navigator) {
      watchId = navigator.geolocation.watchPosition(
        (pos) => {
          const lat = pos.coords.latitude;
          const lng = pos.coords.longitude;

          setMinhaPosicao({ lat, lng });

          // CÁLCULO DE GEOFENCING (GEOLIB) IMEDIATO
          let isDentro = true;
          let dist = 0;
          
          if (passeioCompleto?.endereco?.latitude && passeioCompleto?.endereco?.longitude) {
            dist = getDistance(
              { latitude: lat, longitude: lng },
              {
                latitude: Number(passeioCompleto.endereco.latitude),
                longitude: Number(passeioCompleto.endereco.longitude)
              }
            );

            setDistanciaDoCentro(dist);
            const raioMaximo = Number(passeioCompleto.endereco.raioMetros) || 400;
            isDentro = dist <= raioMaximo;
            setDentroDoPerimetro(isDentro);
          }

          // ATUALIZA MARCADOR PESSOAL NO LEAFLET COM A COR CORRETA
          if (leafletMap.current) {
            const nomeUsuario = (usuario as any)?.nome_completo || (usuario as any)?.nome || 'Eu';
            
            const novoIcone = criarIconeUsuario(nomeUsuario, true, isDentro);

            if (myMarkerRef.current) {
              myMarkerRef.current.setLatLng([lat, lng]);
              myMarkerRef.current.setIcon(novoIcone);
            } else {
              myMarkerRef.current = L.marker([lat, lng], {
                icon: novoIcone
              }).addTo(leafletMap.current);
            }
          }

          channel.send({
            type: 'broadcast',
            event: 'posicao',
            payload: {
              idUsuario: meuId,
              nome: (usuario as any)?.nome_completo || (usuario as any)?.nome || 'Integrante',
              latitude: lat,
              longitude: lng,
              timestamp: Date.now()
            }
          });

          const agora = Date.now();
          if (agora - lastSyncTimeRef.current > 120000) {
            lastSyncTimeRef.current = agora;
            api.post('/localizacao/sync', {
              idGrupo: grupoId,
              latitude: lat,
              longitude: lng
            }).catch(err => console.warn('[AoVivo] Falha ao persistir localização na API:', err));
          }
        },
        (err) => {
          console.warn('[AoVivo] Erro ao obter GPS do navegador:', err.message);
        },
        {
          enableHighAccuracy: true,
          maximumAge: 5000,
          timeout: 10000
        }
      );
    }

    return () => {
      if (watchId !== null) navigator.geolocation.clearWatch(watchId);
      supabase.removeChannel(channel);

      friendsMarkersRef.current.forEach(m => m.remove());
      friendsMarkersRef.current.clear();
      if (myMarkerRef.current) {
        myMarkerRef.current.remove();
        myMarkerRef.current = null;
      }
    };
  }, [grupo?.idGrupo, usuario, passeioCompleto]);

  // =======================================================
  // INICIAR PASSEIO E OUTRAS FUNÇÕES MANTIDAS
  // =======================================================

  const handleIniciarPasseio = async () => {
    if (!grupo) return;
    setStarting(true);
    setErrorMsg('');

    try {
      await iniciarPasseio(grupo.idGrupo, getAuthHeader);
      const grupoAtualizado = await buscarGrupo(grupo.idGrupo, getAuthHeader);
      setGrupo(grupoAtualizado);
      sessionStorage.setItem('activeLiveGrupoId', String(grupo.idGrupo));
      await carregarGruposAoVivo();
    } catch (error) {
      console.error('[AoVivo] Erro ao iniciar passeio:', error);
      setErrorMsg(error instanceof Error ? error.message : 'Não foi possível iniciar o passeio.');
    } finally {
      setStarting(false);
    }
  };

  const handleCopyCode = async () => {
    if (!grupo) return;
    try {
      await navigator.clipboard.writeText(grupo.codigoConvite);
      setCopied(true);
      setTimeout(() => setCopied(false), 2000);
    } catch (error) {
      console.error('[AoVivo] Erro ao copiar código:', error);
    }
  };

  const handleVoltar = () => {
    sessionStorage.removeItem('activeLiveGrupoId');
    if (leafletMap.current) {
      leafletMap.current.remove();
      leafletMap.current = null;
    }
    navigate('/grupos');
  };

  const handleEncerrarPasseio = async () => {
    if (!grupo) return;
    
    const confirmar = window.confirm("Deseja encerrar este passeio definitivamente para todos?");
    if (!confirmar) return;

    try {
      await finalizarPasseio(grupo.idGrupo, getAuthHeader);
      handleVoltar(); 
    } catch (error) {
      console.error('[AoVivo] Erro ao encerrar:', error);
      setErrorMsg('Erro ao encerrar o passeio.');
    }
  };

  // =======================================================
  // CONDICIONAIS DE LOADING / TELA VAZIA
  // =======================================================

  if (loading && loadingGrupos) {
    return (
      <div className="pt-32 pb-28 flex justify-center items-center min-h-[60vh]">
        <div className="w-12 h-12 border-4 border-[#4ecdc4] border-t-transparent rounded-full animate-spin" />
      </div>
    );
  }

  if (!loadingGrupos && gruposAoVivo.length === 0 && !grupo) {
    return (
      <div className="pt-24 pb-28 max-w-lg mx-auto px-4">
        <div className="flex items-center gap-3 mb-8">
          <button
            onClick={() => navigate('/grupos')}
            className="p-3 bg-white rounded-full text-[#1a535c] shadow-md hover:bg-[#4ecdc4] hover:text-white transition"
          >
            <ArrowLeft className="w-5 h-5" />
          </button>
          <div>
            <span className="text-xs font-bold uppercase tracking-wider text-[#ff6b6b]">Rota Livre</span>
            <h1 className="text-3xl font-extrabold text-[#1a535c]">Ao Vivo</h1>
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
          <h2 className="text-xl font-extrabold text-[#1a535c] mb-2">Nenhum passeio ao vivo</h2>
          <p className="text-sm text-slate-500 mb-6">
            Você não possui nenhum grupo com passeio em andamento neste momento.
          </p>
          <button
            onClick={() => navigate('/grupos')}
            className="inline-flex items-center gap-2 bg-[#1a535c] text-white px-6 py-3 rounded-full font-bold text-sm hover:bg-[#4ecdc4] transition"
          >
            <Users className="w-4 h-4" />
            Ver meus grupos
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="pt-20 pb-24 max-w-2xl mx-auto px-4">

      {/* CABEÇALHO */}
      <div className="flex items-center justify-between gap-3 mb-6">
        <button
          onClick={handleVoltar}
          className="p-3 bg-white rounded-full text-[#1a535c] hover:bg-[#4ecdc4] hover:text-white transition shadow-md flex items-center gap-2 font-semibold text-sm"
        >
          <ArrowLeft className="w-5 h-5" />
          <span>Grupos</span>
        </button>

        <button
          onClick={() => carregarGruposAoVivo()}
          className="p-3 bg-white rounded-full text-[#1a535c] hover:bg-[#4ecdc4] hover:text-white transition shadow-md"
          title="Atualizar"
        >
          <RefreshCw className="w-5 h-5" />
        </button>
      </div>

      {/* LISTA DE GRUPOS AO VIVO */}
      {gruposAoVivo.length > 0 && (
        <div className="mb-6">
          <div className="flex items-center justify-between mb-3 px-1">
            <div>
              <h2 className="font-extrabold text-[#1a535c]">Passeios ao vivo</h2>
              <p className="text-xs text-slate-400">Grupos em andamento dos quais você participa</p>
            </div>
            <span className="bg-emerald-100 text-emerald-700 text-xs font-extrabold px-3 py-1 rounded-full">
              {gruposAoVivo.length}
            </span>
          </div>

          <div className="space-y-3">
            {gruposAoVivo.map(item => {
              const selecionado = grupo?.idGrupo === item.idGrupo;
              return (
                <button
                  key={item.idGrupo}
                  onClick={() => carregarGrupo(item.idGrupo)}
                  className={`w-full text-left bg-white rounded-2xl p-4 border shadow-sm transition ${
                    selecionado ? 'border-[#4ecdc4] shadow-md' : 'border-slate-100 hover:border-[#4ecdc4]/50'
                  }`}
                >
                  <div className="flex items-center gap-3">
                    <div className="w-12 h-12 rounded-2xl bg-emerald-50 flex items-center justify-center shrink-0">
                      <Radio className="w-6 h-6 text-emerald-500" />
                    </div>
                    <div className="min-w-0 flex-1">
                      <div className="flex items-center gap-2 mb-1">
                        <h3 className="font-extrabold text-sm text-[#1a535c] truncate">{item.nomeGrupo}</h3>
                        <span className="shrink-0 w-2 h-2 bg-emerald-500 rounded-full animate-pulse" />
                      </div>
                      <p className="text-xs text-slate-500 truncate">{item.passeio?.nome}</p>
                    </div>
                    <ChevronRight className="w-5 h-5 text-slate-300 shrink-0" />
                  </div>
                </button>
              );
            })}
          </div>
        </div>
      )}

      {errorMsg && grupo && (
        <div className="mb-6 p-4 bg-rose-100 border border-rose-200 text-rose-700 rounded-2xl text-xs font-semibold">
          {errorMsg}
        </div>
      )}

      {grupo && (
        <>
          {/* BANNER DO GRUPO */}
          <div className="bg-gradient-to-r from-[#1a535c] to-[#236c78] rounded-3xl p-6 text-white shadow-xl mb-6">
            <div className="flex items-center justify-between gap-3 mb-3">
              <span className="bg-emerald-500 text-white text-[10px] font-bold px-3 py-1 rounded-full uppercase tracking-wider inline-flex items-center gap-1">
                <Radio className="w-3.5 h-3.5 animate-pulse" />
                Ao Vivo
              </span>
              <span className="text-xs text-slate-200 font-semibold">Grupo #{grupo.idGrupo}</span>
            </div>

            <h1 className="text-2xl font-extrabold">{grupo.nome}</h1>
            <p className="text-sm text-slate-200 mt-1">{grupo.passeio?.nome}</p>

            <div className="flex items-center gap-2 mt-4">
              <div className="bg-white/15 rounded-xl px-3 py-2">
                <span className="block text-[9px] uppercase text-slate-300 font-bold">Convite</span>
                <span className="font-extrabold tracking-widest">{grupo.codigoConvite}</span>
              </div>
              <button
                onClick={handleCopyCode}
                className="bg-white/15 hover:bg-white/25 p-3 rounded-xl transition"
                title="Copiar código"
              >
                {copied ? <Check className="w-5 h-5 text-emerald-300" /> : <Copy className="w-5 h-5" />}
              </button>
            </div>
          </div>

          {/* =================================================
              MAPA COM RASTREAMENTO E GEOFENCING EM TEMPO REAL
          ================================================= */}
          <div className="bg-white rounded-3xl p-4 shadow-lg border border-slate-100 mb-6">
            <div className="flex items-center justify-between mb-3 px-1">
              <div>
                <h3 className="font-extrabold text-[#1a535c]">Mapa do Grupo</h3>
                <p className="text-[10px] text-slate-400">Localização dos integrantes em tempo real</p>
              </div>
              <span className="flex items-center gap-1 text-[10px] font-bold text-emerald-600 bg-emerald-50 px-2.5 py-1 rounded-full">
                <span className="w-2 h-2 rounded-full bg-emerald-500 animate-pulse" />
                GPS ATIVO
              </span>
            </div>

            {/* WRAPPER DO MAPA COM O BOTÃO TELA CHEIA */}
            <div
              ref={mapWrapperRef}
              className={`relative w-full bg-slate-100 transition-all ${
                isFullscreen 
                  ? 'h-screen rounded-none z-[9999]' 
                  : 'h-80 rounded-2xl overflow-hidden shadow-inner border border-slate-100'
              }`}
            >
              <div ref={mapRef} className="w-full h-full" />
              
              <button
                onClick={toggleFullscreen}
                type="button"
                className="absolute bottom-4 right-4 z-[1000] bg-white p-2.5 rounded-xl shadow-lg text-[#1a535c] hover:bg-[#4ecdc4] hover:text-white transition-colors border border-slate-200"
                title={isFullscreen ? "Sair da Tela Cheia" : "Tela Cheia"}
              >
                {isFullscreen ? <Minimize className="w-5 h-5" /> : <Maximize className="w-5 h-5" />}
              </button>
            </div>

            {/* STATUS DO GEOFENCING */}
            <div className="mt-3">
              {dentroDoPerimetro === false ? (
                <div className="bg-rose-50 border border-rose-200 rounded-2xl p-3 flex items-center gap-2.5 text-rose-700">
                  <AlertTriangle className="w-5 h-5 shrink-0 text-rose-600" />
                  <div className="text-xs">
                    <p className="font-bold">Atenção: Você saiu da área do passeio!</p>
                    <p className="text-[10px] opacity-80">
                      Você está a {distanciaDoCentro}m do centro. O raio limite deste passeio é de {passeioCompleto?.endereco?.raioMetros ?? 400}m.
                    </p>
                  </div>
                </div>
              ) : dentroDoPerimetro === true ? (
                <div className="bg-emerald-50 border border-emerald-200 rounded-2xl p-3 flex items-center gap-2.5 text-emerald-700">
                  <ShieldCheck className="w-5 h-5 shrink-0 text-emerald-600" />
                  <div className="text-xs">
                    <p className="font-bold">Você está dentro da rota do passeio</p>
                    <p className="text-[10px] opacity-80">
                      Sua localização está sincronizada com todos os colegas do grupo.
                    </p>
                  </div>
                </div>
              ) : (
                <div className="bg-slate-50 border border-slate-200 rounded-2xl p-3 text-[11px] text-slate-500 text-center">
                  Buscando precisão de GPS e conectando aos integrantes...
                </div>
              )}
            </div>
          </div>

          {/* INTEGRANTES */}
          <div className="bg-white rounded-3xl p-5 shadow-lg border border-slate-100 mb-6">
            <h3 className="font-extrabold text-[#1a535c] flex items-center gap-2 mb-4">
              <Users className="w-5 h-5 text-[#4ecdc4]" />
              Integrantes ({grupo.integrantes?.length ?? 0})
            </h3>

            <div className="space-y-3">
              {(grupo.integrantes ?? []).map(integrante => (
                <div
                  key={integrante.idUsuario}
                  className="flex items-center justify-between gap-3 p-3 bg-[#f5f7fa] rounded-2xl"
                >
                  <div className="flex items-center gap-3">
                    <div className="w-10 h-10 rounded-full bg-[#1a535c] text-white flex items-center justify-center font-extrabold text-sm">
                      {integrante.nome.charAt(0).toUpperCase()}
                    </div>
                    <div>
                      <h4 className="text-xs font-extrabold text-[#1a535c]">{integrante.nome}</h4>
                      <div className="flex items-center gap-1 mt-1">
                        {integrante.online ? (
                          <>
                            <span className="w-2 h-2 rounded-full bg-emerald-500" />
                            <span className="text-[9px] text-emerald-600 font-bold">Online</span>
                          </>
                        ) : (
                          <>
                            <span className="w-2 h-2 rounded-full bg-slate-300" />
                            <span className="text-[9px] text-slate-400 font-bold">Offline</span>
                          </>
                        )}
                      </div>
                    </div>
                  </div>

                  <div>
                    {integrante.iniciouPasseio ? (
                      <div className="flex items-center gap-1 bg-emerald-50 text-emerald-600 px-2 py-1 rounded-full">
                        <UserCheck className="w-3.5 h-3.5" />
                        <span className="text-[9px] font-bold">Iniciou</span>
                      </div>
                    ) : (
                      <div className="flex items-center gap-1 bg-slate-100 text-slate-400 px-2 py-1 rounded-full">
                        <UserX className="w-3.5 h-3.5" />
                        <span className="text-[9px] font-bold">Aguardando</span>
                      </div>
                    )}
                  </div>
                </div>
              ))}
            </div>
          </div>

          {/* BOTÕES DE AÇÃO */}
          <div className="space-y-3">
            {!grupo.integrantes?.some(
              integrante => integrante.idUsuario === getUsuarioId() && integrante.iniciouPasseio
            ) && (
              <button
                onClick={handleIniciarPasseio}
                disabled={starting}
                className="w-full bg-[#4ecdc4] hover:bg-[#4ecdc4]/90 disabled:opacity-60 text-white font-extrabold py-4 rounded-2xl text-sm transition shadow-lg flex items-center justify-center gap-2"
              >
                {starting ? (
                  <>
                    <RefreshCw className="w-5 h-5 animate-spin" />
                    <span>Iniciando...</span>
                  </>
                ) : (
                  <>
                    <Play className="w-5 h-5 fill-current" />
                    <span>Iniciar Minha Participação</span>
                  </>
                )}
              </button>
            )}

            <button
              onClick={handleVoltar}
              className="w-full bg-rose-50 hover:bg-rose-100 text-rose-600 border border-rose-200 font-bold py-3 rounded-2xl text-xs transition flex items-center justify-center gap-2"
            >
              <Power className="w-4 h-4" />
              <span>Voltar para Meus Grupos</span>
            </button>

            {grupo.criadorId === getUsuarioId() && (
              <button
                onClick={handleEncerrarPasseio}
                className="w-full bg-rose-600 hover:bg-rose-700 text-white font-extrabold py-4 rounded-2xl text-sm transition shadow-lg flex items-center justify-center gap-2 mt-2"
              >
                <Power className="w-5 h-5" />
                <span>Encerrar Passeio Definitivamente</span>
              </button>
            )}
          </div>
        </>
      )}
    </div>
  );
};