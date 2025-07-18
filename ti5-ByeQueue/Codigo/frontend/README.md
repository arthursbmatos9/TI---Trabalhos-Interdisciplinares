# Dificuldades Encontradas no Projeto Flutter – Pagamentos com QR Code

Este documento resume os principais desafios enfrentados durante o desenvolvimento da interface de pagamento em Flutter, incluindo integração com backend e exibição de QR Code.

## 1. Fluxo de Pagamento

- Ajustar o fluxo correto de criação do `PaymentIntent` via backend.
- Validar o formulário apenas quando o método de pagamento for cartão de crédito.
- Lidar com estados assíncronos como carregamento, sucesso e falha de pagamento.

## 2. Exibição de Diálogos

- Implementar a sequência correta de diálogos: primeiro o de sucesso, depois o com o QR Code.
- Evitar que a tela fique escura ou trave ao tentar abrir múltiplos diálogos seguidos.

## 3. Renderização do QR Code

- Resolver problemas com a tela escura ao mostrar o QR Code.
- Garantir a instalação e importação correta da dependência `qr_flutter`.
- Certificar que os dados do QR Code (como a string "deu certo") estejam válidos antes de renderizar.

## 4. Tratamento de Erros

- Exibir mensagens claras para:
  - Falhas de conexão.
  - Respostas de erro da API.
  - Timeout na requisição HTTP.
- Evitar falhas silenciosas que não apresentam feedback ao usuário.

## 5. Manipulação de Estado

- Bloquear a interface enquanto o pagamento estiver sendo processado.
- Garantir que o `setState` só seja chamado quando o widget ainda estiver montado.
- Limpar campos e estados após a finalização do pagamento.





# OBS -> caso quando for pagar der erro de conexão, so executar:

```bash
flutter clean
flutter pub get
flutter run
```