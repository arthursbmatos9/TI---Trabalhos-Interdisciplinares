import 'package:flutter/material.dart';
import 'Payment/PaymentPage.dart';
import 'package:web_socket_channel/io.dart';
import 'package:web_socket_channel/status.dart' as status;
import 'dart:convert';

void main() => runApp(MyApp());

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Carrinho RFID',
      theme: ThemeData(
        primarySwatch: Colors.blue,
        scaffoldBackgroundColor: const Color(0xFFF8F9FA),
        appBarTheme: const AppBarTheme(
          backgroundColor: Colors.white,
          foregroundColor: Color(0xFF222222),
          elevation: 0,
          titleTextStyle: TextStyle(
            color: Color(0xFF222222),
            fontSize: 20,
            fontWeight: FontWeight.w600,
          ),
        ),
        colorScheme: ColorScheme.fromSeed(
          seedColor: const Color(0xFFFF385C),
          primary: const Color(0xFFFF385C),
        ),
      ),
      home: CarrinhoPage(),
      debugShowCheckedModeBanner: false,
    );
  }
}

class CarrinhoPage extends StatefulWidget {
  const CarrinhoPage({super.key});

  @override
  _CarrinhoPageState createState() => _CarrinhoPageState();
}

class _CarrinhoPageState extends State<CarrinhoPage> {
  final _channel = IOWebSocketChannel.connect('ws://192.168.0.127:81');
  List<Map<String, dynamic>> produtos = [];
  double total = 0.0;

  @override
  void initState() {
    super.initState();

    produtos = [
      {'nome': 'Maçã', 'preco': 2.50},
      {'nome': 'Banana', 'preco': 1.75},
      {'nome': 'Leite', 'preco': 4.30},
    ];
    total = produtos.fold(0.0, (sum, item) => sum + item['preco']);

    _channel.stream.listen(
      (mensagem) {
        try {
          final data = json.decode(mensagem);
          final acao = data['acao'];
          final nome = data['nome'];
          final preco = double.tryParse(data['preco'].toString()) ?? 0.0;
          final novoTotal = double.tryParse(data['total'].toString()) ?? 0.0;

          setState(() {
            if (acao == 'adicionar') {
              produtos.add({'nome': nome, 'preco': preco});
              total = novoTotal;
            } else if (acao == 'remover') {
              produtos.removeWhere((item) => item['nome'] == nome);
              total = novoTotal;
            } else if (acao == 'finalizar') {
              produtos.clear();
              total = 0.0;
              ScaffoldMessenger.of(context).showSnackBar(
                SnackBar(
                  content: Text("Compra finalizada!"),
                  backgroundColor: const Color(0xFF00A699),
                  behavior: SnackBarBehavior.floating,
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(12),
                  ),
                ),
              );
            }
          });
        } catch (e) {
          print('Erro ao processar mensagem WebSocket: $e');
        }
      },
      onError: (erro) {
        print('Erro na conexão WebSocket: $erro');
      },
      onDone: () {
        print('Conexão WebSocket encerrada');
      },
    );
  }

  void finalizarCompra() {
    // Navegar para a tela de pagamento em vez de finalizar diretamente
    Navigator.push(
      context,
      MaterialPageRoute(
        builder: (context) => PaymentScreen(
          totalAmount: total,
          items: produtos
              .map((produto) => {
                    'name': produto['nome'],
                    'price': produto['preco'],
                  })
              .toList(),
          onPaymentSuccess: () {
            // Quando o pagamento for bem-sucedido, enviar comando para WebSocket
            print("Success");
            //_channel.sink.add(json.encode({'acao': 'finalizar'}));
          },
        ),
      ),
    );
  }

