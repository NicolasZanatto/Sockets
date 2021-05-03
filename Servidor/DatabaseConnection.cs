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
                                where edicao.numero = @numeroEdicao and edicao.ano = @anoEdicao
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
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction(IsolationLevel.Serializable))
                {
                    try
                    {
                        var proxCodigoLivro = 0;
                        var proxCodigoAutor = 0;
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
                            command.Parameters.AddWithValue("codigo", proxCodigoAutor);
                            command.Parameters.AddWithValue("nome", autor);

                            int nRows = command.ExecuteNonQuery();
                        }

                        using (var command = new NpgsqlCommand("INSERT INTO livroautor (codigolivro, codigoautor) VALUES (@livro, @autor)", conn))
                        {
                            command.Parameters.AddWithValue("livro", proxCodigoLivro);
                            command.Parameters.AddWithValue("autor", proxCodigoAutor);

                            int nRows = command.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return $"Ocorreu o seguinte erro ao inserir o livro:{ex.Message}";
                    }
                    finally
                    {
                        conn.Close();
                    }
                }

            }

            return $@"Livro Inserido com Sucesso!";

        }

        public string RemoverLivro(string titulo)
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction(IsolationLevel.Serializable))
                {
                    try
                    {
                        int codigoLivro = 0; 
                        using (var cmd = new NpgsqlCommand("select codigo from livros where TRIM(UPPER(titulo)) like TRIM(UPPER(@titulo))", conn))
                        {
                            cmd.Parameters.AddWithValue("titulo", $"%{titulo}%");
                            var reader = cmd.ExecuteScalar();
                            if (reader != null)
                                codigoLivro = Convert.ToInt32(reader);
                        }

                        if (codigoLivro == 0) return $@"Livro não encontrado";
                        using (var cmd = new NpgsqlCommand("delete from edicao where codigolivro = @codigo", conn))
                        {
                            cmd.Parameters.AddWithValue("codigo", codigoLivro);
                            int rows = cmd.ExecuteNonQuery();
                        }

                        using (var cmd = new NpgsqlCommand("delete from livroautor where codigolivro = @codigo", conn))
                        {
                            cmd.Parameters.AddWithValue("codigo", codigoLivro);
                            int rows = cmd.ExecuteNonQuery();
                        }

                        using (var cmd = new NpgsqlCommand("delete from livros where codigo = @codigo", conn))
                        {
                            cmd.Parameters.AddWithValue("codigo", codigoLivro);
                            int rows = cmd.ExecuteNonQuery();
                        }
                        transaction.Commit();
                        return $@"Livro Removido com Sucesso";

                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return $"Ocorreu o seguinte erro ao executar a ação: {ex.Message}";
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }

        public string AlterarLivro(long codigoLivro, string titulo, char numeroEdicao, int anoEdicao)
        {

            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction(IsolationLevel.Serializable))
                {
                    try
                    {
                        using (var cmd = new NpgsqlCommand("update Edicao set numero = @numero, ano = @ano where codigolivro = @codigo", conn))
                        {
                            cmd.Parameters.AddWithValue("numero", NpgsqlTypes.NpgsqlDbType.Char, numeroEdicao);
                            cmd.Parameters.AddWithValue("ano", NpgsqlTypes.NpgsqlDbType.Integer, anoEdicao);
                            cmd.Parameters.AddWithValue("codigo", NpgsqlTypes.NpgsqlDbType.Integer,codigoLivro);
                            int rows = cmd.ExecuteNonQuery();
                            if (rows == 0) return $@"Livro Não Encontrado";
                        }

                        using (var cmd = new NpgsqlCommand("update livros set titulo = @titulo where codigo = @codigo", conn))
                        {
                            cmd.Parameters.AddWithValue("titulo", titulo);
                            cmd.Parameters.AddWithValue("codigo", NpgsqlTypes.NpgsqlDbType.Integer, codigoLivro);
                            int rows = cmd.ExecuteNonQuery();
                            if (rows == 0) return $@"Livro Não Encontrado";

                        }
                        transaction.Commit();
                        return $@"Livro Atualizado com Sucesso";

                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return $"Ocorreu o seguinte erro ao executar a ação: {ex.Message}";
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }
    }
}
