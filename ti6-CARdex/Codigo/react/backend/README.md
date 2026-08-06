# Cardex Backend 🚗

Backend para o sistema Cardex - Aplicação de identificação e coleção de carros.

## 🛠️ Tecnologias

- **Node.js** - Runtime JavaScript
- **Express** - Framework web
- **SQLite** (better-sqlite3) - Banco de dados
- **JWT** - Autenticação
- **Bcrypt** - Hash de senhas
- **Multer** - Upload de imagens
- **CORS** - Comunicação com frontend

## 📁 Estrutura do Projeto

```
backend/
├── config/
│   └── db.js              # Configuração do SQLite
├── controllers/
│   ├── authController.js  # Lógica de autenticação
│   └── carsController.js  # Lógica de carros
├── middleware/
│   └── auth.js            # Middleware JWT
├── models/
│   ├── User.js            # Model de usuário
│   └── Car.js             # Model de carro
├── routes/
│   ├── auth.js            # Rotas de autenticação
│   └── cars.js            # Rotas de carros
├── uploads/               # Imagens dos carros
├── .env                   # Variáveis de ambiente
├── .env.example           # Exemplo de variáveis
├── server.js              # Servidor principal
├── package.json
└── cardex.db              # Banco de dados (gerado automaticamente)
```

## 🚀 Como Rodar

### 1. Instalar dependências

```bash
npm install
```

### 2. Configurar variáveis de ambiente

Renomeie `.env.example` para `.env` e configure:

```env
PORT=3001
JWT_SECRET=sua_chave_secreta_aqui
NODE_ENV=development
```

### 3. Iniciar servidor

```bash
# Modo desenvolvimento (com auto-reload)
npm run dev

# Modo produção
npm start
```

O servidor estará disponível em `http://localhost:3001`

## 📡 API Endpoints

### Autenticação

#### Cadastro
```http
POST /api/auth/register
Content-Type: application/json

{
  "username": "joao",
  "email": "joao@email.com",
  "password": "123456"
}
```

**Resposta:**
```json
{
  "message": "Usuário criado com sucesso",
  "user": {
    "id": 1,
    "username": "joao",
    "email": "joao@email.com"
  },
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

#### Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "joao@email.com",
  "password": "123456"
}
```

**Resposta:**
```json
{
  "message": "Login realizado com sucesso",
  "user": {
    "id": 1,
    "username": "joao",
    "email": "joao@email.com"
  },
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

#### Perfil
```http
GET /api/auth/profile
Authorization: Bearer {token}
```

### Carros (Requer autenticação)

#### Adicionar carro à coleção
```http
POST /api/cars
Authorization: Bearer {token}
Content-Type: multipart/form-data

carBrand: "Toyota"
carModel: "Corolla"
image: [arquivo]
```

**Resposta:**
```json
{
  "message": "Carro adicionado à coleção!",
  "car": {
    "id": 1,
    "carBrand": "Toyota",
    "carModel": "Corolla",
    "imagePath": "/uploads/car-1234567890.jpg"
  }
}
```

#### Listar coleção
```http
GET /api/cars
Authorization: Bearer {token}
```

**Resposta:**
```json
{
  "total": 5,
  "cars": [
    {
      "id": 1,
      "carBrand": "Toyota",
      "carModel": "Corolla",
      "imagePath": "/uploads/car-1234567890.jpg",
      "detectedAt": "2025-11-25T10:30:00.000Z"
    }
  ]
}
```

#### Buscar carro específico
```http
GET /api/cars/:id
Authorization: Bearer {token}
```

#### Deletar carro
```http
DELETE /api/cars/:id
Authorization: Bearer {token}
```

## 🗄️ Banco de Dados

O banco SQLite é criado automaticamente na primeira execução.

### Tabelas

**users**
- `id` - INTEGER (PK, AUTO INCREMENT)
- `username` - TEXT (UNIQUE)
- `email` - TEXT (UNIQUE)
- `password` - TEXT (hash bcrypt)
- `created_at` - DATETIME

**collected_cars**
- `id` - INTEGER (PK, AUTO INCREMENT)
- `user_id` - INTEGER (FK)
- `car_brand` - TEXT
- `car_model` - TEXT
- `image_path` - TEXT
- `detected_at` - DATETIME

## 🔐 Autenticação

O sistema usa JWT (JSON Web Tokens) para autenticação.

1. Usuário faz login/cadastro
2. Recebe um token JWT válido por 7 dias
3. Envia o token no header `Authorization: Bearer {token}` nas requisições protegidas

## 📦 Upload de Imagens

- Tamanho máximo: **5MB**
- Formatos aceitos: **JPEG, PNG, WEBP**
- Imagens são salvas em `/uploads`
- Cada imagem tem nome único com timestamp

## 🔧 Desenvolvimento

```bash
# Instalar dependências
npm install

# Rodar em modo watch (auto-reload)
npm run dev

# Limpar banco (deletar cardex.db e reiniciar)
```

## 📝 Notas

- O arquivo `cardex.db` é criado automaticamente
- A pasta `uploads/` armazena as fotos dos carros
- Token JWT expira em 7 dias
- Senhas são hasheadas com bcrypt (10 rounds)

## 🤝 Integração com Frontend

No frontend React, configure a URL base da API:

```javascript
const API_URL = 'http://localhost:3001/api';

// Exemplo de requisição com token
const response = await fetch(`${API_URL}/cars`, {
  headers: {
    'Authorization': `Bearer ${token}`
  }
});
```

## 🐛 Troubleshooting

- **Erro "Token não fornecido"**: Verifique se está enviando o header Authorization
- **Erro "ENOENT"**: A pasta uploads/ precisa existir
- **Erro de porta**: Altere a PORT no .env
- **Banco não cria**: Verifique permissões de escrita

---

Desenvolvido para o projeto Cardex 🚗✨
