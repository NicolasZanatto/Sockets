using System;
using System.Collections.Generic;
using System.Text;

namespace Common.InputModels
{
    public class AlterarLivroInputModel
    {
        public AlterarLivroInputModel(){}
        public AlterarLivroInputModel(long codigoLivro, string titulo, string autor, long numeroEdicao, long anoEdicao)
        {
            CodigoLivro = codigoLivro;
            Titulo = titulo;
            Autor = autor;
            NumeroEdicao = numeroEdicao;
            AnoEdicao = anoEdicao;
        }

        public long CodigoLivro { get; private set; }
        public string Titulo { get; private set; }
        public string Autor { get; private set; }
        public long NumeroEdicao { get; private set; }
        public long AnoEdicao { get; private set; }
    }
}
