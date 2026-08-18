export interface Endereco {
  nomeRua: string;
  numeroRua: string;
  complemento: string;
  bairro: string;
  cep: string;
  latitude?: number;
  longitude?: number;
  raioMetros?: number;
}

export interface Passeio {
  id_passeio: number;
  id_categoria: number;
  nome_passeio: string;
  funcionamento: string;
  descricao: string;
  img_url: string;
  categoriaNome: string;
  quantidadeCurtidas: number;
  usuarioJaCurtiu?: boolean;
  endereco?: Endereco;
}

export interface Categoria {
  id_categoria: number;
  tipo_categoria: string;
  img: string;
}

export interface Usuario {
  id: number;
  nome: string;
  email: string;
  dataNasc?: string;
  fotoPerfilUrl?: string | null;
}


export interface MembroGrupo {
  id_usuario: number;
  nome: string;
  lat?: number;
  lng?: number;
  ultima_atualizacao?: string;
}

export interface Grupo {
  id_grupo: number;
  codigo_grupo: string;
  nome_grupo: string;
  id_passeio?: number;
  data_passeio?: string;
  horario_passeio?: string;
  criador_id: number;
  membros: MembroGrupo[];
}

// src/types/index.ts

export interface LoginRequest {
  email: string;
  senha: string;
}


export interface LoginResponse {
  token: string;
  usuario: Usuario;
}

export interface PasseioDto {
  id: number;
  nome: string;
  descricao?: string;
  preco?: number;
  funcionamento: string;
  categoriaNome: string;
  imagemUrl?: string;
  usuarioJaCurtiu: boolean;
  usuarioJaPendente?: boolean;
  quantidadeCurtidas: number;
  endereco?: Endereco;
}

// src/types/index.ts

export interface UsuarioCadastroDto {
  nome: string;
  dataNasc: string; // O backend faz DateOnly.Parse, o ideal é enviar 'YYYY-MM-DD'
  email: string;
  senha: string;
  idPergunta: number;
  respostaSeg: string;
}

export interface PerguntaSeguranca {
  id_pergunta: number;
  pergunta_seg: string;
}

export interface CategoriaHomeDto {
  idCategoria: number;
  tipoCategoria: string;
  imgUrl: string;
}

export interface PasseioHomeDto {
  id: number;
  nome: string;
  descricao: string;
  funcionamento: string;
  imagemUrl: string;
  usuarioJaCurtiu: boolean;
  quantidadeCurtidas: number;
}

export interface HomeDto {
  nomeUsuario: string;
  categorias: CategoriaHomeDto[];
  destaques: PasseioHomeDto[];
  favoritados: PasseioHomeDto[];
}

export interface UsuarioPerfilDto {
  id: number;
  nome: string;
  email: string;
  dataNasc: string;
  fotoPerfilUrl?: string | null;
}

export interface CriarAvaliacaoDto {
  idPasseio: number;
  idUsuario: number;
  feedback: string;
}

export interface AvaliacaoDto {
  nomeUsuario: string;
  feedback: string;
  data: string;
}