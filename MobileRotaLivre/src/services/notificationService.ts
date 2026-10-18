export const notificationService = {
  // Regista o Service Worker básico para permitir notificações no Android
  inicializar: async () => {
    if ('serviceWorker' in navigator) {
      try {
        await navigator.serviceWorker.register('/sw.js');
      } catch (err) {
        console.warn('Falha ao registar Service Worker de notificação:', err);
      }
    }
  },

  // Pede permissão ao dispositivo (ideal chamar também em cliques de botão)
  pedirPermissao: async (): Promise<boolean> => {
    if (!('Notification' in window)) {
      return false;
    }

    if (Notification.permission === 'granted') {
      return true;
    }

    if (Notification.permission !== 'denied') {
      const permission = await Notification.requestPermission();
      return permission === 'granted';
    }

    return false;
  },

  // Dispara a notificação isolada por tag
  disparar: async (titulo: string, corpo: string, tagUnica: string) => {
    if (!('Notification' in window) || Notification.permission !== 'granted') {
      return;
    }

    try {
      if ('vibrate' in navigator) {
        navigator.vibrate([200, 100, 200]);
      }

      const opcoes: NotificationOptions = {
        body: corpo,
        icon: '/favicon.ico',
        tag: tagUnica, // Garante que um grupo nunca sobrescreva o alerta de outro grupo
      };

      // Tenta primeiro via Service Worker (Obrigatório no Android Chrome)
      if ('serviceWorker' in navigator) {
        const registration = await navigator.serviceWorker.getRegistration();
        if (registration && 'showNotification' in registration) {
          await registration.showNotification(titulo, opcoes);
          return;
        }
      }

      // Fallback para Navegadores Desktop
      new Notification(titulo, opcoes);
    } catch (err) {
      console.warn('Erro ao disparar notificação:', err);
    }
  }
};