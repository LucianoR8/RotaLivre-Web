import { useEffect } from 'react';
import { buscarMeusPendentes } from '../services/grupoService';
import { notificationService } from '../services/notificationService';
import { useAuth } from '../context/AuthContext';

export const useMonitorPasseios = () => {
  const { usuario, getAuthHeader } = useAuth();

  useEffect(() => {
    // Inicializa o motor do Android assim que o app abre
    notificationService.inicializar();

    if (!usuario) return;

    // Tenta pedir permissão ao carregar
    notificationService.pedirPermissao();

    const verificarHorarios = async () => {
      try {
        // Traz APENAS os grupos em que o utilizador logado está inscrito
        const meusGrupos = await buscarMeusPendentes(getAuthHeader);
        const agora = Date.now();

        meusGrupos.forEach((g) => {
          if (g.status === 'FINALIZADO' || g.status === 'CANCELADO') return;

          const parseUtc = (d?: string) =>
            d ? new Date(d.endsWith('Z') ? d : `${d}Z`).getTime() : null;

          const inicioMs = parseUtc(g.dataInicio);
          const fimMs = parseUtc(g.dataFim);

          // =========================================================
          // ALERTAS DE INÍCIO (Isolados por idGrupo + Horário Exato)
          // =========================================================
          if (inicioMs && inicioMs > agora) {
            const diffMinInicio = (inicioMs - agora) / 60000;

            // Aviso de 30 minutos (dispara se faltar entre 5 e 30 min)
            if (diffMinInicio <= 30 && diffMinInicio > 5) {
              const chave30 = `notif_ini_30_grupo_${g.idGrupo}_time_${inicioMs}`;
              if (!localStorage.getItem(chave30)) {
                localStorage.setItem(chave30, 'true');
                notificationService.disparar(
                  `⏰ [${g.nomeGrupo}] Começa em breve!`,
                  `O passeio "${g.passeio?.nome || g.nomeGrupo}" inicia em menos de 30 minutos.`,
                  chave30
                );
              }
            }

            // Aviso de 5 minutos (dispara se faltar 5 min ou menos)
            if (diffMinInicio <= 5 && diffMinInicio > 0) {
              const chave5 = `notif_ini_5_grupo_${g.idGrupo}_time_${inicioMs}`;
              if (!localStorage.getItem(chave5)) {
                localStorage.setItem(chave5, 'true');
                notificationService.disparar(
                  `🚀 [${g.nomeGrupo}] Faltam 5 minutos!`,
                  `Prepare-se! O passeio "${g.passeio?.nome || g.nomeGrupo}" já vai começar.`,
                  chave5
                );
              }
            }
          }

          // =========================================================
          // ALERTAS DE FIM (Isolados por idGrupo + Horário Exato)
          // =========================================================
          if (fimMs && fimMs > agora) {
            const diffMinFim = (fimMs - agora) / 60000;

            // Aviso de 30 minutos para acabar
            if (diffMinFim <= 30 && diffMinFim > 5) {
              const chaveFim30 = `notif_fim_30_grupo_${g.idGrupo}_time_${fimMs}`;
              if (!localStorage.getItem(chaveFim30)) {
                localStorage.setItem(chaveFim30, 'true');
                notificationService.disparar(
                  `⏳ [${g.nomeGrupo}] Reta final`,
                  `Faltam cerca de 30 minutos para o encerramento automático deste passeio.`,
                  chaveFim30
                );
              }
            }

            // Aviso de 5 minutos para acabar
            if (diffMinFim <= 5 && diffMinFim > 0) {
              const chaveFim5 = `notif_fim_5_grupo_${g.idGrupo}_time_${fimMs}`;
              if (!localStorage.getItem(chaveFim5)) {
                localStorage.setItem(chaveFim5, 'true');
                notificationService.disparar(
                  `🏁 [${g.nomeGrupo}] Encerrando em 5 min!`,
                  `O tempo limite do passeio está prestes a terminar.`,
                  chaveFim5
                );
              }
            }
          }
        });
      } catch (err) {
        console.warn('Erro ao verificar horários dos grupos:', err);
      }
    };

    verificarHorarios();
    const interval = setInterval(verificarHorarios, 30000);

    return () => clearInterval(interval);
  }, [usuario]);
};