import { useState } from 'react';
import { Tour, Category, ActiveScreen } from '../types';
import {
  Plus,
  Search,
  Edit3,
  Trash2,
  Compass,
  Filter,
  Calendar,
} from 'lucide-react';

interface ToursListProps {
  tours: Tour[];
  categories: Category[];
  onNavigate: (screen: ActiveScreen) => void;
  onEdit: (tour: Tour) => void;
  onRequestDelete: (tour: Tour) => void;
}

/**
 * Resolve a categoria do passeio considerando
 * tanto o formato do frontend quanto o formato
 * atual retornado pela API.
 */
function getTourCategoryName(
  tour: Tour,
  categories: Category[]
): string {
  const tourApiData = tour as Tour & {
    categoriaId?: number;
    categoriaNome?: string;
  };

  // Formato do frontend
  if (tour.categoryName) {
    return tour.categoryName;
  }

  // Formato atual da API
  if (tourApiData.categoriaNome) {
    return tourApiData.categoriaNome;
  }

  // Tenta encontrar pelo ID
  const categoryId =
    tour.categoryId ??
    tourApiData.categoriaId;

  if (
    categoryId !== undefined &&
    categoryId !== null
  ) {
    const category = categories.find(
      (cat) =>
        Number(cat.id) ===
        Number(categoryId)
    );

    if (category) {
      return category.name;
    }
  }

  return 'Sem categoria';
}

function getTourCategoryId(
  tour: Tour
): number | null {
  const tourApiData = tour as Tour & {
    categoriaId?: number;
  };

  const id =
    tour.categoryId ??
    tourApiData.categoriaId;

  if (
    id === undefined ||
    id === null
  ) {
    return null;
  }

  return Number(id);
}

