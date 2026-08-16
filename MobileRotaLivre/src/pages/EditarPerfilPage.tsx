import React, { useEffect, useRef, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { UsuarioPerfilDto } from '../types';
import { usuarioService } from '../services/usuarioService';

import {
  ArrowLeft,
  Save,
  User,
  Mail,
  Calendar,
  Camera,
  Trash2
} from 'lucide-react';

export const EditarPerfilPage: React.FC = () => {
  const { usuario, setUsuario } = useAuth();
  const navigate = useNavigate();

  const inputFotoRef = useRef<HTMLInputElement | null>(null);

  const [perfil, setPerfil] = useState<UsuarioPerfilDto | null>(null);

  const [nome, setNome] = useState('');
  const [email, setEmail] = useState('');
  const [dataNasc, setDataNasc] = useState('');

  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);

  const [uploadingFoto, setUploadingFoto] = useState(false);
  const [removendoFoto, setRemovendoFoto] = useState(false);

  const [msg, setMsg] = useState('');
  const [erro, setErro] = useState('');

  useEffect(() => {
    const carregarPerfil = async () => {
      if (!usuario?.id) {
        setLoading(false);
        return;
      }

      try {
        console.log('Buscando perfil para edição:', usuario.id);

        const data = await usuarioService.buscarPerfil(usuario.id);

        console.log('Dados recebidos:', data);

        setPerfil(data);
        setNome(data.nome || '');
        setEmail(data.email || '');
        setDataNasc(data.dataNasc || '');
      } catch (err) {
        console.error('Erro ao buscar perfil:', err);
        setErro('Não foi possível carregar seus dados.');
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

    // Validação básica
    const tiposPermitidos = [
      'image/jpeg',
      'image/png',
      'image/webp'
    ];

    if (!tiposPermitidos.includes(arquivo.type)) {
      alert('Selecione uma imagem JPG, PNG ou WEBP.');
      e.target.value = '';
      return;
    }

    // Limite de 5 MB
    if (arquivo.size > 5 * 1024 * 1024) {
      alert('A imagem deve ter no máximo 5 MB.');
      e.target.value = '';
      return;
    }

    try {
      setUploadingFoto(true);
      setErro('');
      setMsg('');

      console.log('Enviando nova foto:', arquivo.name);

      const novaFotoUrl = await usuarioService.uploadFoto(
        perfil.id,
        arquivo
      );

      console.log('Nova foto recebida:', novaFotoUrl);

      setPerfil(prev =>
        prev
          ? {
              ...prev,
              fotoPerfilUrl: novaFotoUrl
            }
          : prev
      );

      setMsg('Foto de perfil atualizada com sucesso!');
    } catch (err) {
      console.error('Erro ao enviar foto:', err);
      setErro('Não foi possível atualizar sua foto.');
    } finally {
      setUploadingFoto(false);

      // Permite escolher novamente a mesma imagem
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
      setErro('');
      setMsg('');

      await usuarioService.removerFoto(perfil.id);

      setPerfil(prev =>
        prev
          ? {
              ...prev,
              fotoPerfilUrl: null
            }
          : prev
      );

      setMsg('Foto de perfil removida com sucesso!');
    } catch (err) {
      console.error('Erro ao remover foto:', err);
      setErro('Não foi possível remover sua foto.');
    } finally {
      setRemovendoFoto(false);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!perfil) {
      return;
    }

    try {
      setSaving(true);
      setErro('');
      setMsg('');

      const dadosAtualizados: UsuarioPerfilDto = {
        id: perfil.id,
        nome,
        email,
        dataNasc,
        fotoPerfilUrl: perfil.fotoPerfilUrl
      };

      console.log('Atualizando perfil:', dadosAtualizados);

      await usuarioService.atualizarPerfil(dadosAtualizados);

      // Atualiza também os dados armazenados no AuthContext
      if (usuario) {
        setUsuario({
          ...usuario,
          id: usuario.id,
          nome: nome,
          email: email
        });
      }

      setPerfil(prev =>
        prev
          ? {
              ...prev,
              nome,
              email,
              dataNasc
            }
          : prev
      );

      setMsg('Perfil atualizado com sucesso!');

      setTimeout(() => {
        navigate('/perfil');
      }, 1200);

    } catch (err: any) {
      console.error('Erro ao atualizar perfil:', err);

      if (err?.response?.data === 'EMAIL_JA_EXISTE') {
        setErro('Este e-mail já está sendo utilizado.');
      } else {
        setErro('Não foi possível atualizar seu perfil.');
      }
    } finally {
      setSaving(false);
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
            onClick={() => navigate('/perfil')}
            className="bg-[#1a535c] text-white px-6 py-3 rounded-full font-bold text-sm"
          >
            Voltar ao Perfil
          </button>

        </div>
      </div>
    );
  }

  const inicial = nome
    ? nome.charAt(0).toUpperCase()
    : 'U';

  return (
    <div className="pt-20 pb-24 max-w-md mx-auto px-4">

      {/* Voltar */}
      <button
        type="button"
        onClick={() => navigate('/perfil')}
        className="p-3 bg-white rounded-full text-[#1a535c] hover:bg-[#4ecdc4] hover:text-white transition shadow-md flex items-center gap-2 mb-6 font-semibold text-sm"
      >
        <ArrowLeft className="w-5 h-5" />
        <span>Voltar ao Perfil</span>
      </button>

      <div className="bg-white rounded-3xl p-8 shadow-xl border border-slate-100">

        <h1 className="text-2xl font-extrabold text-[#1a535c] mb-6">
          Editar Perfil
        </h1>

        {/* Mensagem de sucesso */}
        {msg && (
          <div className="mb-6 p-4 bg-emerald-100 text-emerald-800 rounded-2xl text-xs font-bold">
            {msg}
          </div>
        )}

        {/* Mensagem de erro */}
        {erro && (
          <div className="mb-6 p-4 bg-red-100 text-red-700 rounded-2xl text-xs font-bold">
            {erro}
          </div>
        )}

        {/* FOTO DE PERFIL */}
        <div className="mb-8">

          <label className="block text-xs font-bold text-slate-500 uppercase mb-3">
            Foto de Perfil
          </label>

          <div className="flex flex-col items-center">

            {/* Avatar */}
            <div className="w-32 h-32 rounded-full bg-white p-1 shadow-lg mb-4">

              {perfil.fotoPerfilUrl ? (
                <img
                  src={perfil.fotoPerfilUrl}
                  alt={`Foto de ${nome}`}
                  className="w-full h-full rounded-full object-cover"
                />
              ) : (
                <div className="w-full h-full rounded-full bg-[#1a535c] text-white flex items-center justify-center font-bold text-4xl">
                  {inicial}
                </div>
              )}

            </div>

            {/* Input escondido */}
            <input
              ref={inputFotoRef}
              type="file"
              accept="image/jpeg,image/png,image/webp"
              onChange={handleFotoChange}
              className="hidden"
            />

            {/* Botões */}
            <div className="flex justify-center gap-2">

              <button
                type="button"
                onClick={selecionarFoto}
                disabled={uploadingFoto || removendoFoto}
                className="bg-[#4ecdc4] hover:bg-[#4ecdc4]/90 disabled:opacity-50 text-white font-bold px-4 py-2.5 rounded-full text-xs transition shadow-md flex items-center gap-2"
              >
                <Camera className="w-4 h-4" />

                {uploadingFoto
                  ? 'Enviando...'
                  : perfil.fotoPerfilUrl
                    ? 'Alterar foto'
                    : 'Adicionar foto'}
              </button>

              {perfil.fotoPerfilUrl && (
                <button
                  type="button"
                  onClick={removerFoto}
                  disabled={uploadingFoto || removendoFoto}
                  className="border-2 border-slate-200 text-slate-500 hover:border-[#ff6b6b] hover:text-[#ff6b6b] disabled:opacity-50 font-bold px-4 py-2.5 rounded-full text-xs transition flex items-center gap-2"
                >
                  <Trash2 className="w-4 h-4" />

                  {removendoFoto
                    ? 'Removendo...'
                    : 'Remover'}
                </button>
              )}

            </div>

            <p className="text-[11px] text-slate-400 mt-3 text-center">
              JPG, PNG ou WEBP • máximo 5 MB
            </p>

          </div>
        </div>

        {/* DIVISOR */}
        <div className="border-t border-slate-100 mb-6" />

        {/* FORMULÁRIO */}
        <form onSubmit={handleSubmit} className="space-y-5">

          {/* Nome */}
          <div>

            <label className="block text-xs font-bold text-slate-500 uppercase mb-1">
              Nome Completo
            </label>

            <div className="relative">

              <User className="w-5 h-5 text-slate-400 absolute left-4 top-3.5" />

              <input
                type="text"
                value={nome}
                onChange={e => setNome(e.target.value)}
                required
                className="w-full pl-12 pr-4 py-3 bg-[#f5f7fa] rounded-2xl border border-slate-200 text-sm focus:outline-none focus:border-[#4ecdc4]"
              />

            </div>
          </div>

          {/* E-mail */}
          <div>

            <label className="block text-xs font-bold text-slate-500 uppercase mb-1">
              E-mail
            </label>

            <div className="relative">

              <Mail className="w-5 h-5 text-slate-400 absolute left-4 top-3.5" />

              <input
                type="email"
                value={email}
                onChange={e => setEmail(e.target.value)}
                required
                className="w-full pl-12 pr-4 py-3 bg-[#f5f7fa] rounded-2xl border border-slate-200 text-sm focus:outline-none focus:border-[#4ecdc4]"
              />

            </div>
          </div>

          {/* Data de nascimento */}
          <div>

            <label className="block text-xs font-bold text-slate-500 uppercase mb-1">
              Data de Nascimento
            </label>

            <div className="relative">

              <Calendar className="w-5 h-5 text-slate-400 absolute left-4 top-3.5" />

              <input
                type="date"
                value={dataNasc}
                onChange={e => setDataNasc(e.target.value)}
                className="w-full pl-12 pr-4 py-3 bg-[#f5f7fa] rounded-2xl border border-slate-200 text-sm focus:outline-none focus:border-[#4ecdc4]"
              />

            </div>
          </div>

          {/* Salvar */}
          <button
            type="submit"
            disabled={saving || uploadingFoto || removendoFoto}
            className="w-full bg-[#1a535c] hover:bg-[#1a535c]/90 disabled:opacity-50 text-white font-bold py-3.5 rounded-2xl text-sm transition shadow-lg flex items-center justify-center gap-2 mt-6"
          >

            <Save className="w-5 h-5" />

            <span>
              {saving ? 'Salvando...' : 'Salvar Alterações'}
            </span>

          </button>

        </form>
      </div>
    </div>
  );
};