using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sockets
{
    public class DatabaseConnection
    {
        private const string connString = "Host=localhost;Username=postgres;Password=12345;Database=livros";
        public string ConsultarLivroPorNomeAutor(string nome)
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                var query = $@"select 
                                    livros.codigo, livros.titulo, autor.nome as autor, edicao.numero as edicao, edicao.ano as ano 
                                from livros
                                inner join livroautor ON livroautor.codigolivro = livros.codigo
                                inner join autor ON autor.codigo = livroautor.codigoautor
                                inner join edicao ON edicao.codigolivro = livros.codigo
                                where UPPER(TRIM(autor.nome)) like UPPER(TRIM(@nome)) or UPPER(TRIM(livros.titulo)) like UPPER(TRIM(@nome))
                                order by livros.titulo";

                // Retrieve all rows
                StringBuilder stringBuilder = new StringBuilder();

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("nome", NpgsqlTypes.NpgsqlDbType.Varchar, $"%{nome}%");
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        stringBuilder.AppendLine($@"Código: {reader.GetInt32(0)}, Titulo:{reader.GetString(1)}, Autor:{reader.GetString(2)}, Edição:{reader.GetChar(3)}, Ano:{reader.GetInt32(4)}");
                    }
                }


                conn.Close();
                return stringBuilder.ToString();

            }
        }

        public string ConsultarLivroPorAnoNumeroEdicao(int ano, char numeroEdicao)
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                var query = $@"select 
                                    livros.codigo, livros.titulo, autor.nome as autor, edicao.numero as edicao, edicao.ano as ano 
                                from livros
                                inner join livroautor ON livroautor.codigolivro = livros.codigo
                                inner join autor ON autor.codigo = livroautor.codigoautor
                                inner join edicao ON edicao.codigolivro = livros.codigo
                                where edicao.numero = @numeroEdicao or edicao.ano = @anoEdicao
                                order by livros.titulo";

                // Retrieve all rows
                StringBuilder stringBuilder = new StringBuilder();

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("numeroEdicao", NpgsqlTypes.NpgsqlDbType.Char, numeroEdicao);
                    cmd.Parameters.AddWithValue("anoEdicao", NpgsqlTypes.NpgsqlDbType.Integer, ano);
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        stringBuilder.AppendLine($@"Código: {reader.GetInt32(0)}, Titulo:{reader.GetString(1)}, Autor:{reader.GetString(2)}, Edição:{reader.GetChar(3)}, Ano:{reader.GetInt32(4)}");
                    }
                }


                conn.Close();
                return stringBuilder.ToString();
            }
        }

        public string CriarLivro(string titulo, string autor, char numeroEdicao, int anoEdicao)
        {
            try
            {
                using (var conn = new NpgsqlConnection(connString))
                {
                    var proxCodigoLivro = 0;
                    var proxCodigoAutor = 0;
                    conn.Open();

                    using (var cmd = new NpgsqlCommand("select max(codigo)+1 from livros", conn))
                    {
                        var reader = cmd.ExecuteScalar();
                        if (reader != null)
                            proxCodigoLivro = Convert.ToInt32(reader);
                    }
                    using (var cmd = new NpgsqlCommand("select max(codigo)+1 from autor", conn))
                    {
                        var reader = cmd.ExecuteScalar();
                        if (reader != null)
                            proxCodigoAutor = Convert.ToInt32(reader);
                    }

                    if (proxCodigoLivro <= 0 || proxCodigoAutor <= 0)
                    {
                        conn.Close();
                        return "Livro Não Inserido";
                    }

                    using (var command = new NpgsqlCommand("INSERT INTO livros (codigo, titulo) VALUES (@codigo, @titulo)", conn))
                    {
                        command.Parameters.AddWithValue("codigo", proxCodigoLivro);
                        command.Parameters.AddWithValue("titulo", titulo);

                        int nRows = command.ExecuteNonQuery();
                    }

                    using (var command = new NpgsqlCommand("INSERT INTO edicao (codigolivro, numero, ano) VALUES (@codigo, @numero, @ano)", conn))
                    {
                        command.Parameters.AddWithValue("codigo", proxCodigoLivro);
                        command.Parameters.AddWithValue("numero", numeroEdicao);
                        command.Parameters.AddWithValue("ano", anoEdicao);

                        int nRows = command.ExecuteNonQuery();
                    }

                    using (var command = new NpgsqlCommand("INSERT INTO autor (codigo, nome) VALUES (@codigo, @nome)", conn))
                    {
                        command.Parameters.AddWithValue("codigo", proxCodigoLivro);
                        command.Parameters.AddWithValue("nome", autor);

                        int nRows = command.ExecuteNonQuery();
                    }

                    using (var command = new NpgsqlCommand("INSERT INTO livroautor (codigolivro, codigoautor) VALUES (@livro, @autor)", conn))
                    {
                        command.Parameters.AddWithValue("livro", proxCodigoLivro);
                        command.Parameters.AddWithValue("autor", proxCodigoAutor);

                        int nRows = command.ExecuteNonQuery();
                    }
                }

                return $@"Livro Inserido com Sucesso!";
            }
            catch (Exception ex)
            {
                return $"Ocorreu o seguindo erro ao inserir o livro:{ex.Message}";
            }
        }

        public string AlterarLivro(int codigoLivro, string titulo,string autor, char numeroEdicao, int anoEdicao)
        {
            return string.Empty;
        }

        public string RemoverLivro(int codigoLivro)
        {
            return string.Empty;
        }
    }
}
