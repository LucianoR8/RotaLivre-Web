export interface PasseioPendenteDto {
  idGrupo: number;
  idPasseio: number;
  nomeGrupo: string;
  codigoConvite: string;
  status: string;
  dataInicio?: string;
  criadorId: number;
  passeio: {
    id: number;
    nome: string;
    descricao?: string;
    imagemUrl?: string;
  };
}

export interface GrupoDetalhesDto {
  idGrupo: number;
  nome: string;
  codigoConvite: string;
  status: string;
  idPasseio: number;

  passeio: {
    id: number;
    nome: string;
    descricao?: string;
    imagemUrl?: string;
  };

  dataCriacao: string;
  dataInicio?: string;

  criadorId: number;

  integrantes: {
    idUsuario: number;
    nome: string;
    iniciouPasseio: boolean;
    dataInicioPasseio?: string | null;
    ultimaAtividade?: string | null;
    online: boolean;
  }[];
}

export interface IniciarPasseioResponse {
  mensagem: string;
  iniciouPasseio: boolean;
  grupoIniciado: boolean;
  status: string;
  dataInicioPasseio: string;
  dataInicioGrupo: string;
}

export async function buscarMeusPendentes(
  getAuthHeader: () => Record<string, string>
): Promise<PasseioPendenteDto[]> {
  const response = await fetch('/api/grupo/meus-pendentes', {
    headers: getAuthHeader()
  });

  if (!response.ok) {
    throw new Error('Erro ao buscar grupos pendentes.');
  }

  return response.json();
}

export async function buscarGrupo(
  idGrupo: number,
  getAuthHeader: () => Record<string, string>
): Promise<GrupoDetalhesDto> {
  const response = await fetch(`/api/grupo/${idGrupo}`, {
    headers: getAuthHeader()
  });

  if (!response.ok) {
    throw new Error('Erro ao buscar grupo.');
  }

  return response.json();
}

export async function iniciarPasseio(
  idGrupo: number,
  getAuthHeader: () => Record<string, string>
): Promise<IniciarPasseioResponse> {
  const response = await fetch(`/api/grupo/${idGrupo}/iniciar`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      ...getAuthHeader()
    }
  });

  const data = await response.json();

  if (!response.ok) {
    throw new Error(data.mensagem || 'Erro ao iniciar passeio.');
  }

  return data;
}


export async function sairDoGrupo(
  idGrupo: number,
  getAuthHeader: () => Record<string, string>
) {
  const response = await fetch(
    `/api/grupo/${idGrupo}/sair`,
    {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        ...getAuthHeader()
      }
    }
  );

  const data = await response.json().catch(() => null);

  if (!response.ok) {
    throw new Error(
      data?.mensagem ||
      'Não foi possível sair do grupo.'
    );
  }

  return data;
}


export async function alterarDataGrupo(
  idGrupo: number,
  dataInicio: string,
  getAuthHeader: () => Record<string, string>
) {
  const response = await fetch(
    `/api/grupo/${idGrupo}/data`,
    {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
        ...getAuthHeader()
      },
      body: JSON.stringify({ dataInicio })
    }
  );

  const data = await response.json().catch(() => null);

  if (!response.ok) {
    throw new Error(
      data?.mensagem ||
      'Não foi possível alterar a data do passeio.'
    );
  }

  return data;
}


export async function cancelarGrupo(
  idGrupo: number,
  getAuthHeader: () => Record<string, string>
) {
  const response = await fetch(
    `/api/grupo/${idGrupo}/cancelar`,
    {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        ...getAuthHeader()
      }
    }
  );

  const data = await response.json().catch(() => null);

  if (!response.ok) {
    throw new Error(
      data?.mensagem ||
      'Não foi possível cancelar o passeio.'
    );
  }

  return data;
}


export async function finalizarPasseio(
  idGrupo: number,
  getAuthHeader: () => Record<string, string>
) {
  const response = await fetch(
    `/api/grupo/${idGrupo}/finalizar`,
    {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        ...getAuthHeader()
      }
    }
  );

  const data = await response.json().catch(() => null);

  if (!response.ok) {
    throw new Error(
      data?.mensagem ||
      'Não foi possível finalizar o passeio.'
    );
  }

  return data;
}