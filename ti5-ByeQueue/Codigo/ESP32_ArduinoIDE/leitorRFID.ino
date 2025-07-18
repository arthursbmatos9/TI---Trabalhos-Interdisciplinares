#include <SPI.h>
#include <MFRC522.h>
#include <Wire.h>
#include <LiquidCrystal_I2C.h>
#include <WiFi.h>
#include <HTTPClient.h>
#include <map>
#include <WebSocketsServer.h>
#include <WebServer.h>  // <== Adicionamos isso

// ---- Configurações Wi-Fi ----
const char* ssid = "APT104_2G_5G";
const char* password = "cuiaba2004";
const char* serverURL = "http://192.168.0.114:5000/produto/";

// ---- Pinos RFID e I2C ----
#define SS_PIN 21
#define RST_PIN 22
#define SDA_PIN 5
#define SCL_PIN 4

// ---- LCD ----
#define LCD_ADDRESS 0x27
#define LCD_COLUMNS 16
#define LCD_ROWS 2

// ---- Objetos ----
MFRC522 mfrc522(SS_PIN, RST_PIN);
LiquidCrystal_I2C lcd(LCD_ADDRESS, LCD_COLUMNS, LCD_ROWS);
WebSocketsServer webSocket = WebSocketsServer(81);
WebServer server(80);  // <== Novo servidor HTTP

float total = 0.0;
std::map<String, float> carrinho;

// ---- Setup ----
void setup() {
  Serial.begin(115200);
  Wire.begin(SDA_PIN, SCL_PIN);
  lcd.init();
  lcd.backlight();
  lcd.clear();
  lcd.setCursor(0, 0);
  lcd.print("Inicializando...");

  WiFi.begin(ssid, password);
  lcd.setCursor(0, 1);
  lcd.print("WiFi...");
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }

  Serial.println("\nWiFi conectado");
  Serial.println(WiFi.localIP());
  lcd.clear();
  lcd.setCursor(0, 0);
  lcd.print("WiFi conectado!");
  delay(1000);

  SPI.begin();
  SPI.setFrequency(4000000);
  pinMode(RST_PIN, OUTPUT);
  digitalWrite(RST_PIN, LOW);
  delay(100);
  digitalWrite(RST_PIN, HIGH);
  delay(100);
  mfrc522.PCD_Init();
  mfrc522.PCD_SetAntennaGain(MFRC522::RxGain_max);
  mfrc522.PCD_WriteRegister(MFRC522::RFCfgReg, 0x70);

  webSocket.begin();
  webSocket.onEvent(webSocketEvent);

  server.on("/cart", HTTP_GET, handleGetCart);       // <== GET do carrinho
  server.on("/checkout", HTTP_POST, handleCheckout); // <== POST para finalizar
  server.begin();

  lcd.clear();
  lcd.setCursor(0, 0);
  lcd.print("Aproxime o");
  lcd.setCursor(0, 1);
  lcd.print("produto...");
}

