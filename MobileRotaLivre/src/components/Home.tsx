// src/components/Home.tsx
import React, { useEffect, useState } from 'react';
import { passeioService } from '../services/passeioService';
import { PasseioDto } from '../types';

export const Home = () => {
  const [passeios, setPasseios] = useState<PasseioDto[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const carregarPasseios = async () => {
      try {
        const dados = await passeioService.listarTodos();
        setPasseios(dados);
      } catch (error) {
        console.error("Erro ao buscar os passeios da API", error);
      } finally {
        setLoading(false);
      }
    };

    carregarPasseios();
  }, []);

  if (loading) return <p>Carregando rotas...</p>;

  return (
    <div>
      {/* Aqui você renderiza a sua interface em React (cards de passeios) */}
      {passeios.map(passeio => (
        <div key={passeio.id}>
          <h3>{passeio.nome}</h3>
          <img src={passeio.imagemUrl} alt={passeio.nome} />
          <p>{passeio.quantidadeCurtidas} curtidas</p>
        </div>
      ))}
    </div>
  );
};