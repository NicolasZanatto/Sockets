using System;
using System.Collections.Generic;
using System.Text;

namespace Common.InputModels
{
    public class AlterarLivroInputModel
    {
        public AlterarLivroInputModel(){}
        public AlterarLivroInputModel(long codigoLivro, string titulo, char numeroEdicao, int anoEdicao)
        {
            CodigoLivro = codigoLivro;
            Titulo = titulo;
            NumeroEdicao = numeroEdicao;
            AnoEdicao = anoEdicao;
        }

        public long CodigoLivro { get; set; }
        public string Titulo { get; set; }
        public char NumeroEdicao { get; set; }
        public int AnoEdicao { get; set; }
    }
}
