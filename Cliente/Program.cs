using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;
using Common.Models;
using Common;
using Common.InputModels;
using System.Text.Json;
using Newtonsoft.Json;

namespace Cliente
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpClient cliente = new TcpClient("localhost", 8888);
            Console.WriteLine("Conectei no servidor!!");

            NetworkStream saida = cliente.GetStream();
            BinaryReader recebe = new BinaryReader(saida);
            BinaryWriter envia = new BinaryWriter(saida);

            //Console.WriteLine("Digite a mensagem: ");
            //string msg = Console.ReadLine();

            //Console.WriteLine("Vou enviar a mensagem: " + msg);
            //envia.Write(msg);

            //Console.WriteLine("Aguardando o ok do servidor");

            var continuar = true;
            while (continuar)
            {
                var opcoes = recebe.ReadString();

                Console.WriteLine(opcoes);

                var opcaoEscolhida = (eOpcaoEscolhida)Convert.ToInt32(Console.ReadLine());
                envia.Write(Convert.ToInt32(opcaoEscolhida));

                switch (opcaoEscolhida)
                {
                    case eOpcaoEscolhida.CriarLivro:
                        CriarLivro(recebe, envia);
                        break;
                    case eOpcaoEscolhida.ConsultarLivro:
                        ConsultarLivroPorAutor(recebe, envia);
                        break;
                    case eOpcaoEscolhida.ConsultarPorAnoNumeroEdicao:
                        ConsultarLivroPorAnoNumeroEdicao(recebe, envia);
                        break;
                    case eOpcaoEscolhida.RemoverLivro:
                        RemoverLivroPorNome(recebe, envia);
                        break;
                    case eOpcaoEscolhida.AlterarLivro:
                        AlterarLivro(recebe, envia);
                        break;
                    case eOpcaoEscolhida.Sair:
                        continuar = false;
                        break;
                }

            }

            saida.Close();
            recebe.Close();
            envia.Close();
            cliente.Close();

            Console.WriteLine("Digite qualquer tecla para sair!");
            Console.ReadKey();
        }

        public static void CriarLivro(BinaryReader recebe, BinaryWriter envia)
        {
            Console.WriteLine("Digite o Titulo:");
            var titulo = Console.ReadLine();
            Console.WriteLine("Digite o Autor:");
            var autor = Console.ReadLine();
            Console.WriteLine("Digite o Número:");
            var numero = Convert.ToChar(Console.ReadLine());
            Console.WriteLine("Digite o Ano:");
            var ano = Convert.ToInt32(Console.ReadLine());

            var livro = new CriarLivroInputModel(titulo, autor, numero, ano);
            var data = JsonConvert.SerializeObject(livro);

            envia.Write(data);
            Console.WriteLine("Resultado:");
            Console.WriteLine(recebe.ReadString());
        }

        public static void ConsultarLivroPorAutor(BinaryReader recebe, BinaryWriter envia)
        {
            Console.WriteLine("Digite a consulta:");
            envia.Write(Console.ReadLine());
            Console.WriteLine("Resultado:");
            Console.WriteLine(recebe.ReadString());
        }

        public static void ConsultarLivroPorAnoNumeroEdicao(BinaryReader recebe, BinaryWriter envia)
        {
            Console.WriteLine("Digite o ano:");
            var ano = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Digite o número da edição");
            var numero = Convert.ToChar(Console.ReadLine());
            var consulta = new ConsultarLivroPorAnoNumeroEdicaoInputModel(ano, numero);
            var data = JsonConvert.SerializeObject(consulta);

            envia.Write(data);
            Console.WriteLine("Resultado:");
            Console.WriteLine(recebe.ReadString());
        }

        public static void RemoverLivroPorNome(BinaryReader recebe, BinaryWriter envia)
        {
            Console.WriteLine("Digite o titulo:");
            var titulo = Console.ReadLine();
            envia.Write(titulo);
            Console.WriteLine("Resultado:");
            Console.WriteLine(recebe.ReadString());
        }
        public static void AlterarLivro(BinaryReader recebe, BinaryWriter envia)
        {
            Console.WriteLine("Digite o código do livro:");
            var codigo = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Digite o novo titulo do livro");
            var titulo = Console.ReadLine();
            Console.WriteLine("Digite o novo número da edição do livro");
            var edicao = Convert.ToChar(Console.ReadLine());
            Console.WriteLine("Digite o novo ano do livro");
            var ano = Convert.ToInt32(Console.ReadLine());

            var alterarLivro = new AlterarLivroInputModel(codigo, titulo, edicao, ano);
            var data = JsonConvert.SerializeObject(alterarLivro);

            envia.Write(data);
            Console.WriteLine("Resultado:");
            Console.WriteLine(recebe.ReadString());
        }
    }
}
