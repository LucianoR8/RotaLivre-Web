import React, { useState, useEffect, useRef } from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { PasseioDto, AvaliacaoDto } from '../types';
import { passeioService } from '../services/passeioService';
import { avaliacaoService } from '../services/avaliacaoService';
import {
  ArrowLeft,
  Heart,
  Clock,
  MapPin,
  Star,
  Share2,
  Users,
  MessageSquare,
  Send,
  Check,
  AlertCircle
} from 'lucide-react';
import L from 'leaflet';

export const PasseioDetalhesPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { usuario, getAuthHeader } = useAuth();

  const [passeio, setPasseio] = useState<PasseioDto | null>(null);
  const [avaliacoes, setAvaliacoes] = useState<AvaliacaoDto[]>([]);
  const [loading, setLoading] = useState(true);

  // Avaliação Form State
  const [feedback, setFeedback] = useState('');
  const [submittingReview, setSubmittingReview] = useState(false);
  const [reviewMsg, setReviewMsg] = useState('');

  // Map Container Ref
  const mapRef = useRef<HTMLDivElement>(null);
  const leafletMap = useRef<L.Map | null>(null);

 useEffect(() => {
  const carregarDados = async () => {
    if (!id) {
      setLoading(false);
      return;
    }

    try {
      const idPasseio = Number(id);

      console.log('Buscando passeio com ID:', idPasseio);

      const data = await passeioService.buscarPorId(idPasseio);

      console.log('Dados recebidos do passeio:', data);

      setPasseio(data);

      console.log(
        'Buscando avaliações do passeio:',
        idPasseio
      );

      const avaliacoesData =
        await avaliacaoService.listarPorPasseio(idPasseio);

      console.log(
        'Avaliações recebidas:',
        avaliacoesData
      );

      setAvaliacoes(avaliacoesData || []);

    } catch (err) {
      console.error(
        'Erro ao carregar dados do passeio:',
        err
      );

      setAvaliacoes([]);

    } finally {
      setLoading(false);
    }
  };

  carregarDados();

}, [id]);

  // Leaflet Map Initialization
  useEffect(() => {
    if (passeio?.endereco && mapRef.current && !leafletMap.current) {
      const {
        latitude,
        longitude,
        nomeRua,
        numeroRua,
        bairro
      } = passeio.endereco;
      if (latitude !== undefined &&
          latitude !== null &&
          longitude !== undefined &&
          longitude !== null) {
        const map = L.map(mapRef.current).setView([latitude, longitude], 15);
        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
          attribution: '&copy; OpenStreetMap'
        }).addTo(map);

        L.marker([latitude, longitude])
          .addTo(map)
          .bindPopup(
            `<b>${passeio.nome}</b><br/>${nomeRua}, ${numeroRua} - ${bairro}`)
          .openPopup();

          L.circle([latitude, longitude], {
            radius: passeio.endereco.raioMetros ?? 350,
            fillOpacity: 0.15,
            weight: 2
          }).addTo(map);

        leafletMap.current = map;
      }
    }
    return () => {
      if (leafletMap.current) {
        leafletMap.current.remove();
        leafletMap.current = null;
      }
    };
  }, [passeio]);

  const handleToggleCurtir = async () => {
  if (!passeio) return;

  try {
    const data = await passeioService.curtirPasseio(passeio.id);

    setPasseio(prev =>
      prev
        ? {
            ...prev,
            usuarioJaCurtiu: data.curtiu,
            quantidadeCurtidas: data.totalCurtidas
          }
        : null
    );
  } catch (err) {
    console.error('Erro ao curtir passeio:', err);
  }
};

  const handleCriarAvaliacao = async (
  e: React.FormEvent
) => {

  e.preventDefault();

  if (!feedback.trim()) {
    return;
  }

  if (!id || !usuario) {
    setReviewMsg('É necessário estar logado para avaliar.');
    return;
  }

  try {

    setSubmittingReview(true);
    setReviewMsg('');

    const novaAvaliacao = {
      idPasseio: Number(id),
      idUsuario: usuario.id,
      feedback: feedback.trim()
    };

    console.log(
      'Enviando avaliação:',
      novaAvaliacao
    );

    await avaliacaoService.comentar(
      novaAvaliacao
    );

    setFeedback('');

    setReviewMsg(
      'Avaliação enviada com sucesso!'
    );

    // Recarrega as avaliações diretamente da API
    const avaliacoesAtualizadas =
      await avaliacaoService.listarPorPasseio(
        Number(id)
      );

    setAvaliacoes(
      avaliacoesAtualizadas
    );

    setTimeout(() => {
      setReviewMsg('');
    }, 4000);

  } catch (err) {

    console.error(
      'Erro ao enviar avaliação:',
      err
    );

    setReviewMsg(
      'Não foi possível enviar a avaliação.'
    );

  } finally {

    setSubmittingReview(false);

  }
};

  const handleCriarGrupo = () => {
  if (passeio) {
    navigate(`/criar-grupo-passeio/${passeio.id}`);
  }
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
        <Link to="/" className="bg-[#1a535c] text-white px-6 py-2.5 rounded-full font-bold">
          Voltar para Home
        </Link>
      </div>
    );
  }

  return (
    <div className="pt-20 pb-24 max-w-md mx-auto px-4">
      {/* Top Controls */}
      <div className="flex items-center justify-between mb-6">
        <button
          onClick={() => navigate(-1)}
          className="p-3 bg-white rounded-full text-[#1a535c] hover:bg-[#4ecdc4] hover:text-white transition shadow-md flex items-center gap-2 font-semibold text-sm"
        >
          <ArrowLeft className="w-5 h-5" />
          <span>Voltar</span>
        </button>

        <div className="flex items-center gap-2">
          <button
            onClick={handleToggleCurtir}
            className={`p-3 rounded-full shadow-md transition ${
              passeio.usuarioJaCurtiu ? 'bg-[#ff6b6b] text-white' : 'bg-white text-slate-600 hover:text-[#ff6b6b]'
            }`}
          >
            <Heart className={`w-5 h-5 ${passeio.usuarioJaCurtiu ? 'fill-current' : ''}`} />
          </button>
        </div>
      </div>

      {/* Main Banner Image */}
      <div className="relative h-80 sm:h-96 rounded-3xl overflow-hidden shadow-2xl mb-8">
        <img
          src={passeio.imagemUrl}
          alt={passeio.nome}
          className="w-full h-full object-cover"
        />
        <div className="absolute inset-0 bg-gradient-to-t from-black/80 via-black/20 to-transparent" />

        <div className="absolute bottom-6 left-6 right-6 text-white">
          <div className="flex items-center gap-2 mb-2">
            <span className="bg-[#4ecdc4] text-white font-bold text-xs px-3 py-1 rounded-full uppercase tracking-wider">
              Gratuito / Aberto
            </span>
            <span className="bg-white/20 backdrop-blur-md text-white font-bold text-xs px-3 py-1 rounded-full flex items-center gap-1">
              <Heart className="w-3.5 h-3.5 fill-current text-[#ff6b6b]" />
              {passeio.quantidadeCurtidas} curtidas
            </span>
          </div>

          <h1 className="text-3xl sm:text-5xl font-extrabold tracking-tight drop-shadow-md">
            {passeio.nome}
          </h1>
        </div>
      </div>

      {/* Grid Details */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-8 mb-12">
        {/* Left Column: Info & Map */}
        <div className="lg:col-span-2 space-y-8">
          {/* Funcionamento Box */}
          <div className="bg-white rounded-3xl p-6 shadow-md border border-slate-100 flex items-start gap-4">
            <div className="p-3 bg-[#4ecdc4]/15 rounded-2xl text-[#1a535c]">
              <Clock className="w-6 h-6" />
            </div>
            <div>
              <h3 className="font-bold text-[#1a535c] text-base mb-1">Horário de Funcionamento</h3>
              <p className="text-sm text-slate-600">{passeio.funcionamento}</p>
            </div>
          </div>

          {/* Description */}
          <div className="bg-white rounded-3xl p-8 shadow-md border border-slate-100">
            <h3 className="text-xl font-bold text-[#1a535c] mb-4">Sobre o Passeio</h3>
            <p className="text-slate-600 leading-relaxed text-base whitespace-pre-line">
              {passeio.descricao}
            </p>
          </div>

          {/* Address & Leaflet Map */}
          {passeio.endereco && (
            <div className="bg-white rounded-3xl p-8 shadow-md border border-slate-100">
              <h3 className="text-xl font-bold text-[#1a535c] flex items-center gap-2 mb-4">
                <MapPin className="w-6 h-6 text-[#ff6b6b]" />
                Localização e Endereço
              </h3>

              <div className="bg-[#f5f7fa] p-4 rounded-2xl mb-6 text-sm text-slate-700 space-y-1">
                <p className="font-semibold text-[#1a535c]">
                  {passeio.endereco.nomeRua}, {passeio.endereco.numeroRua}{' '}
                  {passeio.endereco.complemento}
                </p>

                <p className="text-slate-500">
                  Bairro: {passeio.endereco.bairro} | CEP: {passeio.endereco.cep}
                </p>
              </div>

              {/* Interactive Map View */}
              <div className="w-full h-72 rounded-2xl overflow-hidden shadow-inner border border-slate-200 relative z-10" ref={mapRef} />
            </div>
          )}
        </div>

        {/* Right Column: Actions & Groups */}
        <div className="space-y-6">
          {/* Group Invitation Card */}
          <div className="bg-gradient-to-br from-[#1a535c] to-[#236c78] rounded-3xl p-6 text-white shadow-xl">
            <Users className="w-10 h-10 text-[#4ecdc4] mb-3" />
            <h3 className="text-xl font-bold mb-2">Visitar em Grupo</h3>
            <p className="text-xs text-slate-200 mb-6 leading-relaxed">
              Crie um grupo de passeio com amigos para compartilhar a localização em tempo real no mapa!
            </p>
            <button
              onClick={handleCriarGrupo}
              className="w-full bg-[#4ecdc4] hover:bg-[#4ecdc4]/90 text-white font-bold py-3 px-4 rounded-2xl transition shadow-lg flex items-center justify-center gap-2"
            >
              <Users className="w-5 h-5" />
              <span>Criar Grupo para este Passeio</span>
            </button>
          </div>

          {/* Quick Stats Box */}
          <div className="bg-white rounded-3xl p-6 shadow-md border border-slate-100 space-y-4">
            <h4 className="font-bold text-[#1a535c] text-sm uppercase tracking-wider">Ações Rápidas</h4>
            <button
              onClick={handleToggleCurtir}
              className="w-full py-3 px-4 rounded-2xl border-2 border-[#ff6b6b] text-[#ff6b6b] hover:bg-[#ff6b6b] hover:text-white font-bold text-sm transition flex items-center justify-center gap-2"
            >
              <Heart className={`w-4 h-4 ${passeio.usuarioJaCurtiu ? 'fill-current' : ''}`} />
              <span>{passeio.usuarioJaCurtiu ? 'Você curtiu!' : 'Curtir este passeio'}</span>
            </button>
          </div>
        </div>
      </div>

      {/* Avaliações / Reviews Section */}
      <section className="bg-white rounded-3xl p-8 shadow-lg border border-slate-100">
        <h3 className="text-2xl font-bold text-[#1a535c] flex items-center gap-2 mb-6">
          <MessageSquare className="w-6 h-6 text-[#4ecdc4]" />
          Avaliações dos Visitantes ({avaliacoes.length})
        </h3>

        {/* Add Review Form */}
        <form onSubmit={handleCriarAvaliacao} className="bg-[#f5f7fa] p-6 rounded-2xl mb-8 border border-slate-200">
          <h4 className="font-bold text-[#1a535c] mb-3 text-sm">Deixe sua avaliação</h4>

          <textarea
            value={feedback}
            onChange={e => setFeedback(e.target.value)}
            placeholder="Conte-nos como foi sua experiência neste passeio..."
            rows={3}
            required
            className="w-full p-4 rounded-xl border border-slate-300 bg-white text-sm focus:outline-none focus:border-[#4ecdc4] focus:ring-2 focus:ring-[#4ecdc4]/20 mb-3"
          />

          <button
            type="submit"
            disabled={submittingReview}
            className="bg-[#1a535c] hover:bg-[#1a535c]/90 text-white font-bold py-2.5 px-6 rounded-xl text-sm transition flex items-center gap-2 shadow-md"
          >
            <Send className="w-4 h-4" />
            <span>{submittingReview ? 'Enviando...' : 'Publicar Avaliação'}</span>
          </button>
        </form>

        {/* Reviews List */}
<div className="space-y-4 divide-y divide-slate-100">

  {avaliacoes.length > 0 ? (

    avaliacoes.map((a, index) => (

      <div
        key={`${a.nomeUsuario}-${a.data}-${index}`}
        className="pt-4 first:pt-0"
      >

        <div className="flex items-center gap-2 mb-2">

          <div className="w-9 h-9 rounded-full bg-[#1a535c] text-white font-bold flex items-center justify-center text-xs">
            {a.nomeUsuario.charAt(0).toUpperCase()}
          </div>

          <div>

            <h5 className="font-bold text-sm text-[#1a535c]">
              {a.nomeUsuario}
            </h5>

            <span className="text-[11px] text-slate-400">
              {new Date(a.data).toLocaleDateString('pt-BR')}
            </span>

          </div>

        </div>

        <p className="text-sm text-slate-600 leading-relaxed pl-11">
          {a.feedback}
        </p>

      </div>

    ))

  ) : (

    <p className="text-center py-6 text-sm text-slate-400">
      Seja o primeiro a avaliar este passeio!
    </p>

  )}

</div>
      </section>
    </div>
  );
};
