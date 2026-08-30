import { Category, Tour, Review, ActiveScreen } from '../types';
import {
  Compass,
  FolderTree,
  Star,
  Calendar,
  ArrowUpRight,
  Plus,
  ArrowRight,
  TrendingUp,
} from 'lucide-react';

interface DashboardScreenProps {
  categories: Category[];
  tours: Tour[];
  reviews: Review[];
  onNavigate: (screen: ActiveScreen) => void;
  onEditTour: (tour: Tour) => void;
  onEditCategory: (category: Category) => void;
}

/**
 * O retorno atual da API possui:
 * categoriaId
 * categoriaNome
 *
 * O frontend originalmente trabalhava com:
 * categoryId
 * categoryName
 *
 * Esta função aceita os dois formatos para evitar que
 * a categoria desapareça da interface.
 */
function getTourCategoryName(tour: Tour, categories: Category[]): string {
  const tourData = tour as Tour & {
    categoriaId?: number;
    categoriaNome?: string;
  };

  // Primeiro tenta o formato usado pelo frontend
  if (tour.categoryName) {
    return tour.categoryName;
  }

  // Depois tenta o formato atual da API
  if (tourData.categoriaNome) {
    return tourData.categoriaNome;
  }

  // Por último, procura a categoria pelo ID
  const categoryId =
    tour.categoryId ?? tourData.categoriaId;

  if (categoryId !== undefined && categoryId !== null) {
    const category = categories.find(
      (cat) => Number(cat.id) === Number(categoryId)
    );

    if (category) {
      return category.name;
    }
  }

  return 'Sem categoria';
}

