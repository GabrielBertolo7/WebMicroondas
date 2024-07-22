using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebMicroondas
{
    public class AquecimentoPreDefinido
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Alimento { get; set; }
        public int Tempo { get; set; }
        public int Potencia { get; set; }
        public string Instrucoes { get; set; }
        public string StringAquecimento { get; set; }
        public bool LiberarExcederTempoMaximo { get; set; }
    }
}