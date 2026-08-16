export interface Categoria {
  id: string | number;
  nome: string;
  iconeUrl?: string;
}

export interface Passeio {
  id: string | number;
  titulo: string;
  descricao: string;
  preco: number;
  imagemUrl: string;
  isFeatured?: boolean;
}

export interface HomeData {
  categorias: Categoria[];
  passeiosDestaque: Passeio[];
  favoritos: Passeio[];
}

export interface HomeApiResponse {
  success: boolean;
  data: HomeData;
  message?: string;
}