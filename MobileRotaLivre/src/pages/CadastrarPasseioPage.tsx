import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { Categoria } from '../types';
import { PlusCircle, ArrowLeft, MapPin, Tag, Clock, FileText, Image as ImageIcon } from 'lucide-react';

export const CadastrarPasseioPage: React.FC = () => {
  const { getAuthHeader } = useAuth();
  const navigate = useNavigate();

  const [nomePasseio, setNomePasseio] = useState('');
  const [idCategoria, setIdCategoria] = useState(1);
  const [funcionamento, setFuncionamento] = useState('');
  const [descricao, setDescricao] = useState('');
  const [imgUrl, setImgUrl] = useState('ibirapuera.jpeg');

  // Address
  const [nomeRua, setNomeRua] = useState('');
  const [numeroRua, setNumeroRua] = useState('');
  const [bairro, setBairro] = useState('');
  const [cep, setCep] = useState('');

  const [categorias, setCategorias] = useState<Categoria[]>([]);
  const [saving, setSaving] = useState(false);
  const [msg, setMsg] = useState('');

  useEffect(() => {
    fetch('/api/home')
      .then(res => res.json())
      .then(data => setCategorias(data.categorias || []));
  }, []);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSaving(true);

    try {
      const res = await fetch('/api/passeios/cadastrar', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json', ...getAuthHeader() },
        body: JSON.stringify({
          nome_passeio: nomePasseio,
          id_categoria: Number(idCategoria),
          funcionamento,
          descricao,
          img_url: imgUrl,
          nome_rua: nomeRua,
          numero_rua: numeroRua,
          bairro,
          cep
        })
      });

      const data = await res.json();
      if (data.sucesso) {
        setMsg('Passeio cadastrado com sucesso!');
        setTimeout(() => navigate(`/passeio/${data.passeio.id_passeio}`), 1200);
      }
    } catch (err) {
      console.error('Erro ao cadastrar passeio:', err);
    } finally {
      setSaving(false);
    }
  };

  return (
    <div className="pt-20 pb-24 max-w-md mx-auto px-4">
      <button
        onClick={() => navigate(-1)}
        className="p-3 bg-white rounded-full text-[#1a535c] hover:bg-[#4ecdc4] hover:text-white transition shadow-md flex items-center gap-2 mb-6 font-semibold text-sm"
      >
        <ArrowLeft className="w-5 h-5" />
        <span>Voltar</span>
      </button>

      <div className="bg-white rounded-3xl p-8 shadow-2xl border border-slate-100">
        <div className="flex items-center gap-3 mb-6">
          <div className="p-3 bg-[#4ecdc4]/15 rounded-2xl text-[#1a535c]">
            <PlusCircle className="w-8 h-8 text-[#ff6b6b]" />
          </div>
          <div>
            <h1 className="text-2xl font-extrabold text-[#1a535c]">Cadastrar Novo Passeio</h1>
            <p className="text-xs text-slate-500">Adicione uma nova opção de lazer para a comunidade</p>
          </div>
        </div>

        {msg && (
          <div className="mb-6 p-4 bg-emerald-100 text-emerald-800 rounded-2xl text-xs font-bold">
            {msg}
          </div>
        )}

        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-xs font-bold text-slate-500 uppercase mb-1">Nome do Passeio</label>
            <input
              type="text"
              value={nomePasseio}
              onChange={e => setNomePasseio(e.target.value)}
              placeholder="Ex: Parque do Povo"
              required
              className="w-full p-3 bg-[#f5f7fa] rounded-2xl border border-slate-200 text-sm focus:outline-none focus:border-[#4ecdc4]"
            />
          </div>

          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <div>
              <label className="block text-xs font-bold text-slate-500 uppercase mb-1">Categoria</label>
              <select
                value={idCategoria}
                onChange={e => setIdCategoria(Number(e.target.value))}
                className="w-full p-3 bg-[#f5f7fa] rounded-2xl border border-slate-200 text-sm focus:outline-none focus:border-[#4ecdc4]"
              >
                {categorias.map(c => (
                  <option key={c.id_categoria} value={c.id_categoria}>
                    {c.tipo_categoria}
                  </option>
                ))}
              </select>
            </div>

            <div>
              <label className="block text-xs font-bold text-slate-500 uppercase mb-1">Imagem Ilustrativa</label>
              <select
                value={imgUrl}
                onChange={e => setImgUrl(e.target.value)}
                className="w-full p-3 bg-[#f5f7fa] rounded-2xl border border-slate-200 text-sm focus:outline-none focus:border-[#4ecdc4]"
              >
                <option value="ibirapuera.jpeg">Ibirapuera (Parque)</option>
                <option value="aguabranca.jpeg">Água Branca (Parque)</option>
                <option value="villalobos.jpg">Villa-Lobos (Parque)</option>
                <option value="ipiranga.jpg">Ipiranga (Museu)</option>
                <option value="mac.jpg">MAC USP (Museu)</option>
                <option value="theatro.jpg">Theatro Municipal (Teatro)</option>
                <option value="renault.jpeg">Teatro Renault (Teatro)</option>
              </select>
            </div>
          </div>

          <div>
            <label className="block text-xs font-bold text-slate-500 uppercase mb-1">Horário de Funcionamento</label>
            <input
              type="text"
              value={funcionamento}
              onChange={e => setFuncionamento(e.target.value)}
              placeholder="Ex: Terça a Domingo: 08:00 - 18:00"
              required
              className="w-full p-3 bg-[#f5f7fa] rounded-2xl border border-slate-200 text-sm focus:outline-none focus:border-[#4ecdc4]"
            />
          </div>

          <div>
            <label className="block text-xs font-bold text-slate-500 uppercase mb-1">Descrição</label>
            <textarea
              value={descricao}
              onChange={e => setDescricao(e.target.value)}
              placeholder="Conte mais detalhes sobre este passeio..."
              rows={4}
              required
              className="w-full p-3 bg-[#f5f7fa] rounded-2xl border border-slate-200 text-sm focus:outline-none focus:border-[#4ecdc4]"
            />
          </div>

          {/* Address Fields */}
          <div className="pt-4 border-t border-slate-100">
            <h3 className="font-bold text-[#1a535c] text-sm mb-3">Endereço do Local</h3>

            <div className="grid grid-cols-1 sm:grid-cols-3 gap-3 mb-3">
              <div className="sm:col-span-2">
                <input
                  type="text"
                  value={nomeRua}
                  onChange={e => setNomeRua(e.target.value)}
                  placeholder="Nome da Rua / Av."
                  required
                  className="w-full p-3 bg-[#f5f7fa] rounded-2xl border border-slate-200 text-sm focus:outline-none"
                />
              </div>
              <div>
                <input
                  type="text"
                  value={numeroRua}
                  onChange={e => setNumeroRua(e.target.value)}
                  placeholder="Número"
                  required
                  className="w-full p-3 bg-[#f5f7fa] rounded-2xl border border-slate-200 text-sm focus:outline-none"
                />
              </div>
            </div>

            <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
              <input
                type="text"
                value={bairro}
                onChange={e => setBairro(e.target.value)}
                placeholder="Bairro"
                required
                className="w-full p-3 bg-[#f5f7fa] rounded-2xl border border-slate-200 text-sm focus:outline-none"
              />
              <input
                type="text"
                value={cep}
                onChange={e => setCep(e.target.value)}
                placeholder="CEP (ex: 01000-000)"
                required
                className="w-full p-3 bg-[#f5f7fa] rounded-2xl border border-slate-200 text-sm focus:outline-none"
              />
            </div>
          </div>

          <button
            type="submit"
            disabled={saving}
            className="w-full bg-[#1a535c] hover:bg-[#1a535c]/90 text-white font-bold py-3.5 rounded-2xl text-sm transition shadow-lg flex items-center justify-center gap-2 mt-6"
          >
            <PlusCircle className="w-5 h-5" />
            <span>{saving ? 'Cadastrando...' : 'Cadastrar Passeio'}</span>
          </button>
        </form>
      </div>
    </div>
  );
};
