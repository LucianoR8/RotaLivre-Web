// src/services/passeioService.ts

import { api } from './api';
import { Tour } from '../types';

export const passeioService = {
  // =========================================================
  // LISTAR PASSEIOS
  // =========================================================

  listar: async (): Promise<Tour[]> => {
    try {
      console.log('======================================');
      console.log('📡 BUSCANDO PASSEIOS NA API');
      console.log('GET /PasseiosApi');
      console.log('======================================');

      const response = await api.get('/PasseiosApi');

      console.log('📥 RESPOSTA BRUTA - PASSEIOS:');
      console.log(response.data);

      if (!Array.isArray(response.data)) {
        console.error(
          '❌ A resposta de passeios não é um array:',
          response.data
        );

        return [];
      }

      const passeios: Tour[] = response.data.map(
        (p: any): Tour => {
          const endereco = p.endereco;

          const latitude = endereco?.latitude;
          const longitude = endereco?.longitude;

          const tour: Tour = {
            // =========================
            // DADOS PRINCIPAIS
            // =========================

            id: Number(p.id_passeio ?? p.id ?? 0),

            name: p.nome_passeio ?? p.nome ?? '',

            description: p.descricao ?? '',

            operatingDescription: p.funcionamento ?? '',

            photoUrl: p.img_url ?? p.imagemUrl ?? p.imagem_url ?? '',

            status: p.status ?? 'ativo',

            // =========================
            // CATEGORIA E CIDADE
            // =========================

            categoryId: Number(p.id_categoria ?? p.categoriaId ?? 0),

            categoryName: p.categoriaNome ?? p.nomeCategoria ?? '',

            cityId: Number(p.id_cidade ?? p.cidadeId ?? 0),

            // =========================
            // CURTIDAS
            // =========================

            reviewsCount: p.quantidadeCurtidas ?? 0,

            // =========================
            // ENDEREÇO
            // =========================

            address: endereco
              ? {
                  street: endereco.nomeRua ?? '',
                  number: endereco.numeroRua ?? '',
                  complement: endereco.complemento ?? '',
                  neighborhood: endereco.bairro ?? '',
                  zipCode: endereco.cep ?? '',
                }
              : undefined,

            // =========================
            // GPS
            // =========================

            location:
              latitude != null && longitude != null
                ? {
                    latitude: Number(latitude),
                    longitude: Number(longitude),
                  }
                : undefined,

            // =========================
            // DISPONIBILIDADES
            // =========================

            availabilities: Array.isArray(p.availabilities)
              ? p.availabilities
              : undefined,

            // =========================
            // AUDITORIA
            // =========================

            audit: p.atualizado_em
              ? {
                  lastEditedBy:
                    p.adminAtualizacao?.nome_completo ?? 'Admin',

                  lastEditedAt: new Date(p.atualizado_em).toLocaleString(
                    'pt-BR'
                  ),
                }
              : undefined,
          };

          return tour;
        }
      );

      console.log('🔄 PASSEIOS APÓS CONVERSÃO:');
      console.log(passeios);
      console.log(`✅ TOTAL DE PASSEIOS RECEBIDOS: ${passeios.length}`);

      return passeios;
    } catch (error: any) {
      console.error('❌ ERRO AO BUSCAR PASSEIOS:', error);

      if (error.response) {
        console.error('Status:', error.response.status);
        console.error('Resposta da API:', error.response.data);
      }

      throw error;
    }
  },

  // =========================================================
  // UPLOAD DA IMAGEM
  // =========================================================

  uploadImagem: async (imagem: File): Promise<string> => {
    console.log('======================================');
    console.log('📤 ENVIANDO IMAGEM DO PASSEIO');
    console.log('Arquivo:', imagem.name);
    console.log('Tipo:', imagem.type);
    console.log('Tamanho:', imagem.size);
    console.log('======================================');

    try {
      const formData = new FormData();

      formData.append('imagem', imagem);

      const response = await api.post(
        '/PasseiosApi/upload-imagem',
        formData
      );

      console.log('✅ IMAGEM ENVIADA COM SUCESSO:');
      console.log(response.data);

      return response.data.imagemUrl;
    } catch (error: any) {
      console.error('❌ ERRO AO ENVIAR IMAGEM:', error);

      if (error.response) {
        console.error('Status:', error.response.status);
        console.error('Resposta da API:', error.response.data);
      }

      throw error;
    }
  },

  // =========================================================
  // CRIAR PASSEIO
  // =========================================================

  criar: async (tour: Partial<Tour>, photoFile?: File | null) => {
    let imagemUrl = tour.photoUrl?.trim() ?? '';

    /*
     * Se uma nova imagem foi selecionada,
     * fazemos o upload antes de criar o passeio.
     */
    if (photoFile) {
      imagemUrl = await passeioService.uploadImagem(photoFile);
    }

    const payload = {
      nome: tour.name?.trim() ?? '',
      categoriaId: Number(tour.categoryId),
      
      cidadeId: Number(tour.cityId), // NOVO: Vínculo da cidade enviado pro back
      
      descricao: tour.description?.trim() ?? '',
      funcionamento: tour.operatingDescription?.trim() ?? '',
      imagemUrl,
      // NOVO: Adicionando o mapeamento do endereço e coordenadas
      endereco: {
        nomeRua: tour.address?.street ?? '',
        numeroRua: tour.address?.number ?? '',
        complemento: tour.address?.complement ?? '',
        bairro: tour.address?.neighborhood ?? '',
        cep: tour.address?.zipCode ?? '',
        latitude: tour.location?.latitude ?? 0,
        longitude: tour.location?.longitude ?? 0,
        raioMetros: 500,
      },
    };

    console.log('======================================');
    console.log('📤 CRIANDO PASSEIO');
    console.log('Payload enviado:');
    console.log(payload);
    console.log('======================================');

    try {
      const response = await api.post('/PasseiosApi', payload);

      console.log('✅ PASSEIO CRIADO - RESPOSTA DA API:');
      console.log(response.data);

      return response.data;
    } catch (error: any) {
      console.error('❌ ERRO AO CRIAR PASSEIO:', error);

      if (error.response) {
        console.error(
          'Resposta completa da API:',
          JSON.stringify(error.response.data, null, 2)
        );
        console.error('Status:', error.response.status);
      }

      throw error;
    }
  },

  // =========================================================
  // ATUALIZAR PASSEIO
  // =========================================================

  atualizar: async (
    id: number,
    tour: Partial<Tour>,
    photoFile?: File | null
  ) => {
    let imagemUrl = tour.photoUrl?.trim() ?? '';

    /*
     * Se uma nova imagem foi selecionada,
     * fazemos o upload.
     *
     * Por enquanto a imagem antiga não é
     * removida do Storage.
     */
    if (photoFile) {
      imagemUrl = await passeioService.uploadImagem(photoFile);
    }

    const payload = {
      nome: tour.name?.trim() ?? '',
      categoriaId: Number(tour.categoryId),
      
      cidadeId: Number(tour.cityId), // NOVO: Vínculo da cidade enviado pro back
      
      descricao: tour.description?.trim() ?? '',
      funcionamento: tour.operatingDescription?.trim() ?? '',
      imagemUrl,
      status: tour.status ?? 'ativo',
      // NOVO: Adicionando o mapeamento do endereço e coordenadas
      endereco: {
        nomeRua: tour.address?.street ?? '',
        numeroRua: tour.address?.number ?? '',
        complemento: tour.address?.complement ?? '',
        bairro: tour.address?.neighborhood ?? '',
        cep: tour.address?.zipCode ?? '',
        latitude: tour.location?.latitude ?? 0,
        longitude: tour.location?.longitude ?? 0,
        raioMetros: 500,
      },
    };
    console.log('======================================');
    console.log('📤 ATUALIZANDO PASSEIO');
    console.log('ID:', id);
    console.log('Payload enviado:');
    console.log(payload);
    console.log('======================================');

    try {
      const response = await api.put(`/PasseiosApi/${id}`, payload);

      console.log('✅ PASSEIO ATUALIZADO - RESPOSTA DA API:');
      console.log(response.data);

      return response.data;
    } catch (error: any) {
      console.error('❌ ERRO AO ATUALIZAR PASSEIO:', error);

      if (error.response) {
        console.error('Status:', error.response.status);
        console.error('Resposta:', error.response.data);
      }

      throw error;
    }
  },

  // =========================================================
  // DELETAR PASSEIO
  // =========================================================

  deletar: async (id: number) => {
    console.log('🗑️ EXCLUINDO PASSEIO:', id);

    try {
      const response = await api.delete(`/PasseiosApi/${id}`);

      console.log('✅ PASSEIO EXCLUÍDO:', response.data);

      return response.data;
    } catch (error: any) {
      console.error('❌ ERRO AO EXCLUIR PASSEIO:', error);

      if (error.response) {
        console.error('Status:', error.response.status);
        console.error('Resposta:', error.response.data);
      }

      throw error;
    }
  },
};