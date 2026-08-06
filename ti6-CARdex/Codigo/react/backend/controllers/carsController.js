import Car from '../models/Car.js';
import { unlink } from 'fs/promises';
import { join } from 'path';
import { fileURLToPath } from 'url';
import { dirname } from 'path';

const __filename = fileURLToPath(import.meta.url);
const __dirname = dirname(__filename);

class CarsController {
  // Adicionar carro à coleção
  static async addCar(req, res) {
    try {
      const { carBrand, carModel } = req.body;
      const userId = req.userId;

      // Validações
      if (!carBrand || !carModel) {
        return res.status(400).json({ error: 'Marca e modelo são obrigatórios' });
      }

      if (!req.file) {
        return res.status(400).json({ error: 'Imagem é obrigatória' });
      }

      // Verificar se já tem esse carro
      const existingCar = Car.findByUserAndModel(userId, carBrand, carModel);
      
      if (existingCar) {
        // Deletar arquivo enviado já que não será usado
        await unlink(req.file.path).catch(() => {});
        return res.status(400).json({ 
          error: 'Você já possui este carro na sua coleção',
          duplicate: true 
        });
      }

      // Caminho relativo da imagem
      const imagePath = `/uploads/${req.file.filename}`;

      // Salvar no banco
      const car = Car.create(userId, carBrand, carModel, imagePath);

      return res.status(201).json({
        message: 'Carro adicionado à coleção!',
        car: {
          id: car.id,
          carBrand: car.carBrand,
          carModel: car.carModel,
          imagePath: car.imagePath
        }
      });
    } catch (error) {
      console.error('Erro ao adicionar carro:', error);
      // Deletar arquivo se houver erro
      if (req.file) {
        await unlink(req.file.path).catch(() => {});
      }
      return res.status(500).json({ error: 'Erro ao adicionar carro' });
    }
  }

  // Listar carros do usuário
  static getCollection(req, res) {
    try {
      const userId = req.userId;
      const cars = Car.findByUserId(userId);
      const total = Car.countByUserId(userId);

      return res.json({
        total,
        cars: cars.map(car => ({
          id: car.id,
          carBrand: car.car_brand,
          carModel: car.car_model,
          imagePath: car.image_path,
          detectedAt: car.detected_at
        }))
      });
    } catch (error) {
      console.error('Erro ao buscar coleção:', error);
      return res.status(500).json({ error: 'Erro ao buscar coleção' });
    }
  }

  // Deletar carro da coleção
  static async deleteCar(req, res) {
    try {
      const { id } = req.params;
      const userId = req.userId;

      // Buscar carro
      const car = Car.findById(id);

      if (!car) {
        return res.status(404).json({ error: 'Carro não encontrado' });
      }

      // Verificar se pertence ao usuário
      if (car.user_id !== userId) {
        return res.status(403).json({ error: 'Não autorizado' });
      }

      // Deletar arquivo da imagem
      const imagePath = join(__dirname, '..', car.image_path);
      await unlink(imagePath).catch(() => {});

      // Deletar do banco
      Car.delete(id, userId);

      return res.json({ message: 'Carro removido da coleção' });
    } catch (error) {
      console.error('Erro ao deletar carro:', error);
      return res.status(500).json({ error: 'Erro ao deletar carro' });
    }
  }

  // Buscar um carro específico
  static getCar(req, res) {
    try {
      const { id } = req.params;
      const userId = req.userId;

      const car = Car.findById(id);

      if (!car) {
        return res.status(404).json({ error: 'Carro não encontrado' });
      }

      // Verificar se pertence ao usuário
      if (car.user_id !== userId) {
        return res.status(403).json({ error: 'Não autorizado' });
      }

      return res.json({
        car: {
          id: car.id,
          carBrand: car.car_brand,
          carModel: car.car_model,
          imagePath: car.image_path,
          detectedAt: car.detected_at
        }
      });
    } catch (error) {
      console.error('Erro ao buscar carro:', error);
      return res.status(500).json({ error: 'Erro ao buscar carro' });
    }
  }
}

export default CarsController;
