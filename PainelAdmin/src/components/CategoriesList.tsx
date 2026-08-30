import {
  useState,
  useEffect,
} from 'react';

import {
  Category,
  ActiveScreen,
} from '../types';

import {
  Plus,
  Search,
  Edit3,
  Trash2,
  FolderTree,
  Image as ImageIcon,
} from 'lucide-react';

interface CategoriesListProps {
  categories: Category[];

  onNavigate: (
    screen: ActiveScreen
  ) => void;

  onEdit: (
    category: Category
  ) => void;

  onRequestDelete: (
    category: Category
  ) => void;
}

export function CategoriesList({
  categories,
  onNavigate,
  onEdit,
  onRequestDelete,
}: CategoriesListProps) {

  const [searchTerm, setSearchTerm] =
    useState('');

  // =========================================================
  // LOG
  // =========================================================

  useEffect(() => {

    console.log(
      '======================================'
    );

    console.log(
      '📋 CATEGORIES LIST RECEBEU:'
    );

    console.log(
      'Quantidade:',
      categories.length
    );

    console.log(
      'Categorias:',
      categories
    );

    console.log(
      '======================================'
    );

  }, [categories]);

  // =========================================================
  // FILTRO
  // =========================================================

  const filteredCategories =
    categories.filter((category) => {

      const termo =
        searchTerm
          .toLowerCase()
          .trim();

      return (
        category.name
          .toLowerCase()
          .includes(termo) ||

        String(category.id)
          .toLowerCase()
          .includes(termo)
      );

    });

  // =========================================================
  // RENDER
  // =========================================================

  return (

    <div
      id="categories-list-screen"
      className="space-y-5"
    >

      {/* =====================================================
          CABEÇALHO
      ====================================================== */}

      <div className="bg-white p-5 rounded-2xl border border-slate-200 shadow-sm flex flex-col sm:flex-row items-start sm:items-center justify-between gap-4">

        <div>

          <div className="flex items-center gap-2">

            <h2 className="text-xl font-extrabold text-slate-800">
              Categorias de Passeios
            </h2>

            <span className="px-2.5 py-0.5 rounded-full text-xs font-bold bg-[#1a535c]/10 text-[#1a535c]">
              {categories.length}{' '}
              cadastradas
            </span>

          </div>

          <p className="text-xs text-slate-500 mt-1">
            Organize os tipos de experiências disponíveis no aplicativo Rota Livre
          </p>

        </div>

        <button
          type="button"
          onClick={() =>
            onNavigate(
              'category-new'
            )
          }
          className="w-full sm:w-auto inline-flex items-center justify-center gap-2 bg-[#1a535c] hover:bg-[#154249] text-white font-bold text-sm px-4 py-2.5 rounded-xl shadow-sm transition-all active:scale-98 cursor-pointer"
        >

          <Plus className="w-4 h-4 text-[#4ecdc4]" />

          <span>
            Nova Categoria
          </span>

        </button>

      </div>

      {/* =====================================================
          BUSCA
      ====================================================== */}

      <div className="bg-white p-4 rounded-2xl border border-slate-200 shadow-sm flex items-center gap-3">

        <div className="relative flex-1">

          <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none text-slate-400">

            <Search className="w-4 h-4" />

          </div>

          <input
            type="text"
            value={searchTerm}
            onChange={(e) =>
              setSearchTerm(
                e.target.value
              )
            }
            placeholder="Buscar categoria por nome ou ID..."
            className="w-full pl-9 pr-4 py-2 bg-slate-50 border border-slate-200 rounded-xl text-sm text-slate-800 placeholder-slate-400 focus:outline-none focus:ring-2 focus:ring-[#4ecdc4] focus:border-[#1a535c]"
          />

        </div>

        {searchTerm && (

          <button
            type="button"
            onClick={() =>
              setSearchTerm('')
            }
            className="text-xs text-slate-500 hover:text-slate-800 font-semibold px-2 py-1 cursor-pointer"
          >
            Limpar
          </button>

        )}

      </div>

      {/* =====================================================
          TABELA
      ====================================================== */}

      <div className="bg-white rounded-2xl border border-slate-200 shadow-sm overflow-hidden">

        <div className="overflow-x-auto">

          <table className="w-full text-left border-collapse">

            <thead>

              <tr className="border-b border-slate-200 bg-slate-50/80 text-[11px] font-bold text-slate-500 uppercase tracking-wider">

                <th className="py-3.5 px-4 lg:px-6">
                  ID
                </th>

                <th className="py-3.5 px-4 lg:px-6">
                  Miniatura
                </th>

                <th className="py-3.5 px-4 lg:px-6">
                  Nome da Categoria
                </th>

                <th className="py-3.5 px-4 lg:px-6 text-center">
                  Passeios
                </th>

                <th className="py-3.5 px-4 lg:px-6 text-right">
                  Ações
                </th>

              </tr>

            </thead>

            <tbody className="divide-y divide-slate-100 text-sm">

              {filteredCategories.length === 0 ? (

                <tr>

                  <td
                    colSpan={5}
                    className="py-12 text-center text-slate-400"
                  >

                    <FolderTree className="w-10 h-10 mx-auto text-slate-300 mb-2" />

                    <p className="font-semibold text-slate-600">
                      Nenhuma categoria encontrada
                    </p>

                  </td>

                </tr>

              ) : (

                filteredCategories.map(
                  (category) => (

                    <tr
                      key={category.id}
                      className="hover:bg-slate-50/80 transition-colors group"
                    >

                      {/* ID */}

                      <td className="py-4 px-4 lg:px-6 font-mono text-xs font-bold text-slate-500">
                        {category.id}
                      </td>

                      {/* MINIATURA */}

                      <td className="py-4 px-4 lg:px-6">

                        <div className="relative w-12 h-12 rounded-xl overflow-hidden border border-slate-200 bg-slate-100 shrink-0">

                          {category.imageUrl ? (

                            <img
                              src={
                                category.imageUrl
                              }
                              alt={
                                category.name
                              }
                              className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-200"
                              referrerPolicy="no-referrer"
                              onError={(e) => {

                                console.error(
                                  '❌ ERRO AO CARREGAR IMAGEM DA CATEGORIA:',
                                  category.imageUrl
                                );

                                (
                                  e.currentTarget
                                ).style.display =
                                  'none';

                              }}
                            />

                          ) : (

                            <div className="w-full h-full flex items-center justify-center text-slate-400">

                              <ImageIcon className="w-5 h-5" />

                            </div>

                          )}

                        </div>

                      </td>

                      {/* NOME */}

                      <td className="py-4 px-4 lg:px-6">

                        <div className="font-bold text-slate-800 group-hover:text-[#1a535c] transition-colors">
                          {category.name}
                        </div>

                      </td>

                      {/* PASSEIOS */}

                      <td className="py-4 px-4 lg:px-6 text-center">

                        <span className="inline-flex items-center justify-center px-2.5 py-1 rounded-full text-xs font-bold bg-[#4ecdc4]/15 text-[#1a535c]">

                          {category.tourCount ?? 0}

                        </span>

                      </td>

                      {/* AÇÕES */}

                      <td className="py-4 px-4 lg:px-6 text-right">

                        <div className="flex items-center justify-end gap-2">

                          {/* EDITAR */}

                          <button
                            type="button"
                            onClick={() =>
                              onEdit(
                                category
                              )
                            }
                            className="p-2 text-slate-600 hover:text-[#1a535c] hover:bg-[#1a535c]/10 rounded-lg transition-colors cursor-pointer"
                            title="Editar Categoria"
                          >

                            <Edit3 className="w-4 h-4" />

                          </button>

                          {/* EXCLUIR */}

                          <button
                            type="button"
                            onClick={() =>
                              onRequestDelete(
                                category
                              )
                            }
                            className="p-2 text-slate-400 hover:text-[#ff6b6b] hover:bg-[#ff6b6b]/10 rounded-lg transition-colors cursor-pointer"
                            title="Excluir Categoria"
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