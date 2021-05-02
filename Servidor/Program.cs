using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;
using Common.Models;
using Common.InputModels;
using System.Text.Json;

namespace Sockets
{
    class Program
    {
        static void Main(string[] args)
        {
            //databaseConnection.ConsultarLivroPorNomeAutor("James");

            TcpListener servidor = new TcpListener(8888);

            servidor.Start();
            Console.WriteLine("Servidor Inicializado");

            TcpClient conversacliente = servidor.AcceptTcpClient();
            Console.WriteLine("Cliente Conectou!!");

            NetworkStream saida = conversacliente.GetStream();
            BinaryReader recebe = new BinaryReader(saida);
            BinaryWriter envia = new BinaryWriter(saida);

            //string msg = recebe.ReadString();
            //Console.WriteLine("Recebi a mensagem: " + msg);

            var continuar = true;

            DatabaseConnection databaseConnection = new DatabaseConnection();

            while (continuar)
            {
                var opcoes = $@"
                            --------------------------------------------------
                            Selecione a operação desejada
                            1 - Criar Livro
                            2 - Consultar Livro
                            3 - Consultar por ano e número da edição
                            4 - Remover Livro
                            5 - Alterar Livro
                            0 - Sair
                            ---------------------------------------------------
                            Digite a opção:";

                envia.Write(opcoes);
                Console.WriteLine("Opções Enviadas");

                var opcaoEscolhida = (eOpcaoEscolhida)recebe.ReadInt32();
                var consulta = string.Empty;
                Console.WriteLine($"Opção Escolhida: {opcaoEscolhida}");

                switch (opcaoEscolhida)
                {
                    case eOpcaoEscolhida.CriarLivro:
                        CriarLivro(envia, recebe, databaseConnection);
                        break;
                    case eOpcaoEscolhida.ConsultarLivro:
                        ConsultarLivro(envia, recebe, databaseConnection);
                        break;
                    case eOpcaoEscolhida.ConsultarPorAnoNumeroEdicao:
                        ConsultarPorAnoNumeroEdicao(envia, recebe, databaseConnection);
                        break;
                    case eOpcaoEscolhida.RemoverLivro:
                        RemoverLivroPorNome(envia, recebe, databaseConnection);
                        break;
                    case eOpcaoEscolhida.AlterarLivro:
                        AlterarLivro(envia, recebe, databaseConnection);
                        break;
                    case eOpcaoEscolhida.Sair:
                        continuar = false;
                        break;
                }
            }
            saida.Close();
            recebe.Close();
            envia.Close();
            conversacliente.Close();
            servidor.Stop();

            Console.WriteLine("Digite qualquer tecla para sair!");
            Console.ReadKey();
        }

        public static void CriarLivro(BinaryWriter envia, BinaryReader recebe, DatabaseConnection databaseConnection)
        {
            var data = recebe.ReadString();
            var livro = JsonSerializer.Deserialize<CriarLivroInputModel>(data);
            var retorno = databaseConnection.CriarLivro(livro.Titulo, livro.Autor, livro.NumeroEdicao, livro.AnoEdicao);
            envia.Write(retorno);
        }

        public static void ConsultarLivro(BinaryWriter envia, BinaryReader recebe, DatabaseConnection databaseConnection)
        {
            var consulta = recebe.ReadString();
            var retorno = databaseConnection.ConsultarLivroPorNomeAutor(consulta);
            envia.Write(string.IsNullOrEmpty(retorno) ? "Nenhum dado foi encontrado." : retorno);
        }

        public static void ConsultarPorAnoNumeroEdicao(BinaryWriter envia, BinaryReader recebe, DatabaseConnection databaseConnection)
        {
            var consulta = recebe.ReadString();
            var data = JsonSerializer.Deserialize<ConsultarLivroPorAnoNumeroEdicaoInputModel>(consulta);
            var retorno = databaseConnection.ConsultarLivroPorAnoNumeroEdicao(data.Ano, data.Numero);
            envia.Write(string.IsNullOrEmpty(retorno) ? "Nenhum dado foi encontrado." : retorno);
        }
        public static void RemoverLivroPorNome(BinaryWriter envia, BinaryReader recebe, DatabaseConnection databaseConnection)
        {
            var nomeLivro = recebe.ReadString();
            var retorno = databaseConnection.RemoverLivro(nomeLivro);
            envia.Write(retorno);
        }
        public static void AlterarLivro(BinaryWriter envia, BinaryReader recebe, DatabaseConnection databaseConnection)
        {
            var consulta = recebe.ReadString();
            var data = JsonSerializer.Deserialize<AlterarLivroInputModel>(consulta);
            var retorno = databaseConnection.AlterarLivro(data.CodigoLivro, data.Titulo, data.NumeroEdicao, data.AnoEdicao);
            envia.Write(retorno);
        }
    }
}