// ---- Loop principal ----
void loop() {
  webSocket.loop();
  server.handleClient();  // <== Requisições HTTP

  static unsigned long lastResetTime = 0;
  if (millis() - lastResetTime > 30000) {
    mfrc522.PCD_Reset();
    delay(50);
    mfrc522.PCD_Init();
    mfrc522.PCD_SetAntennaGain(MFRC522::RxGain_max);
    mfrc522.PCD_WriteRegister(MFRC522::RFCfgReg, 0x70);
    lastResetTime = millis();
  }

  if (detectNTAG()) {
    String uid = "";
    for (byte i = 0; i < mfrc522.uid.size; i++) {
      if (mfrc522.uid.uidByte[i] < 0x10) uid += "0";
      uid += String(mfrc522.uid.uidByte[i], HEX);
    }
    uid.toUpperCase();

    Serial.print("UID detectado: ");
    Serial.println(uid);

    String resposta = consultarProduto(uid);
    Serial.println("Resposta da API: ");
    Serial.println(resposta);

    lcd.clear();
    lcd.setCursor(0, 0);

    if (resposta == "Sem WiFi" || resposta == "Erro de consulta") {
      lcd.print("Erro:");
      lcd.setCursor(0, 1);
      lcd.print(resposta);
    } else {
      int separador = resposta.indexOf('|');
      if (separador != -1) {
        String nome = resposta.substring(0, separador);
        String precoStr = resposta.substring(separador + 1);
        float preco = precoStr.toFloat();

        String acao;
        if (carrinho.count(uid) > 0) {
          total -= carrinho[uid];
          carrinho.erase(uid);
          lcd.print("Removido:");
          acao = "remover";
        } else {
          total += preco;
          carrinho[uid] = preco;
          lcd.print("Adicionado:");
          acao = "adicionar";
        }

        lcd.setCursor(0, 1);
        lcd.print(nome.substring(0, 16));
        delay(2000);

        String json = "{\"acao\":\"" + acao + "\",\"nome\":\"" + nome + "\",\"preco\":" + String(preco, 2) + ",\"total\":" + String(total, 2) + "}";
        webSocket.broadcastTXT(json);

        lcd.clear();
        lcd.setCursor(0, 0);
        lcd.print("Total: R$");
        lcd.print(total, 2);
      } else {
        lcd.print(resposta.substring(0, 16));
      }
    }

    mfrc522.PICC_HaltA();
    mfrc522.PCD_StopCrypto1();

    delay(3000);
    lcd.clear();
    lcd.setCursor(0, 0);
    lcd.print("Aproxime o");
    lcd.setCursor(0, 1);
    lcd.print("produto...");
  }

  delay(50);
}

// ---- Função de leitura de cartão ----
bool detectNTAG() {
  mfrc522.PCD_StopCrypto1();
  for (int i = 0; i < 10; i++) {
    if (mfrc522.PICC_IsNewCardPresent()) {
      if (mfrc522.PICC_ReadCardSerial()) {
        return true;
      }
    }
    delay(20);
  }
  return false;
}

// ---- Consulta a API com o UID ----
String consultarProduto(String uid) {
  if (WiFi.status() == WL_CONNECTED) {
    HTTPClient http;
    String url = String(serverURL) + uid;
    http.begin(url);
    int httpCode = http.GET();

    if (httpCode == 200) {
      String payload = http.getString();
      http.end();
      return payload;
    } else {
      Serial.print("Erro HTTP: ");
      Serial.println(httpCode);
      http.end();
      return "Erro de consulta";
    }
  } else {
    return "Sem WiFi";
  }
}

// ---- Evento do WebSocket ----
void webSocketEvent(uint8_t num, WStype_t type, uint8_t * payload, size_t length) {
  if (type == WStype_TEXT) {
    String msg = String((char*)payload);
    if (msg == "finalizar") {
      lcd.clear();
      lcd.setCursor(0, 0);
      lcd.print("Compra finalizada");
      lcd.setCursor(0, 1);
      lcd.print("Total: R$");
      lcd.print(total, 2);

      String json = "{\"acao\":\"finalizar\",\"total\":" + String(total, 2) + "}";
      webSocket.broadcastTXT(json);

      total = 0.0;
      carrinho.clear();
    }
  }
}

// ---- HTTP: Retorna carrinho ----
void handleGetCart() {
  String json = "{\"itens\":[";
  bool first = true;
  for (auto const& item : carrinho) {
    if (!first) json += ",";
    json += "{\"uid\":\"" + item.first + "\",\"preco\":" + String(item.second, 2) + "}";
    first = false;
  }
  json += "],\"total\":" + String(total, 2) + "}";
  server.send(200, "application/json", json);
}

// ---- HTTP: Finaliza compra ----
void handleCheckout() {
  String json = "{\"acao\":\"finalizar\",\"total\":" + String(total, 2) + "}";
  webSocket.broadcastTXT(json);

  lcd.clear();
  lcd.setCursor(0, 0);
  lcd.print("Compra finalizada");
  lcd.setCursor(0, 1);
  lcd.print("Total: R$");
  lcd.print(total, 2);

  total = 0.0;
  carrinho.clear();

  server.send(200, "application/json", json);
}
