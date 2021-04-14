using System;
using System.Collections.Generic;
using System.Text;

namespace Common.InputModels
{
    public class ConsultarLivroPorAnoNumeroEdicaoInputModel
    {
        public ConsultarLivroPorAnoNumeroEdicaoInputModel(){}
        public ConsultarLivroPorAnoNumeroEdicaoInputModel(int ano, char numero)
        {
            Ano = ano;
            Numero = numero;
        }

        public int Ano { get; set; }
        public char Numero { get; set; }
    }
}
