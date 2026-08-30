import { AlertTriangle, Trash2, X } from 'lucide-react';

interface DeleteConfirmModalProps {
  isOpen: boolean;
  itemType: 'category' | 'tour';
  itemName: string;
  onConfirm: () => void;
  onCancel: () => void;
}

export function DeleteConfirmModal({
  isOpen,
  itemType,
  itemName,
  onConfirm,
  onCancel,
}: DeleteConfirmModalProps) {
  if (!isOpen) return null;

  const isCategory = itemType === 'category';

  return (
    <div
      id="delete-confirm-modal-overlay"
      className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-slate-900/60 backdrop-blur-sm animate-in fade-in duration-200"
    >
      <div
        id="delete-confirm-modal-box"
        className="w-full max-w-md bg-white rounded-2xl p-6 shadow-2xl border border-slate-200 space-y-4 animate-in zoom-in-95 duration-200"
      >
        <div className="flex items-start justify-between">
          <div className="w-12 h-12 rounded-2xl bg-[#ff6b6b]/15 text-[#ff6b6b] flex items-center justify-center">
            <AlertTriangle className="w-6 h-6" />
          </div>
          <button
            onClick={onCancel}
            className="text-slate-400 hover:text-slate-600 p-1.5 rounded-lg hover:bg-slate-100"
            aria-label="Fechar"
          >
            <X className="w-5 h-5" />
          </button>
        </div>

        <div>
          <h3 className="text-lg font-bold text-slate-800">
            Confirmar exclusão de {isCategory ? 'categoria' : 'passeio'}?
          </h3>
          <p className="text-sm text-slate-600 mt-1">
            Você está prestes a remover <strong className="text-slate-900">"{itemName}"</strong> do catálogo do Rota Livre. Esta ação não poderá ser desfeita.
          </p>
          {isCategory && (
            <p className="text-xs text-[#ff6b6b] font-medium mt-2 bg-[#ff6b6b]/10 p-2.5 rounded-xl border border-[#ff6b6b]/20">
              Aviso: Caso haja passeios vinculados a esta categoria, eles precisarão ser reclassificados.
            </p>
          )}
        </div>

        <div className="flex items-center justify-end gap-3 pt-2">
          <button
            type="button"
            onClick={onCancel}
            className="px-4 py-2.5 rounded-xl border border-slate-300 text-slate-700 font-semibold text-xs lg:text-sm hover:bg-slate-100 transition-colors"
          >
            Cancelar
          </button>
          <button
            type="button"
            id="btn-confirm-delete-action"
            onClick={onConfirm}
            className="inline-flex items-center gap-2 bg-[#ff6b6b] hover:bg-[#e05656] text-white font-bold text-xs lg:text-sm px-5 py-2.5 rounded-xl shadow-md transition-all active:scale-98"
          >
            <Trash2 className="w-4 h-4" />
            <span>Sim, Excluir</span>
          </button>
        </div>
      </div>
    </div>
  );
}
