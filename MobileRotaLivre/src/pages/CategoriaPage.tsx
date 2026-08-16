import React, { useState, useEffect } from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
import { PasseioDto } from '../types';
import { passeioService } from '../services/passeioService';
import {
  MapPin,
  Heart,
  ArrowLeft,
  Compass,
  Sparkles
} from 'lucide-react';

export const CategoriaPage: React.FC = () => {

  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();

  const [passeios, setPasseios] = useState<PasseioDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [curtindo, setCurtindo] = useState<number | null>(null);
  const [categoriaNome, setCategoriaNome] = useState('');
  useEffect(() => {

    const carregar = async () => {

      if (!id) {
        console.warn(
          '[CategoriaPage] Nenhum ID encontrado na URL.'
        );

        setLoading(false);
        return;
      }

      const categoriaId = Number(id);

      console.log(
        '[CategoriaPage] ID recebido:',
        id
      );

      console.log(
        '[CategoriaPage] ID convertido:',
        categoriaId
      );

      if (Number.isNaN(categoriaId)) {

        console.error(
          '[CategoriaPage] ID inválido:',
          id
        );

        setPasseios([]);
        setLoading(false);

        return;
      }

      try {

        setLoading(true);

        console.log(
          '[CategoriaPage] Buscando passeios da categoria:',
          categoriaId
        );

        const dados =
          await passeioService.buscarPorCategoria(
            categoriaId
          );

        console.log(
          '[CategoriaPage] Passeios recebidos:',
          dados
        );

        console.log(
  '[CategoriaPage] Quantidade:',
  dados?.length ?? 0
);

if (dados && dados.length > 0) {
  console.log(
    '[CategoriaPage] Nome da categoria:',
    dados[0].categoriaNome
  );

  setCategoriaNome(dados[0].categoriaNome);
}

setPasseios(dados || []);

      } catch (err) {

        console.error(
          '[CategoriaPage] Erro ao buscar passeios da categoria:',
          err
        );

        setPasseios([]);

      } finally {

        setLoading(false);

      }
    };

    carregar();

  }, [id]);

  const handleToggleCurtida = async (
    e: React.MouseEvent,
    idPasseio: number
  ) => {

    e.stopPropagation();

    if (curtindo === idPasseio) {
      return;
    }

    try {

      setCurtindo(idPasseio);

      console.log(
        '[CategoriaPage] Alternando curtida:',
        idPasseio
      );

      const data =
        await passeioService.alternarCurtida(
          idPasseio
        );

      console.log(
        '[CategoriaPage] Resultado:',
        data
      );

      setPasseios(prev =>
        prev.map(p =>
          p.id === idPasseio
            ? {
                ...p,
                usuarioJaCurtiu: data.curtiu,
                quantidadeCurtidas:
                  data.totalCurtidas
              }
            : p
        )
      );

    } catch (err) {

      console.error(
        '[CategoriaPage] Erro ao curtir passeio:',
        err
      );

    } finally {

      setCurtindo(null);

    }
  };

  if (loading) {

    return (
      <div className="pt-32 pb-28 flex justify-center items-center">

        <div className="w-12 h-12 border-4 border-[#4ecdc4] border-t-transparent rounded-full animate-spin" />

      </div>
    );
  }

  return (
    <div className="pt-20 pb-24 max-w-6xl mx-auto px-4">

      {/* Header */}

      <div className="flex items-center gap-4 mb-8">

        <Link
          to="/"
          className="p-3 bg-white rounded-full text-[#1a535c] hover:bg-[#4ecdc4] hover:text-white transition shadow-md flex items-center justify-center"
        >
          <ArrowLeft className="w-5 h-5" />
        </Link>

        <div>

          <span className="text-xs font-bold text-[#ff6b6b] uppercase tracking-wider flex items-center gap-1">

            <Compass className="w-4 h-4" />

            Categoria

          </span>

          <h1 className="text-3xl font-extrabold text-[#1a535c]">

            {categoriaNome || 'Passeios'}

          </h1>

        </div>

      </div>

      {/* Passeios */}

      {passeios.length > 0 ? (

        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">

          {passeios.map(p => (

            <div
              key={p.id}
              onClick={() =>
                navigate(`/passeio/${p.id}`)
              }
              className="group bg-white rounded-3xl overflow-hidden shadow-lg hover:shadow-2xl transition-all duration-300 cursor-pointer hover:-translate-y-2 border border-slate-100 flex flex-col justify-between"
            >

              <div>

                {/* Imagem */}

                <div className="relative h-56 overflow-hidden">

                  <img
                    src={p.imagemUrl}
                    alt={p.nome}
                    className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-700"
                    onError={(e) => {
                      e.currentTarget.style.display = 'none';
                    }}
                  />

                  {/* Curtida */}

                  <button
                    type="button"
                    onClick={e =>
                      handleToggleCurtida(
                        e,
                        p.id
                      )
                    }
                    disabled={
                      curtindo === p.id
                    }
                    className={`absolute top-3 right-3 p-2.5 rounded-full backdrop-blur-md transition shadow-md ${
                      p.usuarioJaCurtiu
                        ? 'bg-[#ff6b6b] text-white'
                        : 'bg-white/80 text-slate-600 hover:text-[#ff6b6b]'
                    } ${
                      curtindo === p.id
                        ? 'opacity-50'
                        : ''
                    }`}
                  >

                    <Heart
                      className={`w-5 h-5 ${
                        p.usuarioJaCurtiu
                          ? 'fill-current'
                          : ''
                      }`}
                    />

                  </button>

                </div>

                {/* Informações */}

                <div className="p-6">

                  <h3 className="text-xl font-bold text-[#1a535c] flex items-center gap-2 mb-2">

                    <MapPin className="w-5 h-5 text-[#ff6b6b] shrink-0" />

                    <span>
                      {p.nome}
                    </span>

                  </h3>

                  <p className="text-xs text-slate-500 line-clamp-3 mb-4">

                    {p.descricao}

                  </p>

                </div>

              </div>

              {/* Rodapé */}

              <div className="px-6 pb-6 pt-2 border-t border-slate-50 flex items-center justify-between text-xs text-slate-500 font-semibold">

                <span className="truncate max-w-[180px]">

                  {p.funcionamento}

                </span>

                <span className="flex items-center gap-1 text-[#ff6b6b]">

                  <Heart className="w-4 h-4 fill-current" />

                  {p.quantidadeCurtidas}

                </span>

              </div>

            </div>

          ))}

        </div>

      ) : (

        <div className="bg-white rounded-3xl p-12 text-center shadow-lg max-w-lg mx-auto">

          <Sparkles className="w-12 h-12 text-[#4ecdc4] mx-auto mb-4" />

          <h3 className="text-xl font-bold text-[#1a535c] mb-2">

            Nenhum passeio encontrado

          </h3>

          <p className="text-sm text-slate-500 mb-6">

            Ainda não há passeios cadastrados nesta categoria.

          </p>

          <Link
            to="/"
            className="inline-flex items-center gap-2 bg-[#1a535c] text-white px-6 py-3 rounded-full font-bold text-sm hover:bg-[#1a535c]/90 transition"
          >

            Voltar para Home

          </Link>

        </div>

      )}

    </div>
  );
};