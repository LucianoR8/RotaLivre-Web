import React from 'react';
import { NavLink } from 'react-router-dom';
import { Home, User, Users, Radio } from 'lucide-react';

export const Footer: React.FC = () => {
  return (
    <footer className="fixed bottom-0 left-0 right-0 z-50 bg-white/95 backdrop-blur-md border-t border-slate-200/80 py-1.5 px-3 shadow-[0_-4px_20px_rgba(0,0,0,0.06)]">
      <div className="max-w-md mx-auto grid grid-cols-4 items-center gap-1 text-center">
        <NavLink
          to="/"
          className={({ isActive }) =>
            `flex flex-col items-center justify-center py-1 rounded-2xl transition-all ${
              isActive
                ? 'text-[#1a535c] font-extrabold bg-[#4ecdc4]/15 scale-105'
                : 'text-slate-400 font-medium hover:text-[#1a535c]'
            }`
          }
        >
          <Home className="w-5 h-5 mb-0.5" />
          <span className="text-[10px] tracking-tight">Home</span>
        </NavLink>

        <NavLink
          to="/grupos"
          className={({ isActive }) =>
            `flex flex-col items-center justify-center py-1 rounded-2xl transition-all ${
              isActive
                ? 'text-[#1a535c] font-extrabold bg-[#4ecdc4]/15 scale-105'
                : 'text-slate-400 font-medium hover:text-[#1a535c]'
            }`
          }
        >
          <Users className="w-5 h-5 mb-0.5" />
          <span className="text-[10px] tracking-tight">Grupos</span>
        </NavLink>

        <NavLink
          to="/ao-vivo"
          className={({ isActive }) =>
            `flex flex-col items-center justify-center py-1 rounded-2xl transition-all relative ${
              isActive
                ? 'text-[#1a535c] font-extrabold bg-[#4ecdc4]/15 scale-105'
                : 'text-slate-400 font-medium hover:text-[#1a535c]'
            }`
          }
        >
          <div className="relative">
            <Radio className="w-5 h-5 mb-0.5" />
            <span className="absolute -top-0.5 -right-0.5 flex h-2 w-2">
              <span className="animate-ping absolute inline-flex h-full w-full rounded-full bg-[#ff6b6b] opacity-75"></span>
              <span className="relative inline-flex rounded-full h-2 w-2 bg-[#ff6b6b]"></span>
            </span>
          </div>
          <span className="text-[10px] tracking-tight">Ao Vivo</span>
        </NavLink>

        <NavLink
          to="/perfil"
          className={({ isActive }) =>
            `flex flex-col items-center justify-center py-1 rounded-2xl transition-all ${
              isActive
                ? 'text-[#1a535c] font-extrabold bg-[#4ecdc4]/15 scale-105'
                : 'text-slate-400 font-medium hover:text-[#1a535c]'
            }`
          }
        >
          <User className="w-5 h-5 mb-0.5" />
          <span className="text-[10px] tracking-tight">Perfil</span>
        </NavLink>
      </div>
    </footer>
  );
};
