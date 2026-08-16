import React, { useState, useEffect, useRef } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { UsuarioPerfilDto } from '../types';
import { usuarioService } from '../services/usuarioService';
import {
  User,
  Mail,
  Edit,
  LogOut,
  Calendar,
  Camera,
  Trash2
} from 'lucide-react';

export const PerfilPage: React.FC = () => {
  const { usuario, logout } = useAuth();
  const navigate = useNavigate();

  const [perfil, setPerfil] = useState<UsuarioPerfilDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [uploadingFoto, setUploadingFoto] = useState(false);
  const [removendoFoto, setRemovendoFoto] = useState(false);
  const inputFotoRef = useRef<HTMLInputElement | null>(null);

  useEffect(() => {
    const carregarPerfil = async () => {
      if (!usuario?.id) {
        setLoading(false);
        return;
      }

      try {
        console.log('Buscando perfil do usuário:', usuario.id);

        const data = await usuarioService.buscarPerfil(usuario.id);

        console.log('Dados do perfil:', data);

        setPerfil(data);
      } catch (err) {
        console.error('Erro ao buscar perfil:', err);
      } finally {
        setLoading(false);
      }
    };

    carregarPerfil();
  }, [usuario?.id]);

  const selecionarFoto = () => {
    inputFotoRef.current?.click();
  };

  const handleFotoChange = async (
  e: React.ChangeEvent<HTMLInputElement>
) => {
  const arquivo = e.target.files?.[0];

  if (!arquivo || !perfil) {
    return;
  }

  try {
    setUploadingFoto(true);

    console.log('Enviando foto:', arquivo.name);

    const novaFotoUrl = await usuarioService.uploadFoto(
      perfil.id,
      arquivo
    );

    console.log('Nova foto:', novaFotoUrl);

    setPerfil(prev =>
      prev
        ? {
            ...prev,
            fotoPerfilUrl: novaFotoUrl
          }
        : prev
    );
  } catch (err) {
    console.error('Erro ao enviar foto:', err);
    alert('Não foi possível atualizar sua foto.');
  } finally {
    setUploadingFoto(false);

    // Permite selecionar a mesma imagem novamente
    e.target.value = '';
  }
};

const removerFoto = async () => {
  if (!perfil?.fotoPerfilUrl) {
    return;
  }

  const confirmar = window.confirm(
    'Deseja realmente remover sua foto de perfil?'
  );

  if (!confirmar) {
    return;
  }

  try {
    setRemovendoFoto(true);

    await usuarioService.removerFoto(perfil.id);

    setPerfil(prev =>
      prev
        ? {
            ...prev,
            fotoPerfilUrl: null
          }
        : prev
    );
  } catch (err) {
    console.error('Erro ao remover foto:', err);
    alert('Não foi possível remover sua foto.');
  } finally {
    setRemovendoFoto(false);
  }
};


  if (loading) {
    return (
      <div className="pt-32 pb-28 flex justify-center items-center">
        <div className="w-12 h-12 border-4 border-[#4ecdc4] border-t-transparent rounded-full animate-spin" />
      </div>
    );
  }

  if (!perfil) {
    return (
      <div className="pt-32 pb-28 max-w-md mx-auto px-4 text-center">
        <div className="bg-white rounded-3xl p-8 shadow-lg border border-slate-100">
          <User className="w-12 h-12 mx-auto mb-4 text-[#1a535c]" />

          <h2 className="text-xl font-bold text-[#1a535c] mb-2">
            Não foi possível carregar seu perfil
          </h2>

          <p className="text-sm text-slate-500 mb-6">
            Tente novamente mais tarde.
          </p>

          <button
            onClick={() => window.location.reload()}
            className="bg-[#1a535c] text-white px-6 py-3 rounded-full font-bold text-sm"
          >
            Tentar novamente
          </button>
        </div>
      </div>
    );
  }

  const inicial = perfil.nome
    ? perfil.nome.charAt(0).toUpperCase()
    : 'U';

  return (
    <div className="pt-20 pb-24 max-w-md mx-auto px-4">

      {/* Profile Card */}
      <div className="bg-white rounded-3xl p-8 shadow-xl border border-slate-100 text-center relative overflow-hidden mb-8">

        {/* Header gradient */}
        <div className="absolute top-0 left-0 right-0 h-24 bg-gradient-to-r from-[#1a535c] via-[#236c78] to-[#4ecdc4]" />

        <div className="relative z-10 pt-8">

          {/* Avatar */}
<div className="mb-5">

  <div className="w-24 h-24 rounded-full bg-white p-1 mx-auto shadow-lg">
    {perfil.fotoPerfilUrl ? (
      <img
        src={perfil.fotoPerfilUrl}
        alt={`Foto de ${perfil.nome}`}
        className="w-full h-full rounded-full object-cover"
      />
    ) : (
      <div className="w-full h-full rounded-full bg-[#1a535c] text-white flex items-center justify-center font-bold text-3xl">
        {inicial}
      </div>
    )}
  </div>

  {/* Input escondido */}
  <input
    ref={inputFotoRef}
    type="file"
    accept="image/jpeg,image/png,image/webp, image/jfif"
    onChange={handleFotoChange}
    className="hidden"
  />

  {/* Controles da foto */}
  <div className="flex justify-center gap-2 mt-4">

    <button
      type="button"
      onClick={selecionarFoto}
      disabled={uploadingFoto || removendoFoto}
      className="bg-[#4ecdc4] hover:bg-[#4ecdc4]/90 disabled:opacity-50 text-white font-bold px-4 py-2 rounded-full text-xs transition shadow-md flex items-center gap-2"
    >
      <Camera className="w-4 h-4" />

      <span>
        {uploadingFoto ? 'Enviando...' : 'Alterar foto'}
      </span>
    </button>

    {perfil.fotoPerfilUrl && (
      <button
        type="button"
        onClick={removerFoto}
        disabled={uploadingFoto || removendoFoto}
        className="border-2 border-slate-200 text-slate-500 hover:border-[#ff6b6b] hover:text-[#ff6b6b] disabled:opacity-50 font-bold px-4 py-2 rounded-full text-xs transition flex items-center gap-2"
      >
        <Trash2 className="w-4 h-4" />

        <span>
          {removendoFoto ? 'Removendo...' : 'Remover'}
        </span>
      </button>
    )}

  </div>
</div>

          {/* Nome */}
          <h1 className="text-2xl font-extrabold text-[#1a535c]">
            {perfil.nome}
          </h1>

          {/* Email */}
          <p className="text-xs text-slate-500 font-medium mt-1">
            {perfil.email}
          </p>

          {/* Actions */}
          <div className="mt-6 flex justify-center gap-3">

            <Link
              to="/perfil/editar"
              className="bg-[#4ecdc4] hover:bg-[#4ecdc4]/90 text-white font-bold px-6 py-2.5 rounded-full text-xs transition shadow-md flex items-center gap-2"
            >
              <Edit className="w-4 h-4" />
              <span>Editar Perfil</span>
            </Link>

            <button
              onClick={() => {
                logout();
                navigate('/login');
              }}
              className="border-2 border-slate-200 text-slate-600 hover:border-[#ff6b6b] hover:text-[#ff6b6b] font-bold px-5 py-2.5 rounded-full text-xs transition flex items-center gap-1.5"
            >
              <LogOut className="w-4 h-4" />
              <span>Sair</span>
            </button>

          </div>
        </div>
      </div>

      {/* Personal Information */}
      <div className="bg-white rounded-3xl p-8 shadow-lg border border-slate-100 space-y-6">

        <h3 className="font-bold text-[#1a535c] text-base border-b border-slate-100 pb-3">
          Informações Pessoais
        </h3>

        <div className="space-y-6">

          {/* Nome */}
          <div className="flex items-start gap-3">
            <div className="p-2.5 bg-[#4ecdc4]/15 rounded-xl text-[#1a535c]">
              <User className="w-5 h-5" />
            </div>

            <div>
              <span className="text-[11px] font-bold text-slate-400 uppercase tracking-wider block">
                Nome
              </span>

              <span className="text-sm font-semibold text-slate-800">
                {perfil.nome}
              </span>
            </div>
          </div>

          {/* Email */}
          <div className="flex items-start gap-3">
            <div className="p-2.5 bg-[#4ecdc4]/15 rounded-xl text-[#1a535c]">
              <Mail className="w-5 h-5" />
            </div>

            <div>
              <span className="text-[11px] font-bold text-slate-400 uppercase tracking-wider block">
                E-mail
              </span>

              <span className="text-sm font-semibold text-slate-800">
                {perfil.email}
              </span>
            </div>
          </div>

          {/* Data de nascimento */}
          <div className="flex items-start gap-3">
            <div className="p-2.5 bg-[#4ecdc4]/15 rounded-xl text-[#1a535c]">
              <Calendar className="w-5 h-5" />
            </div>

            <div>
              <span className="text-[11px] font-bold text-slate-400 uppercase tracking-wider block">
                Nascimento
              </span>

              <span className="text-sm font-semibold text-slate-800">
                {perfil.dataNasc
                  ? new Date(`${perfil.dataNasc}T00:00:00`).toLocaleDateString('pt-BR')
                  : 'Não informado'}
              </span>
            </div>
          </div>

        </div>
      </div>
    </div>
  );
};