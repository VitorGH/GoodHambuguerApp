# Contexto do Desafio Técnico - C# Developer (Júnior) - GoodHambuguerAPI

## Visão Geral
* **Cenário**: Desenvolvimento de um sistema de registro de pedidos para a lanchonete "Good Hamburger".
* **Objetivo principal**: Construir uma aplicação que demonstre boas práticas de organização de código, modelagem do problema e decisões técnicas.

## Cardápio
### Sanduíches
* X Burger: R$ 5,00
* X Egg: R$ 4,50
* X Bacon: R$ 7,00

### Acompanhamentos
* Batata frita: R$ 2,00
* Refrigerante: R$ 2,50

## Regras de Negócio e Descontos
* **Regra 1 (20% de desconto)**: Pedidos contendo Sanduíche + batata + refrigerante.
* **Regra 2 (15% de desconto)**: Pedidos contendo Sanduíche + refrigerante.
* **Regra 3 (10% de desconto)**: Pedidos contendo Sanduíche + batata.
* **Validação de Quantidade**: Cada pedido pode conter apenas um sanduíche, uma batata e um refrigerante.
* **Validação de Duplicidade**: A tentativa de adicionar itens duplicados deve ser bloqueada, retornando uma mensagem de erro clara para o cliente.

## Requisitos Técnicos Obrigatórios
* **Tecnologia Base**: Construir uma API REST utilizando C# com .NET/ASP.NET Core.
* **Gestão de Pedidos (CRUD)**: Implementar endpoints para criar, listar, consultar por identificador (ID), atualizar e remover pedidos.
* **Cálculos Matemáticos**: O sistema deve calcular de forma exata o subtotal, o desconto aplicado e o valor total final de cada pedido, respeitando estritamente as regras de negócio descritas acima.
* **Tratamento de Exceções**: A API deve validar erros operacionais e de negócio, retornando respostas HTTP condizentes e claras (ex.: conflito de itens duplicados, pedido em formato inválido ou recurso não encontrado).
* **Gestão do Cardápio**: É necessário expor um endpoint específico para consultar as opções do cardápio.

## Diferenciais (Opcionais)
* Construir um frontend em Blazor consumindo os endpoints da API desenvolvida.
* Criar testes automatizados para garantir o funcionamento correto das regras de negócio.

## Entregáveis e Prazos
* **Repositório**: O código-fonte final deve ser disponibilizado em um repositório Git público.
* **Documentação (README)**: O repositório deve conter um arquivo README com instruções claras de como executar a aplicação, explicações sobre as decisões de arquitetura tomadas e os pontos que porventura foram deixados fora do escopo.
* **Prazo Sugerido**: 7 dias corridos.