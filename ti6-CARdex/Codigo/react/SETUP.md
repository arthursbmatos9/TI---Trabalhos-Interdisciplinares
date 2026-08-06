# 🚀 Setup do Projeto Cardex

## ⚠️ IMPORTANTE - Primeira execução

Antes de rodar o projeto pela primeira vez, siga estes passos:

### 1️⃣ Instalar dependências do Frontend

Na raiz do projeto:
```powershell
npm install
```

### 2️⃣ Instalar dependências do Backend

```powershell
cd backend
npm install
```

### 3️⃣ **CRIAR ARQUIVO .env** (OBRIGATÓRIO)

Dentro da pasta `backend/`, crie um arquivo chamado `.env` (sem extensão) com este conteúdo:

```env
PORT=3001
JWT_SECRET=cardex_secret_key_change_in_production
NODE_ENV=development
```

**OU** copie o arquivo de exemplo:
```powershell
# Dentro da pasta backend/
copy .env.example .env
```

> ⚠️ **SEM ESTE ARQUIVO O BACKEND NÃO FUNCIONARÁ!**

### 4️⃣ Rodar o Backend

Em um terminal, dentro da pasta `backend/`:
```powershell
npm run dev
```

Você deve ver:
```
🚀 Servidor rodando na porta 3001
📍 http://localhost:3001
📁 Banco de dados: cardex.db
✅ Tabelas criadas/verificadas com sucesso!
```

### 5️⃣ Rodar o Frontend

Em outro terminal, na raiz do projeto:
```powershell
npm run dev
```

Acesse: `http://localhost:5173`

## 🐛 Problemas Comuns

### ❌ "secretOrPrivateKey must have a value"
**Solução**: Você esqueceu de criar o arquivo `.env` dentro da pasta `backend/`

### ❌ "EADDRINUSE: address already in use :::3001"
**Solução**: A porta 3001 já está em uso. Mude no `.env` ou mate o processo

### ❌ "Cannot find module 'better-sqlite3'"
**Solução**: Rode `npm install` dentro da pasta `backend/`

## 📝 Estrutura esperada

```
cardex/
├── backend/
│   ├── .env          ← VOCÊ PRECISA CRIAR ESTE ARQUIVO!
│   ├── .env.example  ← Use este como base
│   ├── package.json
│   └── ...
├── src/
├── package.json
└── README.md
```

## ✅ Checklist

- [ ] Rodei `npm install` na raiz
- [ ] Rodei `npm install` no backend
- [ ] Criei o arquivo `backend/.env`
- [ ] Backend rodando (porta 3001)
- [ ] Frontend rodando (porta 5173)
- [ ] Consegui acessar http://localhost:5173

---

**Dúvidas?** Verifique o README.md principal ou o backend/README.md
