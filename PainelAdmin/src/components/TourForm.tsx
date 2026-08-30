import {
  useState,
  useEffect,
  FormEvent,
} from 'react';

import {
  Tour,
  Category,
  AdminUser,
  AvailabilityBlock,
} from '../types';

import { passeioService } from '../services/passeioService';

import {
  ArrowLeft,
  Save,
  Image as ImageIcon,
  History,
  MapPin,
  Plus,
  Trash2,
  FileText,
  Navigation,
  Compass,
  Upload,
  X,
  Clock,
} from 'lucide-react';

interface TourFormProps {
  tourToEdit: Tour | null;
  categories: Category[];
  adminUser: AdminUser;
  onSave: (tourData: Partial<Tour>) => void;
  onCancel: () => void;
}

export function TourForm({
  tourToEdit,
  categories,
  onSave,
  onCancel,
}: TourFormProps) {

  const isEditing =
    !!tourToEdit;

  // ============================================================
  // DADOS DA API PARA COMPATIBILIDADE
  // ============================================================

  const tourApiData = tourToEdit as
    | (Tour & {
        categoriaId?: number;
        categoriaNome?: string;
      })
    | null;

  const initialCategoryId =
    tourToEdit?.categoryId ??
    tourApiData?.categoriaId ??
    (
      tourApiData?.categoriaNome
        ? categories.find(
            (cat) =>
              cat.name ===
              tourApiData.categoriaNome
          )?.id
        : undefined
    ) ??
    (
      categories.length > 0
        ? categories[0].id
        : 0
    );

  // ============================================================
  // ESTADOS PRINCIPAIS
  // ============================================================

  const [name, setName] =
    useState(
      tourToEdit?.name || ''
    );

  const [categoryId, setCategoryId] =
    useState<number>(
      Number(initialCategoryId)
    );

  const [photoUrl, setPhotoUrl] =
    useState(
      tourToEdit?.photoUrl || ''
    );

  /*
   * NOVO:
   * Arquivo escolhido pelo administrador.
   */
  const [photoFile, setPhotoFile] =
    useState<File | null>(null);

  /*
   * NOVO:
   * URL temporária usada somente para preview.
   */
  const [photoPreview, setPhotoPreview] =
    useState(
      tourToEdit?.photoUrl || ''
    );

  const [description, setDescription] =
    useState(
      tourToEdit?.description || ''
    );

  const [
    operatingDescription,
    setOperatingDescription,
  ] = useState(
    tourToEdit?.operatingDescription || ''
  );

  // ============================================================
  // LOCALIZAÇÃO
  // ============================================================

  const [latitude, setLatitude] =
    useState(
      tourToEdit?.location?.latitude?.toString() ||
      ''
    );

  const [longitude, setLongitude] =
    useState(
      tourToEdit?.location?.longitude?.toString() ||
      ''
    );

  // ============================================================
  // ENDEREÇO
  // ============================================================

  const [street, setStreet] =
    useState(
      tourToEdit?.address?.street || ''
    );

  const [number, setNumber] =
    useState(
      tourToEdit?.address?.number || ''
    );

  const [complement, setComplement] =
    useState(
      tourToEdit?.address?.complement || ''
    );

  const [neighborhood, setNeighborhood] =
    useState(
      tourToEdit?.address?.neighborhood || ''
    );

  const [zipCode, setZipCode] =
    useState(
      tourToEdit?.address?.zipCode || ''
    );

  // ============================================================
  // DISPONIBILIDADES
  // ============================================================

  const [
    availabilities,
    setAvailabilities,
  ] = useState<AvailabilityBlock[]>(
    tourToEdit?.availabilities || []
  );

  const [
    newTimeInputs,
    setNewTimeInputs,
  ] = useState<
    Record<string, string>
  >({});

  // ============================================================
  // ESTADOS DA IMAGEM
  // ============================================================

  const [imageError, setImageError] =
    useState(false);

  const [uploadingImage, setUploadingImage] =
    useState(false);

  const [saving, setSaving] =
    useState(false);

  const [errorMessage, setErrorMessage] =
    useState('');

  // ============================================================
  // LIMPA URL TEMPORÁRIA DA IMAGEM
  // ============================================================

  useEffect(() => {

    return () => {

      if (
        photoPreview &&
        photoPreview.startsWith(
          'blob:'
        )
      ) {
        URL.revokeObjectURL(
          photoPreview
        );
      }
    };

  }, [photoPreview]);

  // ============================================================
  // SELECIONAR IMAGEM
  // ============================================================

  const handlePhotoChange = (
    e: React.ChangeEvent<HTMLInputElement>
  ) => {

    const file =
      e.target.files?.[0];

    if (!file) {
      return;
    }

    setErrorMessage('');
    setImageError(false);

    // ----------------------------------------------------------
    // VALIDAÇÃO DO TIPO
    // ----------------------------------------------------------

    const tiposPermitidos = [
      'image/jpeg',
      'image/png',
      'image/webp',
    ];

    if (
      !tiposPermitidos.includes(
        file.type.toLowerCase()
      )
    ) {

      setErrorMessage(
        'Formato de imagem não suportado. Use JPG, PNG ou WEBP.'
      );

      e.target.value = '';

      return;
    }

    // ----------------------------------------------------------
    // VALIDAÇÃO DE TAMANHO
    // ----------------------------------------------------------

    const tamanhoMaximo =
      10 * 1024 * 1024;

    if (
      file.size >
      tamanhoMaximo
    ) {

      setErrorMessage(
        'A imagem deve ter no máximo 10 MB.'
      );

      e.target.value = '';

      return;
    }

    // ----------------------------------------------------------
    // REVOGA PREVIEW ANTERIOR
    // ----------------------------------------------------------

    if (
      photoPreview &&
      photoPreview.startsWith(
        'blob:'
      )
    ) {
      URL.revokeObjectURL(
        photoPreview
      );
    }

    // ----------------------------------------------------------
    // GUARDA O ARQUIVO
    // ----------------------------------------------------------

    setPhotoFile(file);

    // ----------------------------------------------------------
    // CRIA PREVIEW
    // ----------------------------------------------------------

    const previewUrl =
      URL.createObjectURL(file);

    setPhotoPreview(
      previewUrl
    );
  };

  // ============================================================
  // REMOVER NOVA IMAGEM SELECIONADA
  // ============================================================

  const handleRemoveSelectedPhoto = () => {

    if (
      photoPreview &&
      photoPreview.startsWith(
        'blob:'
      )
    ) {
      URL.revokeObjectURL(
        photoPreview
      );
    }

    setPhotoFile(null);

    /*
     * Se estiver editando, volta
     * para a imagem original.
     */
    setPhotoPreview(
      tourToEdit?.photoUrl || ''
    );

    setImageError(false);
  };

  // ============================================================
  // ADICIONAR BLOCO DE DISPONIBILIDADE
  // ============================================================

  const handleAddAvailabilityBlock = () => {

    const today =
      new Date();

    today.setDate(
      today.getDate() + 7
    );

    const defaultDateStr =
      today
        .toISOString()
        .split('T')[0];

    const newBlock:
      AvailabilityBlock = {

      id:
        'av-' +
        Math.random()
          .toString(36)
          .substring(2, 9),

      date:
        defaultDateStr,

      timeSlots: [
        '09:00',
        '14:00',
      ],
    };

    setAvailabilities([
      ...availabilities,
      newBlock,
    ]);
  };

  // ============================================================
  // REMOVER BLOCO
  // ============================================================

  const handleRemoveAvailabilityBlock = (
    blockId: string | number
  ) => {

    setAvailabilities(
      availabilities.filter(
        (b) =>
          b.id !== blockId
      )
    );
  };

  // ============================================================
  // ALTERAR DATA DO BLOCO
  // ============================================================

  const handleUpdateBlockDate = (
    blockId: string | number,
    newDate: string
  ) => {

    setAvailabilities(
      availabilities.map(
        (b) =>
          b.id === blockId
            ? {
                ...b,
                date: newDate,
              }
            : b
      )
    );
  };

  // ============================================================
  // ADICIONAR HORÁRIO
  // ============================================================

  const handleAddTimeSlot = (
    blockId: string | number
  ) => {

    const timeValue =
      (
        newTimeInputs[
          blockId
        ] || ''
      ).trim();

    if (!timeValue) {
      return;
    }

    setAvailabilities(
      availabilities.map(
        (b) => {

          if (
            b.id === blockId
          ) {

            if (
              b.timeSlots.includes(
                timeValue
              )
            ) {
              return b;
            }

            const updatedSlots =
              [
                ...b.timeSlots,
                timeValue,
              ].sort();

            return {
              ...b,
              timeSlots:
                updatedSlots,
            };
          }

          return b;
        }
      )
    );

    setNewTimeInputs({
      ...newTimeInputs,
      [blockId]: '',
    });
  };

  // ============================================================
  // REMOVER HORÁRIO
  // ============================================================

  const handleRemoveTimeSlot = (
    blockId: string | number,
    slotToRemove: string
  ) => {

    setAvailabilities(
      availabilities.map(
        (b) => {

          if (
            b.id === blockId
          ) {

            return {
              ...b,

              timeSlots:
                b.timeSlots.filter(
                  (t) =>
                    t !==
                    slotToRemove
                ),
            };
          }

          return b;
        }
      )
    );
  };

  // ============================================================
  // PRESETS DE HORÁRIO
  // ============================================================

  const handleApplyPresetTimes = (
    blockId: string | number,
    presets: string[]
  ) => {

    setAvailabilities(
      availabilities.map(
        (b) => {

          if (
            b.id === blockId
          ) {

            const uniqueSlots =
              Array.from(
                new Set([
                  ...b.timeSlots,
                  ...presets,
                ])
              ).sort();

            return {
              ...b,
              timeSlots:
                uniqueSlots,
            };
          }

          return b;
        }
      )
    );
  };

  // ============================================================
  // SALVAR
  // ============================================================

  const handleSubmit = async (
    e: FormEvent
  ) => {

    e.preventDefault();

    if (!name.trim()) {
      return;
    }

    setErrorMessage('');
    setSaving(true);

    try {

      let finalPhotoUrl =
        photoUrl.trim();

      // ========================================================
      // SE ESCOLHEU UMA NOVA IMAGEM
      // FAZ O UPLOAD PRIMEIRO
      // ========================================================

      if (photoFile) {

        setUploadingImage(true);

        console.log(
          '📤 Fazendo upload da nova imagem...'
        );

        finalPhotoUrl =
          await passeioService.uploadImagem(
            photoFile
          );

        console.log(
          '✅ URL retornada pela API:',
          finalPhotoUrl
        );

        setUploadingImage(false);
      }

      // ========================================================
      // CATEGORIA
      // ========================================================

      const selectedCategoryObj =
        categories.find(
          (c) =>
            Number(c.id) ===
            Number(categoryId)
        );

      // ========================================================
      // DADOS FINAIS DO PASSEIO
      // ========================================================

      const tourData:
        Partial<Tour> = {

        name:
          name.trim(),

        categoryId:
          Number(categoryId),

        categoryName:
          selectedCategoryObj?.name ||
          'Geral',

        photoUrl:
          finalPhotoUrl,

        description:
          description.trim(),

        operatingDescription:
          operatingDescription.trim(),

        location: {

          latitude:
            parseFloat(
              latitude
            ) || 0,

          longitude:
            parseFloat(
              longitude
            ) || 0,
        },

        address: {

          street:
            street.trim(),

          number:
            number.trim(),

          complement:
            complement.trim(),

          neighborhood:
            neighborhood.trim(),

          zipCode:
            zipCode.trim(),
        },

        availabilities,

        status:
          'ativo',
      };

      console.log(
        '======================================'
      );

      console.log(
        '💾 DADOS FINAIS DO PASSEIO'
      );

      console.log(
        tourData
      );

      console.log(
        '======================================'
      );

      // ========================================================
      // DEVOLVE PARA O COMPONENTE PAI
      // ========================================================

      onSave(
        tourData
      );

    } catch (error: any) {

      console.error(
        '❌ ERRO AO SALVAR PASSEIO:',
        error
      );

      setErrorMessage(
        error?.response?.data ||
        error?.message ||
        'Não foi possível salvar o passeio.'
      );

    } finally {

      setUploadingImage(false);
      setSaving(false);
    }
  };

  // ============================================================
  // RENDER
  // ============================================================

  return (

    <div
      id="tour-form-screen"
      className="max-w-5xl mx-auto space-y-6"
    >

      {/* ======================================================
          CABEÇALHO
      ======================================================= */}

      <div className="bg-white p-5 rounded-2xl border border-slate-200 shadow-sm flex items-center justify-between">

        <div className="flex items-center gap-3">

          <button
            type="button"
            onClick={onCancel}
            className="p-2 text-slate-500 hover:text-slate-800 hover:bg-slate-100 rounded-xl transition-colors cursor-pointer"
            title="Voltar para listagem"
          >
            <ArrowLeft className="w-5 h-5" />
          </button>

          <div>

            <h2 className="text-xl font-extrabold text-slate-800">

              {isEditing
                ? `Editar Passeio: ${tourToEdit?.name}`
                : 'Cadastrar Novo Passeio'}

            </h2>

            <p className="text-xs text-slate-500 mt-0.5">

              Preencha os dados básicos,
              localização, endereço e agenda
              de horários disponíveis.

            </p>

          </div>

        </div>

      </div>

      {/* ======================================================
          MENSAGEM DE ERRO
      ======================================================= */}

      {errorMessage && (

        <div className="bg-red-50 border border-red-200 text-red-700 rounded-xl px-4 py-3 text-sm font-medium">

          {errorMessage}

        </div>

      )}

      <form
        onSubmit={handleSubmit}
        className="space-y-6"
      >

        {/* ====================================================
            INFORMAÇÕES BÁSICAS
        ===================================================== */}

        <div
          id="section-basic-info"
          className="bg-white rounded-2xl border border-slate-200 shadow-sm p-6 space-y-5"
        >

          <div className="flex items-center gap-2.5 pb-3 border-b border-slate-100">

            <div className="p-2 rounded-xl bg-[#1a535c]/10 text-[#1a535c]">

              <FileText className="w-5 h-5" />

            </div>

            <div>

              <h3 className="text-base font-bold text-slate-800">

                1. Informações Básicas

              </h3>

              <p className="text-xs text-slate-500">

                Identificação principal,
                categoria e regras de funcionamento

              </p>

            </div>

          </div>

          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">

            {/* =================================================
                NOME
            ================================================== */}

            <div className="md:col-span-2">

              <label
                htmlFor="input-tour-name"
                className="block text-xs font-bold text-slate-700 uppercase tracking-wider mb-1.5"
              >

                Nome do Passeio{' '}

                <span className="text-[#ff6b6b]">
                  *
                </span>

              </label>

              <input
                id="input-tour-name"
                type="text"
                required
                value={name}
                onChange={(e) =>
                  setName(
                    e.target.value
                  )
                }
                placeholder="Ex: Parque da Aclimação"
                className="w-full px-4 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-sm text-slate-800 focus:outline-none focus:ring-2 focus:ring-[#4ecdc4] focus:border-[#1a535c] transition-all"
              />

            </div>

            {/* =================================================
                CATEGORIA
            ================================================== */}

            <div>

              <label
                htmlFor="select-tour-category"
                className="block text-xs font-bold text-slate-700 uppercase tracking-wider mb-1.5"
              >

                Categoria do Passeio{' '}

                <span className="text-[#ff6b6b]">
                  *
                </span>

              </label>

              <select
                id="select-tour-category"
                required
                value={
                  categoryId || ''
                }
                onChange={(e) =>
                  setCategoryId(
                    Number(
                      e.target.value
                    )
                  )
                }
                className="w-full px-4 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-sm text-slate-800 focus:outline-none focus:ring-2 focus:ring-[#4ecdc4] focus:border-[#1a535c]"
              >

                <option
                  value=""
                  disabled
                >
                  Selecione uma categoria...
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

            {/* =================================================
                FOTO
            ================================================== */}

            <div className="md:col-span-2">

              <div className="flex items-center justify-between mb-1.5">

                <label
                  htmlFor="input-tour-photo"
                  className="block text-xs font-bold text-slate-700 uppercase tracking-wider"
                >

                  Foto Principal{' '}

                  <span className="text-[#ff6b6b]">
                    *
                  </span>

                </label>

                <span className="text-[11px] text-slate-400">

                  JPG, PNG ou WEBP

                </span>

              </div>

              {/* ==============================================
                  SELEÇÃO DO ARQUIVO
              =============================================== */}

              <div className="relative">

                <input
                  id="input-tour-photo"
                  type="file"
                  accept="image/jpeg,image/png,image/webp"
                  onChange={
                    handlePhotoChange
                  }
                  className="hidden"
                />

                <label
                  htmlFor="input-tour-photo"
                  className="flex items-center justify-center gap-2 w-full px-4 py-4 bg-slate-50 border-2 border-dashed border-slate-300 rounded-xl text-sm font-semibold text-slate-600 hover:bg-slate-100 hover:border-[#4ecdc4] transition-all cursor-pointer"
                >

                  <Upload className="w-5 h-5 text-[#1a535c]" />

                  <span>

                    {photoFile
                      ? photoFile.name
                      : 'Clique para escolher uma imagem'}

                  </span>

                </label>

              </div>

              {/* ==============================================
                  PREVIEW
              =============================================== */}

              {photoPreview &&
                !imageError && (

                  <div className="mt-3 relative w-full h-56 rounded-xl overflow-hidden border border-slate-200 bg-slate-100">

                    <img
                      src={photoPreview}
                      alt="Preview do passeio"
                      referrerPolicy="no-referrer"
                      className="w-full h-full object-cover"
                      onError={() =>
                        setImageError(
                          true
                        )
                      }
                    />

                    {photoFile && (

                      <button
                        type="button"
                        onClick={
                          handleRemoveSelectedPhoto
                        }
                        className="absolute top-3 right-3 p-2 bg-white/90 text-red-600 rounded-xl shadow-md hover:bg-white transition-colors"
                        title="Cancelar nova imagem"
                      >

                        <X className="w-4 h-4" />

                      </button>

                    )}

                  </div>

                )}

              {/* ==============================================
                  INFORMAÇÃO SOBRE UPLOAD
              =============================================== */}

              <p className="text-[11px] text-slate-400 mt-2">

                A imagem será enviada automaticamente
                para o Storage ao salvar o passeio.

              </p>

            </div>

            {/* =================================================
                DESCRIÇÃO
            ================================================== */}

            <div className="md:col-span-2">

              <label
                htmlFor="textarea-tour-description"
                className="block text-xs font-bold text-slate-700 uppercase tracking-wider mb-1.5"
              >

                Descrição do Passeio{' '}

                <span className="text-[#ff6b6b]">
                  *
                </span>

              </label>

              <textarea
                id="textarea-tour-description"
                rows={3}
                required
                value={description}
                onChange={(e) =>
                  setDescription(
                    e.target.value
                  )
                }
                placeholder="Detalhe os atrativos..."
                className="w-full px-4 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-sm text-slate-800 focus:outline-none focus:ring-2 focus:ring-[#4ecdc4] focus:border-[#1a535c] transition-all resize-y"
              />

            </div>

            {/* =================================================
                FUNCIONAMENTO
            ================================================== */}

            <div className="md:col-span-2">

              <label
                htmlFor="textarea-tour-operating"
                className="block text-xs font-bold text-slate-700 uppercase tracking-wider mb-1.5"
              >

                Funcionamento e Regras{' '}

                <span className="text-[#ff6b6b]">
                  *
                </span>

              </label>

              <textarea
                id="textarea-tour-operating"
                rows={3}
                required
                value={
                  operatingDescription
                }
                onChange={(e) =>
                  setOperatingDescription(
                    e.target.value
                  )
                }
                placeholder="Informe os horários e regras..."
                className="w-full px-4 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-sm text-slate-800 focus:outline-none focus:ring-2 focus:ring-[#4ecdc4] focus:border-[#1a535c] transition-all resize-y"
              />

            </div>

          </div>

        </div>

        {/* ====================================================
            GPS
        ===================================================== */}

        <div
          id="section-location"
          className="bg-white rounded-2xl border border-slate-200 shadow-sm p-6 space-y-4"
        >

          <div className="flex items-center gap-2.5 pb-3 border-b border-slate-100">

            <div className="p-2 rounded-xl bg-[#4ecdc4]/15 text-[#1a535c]">

              <Navigation className="w-5 h-5" />

            </div>

            <div>

              <h3 className="text-base font-bold text-slate-800">

                2. Coordenadas de Localização (GPS)

              </h3>

              <p className="text-xs text-slate-500">

                Utilizadas para posicionar o pin
                no mapa interativo

              </p>

            </div>

          </div>

          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">

            <div>

              <label
                htmlFor="input-latitude"
                className="block text-xs font-bold text-slate-700 uppercase tracking-wider mb-1.5"
              >

                Latitude{' '}

                <span className="text-[#ff6b6b]">
                  *
                </span>

              </label>

              <input
                id="input-latitude"
                type="text"
                required
                value={latitude}
                onChange={(e) =>
                  setLatitude(
                    e.target.value
                  )
                }
                placeholder="-23.5505"
                className="w-full px-4 py-2.5 bg-slate-50 border border-slate-200 rounded-xl font-mono text-sm text-slate-800 focus:outline-none focus:ring-2 focus:ring-[#4ecdc4] focus:border-[#1a535c]"
              />

            </div>

            <div>

              <label
                htmlFor="input-longitude"
                className="block text-xs font-bold text-slate-700 uppercase tracking-wider mb-1.5"
              >

                Longitude{' '}

                <span className="text-[#ff6b6b]">
                  *
                </span>

              </label>

              <input
                id="input-longitude"
                type="text"
                required
                value={longitude}
                onChange={(e) =>
                  setLongitude(
                    e.target.value
                  )
                }
                placeholder="-46.6333"
                className="w-full px-4 py-2.5 bg-slate-50 border border-slate-200 rounded-xl font-mono text-sm text-slate-800 focus:outline-none focus:ring-2 focus:ring-[#4ecdc4] focus:border-[#1a535c]"
              />

            </div>

          </div>

          <div className="p-3.5 rounded-xl bg-slate-50 border border-slate-200 text-xs flex items-center justify-between text-slate-600">

            <div className="flex items-center gap-2">

              <MapPin className="w-4 h-4 text-[#1a535c]" />

              <span>

                Coordenadas ativas:{' '}

                <strong className="font-mono text-slate-900">

                  {latitude}, {longitude}

                </strong>

              </span>

            </div>

            <a
              href={`https://www.google.com/maps?q=${latitude},${longitude}`}
              target="_blank"
              rel="noopener noreferrer"
              className="text-[#1a535c] font-bold hover:underline flex items-center gap-1"
            >

              <span>
                Ver no Google Maps
              </span>

              <Compass className="w-3.5 h-3.5" />

            </a>

          </div>

        </div>

        {/* ====================================================
            ENDEREÇO
        ===================================================== */}

        <div
          id="section-address"
          className="bg-white rounded-2xl border border-slate-200 shadow-sm p-6 space-y-4"
        >

          <div className="flex items-center gap-2.5 pb-3 border-b border-slate-100">

            <div className="p-2 rounded-xl bg-[#1a535c]/10 text-[#1a535c]">

              <MapPin className="w-5 h-5" />

            </div>

            <div>

              <h3 className="text-base font-bold text-slate-800">

                3. Endereço do Passeio

              </h3>

              <p className="text-xs text-slate-500">

                Local físico onde o turista
                deve comparecer

              </p>

            </div>

          </div>

          <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-4 gap-4">

            {/* CEP */}

            <div>

              <label
                htmlFor="input-zipcode"
                className="block text-xs font-bold text-slate-700 uppercase tracking-wider mb-1.5"
              >
                CEP
              </label>

              <input
                id="input-zipcode"
                type="text"
                value={zipCode}
                onChange={(e) =>
                  setZipCode(
                    e.target.value
                  )
                }
                placeholder="01000-000"
                className="w-full px-4 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-sm text-slate-800 focus:outline-none focus:ring-2 focus:ring-[#4ecdc4] focus:border-[#1a535c]"
              />

            </div>

            {/* BAIRRO */}

            <div>

              <label
                htmlFor="input-neighborhood"
                className="block text-xs font-bold text-slate-700 uppercase tracking-wider mb-1.5"
              >

                Bairro{' '}

                <span className="text-[#ff6b6b]">
                  *
                </span>

              </label>

              <input
                id="input-neighborhood"
                type="text"
                required
                value={neighborhood}
                onChange={(e) =>
                  setNeighborhood(
                    e.target.value
                  )
                }
                placeholder="Centro"
                className="w-full px-4 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-sm text-slate-800 focus:outline-none focus:ring-2 focus:ring-[#4ecdc4] focus:border-[#1a535c]"
              />

            </div>

            {/* RUA */}

            <div className="sm:col-span-2 md:col-span-2">

              <label
                htmlFor="input-street"
                className="block text-xs font-bold text-slate-700 uppercase tracking-wider mb-1.5"
              >

                Rua / Avenida{' '}

                <span className="text-[#ff6b6b]">
                  *
                </span>

              </label>

              <input
                id="input-street"
                type="text"
                required
                value={street}
                onChange={(e) =>
                  setStreet(
                    e.target.value
                  )
                }
                placeholder="Av. Paulista"
                className="w-full px-4 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-sm text-slate-800 focus:outline-none focus:ring-2 focus:ring-[#4ecdc4] focus:border-[#1a535c]"
              />

            </div>

            {/* NÚMERO */}

            <div>

              <label
                htmlFor="input-number"
                className="block text-xs font-bold text-slate-700 uppercase tracking-wider mb-1.5"
              >

                Número{' '}

                <span className="text-[#ff6b6b]">
                  *
                </span>

              </label>

              <input
                id="input-number"
                type="text"
                required
                value={number}
                onChange={(e) =>
                  setNumber(
                    e.target.value
                  )
                }
                placeholder="450 ou S/N"
                className="w-full px-4 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-sm text-slate-800 focus:outline-none focus:ring-2 focus:ring-[#4ecdc4] focus:border-[#1a535c]"
              />

            </div>

            {/* COMPLEMENTO */}

            <div>

              <label
                htmlFor="input-complement"
                className="block text-xs font-bold text-slate-700 uppercase tracking-wider mb-1.5"
              >
                Complemento
              </label>

              <input
                id="input-complement"
                type="text"
                value={complement}
                onChange={(e) =>
                  setComplement(
                    e.target.value
                  )
                }
                placeholder="Bloco A"
                className="w-full px-4 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-sm text-slate-800 focus:outline-none focus:ring-2 focus:ring-[#4ecdc4] focus:border-[#1a535c]"
              />

            </div>

          </div>

        </div>

        {/* ====================================================
            DISPONIBILIDADES
        ===================================================== */}

        <div
          id="section-availability"
          className="bg-white rounded-2xl border border-slate-200 shadow-sm p-6 space-y-4"
        >

          <div className="flex items-center justify-between pb-3 border-b border-slate-100">

            <div className="flex items-center gap-2.5">

              <div className="p-2 rounded-xl bg-[#1a535c]/10 text-[#1a535c]">

                <Clock className="w-5 h-5" />

              </div>

              <div>

                <h3 className="text-base font-bold text-slate-800">

                  4. Disponibilidade de Horários

                </h3>

                <p className="text-xs text-slate-500">

                  Datas e horários disponíveis
                  para o passeio

                </p>

              </div>

            </div>

            <button
              type="button"
              onClick={
                handleAddAvailabilityBlock
              }
              className="inline-flex items-center gap-2 px-3 py-2 bg-[#1a535c] hover:bg-[#154249] text-white text-xs font-bold rounded-xl transition-colors"
            >

              <Plus className="w-4 h-4" />

              Adicionar data

            </button>

          </div>

          {availabilities.length === 0 && (

            <div className="py-8 text-center text-sm text-slate-400">

              Nenhuma data de disponibilidade
              cadastrada.

            </div>

          )}

          <div className="space-y-4">

            {availabilities.map(
              (block) => (

                <div
                  key={block.id}
                  className="border border-slate-200 rounded-xl p-4 bg-slate-50"
                >

                  <div className="flex items-center justify-between gap-4">

                    <div className="flex-1">

                      <label className="block text-xs font-bold text-slate-600 uppercase tracking-wider mb-1.5">

                        Data

                      </label>

                      <input
                        type="date"
                        value={block.date}
                        onChange={(e) =>
                          handleUpdateBlockDate(
                            block.id,
                            e.target.value
                          )
                        }
                        className="w-full px-3 py-2 bg-white border border-slate-200 rounded-lg text-sm"
                      />

                    </div>

                    <button
                      type="button"
                      onClick={() =>
                        handleRemoveAvailabilityBlock(
                          block.id
                        )
                      }
                      className="p-2 text-red-500 hover:bg-red-50 rounded-lg"
                      title="Remover data"
                    >

                      <Trash2 className="w-4 h-4" />

                    </button>

                  </div>

                  <div className="mt-4">

                    <label className="block text-xs font-bold text-slate-600 uppercase tracking-wider mb-2">

                      Horários

                    </label>

                    <div className="flex flex-wrap gap-2">

                      {block.timeSlots.map(
                        (slot) => (

                          <div
                            key={slot}
                            className="inline-flex items-center gap-1.5 bg-white border border-slate-200 rounded-lg px-3 py-1.5 text-sm font-medium text-slate-700"
                          >

                            {slot}

                            <button
                              type="button"
                              onClick={() =>
                                handleRemoveTimeSlot(
                                  block.id,
                                  slot
                                )
                              }
                              className="text-red-400 hover:text-red-600"
                            >

                              <X className="w-3.5 h-3.5" />

                            </button>

                          </div>

                        )
                      )}

                    </div>

                    <div className="flex flex-wrap gap-2 mt-3">

                      <input
                        type="time"
                        value={
                          newTimeInputs[
                            block.id
                          ] || ''
                        }
                        onChange={(e) =>
                          setNewTimeInputs({
                            ...newTimeInputs,
                            [block.id]:
                              e.target.value,
                          })
                        }
                        className="px-3 py-2 bg-white border border-slate-200 rounded-lg text-sm"
                      />

                      <button
                        type="button"
                        onClick={() =>
                          handleAddTimeSlot(
                            block.id
                          )
                        }
                        className="inline-flex items-center gap-1.5 px-3 py-2 bg-white border border-slate-200 rounded-lg text-xs font-bold text-slate-700 hover:bg-slate-100"
                      >

                        <Plus className="w-3.5 h-3.5" />

                        Adicionar

                      </button>

                      <button
                        type="button"
                        onClick={() =>
                          handleApplyPresetTimes(
                            block.id,
                            [
                              '09:00',
                              '12:00',
                              '14:00',
                              '16:00',
                            ]
                          )
                        }
                        className="px-3 py-2 bg-white border border-slate-200 rounded-lg text-xs font-bold text-slate-700 hover:bg-slate-100"
                      >

                        Horários comuns

                      </button>

                    </div>

                  </div>

                </div>

              )
            )}

          </div>

        </div>

        {/* ====================================================
            AUDITORIA
        ===================================================== */}

        {isEditing &&
          tourToEdit?.audit && (

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
                      tourToEdit
                        .audit
                        .lastEditedBy
                    }

                  </strong>

                </p>

                <p className="text-slate-500 font-medium">

                  Em:{' '}

                  <span className="font-semibold text-slate-800">

                    {
                      tourToEdit
                        .audit
                        .lastEditedAt
                    }

                  </span>

                </p>

              </div>

            </div>

          )}

        {/* ====================================================
            BOTÕES
        ===================================================== */}

        <div className="flex items-center justify-end gap-3 pt-2">

          <button
            type="button"
            onClick={onCancel}
            disabled={saving}
            className="px-5 py-2.5 rounded-xl border border-slate-300 text-slate-700 font-semibold text-sm hover:bg-slate-100 transition-colors cursor-pointer disabled:opacity-50"
          >

            Cancelar

          </button>

          <button
            type="submit"
            disabled={
              saving ||
              uploadingImage
            }
            className="inline-flex items-center gap-2 bg-[#1a535c] hover:bg-[#154249] text-white font-bold text-sm px-6 py-2.5 rounded-xl shadow-md transition-all active:scale-98 cursor-pointer disabled:opacity-60 disabled:cursor-not-allowed"
          >

            {uploadingImage ? (

              <>
                <Upload className="w-4 h-4 animate-pulse text-[#4ecdc4]" />

                <span>
                  Enviando imagem...
                </span>
              </>

            ) : saving ? (

              <>
                <Save className="w-4 h-4 animate-pulse text-[#4ecdc4]" />

                <span>
                  Salvando...
                </span>
              </>

            ) : (

              <>
                <Save className="w-4 h-4 text-[#4ecdc4]" />

                <span>
                  Salvar Passeio
                </span>
              </>

            )}

          </button>

        </div>

      </form>

    </div>
  );
}