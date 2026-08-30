// src/services/categoriaService.ts

import { api } from './api';
import { Category } from '../types';

export const categoriaService = {

  // =========================================================
  // LISTAR CATEGORIAS
  // =========================================================

  listar: async (): Promise<Category[]> => {
    try {
      console.log('======================================');
      console.log('📡 BUSCANDO CATEGORIAS NA API');
      console.log('GET /CategoriaApi');
      console.log('======================================');

      const response =
        await api.get('/CategoriaApi');

      console.log(
        '📥 RESPOSTA BRUTA - CATEGORIAS:'
      );

      console.log(response.data);

      if (!Array.isArray(response.data)) {
        console.error(
          '❌ A resposta de categorias não é um array:',
          response.data
        );

        return [];
      }

      const categorias: Category[] =
        response.data.map(
          (c: any): Category => ({
            id: Number(
              c.id_categoria ??
              c.id ??
              0
            ),

            name:
              c.tipo_categoria ??
              c.nome ??
              '',

            imageUrl:
              c.img ??
              c.imagemUrl ??
              c.imagem_url ??
              '',

            description:
              c.descricao ??
              '',

            isActive:
              c.ativo ??
              true,

            tourCount:
              c.tourCount ??
              0,

            audit:
              c.atualizado_em
                ? {
                    lastEditedBy:
                      c.adminAtualizacao
                        ?.nome_completo ??
                      'Admin',

                    lastEditedAt:
                      new Date(
                        c.atualizado_em
                      ).toLocaleString(
                        'pt-BR'
                      ),
                  }
                : undefined,
          })
        );

      console.log(
        '🔄 CATEGORIAS APÓS CONVERSÃO:'
      );

      console.log(categorias);

      console.log(
        `✅ TOTAL DE CATEGORIAS RECEBIDAS: ${categorias.length}`
      );

      return categorias;

    } catch (error: any) {

      console.error(
        '❌ ERRO AO BUSCAR CATEGORIAS'
      );

      console.error(
        'Erro completo:',
        error
      );

      if (error.response) {

        console.error(
          'Status:',
          error.response.status
        );

        console.error(
          'Resposta da API:',
          error.response.data
        );
      }

      throw error;
    }
  },

  // =========================================================
  // UPLOAD DA IMAGEM
  // =========================================================

  uploadImagem: async (
    imagem: File
  ): Promise<string> => {

    console.log('======================================');
    console.log('📤 ENVIANDO IMAGEM DA CATEGORIA');
    console.log('Arquivo:', imagem.name);
    console.log('Tipo:', imagem.type);
    console.log('Tamanho:', imagem.size);
    console.log('======================================');

    try {

      const formData =
        new FormData();

      formData.append(
        'imagem',
        imagem
      );

      const response =
        await api.post(
          '/CategoriaApi/upload-imagem',
          formData
        );

      console.log(
        '✅ IMAGEM DA CATEGORIA ENVIADA:'
      );

      console.log(response.data);

      return response.data.imagemUrl;

    } catch (error: any) {

      console.error(
        '❌ ERRO AO ENVIAR IMAGEM DA CATEGORIA:',
        error
      );

      if (error.response) {

        console.error(
          'Status:',
          error.response.status
        );

        console.error(
          'Resposta:',
          error.response.data
        );
      }

      throw error;
    }
  },

  // =========================================================
  // CRIAR CATEGORIA
  // =========================================================

  criar: async (
    categoria: Partial<Category>,
    photoFile?: File | null
  ) => {

    let imagemUrl =
      categoria.imageUrl?.trim() ??
      '';

    // -------------------------------------------------------
    // UPLOAD
    // -------------------------------------------------------

    if (photoFile) {

      console.log(
        '📤 Fazendo upload da imagem da nova categoria...'
      );

      imagemUrl =
        await categoriaService.uploadImagem(
          photoFile
        );
    }

    // -------------------------------------------------------
    // PAYLOAD
    // -------------------------------------------------------

    const payload = {

      tipo_categoria:
        categoria.name?.trim() ??
        '',

      img:
        imagemUrl,

      ativo:
        categoria.isActive ??
        true,
    };

    console.log('======================================');
    console.log('📤 CRIANDO CATEGORIA');
    console.log('Payload enviado:');
    console.log(payload);
    console.log('======================================');

    try {

      const response =
        await api.post(
          '/CategoriaApi',
          payload
        );

      console.log(
        '✅ CATEGORIA CRIADA:'
      );

      console.log(
        response.data
      );

      return response.data;

    } catch (error: any) {

      console.error(
        '❌ ERRO AO CRIAR CATEGORIA:',
        error
      );

      if (error.response) {

        console.error(
          'Status:',
          error.response.status
        );

        console.error(
          'Resposta:',
          error.response.data
        );
      }

      throw error;
    }
  },

  // =========================================================
  // ATUALIZAR CATEGORIA
  // =========================================================

  atualizar: async (
    id: number,
    categoria: Partial<Category>,
    photoFile?: File | null
  ) => {

    let imagemUrl =
      categoria.imageUrl?.trim() ??
      '';

    // -------------------------------------------------------
    // UPLOAD DE NOVA IMAGEM
    // -------------------------------------------------------

    if (photoFile) {

      console.log(
        '📤 Fazendo upload da nova imagem da categoria...'
      );

      imagemUrl =
        await categoriaService.uploadImagem(
          photoFile
        );
    }

    // -------------------------------------------------------
    // PAYLOAD
    // -------------------------------------------------------

    const payload = {

      id_categoria:
        id,

      tipo_categoria:
        categoria.name?.trim() ??
        '',

      img:
        imagemUrl,

      ativo:
        categoria.isActive ??
        true,
    };

    console.log('======================================');
    console.log('📤 ATUALIZANDO CATEGORIA');
    console.log('ID:', id);
    console.log('Payload enviado:');
    console.log(payload);
    console.log('======================================');

    try {

      const response =
        await api.put(
          `/CategoriaApi/${id}`,
          payload
        );

      console.log(
        '✅ CATEGORIA ATUALIZADA:'
      );

      console.log(
        response.data
      );

      return response.data;

    } catch (error: any) {

      console.error(
        '❌ ERRO AO ATUALIZAR CATEGORIA:',
        error
      );

      if (error.response) {

        console.error(
          'Status:',
          error.response.status
        );

        console.error(
          'Resposta:',
          error.response.data
        );
      }

      throw error;
    }
  },

  // =========================================================
  // DELETAR CATEGORIA
  // =========================================================

  deletar: async (
    id: number
  ) => {

    console.log(
      '🗑️ EXCLUINDO CATEGORIA:',
      id
    );

    try {

      const response =
        await api.delete(
          `/CategoriaApi/${id}`
        );

      console.log(
        '✅ CATEGORIA EXCLUÍDA:',
        response.data
      );

      return response.data;

    } catch (error: any) {

      console.error(
        '❌ ERRO AO EXCLUIR CATEGORIA:',
        error
      );

      if (error.response) {

        console.error(
          'Status:',
          error.response.status
        );

        console.error(
          'Resposta:',
          error.response.data
        );
      }

      throw error;
    }
  },
};