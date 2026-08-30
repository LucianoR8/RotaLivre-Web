import { CheckCircle2, AlertTriangle, X } from 'lucide-react';

export interface ToastMessage {
  id: string;
  type: 'success' | 'error' | 'info';
  title: string;
  description?: string;
}

interface ToastProps {
  toasts: ToastMessage[];
  onDismiss: (id: string) => void;
}

export function ToastContainer({ toasts, onDismiss }: ToastProps) {
  if (toasts.length === 0) return null;

  return (
    <div className="fixed bottom-5 right-5 z-50 flex flex-col gap-2 max-w-md w-full px-4 pointer-events-none">
      {toasts.map((toast) => {
        const isSuccess = toast.type === 'success';
        const isError = toast.type === 'error';

        return (
          <div
            key={toast.id}
            id={`toast-${toast.id}`}
            className="pointer-events-auto flex items-start gap-3 p-4 rounded-xl shadow-lg border bg-white animate-in fade-in slide-in-from-bottom-5 duration-200"
            style={{
              borderColor: isSuccess ? '#4ecdc4' : isError ? '#ff6b6b' : '#cbd5e1',
            }}
          >
            <div className="shrink-0 mt-0.5">
              {isSuccess && <CheckCircle2 className="w-5 h-5 text-[#1a535c]" />}
              {isError && <AlertTriangle className="w-5 h-5 text-[#ff6b6b]" />}
              {!isSuccess && !isError && <div className="w-2 h-2 rounded-full bg-[#4ecdc4] mt-1.5" />}
            </div>
            <div className="flex-1 text-sm">
              <p className="font-semibold text-slate-800">{toast.title}</p>
              {toast.description && (
                <p className="text-slate-600 text-xs mt-0.5">{toast.description}</p>
              )}
            </div>
            <button
              onClick={() => onDismiss(toast.id)}
              className="text-slate-400 hover:text-slate-600 transition-colors p-1 rounded-lg hover:bg-slate-100"
              aria-label="Fechar notificação"
            >
              <X className="w-4 h-4" />
            </button>
          </div>
        );
      })}
    </div>
  );
}
