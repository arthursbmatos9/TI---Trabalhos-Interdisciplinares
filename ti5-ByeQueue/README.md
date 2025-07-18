# ByeQueue

O objetivo do projeto **ByeQueue** é desenvolver um carrinho de compras inteligente capaz de automatizar o processo de compra em supermercados, eliminando filas e otimizando a experiência do consumidor. Para isso, o sistema utiliza tecnologias como RFID, um microcontrolador ESP32, comunicação via Wi-Fi e integração com uma API e o sistema de pagamento Stripe. Os produtos são automaticamente identificados ao serem colocados no carrinho, permitindo que o cliente visualize os itens em um aplicativo mobile e finalize a compra sem precisar passar por um caixa tradicional.

A proposta busca não apenas agilizar o processo de checkout, mas também reduzir perdas operacionais e erros humanos, oferecendo uma solução tecnicamente viável e economicamente acessível em comparação com alternativas comerciais. O projeto representa um avanço significativo rumo ao chamado “varejo sem fricção”, onde a experiência de compra é mais fluida, segura e integrada à tecnologia.


## Alunos integrantes da equipe

* Ana Fernanda Souza Cancado
* Arthur de Sá Braz de Matos
* Gabriel Araújo Campos Silva
* Gabriel Praes Bernardes Nunes
* Guilherme Otávio de Oliveira
* Vitória Símil de Araújo

## Professores responsáveis

* Felipe Domingos da Cunha
* Matheus Barros Pereira

## Instruções de utilização

### ✅ Requisitos

- Python 3.9 ou superior  
- Pip (gerenciador de pacotes Python)  
- Placa **ESP32** com suporte a Wi-Fi  
- Leitor RFID **RC522**  
- Tags RFID (**MIFARE Classic 1K**)  
- Display **LCD 16x2 com módulo I2C**  
- Conexão Wi-Fi  
- **Flutter** instalado (para o app mobile)

---

## Estrutura do Projeto

- **ESP32 (Arduino IDE)**: Lê tags RFID, exibe no LCD e se comunica via HTTP e WebSocket.
- **API Flask (Python)**: Fornece informações dos produtos e gerencia o banco SQLite.
- **Flutter App**: Exibe produtos em tempo real e permite finalizar a compra.

---

## Pré-requisitos

### Geral
- Wi-Fi local estável
- Dispositivos conectados à mesma rede

### Arduino (ESP32)
- Arduino IDE
- Bibliotecas:
  - `SPI.h`
  - `MFRC522.h`
  - `Wire.h`
  - `LiquidCrystal_I2C.h`
  - `WiFi.h`
  - `HTTPClient.h`
  - `WebSocketsServer.h`
  - `WebServer.h`

### Backend Flask (consulta produtos)
- Python 3.9+
- Bibliotecas:
  - `flask`
  - `sqlite3`

### Backend FastAPI (pagamento)
- Python 3.9+
- Bibliotecas:
  - `fastapi`
  - `stripe`
  - `python-dotenv`
  - `uvicorn`

### Flutter
- Flutter SDK instalado
- Celular Android/iOS ou emulador
- Dependências (`pubspec.yaml`):
  - `web_socket_channel`
  - `http`
  - `qr_flutter`

---

## Configuração e Execução

### 1. Banco de dados e API Flask

1. Execute o código Python para criar o banco e subir a API:

   ```bash
   python flask_api.py
   ```

2. A API ficará disponível em:

   ```
   http://0.0.0.0:5000/produto/<uid>
   ```

3. Certifique-se de que o IP local da sua máquina é acessível pelo ESP32.

---

### 2. Backend de pagamento (FastAPI + Stripe)

1. Crie um `.env` com sua chave do Stripe:

```env
STRIPE_SECRET_KEY=sk_test_xxxxxxxxxxxxxxxxxxxxx
```

2. Execute com:

```bash
uvicorn main:app --host 0.0.0.0 --port 8000
```

---

### 3. Código Arduino (ESP32 com RFID)

1. Abra o código no Arduino IDE.
2. Configure sua rede Wi-Fi no início do código:

   ```cpp
   const char* ssid = "SEU_WIFI";
   const char* password = "SUA_SENHA";
   ```

3. Altere o IP da API Flask:

   ```cpp
   const char* serverURL = "http://SEU_IP_LOCAL:5000/produto/";
   ```

4. Faça upload do código para o ESP32.
5. No monitor serial, verifique o IP local que o ESP32 obteve (será usado no app Flutter).

---

### 4. Aplicativo Flutter

1. No código Flutter, edite a conexão WebSocket:

   ```dart
   final _channel = IOWebSocketChannel.connect('ws://IP_DO_ESP32:81');
   ```

2. Execute:

   ```bash
   flutter pub get
   flutter run
   ```

---

##  Como funciona

1. Produto com RFID é aproximado do leitor
2. UID é enviado via HTTP para o Flask (`/produto/<uid>`)
3. Nome e preço retornam e são exibidos no LCD
4. Via WebSocket, o produto é enviado ao app Flutter
5. No app, é possível remover ou finalizar a compra
6. No pagamento:
   - Cartão: envia ao Stripe
   - PIX: gera código e QR

---

##  Dicas

- Todos os dispositivos devem estar na mesma rede
- Para testes locais, use o IP real da máquina — **não use `localhost`**
- Para produção, substitua IPs por domínios ou use DNS local


### 📥 Clonando o Repositório

```bash
git clone https://github.com/usuario/ByeQueue.git
cd ByeQueue
