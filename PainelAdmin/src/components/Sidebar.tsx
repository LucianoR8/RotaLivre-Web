import { ActiveScreen } from '../types';
import {
  LayoutDashboard,
  FolderTree,
  Compass,
  LogOut,
  X,
  Sparkles,
  MapPin,
  CheckCircle2,
} from 'lucide-react';

interface SidebarProps {
  activeScreen: ActiveScreen;
  onNavigate: (screen: ActiveScreen) => void;
  onLogout: () => void;
  categoriesCount: number;
  toursCount: number;
  mobileOpen: boolean;
  onCloseMobile: () => void;
}

export function Sidebar({
  activeScreen,
  onNavigate,
  onLogout,
  categoriesCount,
  toursCount,
  mobileOpen,
  onCloseMobile,
}: SidebarProps) {
  const isCategoryActive = activeScreen.startsWith('category');
  const isTourActive = activeScreen.startsWith('tour');
  const isDashboardActive = activeScreen === 'dashboard';

  const navItems = [
    {
      id: 'nav-dashboard',
      label: 'Dashboard',
      screen: 'dashboard' as ActiveScreen,
      icon: LayoutDashboard,
      isActive: isDashboardActive,
      badge: null,
    },
    {
      id: 'nav-categories',
      label: 'Categorias',
      screen: 'categories' as ActiveScreen,
      icon: FolderTree,
      isActive: isCategoryActive,
      badge: categoriesCount,
    },
    {
      id: 'nav-tours',
      label: 'Passeios',
      screen: 'tours' as ActiveScreen,
      icon: Compass,
      isActive: isTourActive,
      badge: toursCount,
    },
  ];

  return (
    <>
      {/* Mobile overlay backdrop */}
      {mobileOpen && (
        <div
          id="sidebar-backdrop"
          onClick={onCloseMobile}
          className="fixed inset-0 z-40 bg-slate-900/50 backdrop-blur-sm lg:hidden transition-opacity"
        />
      )}

      {/* Main Sidebar */}
      <aside
        id="admin-sidebar"
        className={`fixed top-0 left-0 bottom-0 z-50 w-64 bg-[#1a535c] text-white flex flex-col justify-between shadow-xl transition-transform duration-300 ease-in-out lg:translate-x-0 ${
          mobileOpen ? 'translate-x-0' : '-translate-x-full'
        }`}
      >
        {/* Top: Brand & Close button for mobile */}
        <div>
          <div className="h-16 px-6 flex items-center justify-between border-b border-white/10">
            <div className="flex items-center gap-3">
              <div className="w-10 h-10 rounded-xl bg-[#4ecdc4] flex items-center justify-center shadow-md">
                <Compass className="w-6 h-6 text-[#1a535c]" />
              </div>
              <div className="flex flex-col">
                <span className="font-extrabold text-lg tracking-tight text-white leading-none">
                  Rota Livre
                </span>
                <span className="text-[11px] font-semibold text-[#4ecdc4] tracking-wider uppercase mt-1">
                  Painel Admin
                </span>
              </div>
            </div>

            <button
              onClick={onCloseMobile}
              className="lg:hidden p-1.5 text-white/70 hover:text-white hover:bg-white/10 rounded-lg"
              aria-label="Fechar menu"
            >
              <X className="w-5 h-5" />
            </button>
          </div>

          {/* Navigation Links */}
          <div className="px-4 py-6 space-y-1.5">
            <p className="px-3 text-[11px] font-bold text-white/50 uppercase tracking-wider mb-2">
              Menu Principal
            </p>

            {navItems.map((item) => {
              const Icon = item.icon;
              return (
                <button
                  key={item.id}
                  id={item.id}
                  onClick={() => {
                    onNavigate(item.screen);
                    onCloseMobile();
                  }}
                  className={`w-full flex items-center justify-between px-3.5 py-3 rounded-xl font-medium text-sm transition-all text-left ${
                    item.isActive
                      ? 'bg-white text-[#1a535c] font-bold shadow-md shadow-black/10'
                      : 'text-white/80 hover:text-white hover:bg-white/10'
                  }`}
                >
                  <div className="flex items-center gap-3">
                    <Icon
                      className={`w-5 h-5 ${
                        item.isActive ? 'text-[#1a535c]' : 'text-[#4ecdc4]'
                      }`}
                    />
                    <span>{item.label}</span>
                  </div>

                  {item.badge !== null && (
                    <span
                      className={`text-xs px-2 py-0.5 rounded-full font-bold ${
                        item.isActive
                          ? 'bg-[#1a535c]/15 text-[#1a535c]'
                          : 'bg-white/15 text-white'
                      }`}
                    >
                      {item.badge}
                    </span>
                  )}
                </button>
              );
            })}
          </div>

          {/* App preview teaser card */}
          <div className="px-4">
            <div className="p-3.5 rounded-xl bg-white/5 border border-white/10 text-xs">
              <div className="flex items-center gap-2 text-[#4ecdc4] font-semibold mb-1">
                <Sparkles className="w-3.5 h-3.5" />
                <span>App Rota Livre Ativo</span>
              </div>
              <p className="text-white/70 text-[11px] leading-relaxed">
                As alterações publicadas aqui refletem no app mobile em tempo real.
              </p>
            </div>
          </div>
        </div>

        {/* Bottom: Logout & System info */}
        <div className="p-4 border-t border-white/10 space-y-3">
          <button
            id="btn-logout"
            onClick={onLogout}
            className="w-full flex items-center gap-3 px-3.5 py-2.5 rounded-xl font-medium text-sm text-white/90 hover:text-white hover:bg-[#ff6b6b] transition-colors active:scale-98"
          >
            <LogOut className="w-5 h-5 text-[#ff6b6b] group-hover:text-white" />
            <span>Sair do Painel</span>
          </button>

          <div className="flex items-center justify-between text-[11px] text-white/40 px-2 pt-1 border-t border-white/5">
            <span className="flex items-center gap-1">
              <span className="w-1.5 h-1.5 rounded-full bg-emerald-400"></span>
              v2.4.0 Online
            </span>
            <span>Turismo Brasil</span>
          </div>
        </div>
      </aside>
    </>
  );
}
