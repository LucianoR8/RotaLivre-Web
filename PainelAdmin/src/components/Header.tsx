import { AdminUser, ActiveScreen } from '../types';
import { Menu, Search, Plus, Compass } from 'lucide-react';

interface HeaderProps {
  user: AdminUser;
  activeScreen: ActiveScreen;
  onNavigate: (screen: ActiveScreen) => void;
  onToggleMobileSidebar: () => void;
}

export function Header({
  user,
  activeScreen,
  onNavigate,
  onToggleMobileSidebar,
}: HeaderProps) {
  const getScreenTitle = () => {
    switch (activeScreen) {
      case 'dashboard':
        return 'Visão Geral do Sistema';
      case 'categories':
        return 'Gestão de Categorias';
      case 'category-new':
        return 'Cadastrar Nova Categoria';
      case 'category-edit':
        return 'Editar Categoria';
      case 'tours':
        return 'Gestão de Passeios e Experiências';
      case 'tour-new':
        return 'Cadastrar Novo Passeio';
      case 'tour-edit':
        return 'Editar Passeio';
      default:
        return 'Painel Administrativo';
    }
  };

  return (
    <header
      id="admin-header"
      className="sticky top-0 z-30 bg-white/95 backdrop-blur border-b border-slate-200 h-16 px-4 lg:px-8 flex items-center justify-between transition-all"
    >
      {/* Left section: mobile hamburger & breadcrumb */}
      <div className="flex items-center gap-3">
        <button
          id="btn-mobile-sidebar-toggle"
          onClick={onToggleMobileSidebar}
          className="lg:hidden p-2 text-slate-600 hover:text-[#1a535c] hover:bg-slate-100 rounded-lg transition-colors"
          aria-label="Abrir Menu Lateral"
        >
          <Menu className="w-5 h-5" />
        </button>

        <div>
          <div className="flex items-center gap-1.5 text-xs text-slate-500 font-medium">
            <span>Rota Livre</span>
            <span>/</span>
            <span className="text-[#1a535c] font-semibold capitalize">
              {activeScreen.startsWith('category')
                ? 'Categorias'
                : activeScreen.startsWith('tour')
                ? 'Passeios'
                : 'Dashboard'}
            </span>
          </div>
          <h1 className="text-base lg:text-lg font-bold text-slate-800 leading-tight">
            {getScreenTitle()}
          </h1>
        </div>
      </div>

      {/* Right section: Quick action, search, Admin profile info */}
      <div className="flex items-center gap-3 lg:gap-5">
        {/* Quick action button */}
        {activeScreen !== 'tour-new' && (
          <button
            id="btn-quick-new-tour"
            onClick={() => onNavigate('tour-new')}
            className="hidden sm:flex items-center gap-2 bg-[#1a535c] hover:bg-[#154249] text-white text-xs lg:text-sm font-semibold px-3.5 py-2 rounded-xl transition-all shadow-sm active:scale-98"
          >
            <Plus className="w-4 h-4 text-[#4ecdc4]" />
            <span>Novo Passeio</span>
          </button>
        )}

        {/* Admin profile snippet */}
        <div
          id="admin-profile-pill"
          className="flex items-center gap-3 pl-2 sm:pl-3 sm:border-l sm:border-slate-200"
        >
          <div className="relative">
            <img
              src={user.avatarUrl}
              alt={user.name}
              referrerPolicy="no-referrer"
              className="w-9 h-9 rounded-full object-cover ring-2 ring-[#4ecdc4]/40"
            />
            <span className="absolute bottom-0 right-0 w-2.5 h-2.5 bg-emerald-500 rounded-full ring-2 ring-white" />
          </div>

          <div className="hidden md:flex flex-col text-left">
            <span className="text-sm font-bold text-slate-800 leading-none">
              {user.name}
            </span>
            <span className="text-xs text-slate-500 font-medium mt-0.5">
              {user.email}
            </span>
          </div>

          <span className="hidden xl:inline-flex items-center px-2 py-0.5 rounded-md text-[11px] font-semibold bg-[#1a535c]/10 text-[#1a535c]">
            {user.role}
          </span>
        </div>
      </div>
    </header>
  );
}
