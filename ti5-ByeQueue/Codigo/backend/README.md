# 💳 API de Pagamentos com Stripe (FastAPI)

Esta API permite a criação de um **Payment Intent** usando a [Stripe API](https://stripe.com), retornando o `client_secret` necessário para confirmação de pagamento no frontend (como apps Flutter ou web).

---

## 📌 Endpoint

```
POST /create-payment-intent
```

---

## 📥 Corpo da Requisição

A requisição deve ser enviada em formato JSON com os seguintes campos:

```json
{
  "total_amount": 199.90,
  "items": [
    {
      "id": "item01",
      "name": "Produto Exemplo"
    },
    {
      "id": "item02",
      "name": "Outro Produto"
    }
  ]
}
```

| Campo         | Tipo     | Descrição                                                   |
|---------------|----------|-------------------------------------------------------------|
| `total_amount`| `float`  | Valor total da compra em reais (será convertido para centavos)|
| `items`       | `list`   | Lista de itens adquiridos (pode ser usada para logs ou recibos) |

---

## 📤 Resposta (200 OK)

```json
{
  "client_secret": "pi_1HxXXX...secret_abc123"
}
```

| Campo          | Tipo     | Descrição                                                                 |
|----------------|----------|---------------------------------------------------------------------------|
| `client_secret`| `string` | Token secreto usado no frontend para confirmar o pagamento via Stripe SDK |

---

## ⚠️ Erros Comuns

| Código | Mensagem                           | Causa possível                              |
|--------|------------------------------------|---------------------------------------------|
| 500    | `"StripeInvalidRequestError: ..."` | Erro ao criar o pagamento (valor inválido, chave incorreta, etc.) |
| 500    | `"Environment variable not set"`   | A variável `STRIPE_SECRET_KEY` não foi definida corretamente |

---

## ⚙️ Configuração do Ambiente

Crie um arquivo `.env` na raiz do projeto e defina a chave secreta do Stripe:

```env
STRIPE_SECRET_KEY=sk_test_xxxxxxxxxxxxxxxxxxxxx
```

---

## ▶️ Como Rodar o Projeto

1. Clone o repositório e acesse a pasta do projeto:

```bash
git clone https://github.com/seu-usuario/seu-repositorio.git
cd seu-repositorio
```

2. Instale as dependências:

```bash
pip install -r requirements.txt
```

3. Execute o servidor:

```bash
uvicorn main:app --host 0.0.0.0 --port 8000
```

---