export function DashboardScreen({
  categories,
  tours,
  reviews,
  onNavigate,
  onEditTour,
  onEditCategory,
}: DashboardScreenProps) {
  const totalTours = tours.length;
  const totalCategories = categories.length;
  const totalReviews = reviews.length;

  const totalAvailabilityBlocks = tours.reduce(
    (acc, tour) => acc + (tour.availabilities?.length || 0),
    0
  );

  const averageRating = (
    tours.reduce((acc, tour) => acc + (tour.rating || 0), 0) /
    (tours.length || 1)
  ).toFixed(1);

  return (
    <div id="dashboard-screen" className="space-y-6">

      {/* Welcome Banner */}
      <div className="bg-gradient-to-r from-[#1a535c] to-[#206873] rounded-2xl p-6 lg:p-8 text-white shadow-lg shadow-[#1a535c]/10 relative overflow-hidden">
        <div className="absolute right-0 top-0 bottom-0 w-1/3 bg-gradient-to-l from-[#4ecdc4]/20 to-transparent pointer-events-none" />

        <div className="relative z-10 max-w-2xl">
          <div className="inline-flex items-center gap-2 px-3 py-1 rounded-full bg-white/10 text-[#4ecdc4] text-xs font-semibold mb-3 backdrop-blur-sm">
            <TrendingUp className="w-3.5 h-3.5" />
            <span>Temporada de Turismo 2026</span>
          </div>

          <h2 className="text-2xl lg:text-3xl font-extrabold tracking-tight">
            Bem-vindo ao Painel Rota Livre
          </h2>

          <p className="text-white/80 text-sm mt-2 leading-relaxed">
            Gerencie os passeios, categorias e blocos de horários disponíveis.
            Todas as alterações são sincronizadas com o aplicativo dos turistas.
          </p>

          <div className="flex flex-wrap gap-3 mt-5">
            <button
              id="btn-dash-new-tour"
              onClick={() => onNavigate('tour-new')}
              className="inline-flex items-center gap-2 bg-[#4ecdc4] hover:bg-[#42b8b0] text-[#1a535c] font-bold text-xs lg:text-sm px-4 py-2.5 rounded-xl shadow-md transition-all active:scale-98"
            >
              <Plus className="w-4 h-4" />
              <span>Cadastrar Novo Passeio</span>
            </button>

            <button
              id="btn-dash-new-category"
              onClick={() => onNavigate('category-new')}
              className="inline-flex items-center gap-2 bg-white/10 hover:bg-white/20 text-white font-semibold text-xs lg:text-sm px-4 py-2.5 rounded-xl transition-all border border-white/20"
            >
              <FolderTree className="w-4 h-4 text-[#4ecdc4]" />
              <span>Nova Categoria</span>
            </button>
          </div>
        </div>
      </div>

      {/* Cards */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 lg:gap-5">

        {/* Passeios */}
        <div
          id="stat-card-tours"
          onClick={() => onNavigate('tours')}
          className="bg-white p-5 rounded-2xl border border-slate-200 shadow-sm hover:shadow-md transition-all cursor-pointer group"
        >
          <div className="flex items-center justify-between">
            <div className="w-12 h-12 rounded-xl bg-[#1a535c]/10 text-[#1a535c] flex items-center justify-center group-hover:bg-[#1a535c] group-hover:text-white transition-colors">
              <Compass className="w-6 h-6" />
            </div>

            <span className="text-xs font-semibold text-emerald-600 bg-emerald-50 px-2 py-0.5 rounded-full flex items-center gap-1">
              <ArrowUpRight className="w-3.5 h-3.5" />
              +12% mês
            </span>
          </div>

          <div className="mt-4">
            <p className="text-xs font-bold text-slate-500 uppercase tracking-wider">
              Total de Passeios
            </p>

            <div className="flex items-baseline gap-2 mt-1">
              <span className="text-2xl lg:text-3xl font-extrabold text-slate-900">
                {totalTours}
              </span>

              <span className="text-xs text-slate-500 font-medium">
                cadastrados
              </span>
            </div>
          </div>
        </div>

        {/* Categorias */}
        <div
          id="stat-card-categories"
          onClick={() => onNavigate('categories')}
          className="bg-white p-5 rounded-2xl border border-slate-200 shadow-sm hover:shadow-md transition-all cursor-pointer group"
        >
          <div className="flex items-center justify-between">
            <div className="w-12 h-12 rounded-xl bg-[#4ecdc4]/15 text-[#1a535c] flex items-center justify-center group-hover:bg-[#4ecdc4] group-hover:text-[#1a535c] transition-colors">
              <FolderTree className="w-6 h-6" />
            </div>

            <span className="text-xs font-semibold text-[#1a535c] bg-[#1a535c]/5 px-2 py-0.5 rounded-full">
              Ativas
            </span>
          </div>

          <div className="mt-4">
            <p className="text-xs font-bold text-slate-500 uppercase tracking-wider">
              Total de Categorias
            </p>

            <div className="flex items-baseline gap-2 mt-1">
              <span className="text-2xl lg:text-3xl font-extrabold text-slate-900">
                {totalCategories}
              </span>

              <span className="text-xs text-slate-500 font-medium">
                segmentos
              </span>
            </div>
          </div>
        </div>

        {/* Avaliações */}
        <div
          id="stat-card-reviews"
          className="bg-white p-5 rounded-2xl border border-slate-200 shadow-sm"
        >
          <div className="flex items-center justify-between">
            <div className="w-12 h-12 rounded-xl bg-amber-50 text-amber-600 flex items-center justify-center">
              <Star className="w-6 h-6 fill-amber-400" />
            </div>

            <span className="text-xs font-bold text-amber-700 bg-amber-100/70 px-2 py-0.5 rounded-full flex items-center gap-1">
              ★ {averageRating} Média
            </span>
          </div>

          <div className="mt-4">
            <p className="text-xs font-bold text-slate-500 uppercase tracking-wider">
              Novas Avaliações
            </p>

            <div className="flex items-baseline gap-2 mt-1">
              <span className="text-2xl lg:text-3xl font-extrabold text-slate-900">
                {totalReviews}
              </span>

              <span className="text-xs text-slate-500 font-medium">
                nesta semana
              </span>
            </div>
          </div>
        </div>

        {/* Disponibilidade */}
        <div
          id="stat-card-availability"
          className="bg-white p-5 rounded-2xl border border-slate-200 shadow-sm"
        >
          <div className="flex items-center justify-between">
            <div className="w-12 h-12 rounded-xl bg-[#1a535c]/10 text-[#1a535c] flex items-center justify-center">
              <Calendar className="w-6 h-6" />
            </div>

            <span className="text-xs font-semibold text-emerald-600 bg-emerald-50 px-2 py-0.5 rounded-full">
              Sincronizado
            </span>
          </div>

          <div className="mt-4">
            <p className="text-xs font-bold text-slate-500 uppercase tracking-wider">
              Blocos de Agenda
            </p>

            <div className="flex items-baseline gap-2 mt-1">
              <span className="text-2xl lg:text-3xl font-extrabold text-slate-900">
                {totalAvailabilityBlocks}
              </span>

              <span className="text-xs text-slate-500 font-medium">
                datas abertas
              </span>
            </div>
          </div>
        </div>
      </div>

      {/* Conteúdo principal */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">

        {/* Passeios */}
        <div className="lg:col-span-2 bg-white rounded-2xl border border-slate-200 p-6 shadow-sm">
          <div className="flex items-center justify-between mb-5">
            <div>
              <h3 className="text-base font-bold text-slate-800">
                Passeios em Destaque no Catálogo
              </h3>

              <p className="text-xs text-slate-500 mt-0.5">
                Experiências mais recentes cadastradas no Rota Livre
              </p>
            </div>

            <button
              onClick={() => onNavigate('tours')}
              className="text-xs font-bold text-[#1a535c] hover:text-[#4ecdc4] flex items-center gap-1 transition-colors"
            >
              <span>Ver todos</span>
              <ArrowRight className="w-3.5 h-3.5" />
            </button>
          </div>

          <div className="divide-y divide-slate-100">
            {tours.slice(0, 4).map((tour) => (
              <div
                key={tour.id}
                className="py-3.5 first:pt-0 last:pb-0 flex items-center justify-between gap-4 group"
              >
                <div className="flex items-center gap-3.5 min-w-0">
                  <img
                    src={tour.photoUrl}
                    alt={tour.name}
                    referrerPolicy="no-referrer"
                    className="w-12 h-12 rounded-xl object-cover shrink-0 border border-slate-200"
                  />

                  <div className="min-w-0">
                    <p className="text-sm font-bold text-slate-800 truncate group-hover:text-[#1a535c] transition-colors">
                      {tour.name}
                    </p>

                    <div className="flex items-center gap-2 text-xs text-slate-500 mt-0.5">
                      <span className="inline-flex items-center gap-1 font-semibold text-[#1a535c]">
                        {getTourCategoryName(tour, categories)}
                      </span>
                    </div>
                  </div>
                </div>

                <div className="flex items-center shrink-0">
                  <button
                    onClick={() => onEditTour(tour)}
                    className="text-xs font-semibold px-3 py-1.5 rounded-lg border border-slate-200 text-slate-700 hover:border-[#1a535c] hover:text-[#1a535c] hover:bg-slate-50 transition-all"
                  >
                    Editar
                  </button>
                </div>
              </div>
            ))}
          </div>
        </div>

        {/* Coluna direita */}
        <div className="space-y-6">

          {/* Categorias */}
          <div className="bg-white rounded-2xl border border-slate-200 p-6 shadow-sm">
            <div className="flex items-center justify-between mb-4">
              <h3 className="text-base font-bold text-slate-800">
                Categorias Ativas
              </h3>

              <button
                onClick={() => onNavigate('categories')}
                className="text-xs font-bold text-[#1a535c] hover:underline"
              >
                Gerenciar
              </button>
            </div>

            <div className="space-y-3">
              {categories.map((cat) => (
                <div
                  key={cat.id}
                  onClick={() => onEditCategory(cat)}
                  className="flex items-center justify-between p-2 rounded-xl hover:bg-slate-50 transition-colors cursor-pointer"
                >
                  <div className="flex items-center gap-2.5">
                    <img
                      src={cat.imageUrl}
                      alt={cat.name}
                      referrerPolicy="no-referrer"
                      className="w-8 h-8 rounded-lg object-cover border border-slate-200"
                    />

                    <div>
                      <p className="text-xs font-bold text-slate-800">
                        {cat.name}
                      </p>

                      <p className="text-[10px] text-slate-400">
                        {cat.tourCount} passeios
                      </p>
                    </div>
                  </div>

                  <span className="w-2 h-2 rounded-full bg-emerald-500" />
                </div>
              ))}
            </div>
          </div>

          {/* Avaliações */}
          <div className="bg-white rounded-2xl border border-slate-200 p-6 shadow-sm">
            <div className="flex items-center justify-between mb-4">
              <h3 className="text-base font-bold text-slate-800">
                Últimas Avaliações
              </h3>

              <span className="text-xs text-slate-400 font-medium">
                Recentes
              </span>
            </div>

            <div className="space-y-3.5">
              {reviews.slice(0, 2).map((rev) => (
                <div
                  key={rev.id}
                  className="p-3 rounded-xl bg-slate-50 border border-slate-100 text-xs"
                >
                  <div className="flex items-center justify-between mb-1.5">
                    <div className="flex items-center gap-2">
                      <img
                        src={rev.userAvatar}
                        alt={rev.userName}
                        referrerPolicy="no-referrer"
                        className="w-6 h-6 rounded-full object-cover"
                      />

                      <span className="font-bold text-slate-800">
                        {rev.userName}
                      </span>
                    </div>

                    <div className="flex text-amber-400">
                      {'★'.repeat(rev.rating)}
                    </div>
                  </div>

                  <p className="text-slate-600 line-clamp-2 italic">
                    "{rev.comment}"
                  </p>

                  <p className="text-[10px] text-slate-400 font-medium mt-1">
                    Passeio: {rev.tourName}
                  </p>
                </div>
              ))}
            </div>
          </div>

        </div>
      </div>
    </div>
  );
}