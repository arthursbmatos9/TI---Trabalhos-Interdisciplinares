import db from '../config/db.js';

class User {
  // Criar usuário
  static create(username, email, hashedPassword) {
    const stmt = db.prepare(`
      INSERT INTO users (username, email, password)
      VALUES (?, ?, ?)
    `);
    
    try {
      const result = stmt.run(username, email, hashedPassword);
      return { id: result.lastInsertRowid, username, email };
    } catch (error) {
      if (error.message.includes('UNIQUE')) {
        throw new Error('Usuário ou email já existe');
      }
      throw error;
    }
  }

  // Buscar usuário por email
  static findByEmail(email) {
    const stmt = db.prepare('SELECT * FROM users WHERE email = ?');
    return stmt.get(email);
  }

  // Buscar usuário por username
  static findByUsername(username) {
    const stmt = db.prepare('SELECT * FROM users WHERE username = ?');
    return stmt.get(username);
  }

  // Buscar usuário por ID
  static findById(id) {
    const stmt = db.prepare('SELECT id, username, email, created_at FROM users WHERE id = ?');
    return stmt.get(id);
  }

  // Listar todos os usuários
  static findAll() {
    const stmt = db.prepare('SELECT id, username, email, created_at FROM users');
    return stmt.all();
  }

  // Deletar usuário
  static delete(id) {
    const stmt = db.prepare('DELETE FROM users WHERE id = ?');
    return stmt.run(id);
  }
}

export default User;
