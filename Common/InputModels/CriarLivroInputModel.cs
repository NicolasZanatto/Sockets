
using System;

namespace Common.InputModels
{
    public class CriarLivroInputModel
    {
        public CriarLivroInputModel(){}
        public CriarLivroInputModel(string titulo, string autor, char numeroEdicao, int anoEdicao)
        {
            Titulo = titulo;
            Autor = autor;
            NumeroEdicao = numeroEdicao;
            AnoEdicao = anoEdicao;
        }

        public string Titulo { get; set; }
        public string Autor { get; set; }
        public char NumeroEdicao { get; set; }
        public int AnoEdicao { get; set; }
    }
}
