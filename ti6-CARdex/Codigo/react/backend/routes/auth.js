import { Router } from 'express';
import AuthController from '../controllers/authController.js';
import authMiddleware from '../middleware/auth.js';

const router = Router();

// Rotas públicas
router.post('/register', AuthController.register);
router.post('/login', AuthController.login);

// Rotas protegidas
router.get('/profile', authMiddleware, AuthController.getProfile);

export default router;
