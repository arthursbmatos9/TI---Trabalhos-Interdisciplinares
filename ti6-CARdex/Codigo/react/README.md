# Cardex - Sistema de Identificação e Coleção de Carros 🚗

Aplicação completa com frontend React e backend Node.js para identificar e colecionar carros através de fotos.

## 🎯 Funcionalidades

- ✅ Sistema de autenticação (Login/Cadastro)
- ✅ Captura de fotos de carros via câmera
- ✅ Identificação de carros por IA
- ✅ Coleção personalizada por usuário
- ✅ Armazenamento de fotos
- ✅ Banco de dados SQLite

## 📁 Estrutura do Projeto

```
cardex/
├── backend/              # API Node.js + Express + SQLite
│   ├── config/
│   ├── controllers/
│   ├── middleware/
│   ├── models/
│   ├── routes/
│   ├── uploads/
│   └── server.js
├── src/                  # Frontend React
│   ├── pages/
│   │   ├── WelcomePage.jsx
│   │   ├── LoginPage.jsx
│   │   ├── RegisterPage.jsx
│   │   ├── HomePage.jsx
│   │   ├── CameraPage.jsx
│   │   ├── ResultPage.jsx
│   │   └── CardexPage.jsx
│   ├── services/
│   │   ├── authService.js
│   │   └── aiService.js
│   └── App.jsx
└── README.md
```

## 🚀 Como Rodar o Projeto

### Pré-requisitos

- Node.js (v16 ou superior)
- npm ou yarn

### ⚠️ IMPORTANTE - Configuração obrigatória do Backend

**Antes de rodar o backend**, você precisa criar o arquivo `.env`:

1. Entre na pasta `backend/`
2. Crie um arquivo chamado `.env` (ou copie o `.env.example`)
3. Adicione o seguinte conteúdo:

```env
PORT=3001
JWT_SECRET=cardex_secret_key_change_in_production
NODE_ENV=development
```

**OU** copie o arquivo de exemplo:
```powershell
cd backend
copy .env.example .env
```

> 🚨 **SEM O ARQUIVO .env O BACKEND NÃO FUNCIONARÁ!** Você verá o erro: `secretOrPrivateKey must have a value`

### 1. Instalar dependências do Frontend

```powershell
npm install
```

### 2. Instalar dependências do Backend

```powershell
cd backend
npm install
```

### 3. Iniciar o Backend

Em um terminal, dentro da pasta `backend/`:

```powershell
npm run dev
```

O backend estará rodando em `http://localhost:3001`

### 4. Iniciar o Frontend

Em outro terminal, na raiz do projeto:

```powershell
npm run dev
```

O frontend estará rodando em `http://localhost:5173`

## 📱 Fluxo da Aplicação

1. **Tela de Boas-vindas** - Opções de Login ou Cadastro
2. **Login/Cadastro** - Autenticação de usuário
3. **Home** - Navegação principal
4. **Câmera** - Captura de fotos de carros
5. **Resultado** - Visualização do carro identificado
6. **Cardex** - Coleção completa de carros do usuário

## 🔐 Autenticação

O sistema usa JWT (JSON Web Tokens) para autenticação:

- Token é salvo no `localStorage`
- Válido por 7 dias
- Enviado no header `Authorization: Bearer {token}`

## 📡 API Endpoints

### Autenticação

- `POST /api/auth/register` - Cadastro de usuário
- `POST /api/auth/login` - Login
- `GET /api/auth/profile` - Perfil do usuário (protegido)

### Carros

- `POST /api/cars` - Adicionar carro à coleção (protegido)
- `GET /api/cars` - Listar coleção (protegido)
- `GET /api/cars/:id` - Buscar carro específico (protegido)
- `DELETE /api/cars/:id` - Remover carro (protegido)

## 🗄️ Banco de Dados

SQLite com 2 tabelas principais:

### users
- id, username, email, password, created_at

### collected_cars
- id, user_id, car_brand, car_model, image_path, detected_at

## 🛠️ Tecnologias

### Frontend
- React 19
- React Router DOM
- Vite
- Tailwind CSS
- Framer Motion
- React Webcam

### Backend
- Node.js
- Express
- SQLite (better-sqlite3)
- JWT (jsonwebtoken)
- Bcrypt
- Multer
- CORS

## 📝 Testando o Sistema

### 1. Criar uma conta

1. Acesse `http://localhost:5173`
2. Clique em "Criar Conta"
3. Preencha: username, email, senha
4. Você será automaticamente logado

### 2. Fazer login

1. Clique em "Entrar" na tela inicial
2. Use email e senha cadastrados
3. Acesse a aplicação

### 3. Capturar carros

1. Na home, vá para a câmera
2. Tire foto de um carro
3. Confirme a foto
4. Veja o carro identificado
5. Acesse o Cardex para ver sua coleção

## 🔧 Configuração

### Backend (.env)

```env
PORT=3001
JWT_SECRET=sua_chave_secreta_aqui
NODE_ENV=development
```

### Frontend

A URL da API está configurada em `src/services/authService.js`:

```javascript
const API_URL = 'http://localhost:3001/api';
```

## 🐛 Troubleshooting

### Backend não inicia
- Verifique se a porta 3001 está livre
- Rode `npm install` na pasta backend
- Verifique o arquivo `.env`

### Frontend não conecta ao backend
- Certifique-se que o backend está rodando
- Verifique CORS no backend
- Confira a URL da API no código

### Câmera não funciona
- Dê permissão para a câmera no navegador
- Use HTTPS em produção
- Teste em um dispositivo com câmera

### Erro de autenticação
- Limpe o localStorage
- Faça login novamente
- Verifique se o token é válido

## 📦 Build para Produção

### Frontend

```powershell
npm run build
```

### Backend

```powershell
cd backend
npm start
```

---

Desenvolvido com ❤️ para o projeto Cardex 🚗✨