/*   @override
  void dispose() {
    _channel.sink.close(status.goingAway);
    super.dispose();
  } */

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: const Color(0xFFF8F9FA),
      appBar: AppBar(
        title: const Text("Meu Carrinho"),
        centerTitle: true,
        backgroundColor: Colors.white,
        shadowColor: Colors.black.withOpacity(0.1),
        elevation: 1,
      ),
      body: Column(
        children: [
          // Header Card
          Container(
            margin: const EdgeInsets.all(16),
            padding: const EdgeInsets.all(20),
            decoration: BoxDecoration(
              color: Colors.white,
              borderRadius: BorderRadius.circular(16),
              boxShadow: [
                BoxShadow(
                  color: Colors.black.withOpacity(0.08),
                  blurRadius: 10,
                  offset: const Offset(0, 2),
                ),
              ],
            ),
            child: Row(
              children: [
                Container(
                  width: 48,
                  height: 48,
                  decoration: BoxDecoration(
                    color: const Color(0xFFFF385C).withOpacity(0.1),
                    borderRadius: BorderRadius.circular(12),
                  ),
                  child: const Icon(
                    Icons.shopping_cart_outlined,
                    color: Color(0xFFFF385C),
                    size: 24,
                  ),
                ),
                const SizedBox(width: 16),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        '${produtos.length} ${produtos.length == 1 ? 'item' : 'itens'}',
                        style: const TextStyle(
                          fontSize: 18,
                          fontWeight: FontWeight.w600,
                          color: Color(0xFF222222),
                        ),
                      ),
                      const SizedBox(height: 4),
                      Text(
                        'no seu carrinho',
                        style: TextStyle(
                          fontSize: 14,
                          color: Colors.grey[600],
                        ),
                      ),
                    ],
                  ),
                ),
              ],
            ),
          ),

          // Products List
          Expanded(
            child: produtos.isEmpty
                ? _buildEmptyState()
                : Container(
                    margin: const EdgeInsets.symmetric(horizontal: 16),
                    child: ListView.builder(
                      itemCount: produtos.length,
                      itemBuilder: (context, index) {
                        final item = produtos[index];
                        return Container(
                          margin: const EdgeInsets.only(bottom: 12),
                          decoration: BoxDecoration(
                            color: Colors.white,
                            borderRadius: BorderRadius.circular(16),
                            boxShadow: [
                              BoxShadow(
                                color: Colors.black.withOpacity(0.06),
                                blurRadius: 8,
                                offset: const Offset(0, 2),
                              ),
                            ],
                          ),
                          child: ListTile(
                            contentPadding: const EdgeInsets.all(16),
                            leading: Container(
                              width: 56,
                              height: 56,
                              decoration: BoxDecoration(
                                color: const Color(0xFF00A699).withOpacity(0.1),
                                borderRadius: BorderRadius.circular(12),
                              ),
                              child: Icon(
                                _getProductIcon(item[
                                    'nome']), // this will change after, can make a similar function to get the images
                                color: const Color(0xFF00A699),
                                size: 28,
                              ),
                            ),
                            title: Text(
                              item['nome'],
                              style: const TextStyle(
                                fontSize: 16,
                                fontWeight: FontWeight.w600,
                                color: Color(0xFF222222),
                              ),
                            ),
                            subtitle: const Text(
                              '1 unidade',
                              style: TextStyle(
                                fontSize: 14,
                                color: Color(0xFF717171),
                              ),
                            ),
                            trailing: Container(
                              padding: const EdgeInsets.symmetric(
                                horizontal: 12,
                                vertical: 6,
                              ),
                              decoration: BoxDecoration(
                                color: const Color(0xFFFF385C).withOpacity(0.1),
                                borderRadius: BorderRadius.circular(8),
                              ),
                              child: Text(
                                "R\$ ${item['preco'].toStringAsFixed(2)}",
                                style: const TextStyle(
                                  fontSize: 16,
                                  fontWeight: FontWeight.w600,
                                  color: Color(0xFFFF385C),
                                ),
                              ),
                            ),
                          ),
                        );
                      },
                    ),
                  ),
          ),

          // Bottom Section
          Container(
            decoration: BoxDecoration(
              color: Colors.white,
              borderRadius: const BorderRadius.only(
                topLeft: Radius.circular(24),
                topRight: Radius.circular(24),
              ),
              boxShadow: [
                BoxShadow(
                  color: Colors.black.withOpacity(0.1),
                  blurRadius: 16,
                  offset: const Offset(0, -4),
                ),
              ],
            ),
            child: SafeArea(
              child: Padding(
                padding: const EdgeInsets.all(24),
                child: Column(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    // Total Row
                    Row(
                      mainAxisAlignment: MainAxisAlignment.spaceBetween,
                      children: [
                        const Text(
                          "Total",
                          style: TextStyle(
                            fontSize: 18,
                            fontWeight: FontWeight.w600,
                            color: Color(0xFF222222),
                          ),
                        ),
                        Text(
                          "R\$ ${total.toStringAsFixed(2)}",
                          style: const TextStyle(
                            fontSize: 24,
                            fontWeight: FontWeight.w700,
                            color: Color(0xFFFF385C),
                          ),
                        ),
                      ],
                    ),
                    const SizedBox(height: 20),
                    // Checkout Button
                    SizedBox(
                      width: double.infinity,
                      height: 56,
                      child: ElevatedButton(
                        onPressed: produtos.isEmpty ? null : finalizarCompra,
                        style: ElevatedButton.styleFrom(
                          backgroundColor: produtos.isEmpty
                              ? Colors.grey[300]
                              : const Color(0xFFFF385C),
                          foregroundColor: produtos.isEmpty
                              ? Colors.grey[600]
                              : Colors.white,
                          elevation: produtos.isEmpty ? 0 : 2,
                          shadowColor: const Color(0xFFFF385C).withOpacity(0.3),
                          shape: RoundedRectangleBorder(
                            borderRadius: BorderRadius.circular(16),
                          ),
                        ),
                        child: Row(
                          mainAxisAlignment: MainAxisAlignment.center,
                          children: [
                            Icon(
                              Icons.payment,
                              size: 20,
                            ),
                            const SizedBox(width: 8),
                            const Text(
                              "Finalizar Compra",
                              style: TextStyle(
                                fontSize: 16,
                                fontWeight: FontWeight.w600,
                              ),
                            ),
                          ],
                        ),
                      ),
                    ),
                  ],
                ),
              ),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildEmptyState() {
    return Center(
      child: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          Container(
            width: 80,
            height: 80,
            decoration: BoxDecoration(
              color: Colors.grey[100],
              borderRadius: BorderRadius.circular(20),
            ),
            child: Icon(
              Icons.shopping_cart_outlined,
              size: 40,
              color: Colors.grey[400],
            ),
          ),
          const SizedBox(height: 24),
          Text(
            "Seu carrinho está vazio",
            style: TextStyle(
              fontSize: 18,
              fontWeight: FontWeight.w600,
              color: Colors.grey[700],
            ),
          ),
          const SizedBox(height: 8),
          Text(
            "Adicione produtos usando o RFID",
            style: TextStyle(
              fontSize: 14,
              color: Colors.grey[500],
            ),
          ),
        ],
      ),
    );
  }

  // This will have to be deleted after adding images
  IconData _getProductIcon(String productName) {
    switch (productName.toLowerCase()) {
      case 'maçã':
      case 'maca':
        return Icons.apple;
      case 'banana':
        return Icons.eco;
      case 'leite':
        return Icons.local_drink;
      default:
        return Icons.shopping_bag;
    }
  }
}
