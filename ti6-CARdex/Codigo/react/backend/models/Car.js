import db from '../config/db.js';

class Car {
  // Adicionar carro à coleção
  static create(userId, carBrand, carModel, imagePath) {
    const stmt = db.prepare(`
      INSERT INTO collected_cars (user_id, car_brand, car_model, image_path)
      VALUES (?, ?, ?, ?)
    `);
    
    const result = stmt.run(userId, carBrand, carModel, imagePath);
    return { 
      id: result.lastInsertRowid, 
      userId, 
      carBrand, 
      carModel, 
      imagePath 
    };
  }

  // Buscar todos os carros de um usuário
  static findByUserId(userId) {
    const stmt = db.prepare(`
      SELECT * FROM collected_cars 
      WHERE user_id = ? 
      ORDER BY detected_at DESC
    `);
    return stmt.all(userId);
  }

  // Buscar carro por ID
  static findById(id) {
    const stmt = db.prepare('SELECT * FROM collected_cars WHERE id = ?');
    return stmt.get(id);
  }

  // Verificar se usuário já tem o carro
  static findByUserAndModel(userId, carBrand, carModel) {
    const stmt = db.prepare(`
      SELECT * FROM collected_cars 
      WHERE user_id = ? AND car_brand = ? AND car_model = ?
    `);
    return stmt.get(userId, carBrand, carModel);
  }

  // Deletar carro da coleção
  static delete(id, userId) {
    const stmt = db.prepare('DELETE FROM collected_cars WHERE id = ? AND user_id = ?');
    return stmt.run(id, userId);
  }

  // Contar carros na coleção do usuário
  static countByUserId(userId) {
    const stmt = db.prepare('SELECT COUNT(*) as total FROM collected_cars WHERE user_id = ?');
    return stmt.get(userId).total;
  }

  // Listar todos os carros (admin)
  static findAll() {
    const stmt = db.prepare(`
      SELECT c.*, u.username 
      FROM collected_cars c 
      JOIN users u ON c.user_id = u.id 
      ORDER BY c.detected_at DESC
    `);
    return stmt.all();
  }
}

export default Car;
