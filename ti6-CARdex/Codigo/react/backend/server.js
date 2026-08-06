import 'dotenv/config';
import express from 'express';
import cors from 'cors';
import { fileURLToPath } from 'url';
import { dirname, join } from 'path';
import authRoutes from './routes/auth.js';
import carsRoutes from './routes/cars.js';
import './config/db.js'; // Inicializar banco

const __filename = fileURLToPath(import.meta.url);
const __dirname = dirname(__filename);

const app = express();
const PORT = process.env.PORT || 3001;

// Middlewares
app.use(cors());
app.use(express.json());
app.use(express.urlencoded({ extended: true }));

// Servir arquivos estáticos (imagens)
app.use('/uploads', express.static(join(__dirname, 'uploads')));

// Rota de teste
app.get('/', (req, res) => {
  res.json({ 
    message: 'Cardex API está funcionando! 🚗',
    version: '1.0.0',
    endpoints: {
      auth: '/api/auth',
      cars: '/api/cars'
    }
  });
});

// Rotas
app.use('/api/auth', authRoutes);
app.use('/api/cars', carsRoutes);

// Tratamento de erros
app.use((err, req, res, next) => {
  console.error(err.stack);
  
  if (err instanceof multer.MulterError) {
    if (err.code === 'LIMIT_FILE_SIZE') {
      return res.status(400).json({ error: 'Arquivo muito grande. Máximo: 5MB' });
    }
  }
  
  res.status(500).json({ error: err.message || 'Erro interno do servidor' });
});

// Rota 404
app.use((req, res) => {
  res.status(404).json({ error: 'Rota não encontrada' });
});

// Iniciar servidor
app.listen(PORT, () => {
  console.log(`🚀 Servidor rodando na porta ${PORT}`);
  console.log(`📍 http://localhost:${PORT}`);
  console.log(`📁 Banco de dados: cardex.db`);
});
