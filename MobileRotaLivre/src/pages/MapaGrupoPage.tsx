import React, { useState, useEffect, useRef } from 'react';
import { useParams, useNavigate, useSearchParams, Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { Grupo, Passeio } from '../types';
import { ArrowLeft, Users, Copy, Check, MapPin, RefreshCw, Radio, Play, ShieldAlert, Navigation, Power } from 'lucide-react';
import L from 'leaflet';

interface GrupoComPasseio extends Grupo {
  passeio?: Passeio | null;
}

export const MapaGrupoPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const { usuario, getAuthHeader } = useAuth();

  const [grupo, setGrupo] = useState<GrupoComPasseio | null>(null);
  const [loading, setLoading] = useState(true);
  const [copied, setCopied] = useState(false);
  const [userLocation, setUserLocation] = useState<{ lat: number; lng: number } | null>(null);

  // Modal confirmation state
  const [isStarted, setIsStarted] = useState(false);

  const mapRef = useRef<HTMLDivElement>(null);
  const leafletMap = useRef<L.Map | null>(null);
  const markersRef = useRef<{ [key: number]: L.Marker }>({});

  const loadGrupoData = async () => {
    setLoading(true);

    // Only load if an active ride was explicitly initiated or ID provided in URL/sessionStorage
    const activeSessionId = sessionStorage.getItem('activeLiveGrupoId');
    const paramGrupoId = id || searchParams.get('grupoId') || activeSessionId;

    if (!paramGrupoId) {
      setGrupo(null);
      setLoading(false);
      return;
    }

    try {
      const res = await fetch(`/api/grupo/${paramGrupoId}`, { headers: getAuthHeader() });
      const data = await res.json();
      if (data && data.id_grupo) {
        setGrupo(data);
      } else {
        setGrupo(null);
        sessionStorage.removeItem('activeLiveGrupoId');
      }
    } catch (err) {
      console.error('Erro ao buscar grupo:', err);
      setGrupo(null);
      sessionStorage.removeItem('activeLiveGrupoId');
    } finally {
      setLoading(false);
    }
  };

  const handleEncerrarPasseio = () => {
    sessionStorage.removeItem('activeLiveGrupoId');
    setGrupo(null);
    setIsStarted(false);
    navigate('/ao-vivo');
  };

  useEffect(() => {
    loadGrupoData();
  }, [id, searchParams]);

  // Polling for live updates ONLY after started
  useEffect(() => {
    if (!isStarted || !grupo) return;

    const interval = setInterval(() => {
      fetch(`/api/grupo/${grupo.id_grupo}`, { headers: getAuthHeader() })
        .then(res => res.json())
        .then(data => {
          if (data && data.id_grupo) setGrupo(data);
        })
        .catch(err => console.error('Erro ao atualizar posições:', err));
    }, 5000);

    return () => clearInterval(interval);
  }, [isStarted, grupo?.id_grupo]);

  // Track user geolocation ONLY after user clicks "Iniciar"
  useEffect(() => {
    if (!isStarted || !grupo) return;

    if ('geolocation' in navigator) {
      const watchId = navigator.geolocation.watchPosition(
        pos => {
          const lat = pos.coords.latitude;
          const lng = pos.coords.longitude;
          setUserLocation({ lat, lng });

          // Send coordinates to server
          fetch(`/api/grupo/${grupo.id_grupo}/localizacao`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json', ...getAuthHeader() },
            body: JSON.stringify({ lat, lng })
          });
        },
        err => console.log('Geolocalização indisponível:', err),
        { enableHighAccuracy: true }
      );
      return () => navigator.geolocation.clearWatch(watchId);
    }
  }, [isStarted, grupo?.id_grupo]);

  // Initialize and Update Leaflet Map ONLY after started
  useEffect(() => {
    if (!isStarted || !grupo) return;

    if (mapRef.current && !leafletMap.current) {
      // Center map around São Paulo default or group location
      const map = L.map(mapRef.current).setView([-23.5874, -46.6576], 14);
      L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; OpenStreetMap'
      }).addTo(map);

      leafletMap.current = map;
    }

    if (leafletMap.current && grupo) {
      grupo.membros.forEach(m => {
        if (m.lat && m.lng) {
          if (markersRef.current[m.id_usuario]) {
            markersRef.current[m.id_usuario].setLatLng([m.lat, m.lng]);
          } else {
            const marker = L.marker([m.lat, m.lng])
              .addTo(leafletMap.current!)
              .bindPopup(`<b>${m.nome}</b><br/>Última atualização: ${new Date().toLocaleTimeString('pt-BR')}`);
            markersRef.current[m.id_usuario] = marker;
          }
        }
      });
    }
  }, [isStarted, grupo]);

  const handleCopyCode = () => {
    if (!grupo) return;
    navigator.clipboard.writeText(grupo.codigo_grupo);
    setCopied(true);
    setTimeout(() => setCopied(false), 2000);
  };

  if (loading) {
    return (
      <div className="pt-32 pb-28 flex justify-center items-center">
        <div className="w-12 h-12 border-4 border-[#4ecdc4] border-t-transparent rounded-full animate-spin" />
      </div>
    );
  }

  if (!grupo) {
    return (
      <div className="pt-32 pb-28 max-w-lg mx-auto text-center px-4">
        <Navigation className="w-12 h-12 text-[#4ecdc4] mx-auto mb-4" />
        <h2 className="text-2xl font-extrabold text-[#1a535c] mb-2">Nenhum passeio ao vivo ativo</h2>
        <p className="text-sm text-slate-500 mb-6">
          Inicie um passeio para ver a localização do seu grupo em tempo real.
        </p>
        <Link
          to="/grupos"
          className="inline-flex items-center gap-2 bg-[#1a535c] text-white px-6 py-3 rounded-full font-bold text-sm shadow-md hover:bg-[#4ecdc4] transition"
        >
          <Users className="w-4 h-4" />
          <span>Ir para Meus Grupos</span>
        </Link>
      </div>
    );
  }

  return (
    <div className="pt-20 pb-24 max-w-md mx-auto px-4 relative">
      {/* CONFIRMATION MODAL POP-UP BEFORE STARTING LIVE MAP */}
      {!isStarted && (
        <div className="fixed inset-0 z-50 bg-slate-900/80 backdrop-blur-md flex items-center justify-center p-4 animate-fadeIn">
          <div className="bg-white rounded-3xl max-w-md w-full shadow-2xl overflow-hidden border border-slate-100 p-8 text-center animate-scaleUp">
            <div className="w-16 h-16 bg-[#4ecdc4]/20 text-[#4ecdc4] rounded-3xl flex items-center justify-center mx-auto mb-4">
              <Radio className="w-8 h-8 animate-pulse" />
            </div>

            <span className="text-[10px] font-bold uppercase tracking-widest text-[#ff6b6b] bg-[#ff6b6b]/10 px-3 py-1 rounded-full">
              Confirmação de Transmissão
            </span>

            <h2 className="text-2xl font-extrabold text-[#1a535c] mt-3 mb-2">
              Iniciar Passeio Ao Vivo?
            </h2>

            <p className="text-xs text-slate-600 leading-relaxed mb-6">
              Você está prestes a entrar na transmissão ao vivo do grupo{' '}
              <strong className="text-[#1a535c] font-bold">{grupo.nome_grupo}</strong>.
              Sua localização via GPS será compartilhada em tempo real no mapa com os participantes.
            </p>

            <div className="bg-[#f5f7fa] p-4 rounded-2xl mb-6 text-left border border-slate-200 text-xs space-y-2">
              <div className="flex justify-between">
                <span className="text-slate-400 font-semibold">Código do Grupo:</span>
                <span className="font-extrabold text-[#1a535c]">{grupo.codigo_grupo}</span>
              </div>
              {grupo.data_passeio && (
                <div className="flex justify-between">
                  <span className="text-slate-400 font-semibold">Data:</span>
                  <span className="font-bold text-[#1a535c]">{new Date(grupo.data_passeio + 'T00:00:00').toLocaleDateString('pt-BR')}</span>
                </div>
              )}
              {grupo.horario_passeio && (
                <div className="flex justify-between">
                  <span className="text-slate-400 font-semibold">Horário:</span>
                  <span className="font-bold text-[#1a535c]">{grupo.horario_passeio}</span>
                </div>
              )}
            </div>

            <button
              onClick={() => setIsStarted(true)}
              className="w-full bg-[#4ecdc4] hover:bg-[#4ecdc4]/90 text-white font-extrabold py-4 rounded-2xl text-sm transition shadow-xl flex items-center justify-center gap-2 hover:scale-[1.02]"
            >
              <Play className="w-5 h-5 fill-current" />
              <span>Iniciar</span>
            </button>
          </div>
        </div>
      )}

      {/* Top Navigation & Code Bar */}
      <div className="flex flex-wrap items-center justify-between gap-4 mb-6">
        <button
          onClick={() => navigate('/grupos')}
          className="p-3 bg-white rounded-full text-[#1a535c] hover:bg-[#4ecdc4] hover:text-white transition shadow-md flex items-center gap-2 font-semibold text-sm"
        >
          <ArrowLeft className="w-5 h-5" />
          <span>Voltar para Grupos</span>
        </button>

        <div className="flex items-center gap-2 bg-white px-4 py-2 rounded-full shadow-md border border-slate-100">
          <span className="text-xs text-slate-500 font-bold uppercase">Código do Grupo:</span>
          <span className="font-extrabold text-[#1a535c] tracking-widest">{grupo.codigo_grupo}</span>
          <button
            onClick={handleCopyCode}
            className="p-1.5 text-slate-400 hover:text-[#4ecdc4] rounded-full transition"
            title="Copiar código"
          >
            {copied ? <Check className="w-4 h-4 text-emerald-500" /> : <Copy className="w-4 h-4" />}
          </button>
        </div>
      </div>

      {/* Group Header Banner */}
      <div className="bg-gradient-to-r from-[#1a535c] to-[#236c78] rounded-3xl p-6 text-white shadow-xl mb-6 flex flex-wrap items-center justify-between gap-4">
        <div>
          <span className="bg-[#4ecdc4] text-white text-[11px] font-bold px-3 py-1 rounded-full uppercase tracking-wider inline-flex items-center gap-1 mb-2">
            <Radio className="w-3.5 h-3.5 animate-pulse" /> Ao Vivo
          </span>
          <h1 className="text-2xl font-extrabold">{grupo.nome_grupo}</h1>
          <div className="flex flex-wrap items-center gap-3 text-xs text-slate-200 mt-1">
            <span>{grupo.membros.length} membro(s) participando neste mapa.</span>
            {grupo.data_passeio && (
              <span className="bg-white/15 px-2.5 py-0.5 rounded-full font-semibold">
                Data: {new Date(grupo.data_passeio + 'T00:00:00').toLocaleDateString('pt-BR')}
              </span>
            )}
            {grupo.horario_passeio && (
              <span className="bg-white/15 px-2.5 py-0.5 rounded-full font-semibold">
                Horário: {grupo.horario_passeio}
              </span>
            )}
          </div>
        </div>

        <div className="flex items-center gap-2">
          {isStarted && (
            <button
              onClick={loadGrupoData}
              className="bg-white/20 hover:bg-white/30 text-white font-bold px-4 py-2 rounded-full text-xs transition flex items-center gap-2"
            >
              <RefreshCw className="w-4 h-4" />
              <span>Atualizar</span>
            </button>
          )}

          <button
            onClick={handleEncerrarPasseio}
            className="bg-rose-500/80 hover:bg-rose-600 text-white font-bold px-4 py-2 rounded-full text-xs transition flex items-center gap-1.5 shadow-md"
          >
            <Power className="w-4 h-4" />
            <span>Encerrar Transmissão</span>
          </button>
        </div>
      </div>

      {/* Map & Member List Layout */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        {/* Map View Container */}
        <div className="lg:col-span-2 bg-white rounded-3xl p-4 shadow-lg border border-slate-100 relative">
          {!isStarted && (
            <div className="absolute inset-0 bg-slate-100/70 backdrop-blur-sm rounded-3xl z-20 flex flex-col items-center justify-center p-6 text-center">
              <ShieldAlert className="w-12 h-12 text-[#ff6b6b] mb-3 animate-bounce-slow" />
              <p className="text-sm font-bold text-[#1a535c] max-w-xs">
                Aguardando confirmação para iniciar o mapa ao vivo...
              </p>
            </div>
          )}
          <div className="w-full h-96 sm:h-[450px] rounded-2xl overflow-hidden shadow-inner relative z-10" ref={mapRef} />
        </div>

        {/* Member Sidebar */}
        <div className="bg-white rounded-3xl p-6 shadow-lg border border-slate-100">
          <h3 className="font-bold text-[#1a535c] text-base mb-4 flex items-center gap-2">
            <Users className="w-5 h-5 text-[#4ecdc4]" />
            Membros no Grupo
          </h3>

          <div className="space-y-3 divide-y divide-slate-100">
            {grupo.membros.map(m => (
              <div key={m.id_usuario} className="pt-3 first:pt-0 flex items-center justify-between">
                <div className="flex items-center gap-2.5">
                  <div className="w-8 h-8 rounded-full bg-[#1a535c] text-white font-bold flex items-center justify-center text-xs">
                    {m.nome.charAt(0)}
                  </div>
                  <div>
                    <h5 className="font-bold text-xs text-[#1a535c]">{m.nome}</h5>
                    <span className="text-[10px] text-slate-400 block">
                      {isStarted && m.lat && m.lng ? 'Localização ativa' : 'Aguardando início do passeio'}
                    </span>
                  </div>
                </div>

                <div className="flex items-center gap-1 text-[10px] font-bold text-[#4ecdc4] bg-[#4ecdc4]/10 px-2 py-0.5 rounded-full">
                  <MapPin className="w-3 h-3 text-[#ff6b6b]" />
                  <span>{isStarted && m.lat ? 'Ao Vivo' : 'Off-line'}</span>
                </div>
              </div>
            ))}
          </div>
        </div>
      </div>
    </div>
  );
};
