import React, { useEffect, useRef, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';

import {
  Search,
  Compass,
  Star,
  Heart,
  MapPin,
  Tag,
  Trophy,
  ArrowRight,
  Sparkles,
  ChevronLeft,
  ChevronRight,
} from 'lucide-react';

import { homeService } from '../services/homeService';
import { passeioService } from '../services/passeioService';

import {
  CategoriaHomeDto,
  PasseioHomeDto,
  PasseioDto,
} from '../types';

export const HomePage: React.FC = () => {
  const navigate = useNavigate();

  const [nomeUsuario, setNomeUsuario] = useState('');

  const [categorias, setCategorias] =
    useState<CategoriaHomeDto[]>([]);

  const [destaques, setDestaques] =
    useState<PasseioHomeDto[]>([]);

  const [favoritados, setFavoritados] =
    useState<PasseioHomeDto[]>([]);

  const [searchTerm, setSearchTerm] = useState('');

  const [searchResults, setSearchResults] =
    useState<PasseioDto[]>([]);

  const [isSearching, setIsSearching] =
    useState(false);

  const [loading, setLoading] = useState(true);

  const [error, setError] =
    useState<string | null>(null);

  const categoriasRef =
    useRef<HTMLDivElement>(null);

  const destaquesRef =
    useRef<HTMLDivElement>(null);

  const favoritadosRef =
    useRef<HTMLDivElement>(null);


  // ==========================================
  // CARREGAR DADOS DA HOME
  // ==========================================

  const carregarDadosHome = async () => {
    try {
      setLoading(true);
      setError(null);

      const dados = await homeService.carregarHome();

      console.log(
        'Dados recebidos da Home:',
        dados
      );

      setNomeUsuario(
        dados.nomeUsuario ?? ''
      );

      setCategorias(
        dados.categorias ?? []
      );

      setDestaques(
        dados.destaques ?? []
      );

      setFavoritados(
        dados.favoritados ?? []
      );
    } catch (err: any) {
      console.error(
        'Erro ao carregar dados da Home:',
        err
      );

      if (err.response?.status === 401) {
        setError(
          'Sua sessão expirou. Faça login novamente.'
        );
      } else {
        setError(
          'Não foi possível carregar a Home.'
        );
      }
    } finally {
      setLoading(false);
    }
  };


  // ==========================================
  // CARREGAMENTO INICIAL
  // ==========================================

  useEffect(() => {
    carregarDadosHome();
  }, []);


  // ==========================================
  // SCROLL DOS CARROSSÉIS
  // ==========================================

  const scroll = (
    ref: React.RefObject<HTMLDivElement | null>,
    offset: number
  ) => {
    if (ref.current) {
      ref.current.scrollBy({
        left: offset,
        behavior: 'smooth',
      });
    }
  };


  // ==========================================
  // CURTIR / DESCURTIR
  // ==========================================

  const handleToggleCurtida = async (
    e: React.MouseEvent,
    idPasseio: number
  ) => {
    e.stopPropagation();

    try {
      await passeioService.curtirPasseio(
        idPasseio
      );

      // Recarregamos a Home porque a curtida
      // pode alterar:
      //
      // 1. quantidade de curtidas
      // 2. usuarioJaCurtiu
      // 3. lista de favoritados
      //
      await carregarDadosHome();

    } catch (err) {
      console.error(
        'Erro ao alterar curtida:',
        err
      );
    }
  };


  // ==========================================
  // BUSCA
  // ==========================================

  const handleSearchChange = async (
    e: React.ChangeEvent<HTMLInputElement>
  ) => {
    const valor = e.target.value;

    setSearchTerm(valor);

    if (!valor.trim()) {
      setSearchResults([]);
      setIsSearching(false);
      return;
    }

    try {
      setIsSearching(true);

      const resultados =
        await passeioService.buscarPorNome(
          valor.trim()
        );

      setSearchResults(
        resultados
      );
    } catch (err) {
      console.error(
        'Erro na busca:',
        err
      );

      setSearchResults([]);
    }
  };


  // ==========================================
  // PORCENTAGEM DE CURTIDAS
  // ==========================================

  const maxCurtidas =
    destaques.length > 0
      ? Math.max(
          ...destaques.map(
            p => p.quantidadeCurtidas
          )
        )
      : 1;


  // ==========================================
  // LOADING
  // ==========================================

  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="text-center">
          <p className="text-lg font-semibold text-[#1a535c]">
            Carregando tela inicial...
          </p>
        </div>
      </div>
    );
  }


  // ==========================================
  // ERRO
  // ==========================================

  if (error) {
    return (
      <div className="min-h-screen flex items-center justify-center px-4">
        <div className="bg-white rounded-3xl shadow-lg p-8 text-center max-w-md">
          <h2 className="text-xl font-bold text-red-600 mb-3">
            Erro ao carregar a Home
          </h2>

          <p className="text-slate-600 mb-6">
            {error}
          </p>

          <button
            onClick={carregarDadosHome}
            className="px-6 py-3 rounded-full bg-[#1a535c] text-white font-bold"
          >
            Tentar novamente
          </button>
        </div>
      </div>
    );
  }


  // ==========================================
  // HOME
  // ==========================================

  return (
    <div className="pt-20 pb-24 max-w-4xl mx-auto px-4">


      {/* ======================================
          SAUDAÇÃO
      ======================================= */}

      <div className="mb-8 animate-fadeIn">
        <h1 className="text-xs sm:text-sm font-semibold text-[#1a535c] uppercase tracking-widest mb-1">
          Olá {nomeUsuario
            ? nomeUsuario.split(' ')[0]
            : 'Visitante'}
        </h1>

        <div className="relative inline-block">
          <p className="text-3xl sm:text-5xl font-extrabold text-[#1a535c] tracking-tight">
            Pra onde iremos hoje?{' '}
            <span className="inline-block animate-bounce-slow">
              🌍
            </span>
          </p>

          <div className="h-1.5 w-36 bg-gradient-to-r from-[#1a535c] via-[#4ecdc4] to-[#ff6b6b] rounded-full mt-2" />
        </div>
      </div>


      {/* ======================================
          BUSCA
      ======================================= */}

      <div className="relative max-w-2xl mx-auto my-8">

        <div className="relative flex items-center">

          <input
            type="text"
            value={searchTerm}
            onChange={handleSearchChange}
            placeholder="🔍 Para onde deseja ir? Digite um destino..."
            className="w-full pl-6 pr-16 py-4 rounded-full border-2 border-transparent bg-white shadow-lg text-slate-700 text-base focus:outline-none focus:border-[#4ecdc4] focus:ring-4 focus:ring-[#4ecdc4]/20 transition-all"
          />

          <button
            type="button"
            className="absolute right-2 p-3 bg-gradient-to-r from-[#1a535c] to-[#4ecdc4] text-white rounded-full hover:scale-105 transition shadow-md"
          >
            <Search className="w-5 h-5" />
          </button>

        </div>


        {/* RESULTADOS DA BUSCA */}

        {isSearching && (
          <div className="mt-4 bg-white rounded-2xl p-4 shadow-xl border border-slate-100 divide-y divide-slate-100 max-h-96 overflow-y-auto">

            {searchResults.length > 0 ? (

              searchResults.map(passeio => (

                <div
                  key={passeio.id}
                  onClick={() =>
                    navigate(
                      `/passeio/${passeio.id}`
                    )
                  }
                  className="py-3 px-2 flex items-center justify-between hover:bg-slate-50 cursor-pointer rounded-xl transition"
                >

                  <div className="flex items-center gap-3">

                    {passeio.imagemUrl && (
                      <img
                        src={passeio.imagemUrl}
                        alt={passeio.nome}
                        className="w-12 h-12 rounded-lg object-cover"
                      />
                    )}

                    <div>
                      <h4 className="font-bold text-[#1a535c]">
                        {passeio.nome}
                      </h4>

                      <p className="text-xs text-slate-500 line-clamp-1">
                        {passeio.descricao}
                      </p>
                    </div>

                  </div>

                  <div className="flex items-center gap-1 text-xs text-[#ff6b6b] font-semibold">

                    <Heart className="w-3.5 h-3.5 fill-current" />

                    <span>
                      {passeio.quantidadeCurtidas}
                    </span>

                  </div>

                </div>

              ))

            ) : (

              <p className="text-center py-4 text-sm text-slate-500">
                Nenhum passeio encontrado para "{searchTerm}".
              </p>

            )}

          </div>
        )}

      </div>


      {/* ======================================
          CATEGORIAS
      ======================================= */}

      <section className="my-10">

        <div className="flex items-center justify-between mb-6">

          <h2 className="text-2xl font-bold text-[#1a535c] flex items-center gap-2">

            <Compass className="w-7 h-7 text-[#ff6b6b] animate-bounce-slow" />

            Categorias

          </h2>

          <div className="flex items-center gap-2">

            <button
              type="button"
              onClick={() =>
                scroll(categoriasRef, -320)
              }
              aria-label="Anterior"
              className="p-2.5 rounded-full bg-white text-[#1a535c] border border-slate-200 hover:bg-[#4ecdc4] hover:text-white transition shadow-sm"
            >
              <ChevronLeft className="w-5 h-5" />
            </button>

            <button
              type="button"
              onClick={() =>
                scroll(categoriasRef, 320)
              }
              aria-label="Próximo"
              className="p-2.5 rounded-full bg-white text-[#1a535c] border border-slate-200 hover:bg-[#4ecdc4] hover:text-white transition shadow-sm"
            >
              <ChevronRight className="w-5 h-5" />
            </button>

          </div>

        </div>


        <div
          ref={categoriasRef}
          className="flex gap-4 overflow-x-auto no-scrollbar scroll-smooth snap-x snap-mandatory py-2 px-0.5"
        >

          {categorias.map(categoria => (

            <Link
              key={categoria.idCategoria}
              to={`/categoria/${categoria.idCategoria}`}
              className="group relative h-64 w-[75vw] max-w-[260px] shrink-0 snap-start rounded-3xl overflow-hidden shadow-md bg-white transition-all duration-300 hover:shadow-xl"
            >

              <img
                src={categoria.imgUrl}
                alt={categoria.tipoCategoria}
                className="w-full h-full object-cover transition-transform duration-700 group-hover:scale-110"
              />

              <div className="absolute inset-0 bg-gradient-to-t from-[#1a535c]/90 via-[#1a535c]/40 to-transparent" />

              <div className="absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 opacity-0 group-hover:opacity-100 scale-90 group-hover:scale-100 transition-all duration-300 bg-gradient-to-r from-[#4ecdc4] to-[#ff6b6b] text-white px-6 py-3 rounded-full font-bold text-sm tracking-wider flex items-center gap-2 shadow-xl border-2 border-white">

                <span>EXPLORAR</span>

                <ArrowRight className="w-4 h-4" />

              </div>

              <div className="absolute bottom-0 left-0 right-0 p-6">

                <h3 className="text-2xl font-bold text-white flex items-center gap-2">

                  <Tag className="w-5 h-5 text-[#4ecdc4]" />

                  {categoria.tipoCategoria}

                </h3>

                <div className="h-1 w-12 bg-[#4ecdc4] rounded-full mt-2 group-hover:w-24 transition-all duration-300" />

              </div>

            </Link>

          ))}

        </div>

      </section>


      {/* ======================================
          PASSEIOS EM DESTAQUE
      ======================================= */}

      <section className="my-12">

        <div className="flex items-center justify-between mb-6">

          <h2 className="text-2xl font-bold text-[#1a535c] flex items-center gap-2">

            <Star className="w-7 h-7 text-[#ff6b6b] fill-current animate-bounce-slow" />

            Passeios em Destaque

          </h2>

          <div className="flex items-center gap-2">

            <button
              type="button"
              onClick={() =>
                scroll(destaquesRef, -320)
              }
              aria-label="Anterior"
              className="p-2.5 rounded-full bg-white text-[#1a535c] border border-slate-200 hover:bg-[#4ecdc4] hover:text-white transition shadow-sm"
            >
              <ChevronLeft className="w-5 h-5" />
            </button>

            <button
              type="button"
              onClick={() =>
                scroll(destaquesRef, 320)
              }
              aria-label="Próximo"
              className="p-2.5 rounded-full bg-white text-[#1a535c] border border-slate-200 hover:bg-[#4ecdc4] hover:text-white transition shadow-sm"
            >
              <ChevronRight className="w-5 h-5" />
            </button>

          </div>

        </div>


        <div
          ref={destaquesRef}
          className="flex gap-4 overflow-x-auto no-scrollbar scroll-smooth snap-x snap-mandatory py-2 px-0.5"
        >

          {destaques.map(passeio => {

            const porcentagem =
              maxCurtidas > 0
                ? Math.round(
                    (passeio.quantidadeCurtidas * 100) /
                    maxCurtidas
                  )
                : 0;

            return (

              <div
                key={passeio.id}
                onClick={() =>
                  navigate(
                    `/passeio/${passeio.id}`
                  )
                }
                className="group w-[78vw] max-w-[270px] shrink-0 snap-start bg-white rounded-3xl overflow-hidden shadow-md hover:shadow-xl transition-all duration-300 cursor-pointer border border-slate-100"
              >

                <div className="relative h-52 overflow-hidden">

                  {passeio.imagemUrl && (
                    <img
                      src={passeio.imagemUrl}
                      alt={passeio.nome}
                      className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-700"
                    />
                  )}

                  <div className="absolute top-3 right-3 bg-white/90 backdrop-blur-md px-3 py-1 rounded-full text-xs font-bold text-[#ff6b6b] flex items-center gap-1 shadow-sm">

                    <Heart className="w-3.5 h-3.5 fill-current" />

                    <span>
                      {passeio.quantidadeCurtidas}
                    </span>

                  </div>

                </div>


                <div className="p-6">

                  <h3 className="text-xl font-bold text-[#1a535c] flex items-center gap-2 mb-2">

                    <MapPin className="w-5 h-5 text-[#ff6b6b] shrink-0" />

                    <span className="truncate">
                      {passeio.nome}
                    </span>

                  </h3>


                  <p className="text-xs text-slate-500 line-clamp-2 mb-4">
                    {passeio.descricao}
                  </p>


                  <div className="space-y-1.5 pt-2 border-t border-slate-100">

                    <div className="flex items-center justify-between text-xs font-semibold text-[#1a535c]">

                      <span className="flex items-center gap-1">

                        <Heart className="w-4 h-4 text-[#ff6b6b] fill-current" />

                        {passeio.quantidadeCurtidas} curtidas

                      </span>

                      <span className="text-slate-400 font-medium">
                        {porcentagem}% do topo
                      </span>

                    </div>


                    <div className="w-full h-2.5 bg-[#4ecdc4]/20 rounded-full overflow-hidden">

                      <div
                        className="h-full bg-gradient-to-r from-[#4ecdc4] to-[#ff6b6b] rounded-full transition-all duration-1000"
                        style={{
                          width: `${porcentagem}%`,
                        }}
                      />

                    </div>


                    <div className="flex items-center justify-between text-[11px] text-slate-400 pt-1">

                      <span className="flex items-center gap-1">

                        <Sparkles className="w-3 h-3 text-[#4ecdc4]" />

                        Populares

                      </span>

                      <span className="flex items-center gap-1">

                        <Trophy className="w-3 h-3 text-amber-500" />

                        Meta: {maxCurtidas}

                      </span>

                    </div>

                  </div>

                </div>

              </div>

            );
          })}

        </div>

      </section>


      {/* ======================================
          FAVORITADOS
      ======================================= */}

      <section className="my-12">

        <div className="flex items-center justify-between mb-6">

          <h2 className="text-2xl font-bold text-[#1a535c] flex items-center gap-2">

            <Heart className="w-7 h-7 text-[#ff6b6b] fill-current animate-bounce-slow" />

            Favoritados

          </h2>


          {favoritados.length > 0 && (

            <div className="flex items-center gap-2">

              <button
                type="button"
                onClick={() =>
                  scroll(favoritadosRef, -320)
                }
                aria-label="Anterior"
                className="p-2.5 rounded-full bg-white text-[#1a535c] border border-slate-200 hover:bg-[#4ecdc4] hover:text-white transition shadow-sm"
              >
                <ChevronLeft className="w-5 h-5" />
              </button>

              <button
                type="button"
                onClick={() =>
                  scroll(favoritadosRef, 320)
                }
                aria-label="Próximo"
                className="p-2.5 rounded-full bg-white text-[#1a535c] border border-slate-200 hover:bg-[#4ecdc4] hover:text-white transition shadow-sm"
              >
                <ChevronRight className="w-5 h-5" />
              </button>

            </div>

          )}

        </div>


        {favoritados.length > 0 ? (

          <div
            ref={favoritadosRef}
            className="flex gap-4 overflow-x-auto no-scrollbar scroll-smooth snap-x snap-mandatory py-2 px-0.5"
          >

            {favoritados.map(passeio => (

              <div
                key={passeio.id}
                onClick={() =>
                  navigate(
                    `/passeio/${passeio.id}`
                  )
                }
                className="group w-[78vw] max-w-[270px] shrink-0 snap-start bg-white rounded-3xl overflow-hidden shadow-md hover:shadow-xl transition-all duration-300 cursor-pointer border border-slate-100"
              >

                <div className="relative h-52 overflow-hidden">

                  {passeio.imagemUrl && (
                    <img
                      src={passeio.imagemUrl}
                      alt={passeio.nome}
                      className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-700"
                    />
                  )}


                  {/* BOTÃO DESCURTIR */}

                  <button
                    type="button"
                    onClick={(e) =>
                      handleToggleCurtida(
                        e,
                        passeio.id
                      )
                    }
                    title="Descurtir"
                    className="absolute top-3 right-3 p-2.5 rounded-full bg-[#ff6b6b] text-white transition shadow-md hover:scale-110"
                  >

                    <Heart className="w-5 h-5 fill-current" />

                  </button>

                </div>


                <div className="p-6">

                  <h3 className="text-xl font-bold text-[#1a535c] flex items-center gap-2 mb-2">

                    <MapPin className="w-5 h-5 text-[#ff6b6b] shrink-0" />

                    <span className="truncate">
                      {passeio.nome}
                    </span>

                  </h3>


                  <p className="text-xs text-slate-500 line-clamp-2 mb-4">
                    {passeio.descricao}
                  </p>

                </div>


                <div className="px-6 pb-6 pt-2 border-t border-slate-50 flex items-center justify-between text-xs text-slate-500 font-semibold">

                  <span className="truncate max-w-[180px]">
                    {passeio.funcionamento}
                  </span>

                  <span className="flex items-center gap-1 text-[#ff6b6b]">

                    <Heart className="w-4 h-4 fill-current" />

                    {passeio.quantidadeCurtidas}

                  </span>

                </div>

              </div>

            ))}

          </div>

        ) : (

          <div className="bg-white rounded-3xl p-8 text-center shadow-md border border-slate-100">

            <Sparkles className="w-10 h-10 text-[#4ecdc4] mx-auto mb-3" />

            <p className="text-sm text-slate-600 font-medium">

              Você ainda não possui nenhum passeio favoritado.
              Curta um passeio para adicioná-lo aqui!

            </p>

          </div>

        )}

      </section>

    </div>
  );
};