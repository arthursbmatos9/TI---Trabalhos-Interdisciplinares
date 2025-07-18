import sqlite3
from flask import Flask, request

conn = sqlite3.connect('produtos.db')
cursor = conn.cursor()

# Cria a tabela
cursor.execute('''
CREATE TABLE IF NOT EXISTS produtos (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    uid TEXT UNIQUE NOT NULL,
    nome TEXT NOT NULL,
    preco REAL NOT NULL
)
''')

produtos = [
    ("53D26C64720001", "Trident", 2.90),
    ("53DA6C64720001", "Mel", 13.79),
    ("53DB6C64720001", "Fermento", 3.49),
    ("53D06C64720001", "KitKat", 6.49),
    ("53D16C64720001", "Adocante Zero Cal", 6.59)
]

# Atualiza os nomes e insere caso não exista
for uid, nome, preco in produtos:
    cursor.execute("SELECT uid FROM produtos WHERE uid = ?", (uid,))
    if cursor.fetchone():
        cursor.execute("UPDATE produtos SET nome = ?, preco = ? WHERE uid = ?", (nome, preco, uid))
    else:
        cursor.execute("INSERT INTO produtos (uid, nome, preco) VALUES (?, ?, ?)", (uid, nome, preco))

conn.commit()
conn.close()

#API Flask para consulta 
app = Flask(__name__)

@app.route('/produto/<uid>', methods=['GET'])
def get_produto(uid):
    conn = sqlite3.connect('produtos.db')
    cursor = conn.cursor()
    cursor.execute("SELECT nome, preco FROM produtos WHERE uid = ?", (uid,))
    row = cursor.fetchone()
    conn.close()

    if row:
        nome = row[0]
        preco = row[1]
        return f"{nome}|{preco:.2f}"
    else:
        return "Produto nao encontrado", 404

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5000)
