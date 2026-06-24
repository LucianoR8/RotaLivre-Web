# Rota Livre

Plataforma web e mobile para gerenciamento de passeios turísticos na cidade de São Paulo.

Site disponível em: [Rota Livre](https://rotalivre-web.onrender.com/)

## Sobre o Projeto

O Rota Livre é uma plataforma desenvolvida para auxiliar usuários na descoberta, organização e participação em passeios turísticos.

A solução é composta por uma API desenvolvida em ASP.NET Core, um aplicativo mobile desenvolvido em .NET MAUI e um banco de dados PostgreSQL.

A plataforma permite o gerenciamento de usuários, passeios, grupos e avaliações, centralizando informações e facilitando a interação entre participantes.

---

## Arquitetura

O backend foi desenvolvido utilizando ASP.NET Core seguindo uma arquitetura em camadas:

```text
Controllers
    ↓
Services
    ↓
Repositories
    ↓
Entity Framework Core
    ↓
PostgreSQL
```

### Camadas da aplicação

**Controllers**

* Recebem as requisições HTTP.
* Validam parâmetros.
* Retornam respostas para o cliente.

**Services**

* Implementam as regras de negócio da aplicação.

**Repositories**

* Responsáveis pelo acesso e manipulação dos dados.

**DTOs**

* Utilizados para transferência de dados entre cliente e servidor.

**Models**

* Representam as entidades do domínio da aplicação.

---

## Tecnologias Utilizadas

* C#
* ASP.NET Core
* .NET MAUI
* PostgreSQL
* Entity Framework Core
* Supabase
* Git
* GitHub

![C#](https://img.shields.io/badge/C%23-512BD4?style=for-the-badge\&logo=csharp\&logoColor=white)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-512BD4?style=for-the-badge\&logo=dotnet\&logoColor=white)
![.NET MAUI](https://img.shields.io/badge/.NET_MAUI-512BD4?style=for-the-badge\&logo=dotnet\&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-316192?style=for-the-badge\&logo=postgresql\&logoColor=white)
![Entity Framework Core](https://img.shields.io/badge/Entity_Framework_Core-68217A?style=for-the-badge\&logo=dotnet\&logoColor=white)

---

## Funcionalidades

### Usuários

* Cadastro de usuários
* Autenticação
* Gerenciamento de perfil
* Recuperação de senha

### Passeios

* Cadastro de passeios
* Consulta de passeios
* Favoritos
* Avaliações

### Grupos

* Criação de grupos
* Participação em grupos
* Compartilhamento por convite

### Endereços e Categorias

* Gerenciamento de categorias
* Gerenciamento de endereços

---

## Estrutura do Backend

```text
Controllers/
DTOs/
Data/
Hubs/
Interfaces/
Migrations/
Models/
Repositories/
Services/
Utilidades/
```
---
## Próximas Funcionalidades

* Autenticação JWT
* Melhorias no sistema de localização
* Geofencing
* Melhorias de usabilidade
* Novas funcionalidades para gerenciamento de passeios

---

## Autores

[Breno Estevo](https://github.com/Bxnog), [Cauã Macedo](https://github.com/cauamacedo497), Giovanna Alves, [Iara Laeber](https://github.com/iaralae), [Luciano Ribeiro](https://github.com/LucianoR8), [Samira Camargo](https://github.com/SamiraCamargo)

### Contato
[![Email](https://img.shields.io/badge/Email-D14836?style=for-the-badge&logo=gmail&logoColor=white)](https://mail.google.com/mail/?view=cm&fs=1&to=rotalivrefam@gmail.com)