export function ToursList({
  tours,
  categories,
  onNavigate,
  onEdit,
  onRequestDelete,
}: ToursListProps) {
  const [searchTerm, setSearchTerm] =
    useState('');

  const [selectedCategory, setSelectedCategory] =
    useState('all');

  const filteredTours = tours.filter(
    (tour) => {
      /*
       * Busca somente por:
       * - nome
       * - ID
       *
       * Cidade e estado foram removidos.
       */
      const matchesSearch =
        tour.name
          .toLowerCase()
          .includes(
            searchTerm.toLowerCase()
          ) ||
        String(tour.id)
          .toLowerCase()
          .includes(
            searchTerm.toLowerCase()
          );

      const tourCategoryId =
        getTourCategoryId(tour);

      const matchesCategory =
        selectedCategory === 'all' ||
        tourCategoryId ===
          Number(selectedCategory);

      return (
        matchesSearch &&
        matchesCategory
      );
    }
  );

  return (
    <div
      id="tours-list-screen"
      className="space-y-5"
    >
      {/* Cabeçalho */}
      <div className="bg-white p-5 rounded-2xl border border-slate-200 shadow-sm flex flex-col sm:flex-row items-start sm:items-center justify-between gap-4">
        <div>
          <div className="flex items-center gap-2">
            <h2 className="text-xl font-extrabold text-slate-800">
              Passeios e Atrações Turísticas
            </h2>

            <span className="px-2.5 py-0.5 rounded-full text-xs font-bold bg-[#1a535c]/10 text-[#1a535c]">
              {tours.length} passeios
            </span>
          </div>

          <p className="text-xs text-slate-500 mt-1">
            Cadastre roteiros, gerencie disponibilidades de horários e endereços
          </p>
        </div>

        <button
          id="btn-new-tour-list"
          onClick={() =>
            onNavigate('tour-new')
          }
          className="w-full sm:w-auto inline-flex items-center justify-center gap-2 bg-[#1a535c] hover:bg-[#154249] text-white font-bold text-sm px-4 py-2.5 rounded-xl shadow-sm transition-all active:scale-98 cursor-pointer"
        >
          <Plus className="w-4 h-4 text-[#4ecdc4]" />

          <span>
            Novo Passeio
          </span>
        </button>
      </div>

      {/* Busca e filtro */}
      <div className="bg-white p-4 rounded-2xl border border-slate-200 shadow-sm flex flex-col md:flex-row items-stretch md:items-center gap-3">

        <div className="relative flex-1">
          <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none text-slate-400">
            <Search className="w-4 h-4" />
          </div>

          <input
            id="input-search-tours"
            type="text"
            value={searchTerm}
            onChange={(e) =>
              setSearchTerm(
                e.target.value
              )
            }
            placeholder="Buscar por nome do passeio ou ID..."
            className="w-full pl-9 pr-4 py-2 bg-slate-50 border border-slate-200 rounded-xl text-sm text-slate-800 placeholder-slate-400 focus:outline-none focus:ring-2 focus:ring-[#4ecdc4] focus:border-[#1a535c]"
          />
        </div>

        <div className="flex items-center gap-2">
          <div className="relative shrink-0">
            <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none text-slate-400">
              <Filter className="w-4 h-4" />
            </div>

            <select
              id="select-filter-category"
              value={
                selectedCategory
              }
              onChange={(e) =>
                setSelectedCategory(
                  e.target.value
                )
              }
              className="pl-9 pr-8 py-2 bg-slate-50 border border-slate-200 rounded-xl text-sm text-slate-700 font-medium focus:outline-none focus:ring-2 focus:ring-[#4ecdc4] focus:border-[#1a535c] appearance-none"
            >
              <option value="all">
                Todas as Categorias
              </option>

              {categories.map(
                (cat) => (
                  <option
                    key={cat.id}
                    value={cat.id}
                  >
                    {cat.name}
                  </option>
                )
              )}
            </select>
          </div>

          {(searchTerm ||
            selectedCategory !==
              'all') && (
            <button
              onClick={() => {
                setSearchTerm('');
                setSelectedCategory(
                  'all'
                );
              }}
              className="text-xs text-slate-500 hover:text-slate-800 font-semibold px-2 py-2 cursor-pointer"
            >
              Limpar
            </button>
          )}
        </div>
      </div>

      {/* Tabela */}
      <div className="bg-white rounded-2xl border border-slate-200 shadow-sm overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full text-left border-collapse">

            <thead>
              <tr className="border-b border-slate-200 bg-slate-50/80 text-[11px] font-bold text-slate-500 uppercase tracking-wider">
                <th className="py-3.5 px-4 lg:px-6">
                  ID
                </th>

                <th className="py-3.5 px-4 lg:px-6">
                  Foto
                </th>

                <th className="py-3.5 px-4 lg:px-6">
                  Nome do Passeio
                </th>

                <th className="py-3.5 px-4 lg:px-6">
                  Categoria
                </th>

                <th className="py-3.5 px-4 lg:px-6 hidden lg:table-cell">
                  Agenda
                </th>

                <th className="py-3.5 px-4 lg:px-6 text-right">
                  Ações
                </th>
              </tr>
            </thead>

            <tbody className="divide-y divide-slate-100 text-sm">
              {filteredTours.length ===
              0 ? (
                <tr>
                  <td
                    colSpan={6}
                    className="py-12 text-center text-slate-400"
                  >
                    <Compass className="w-10 h-10 mx-auto text-slate-300 mb-2" />

                    <p className="font-semibold text-slate-600">
                      Nenhum passeio encontrado
                    </p>

                    <p className="text-xs text-slate-400 mt-0.5">
                      Verifique os filtros aplicados ou cadastre um novo passeio.
                    </p>
                  </td>
                </tr>
              ) : (
                filteredTours.map(
                  (tour) => (
                    <tr
                      key={tour.id}
                      id={`tour-row-${tour.id}`}
                      className="hover:bg-slate-50/80 transition-colors group"
                    >
                      {/* ID */}
                      <td className="py-4 px-4 lg:px-6 font-mono text-xs font-bold text-slate-500">
                        {tour.id}
                      </td>

                      {/* Foto */}
                      <td className="py-4 px-4 lg:px-6">
                        <div className="relative w-14 h-14 rounded-xl overflow-hidden border border-slate-200 bg-slate-100 shrink-0">
                          <img
                            src={
                              tour.photoUrl
                            }
                            alt={
                              tour.name
                            }
                            referrerPolicy="no-referrer"
                            className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-200"
                          />
                        </div>
                      </td>

                      {/* Nome */}
                      <td className="py-4 px-4 lg:px-6">
                        <div className="font-bold text-slate-800 group-hover:text-[#1a535c] transition-colors">
                          {tour.name}
                        </div>
                      </td>

                      {/* Categoria */}
                      <td className="py-4 px-4 lg:px-6">
                        <span className="inline-flex items-center px-2.5 py-1 rounded-full text-xs font-bold bg-[#1a535c]/10 text-[#1a535c]">
                          {getTourCategoryName(
                            tour,
                            categories
                          )}
                        </span>
                      </td>

                      {/* Agenda */}
                      <td className="py-4 px-4 lg:px-6 hidden lg:table-cell">
                        <div className="flex items-center gap-1.5">
                          <Calendar className="w-4 h-4 text-[#4ecdc4]" />

                          <span className="text-xs font-semibold text-slate-700">
                            {tour
                              .availabilities
                              ?.length ||
                              0}{' '}
                            datas cadastradas
                          </span>
                        </div>

                        <div className="flex flex-wrap gap-1 mt-1 max-w-[180px]">
                          {tour.availabilities
                            ?.slice(0, 2)
                            .map(
                              (
                                av,
                                idx
                              ) => (
                                <span
                                  key={
                                    idx
                                  }
                                  className="text-[10px] bg-slate-100 text-slate-600 px-1.5 py-0.5 rounded font-mono font-medium"
                                >
                                  {av.date
                                    .split(
                                      '-'
                                    )
                                    .reverse()
                                    .join(
                                      '/'
                                    )}{' '}
                                  (
                                  {
                                    av
                                      .timeSlots
                                      .length
                                  }{' '}
                                  horários)
                                </span>
                              )
                            )}
                        </div>
                      </td>

                      {/* Ações */}
                      <td className="py-4 px-4 lg:px-6 text-right">
                        <div className="flex items-center justify-end gap-2">
                          <button
                            id={`btn-edit-tour-${tour.id}`}
                            onClick={() =>
                              onEdit(
                                tour
                              )
                            }
                            className="p-2 text-slate-600 hover:text-[#1a535c] hover:bg-[#1a535c]/10 rounded-lg transition-colors cursor-pointer"
                            title="Editar Passeio"
                          >
                            <Edit3 className="w-4 h-4" />
                          </button>

                          <button
                            id={`btn-delete-tour-${tour.id}`}
                            onClick={() =>
                              onRequestDelete(
                                tour
                              )
                            }
                            className="p-2 text-slate-400 hover:text-[#ff6b6b] hover:bg-[#ff6b6b]/10 rounded-lg transition-colors cursor-pointer"
                            title="Excluir Passeio"
                          >
                            <Trash2 className="w-4 h-4" />
                          </button>
                        </div>
                      </td>
                    </tr>
                  )
                )
              )}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}