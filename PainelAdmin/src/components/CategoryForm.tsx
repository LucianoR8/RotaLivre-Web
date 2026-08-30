import {
  useState,
  useEffect,
  FormEvent,
  ChangeEvent,
} from 'react';

import { Category } from '../types';

import {
  ArrowLeft,
  Save,
  Image as ImageIcon,
  Upload,
  History,
} from 'lucide-react';

interface CategoryFormProps {
  categoryToEdit: Category | null;

  onSave: (
    categoryData: Partial<Category>,
    imageFile?: File | null
  ) => void;

  onCancel: () => void;
}

export function CategoryForm({
  categoryToEdit,
  onSave,
  onCancel,
}: CategoryFormProps) {
  const isEditing = !!categoryToEdit;

  // =========================================================
  // ESTADOS
  // =========================================================

  const [name, setName] = useState(
    categoryToEdit?.name || ''
  );

  const [imageUrl, setImageUrl] = useState(
    categoryToEdit?.imageUrl || ''
  );

  const [imageFile, setImageFile] =
    useState<File | null>(null);

  const [imagePreview, setImagePreview] =
    useState(
      categoryToEdit?.imageUrl || ''
    );

  const [isActive, setIsActive] = useState(
    categoryToEdit?.isActive ?? true
  );

  // =========================================================
  // ATUALIZA FORM AO TROCAR CATEGORIA
  // =========================================================

  useEffect(() => {
    setName(
      categoryToEdit?.name || ''
    );

    setImageUrl(
      categoryToEdit?.imageUrl || ''
    );

    setImagePreview(
      categoryToEdit?.imageUrl || ''
    );

    setImageFile(null);

    setIsActive(
      categoryToEdit?.isActive ?? true
    );
  }, [categoryToEdit]);

  // =========================================================
  // SELECIONAR IMAGEM
  // =========================================================

  const handleImageChange = (
    e: ChangeEvent<HTMLInputElement>
  ) => {
    const file =
      e.target.files?.[0];

    if (!file) {
      return;
    }

    console.log('======================================');
    console.log('🖼️ IMAGEM DE CATEGORIA SELECIONADA');
    console.log('Arquivo:', file.name);
    console.log('Tipo:', file.type);
    console.log('Tamanho:', file.size);
    console.log('======================================');

    setImageFile(file);

    const previewUrl =
      URL.createObjectURL(file);

    setImagePreview(previewUrl);
  };

  // =========================================================
  // SUBMIT
  // =========================================================

  const handleSubmit = (
    e: FormEvent
  ) => {
    e.preventDefault();

    if (!name.trim()) {
      console.warn(
        '⚠️ Nome da categoria não informado.'
      );

      return;
    }

    const data: Partial<Category> = {
      name: name.trim(),

      imageUrl:
        imageUrl?.trim() || '',

      isActive,
    };

    console.log('======================================');
    console.log('📤 CATEGORY FORM ENVIANDO');
    console.log('Dados:', data);
    console.log(
      'Arquivo:',
      imageFile
    );
    console.log('======================================');

    onSave(
      data,
      imageFile
    );
  };

  // =========================================================
  // RENDER
  // =========================================================

  return (
    <div
      id="category-form-screen"
      className="max-w-4xl mx-auto space-y-6"
    >

      {/* =====================================================
          CABEÇALHO
      ====================================================== */}

      <div className="bg-white p-5 rounded-2xl border border-slate-200 shadow-sm flex items-center justify-between">

        <div className="flex items-center gap-3">

          <button
            type="button"
            onClick={onCancel}
            className="p-2 text-slate-500 hover:text-slate-800 hover:bg-slate-100 rounded-xl transition-colors cursor-pointer"
            title="Voltar"
          >
            <ArrowLeft className="w-5 h-5" />
          </button>

          <div>

            <h2 className="text-xl font-extrabold text-slate-800">
              {isEditing
                ? 'Editar Categoria'
                : 'Cadastrar Nova Categoria'}
            </h2>

            <p className="text-xs text-slate-500 mt-1">
              {isEditing
                ? 'Atualize as informações da categoria.'
                : 'Cadastre uma nova categoria de passeios.'}
            </p>

          </div>

        </div>

      </div>

      {/* =====================================================
          FORMULÁRIO
      ====================================================== */}

      <form
        onSubmit={handleSubmit}
        className="space-y-6"
      >

        <div className="bg-white rounded-2xl border border-slate-200 shadow-sm p-6 space-y-6">

          {/* =================================================
              NOME
          ================================================== */}

          <div>

            <label
              htmlFor="category-name"
              className="block text-xs font-bold text-slate-700 uppercase tracking-wider mb-2"
            >
              Nome da Categoria{' '}
              <span className="text-[#ff6b6b]">
                *
              </span>
            </label>

            <input
              id="category-name"
              type="text"
              required
              value={name}
              onChange={(e) =>
                setName(
                  e.target.value
                )
              }
              placeholder="Ex.: Museus"
              className="w-full px-4 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-sm text-slate-800 focus:outline-none focus:ring-2 focus:ring-[#4ecdc4] focus:border-[#1a535c] transition-all"
            />

          </div>

          {/* =================================================
              UPLOAD DA IMAGEM
          ================================================== */}

          <div>

            <label className="block text-xs font-bold text-slate-700 uppercase tracking-wider mb-2">
              Imagem da Categoria{' '}
              <span className="text-[#ff6b6b]">
                *
              </span>
            </label>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-5">

              {/* SELEÇÃO */}

              <div>

                <label
                  htmlFor="category-image"
                  className="w-full min-h-[180px] rounded-xl border-2 border-dashed border-slate-300 hover:border-[#4ecdc4] bg-slate-50 hover:bg-slate-100 flex flex-col items-center justify-center cursor-pointer transition-all"
                >

                  <div className="p-3 rounded-xl bg-white shadow-sm mb-3">

                    <Upload className="w-6 h-6 text-[#1a535c]" />

                  </div>

                  <span className="text-sm font-bold text-slate-700">
                    Selecionar imagem
                  </span>

                  <span className="text-xs text-slate-400 mt-1">
                    JPG, PNG ou WEBP
                  </span>

                  {imageFile && (

                    <span className="text-xs text-[#1a535c] font-semibold mt-3 px-3 text-center break-all">
                      {imageFile.name}
                    </span>

                  )}

                </label>

                <input
                  id="category-image"
                  type="file"
                  accept="image/jpeg,image/png,image/webp"
                  onChange={
                    handleImageChange
                  }
                  className="hidden"
                />

              </div>

              {/* PREVIEW */}

              <div>

                <p className="text-xs font-bold text-slate-500 uppercase tracking-wider mb-2">
                  Pré-visualização
                </p>

                <div className="w-full h-[180px] rounded-xl overflow-hidden border border-slate-200 bg-slate-100">

                  {imagePreview ? (

                    <img
                      src={imagePreview}
                      alt="Preview da categoria"
                      className="w-full h-full object-cover"
                      onError={() => {
                        setImagePreview('');
                      }}
                    />

                  ) : (

                    <div className="w-full h-full flex flex-col items-center justify-center text-slate-400">

                      <ImageIcon className="w-10 h-10 text-slate-300 mb-2" />

                      <span className="text-xs">
                        Nenhuma imagem selecionada
                      </span>

                    </div>

                  )}

                </div>

              </div>

            </div>

            {/* URL ATUAL */}

            {imageUrl && !imageFile && (

              <p className="text-[11px] text-slate-400 mt-2 break-all">
                Imagem atual: {imageUrl}
              </p>

            )}

          </div>

          {/* =================================================
              STATUS
          ================================================== */}

          <div className="flex items-center justify-between p-4 rounded-xl bg-slate-50 border border-slate-200">

            <div>

              <p className="text-sm font-bold text-slate-700">
                Categoria ativa
              </p>

              <p className="text-xs text-slate-500 mt-1">
                Categorias ativas ficam disponíveis no sistema.
              </p>

            </div>

            <button
              type="button"
              onClick={() =>
                setIsActive(
                  !isActive
                )
              }
              className={`relative inline-flex h-6 w-11 items-center rounded-full transition-colors ${
                isActive
                  ? 'bg-[#1a535c]'
                  : 'bg-slate-300'
              }`}
            >

              <span
                className={`inline-block h-4 w-4 transform rounded-full bg-white transition-transform ${
                  isActive
                    ? 'translate-x-6'
                    : 'translate-x-1'
                }`}
              />

            </button>

          </div>

        </div>

        {/* =====================================================
            AUDITORIA
        ====================================================== */}

        {isEditing &&
          categoryToEdit?.audit && (

            <div className="bg-slate-100/80 rounded-2xl border border-slate-200/80 p-5 flex items-start gap-3.5">

              <div className="p-2 rounded-xl bg-white text-[#1a535c] shadow-sm shrink-0">

                <History className="w-5 h-5" />

              </div>

              <div className="text-xs space-y-1">

                <span className="font-bold text-slate-700 uppercase tracking-wider block">
                  Registro de Auditoria
                </span>

                <p className="text-slate-600 font-medium">

                  Última edição por:{' '}

                  <strong className="text-slate-900">
                    {
                      categoryToEdit
                        .audit
                        .lastEditedBy
                    }
                  </strong>

                </p>

                <p className="text-slate-500 font-medium">

                  Em:{' '}

                  <span className="font-semibold text-slate-800">
                    {
                      categoryToEdit
                        .audit
                        .lastEditedAt
                    }
                  </span>

                </p>

              </div>

            </div>

          )}

        {/* =====================================================
            BOTÕES
        ====================================================== */}

        <div className="flex items-center justify-end gap-3 pt-2">

          <button
            type="button"
            onClick={onCancel}
            className="px-5 py-2.5 rounded-xl border border-slate-300 text-slate-700 font-semibold text-sm hover:bg-slate-100 transition-colors cursor-pointer"
          >
            Cancelar
          </button>

          <button
            type="submit"
            className="inline-flex items-center gap-2 bg-[#1a535c] hover:bg-[#154249] text-white font-bold text-sm px-6 py-2.5 rounded-xl shadow-md transition-all active:scale-98 cursor-pointer"
          >

            <Save className="w-4 h-4 text-[#4ecdc4]" />

            <span>
              Salvar Categoria
            </span>

          </button>

        </div>

      </form>

    </div>
  );
}