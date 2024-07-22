using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebMicroondas
{
    public class WebMicroondasServico
    {
        private Data DataAcesso;

        public WebMicroondasServico(Data dataAcesso)
        {
            DataAcesso = dataAcesso;
        }

        public List<AquecimentoPreDefinido> ObterTodosProgramas()
        {
            var programasPredefinidos = ObterProgramasPredefinidos();
            var programasCustomizados = DataAcesso.GetProgramasAdicionados();
            return programasPredefinidos.Concat(programasCustomizados).ToList();
        }

        public bool ValidarCadastro(string nome, string alimento, int potencia, int tempo, string caractere)
        {
            return !(string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(alimento) ||
                     potencia < 1 || tempo < 1 || string.IsNullOrWhiteSpace(caractere) || caractere == ".");
        }

        public bool CaractereExistente(string caractere)
        {
            var todosProgramas = ObterTodosProgramas();
            return todosProgramas.Any(p => p.StringAquecimento == caractere);
        }

        public void CadastrarPrograma(AquecimentoPreDefinido programa)
        {
            DataAcesso.SaveProgramasAdicionados(programa);
        }

        private List<AquecimentoPreDefinido> ObterProgramasPredefinidos()
        {
            return new List<AquecimentoPreDefinido>
        {
            new AquecimentoPreDefinido
            {
            Nome = "Pipoca",
            Alimento = "Pipoca (de micro-ondas)",
            Tempo = 180,
            Potencia = 7,
            Instrucoes = "Instruções: Observar o barulho de estouros do milho, caso houver um intervalo de mais de 10 segundos entre um estouro e outro, interrompa o aquecimento.",
            StringAquecimento = "PIPOCA",
            LiberarExcederTempoMaximo = true
            },

            new AquecimentoPreDefinido
            {
            Nome = "Leite",
            Alimento = "Leite",
            Tempo = 300,
            Potencia = 5,
            Instrucoes = "Instruções: Cuidado com aquecimento de líquidos, o choque térmico aliado ao movimento do recipiente pode causar fervura imediata causando risco de queimaduras.",
            StringAquecimento = "LEITE",
            LiberarExcederTempoMaximo = true
            },

            new AquecimentoPreDefinido
            {
            Nome = "Carnes de boi",
            Alimento = "Carne em pedaço ou fatias",
            Tempo = 840,
            Potencia = 4,
            Instrucoes = "Instruções: Interrompa o processo na metade e vire o conteúdo com a parte de baixo para cima para o descongelamento uniforme.",
            StringAquecimento = "CARNE",
            LiberarExcederTempoMaximo = true
            },

            new AquecimentoPreDefinido
            {
            Nome = "Frango",
            Alimento = "Frango (qualquer corte)",
            Tempo = 480,
            Potencia = 7,
            Instrucoes = "Instruções: Interrompa o processo na metade e vire o conteúdo com a parte de baixo para cima para o descongelamento uniforme.",
            StringAquecimento = "FRANGO",
            LiberarExcederTempoMaximo = true
            },

            new AquecimentoPreDefinido
            {
            Nome = "Feijão",
            Alimento = "Feijão congelado",
            Tempo = 480,
            Potencia = 9,
            Instrucoes = "Instruções: Deixe o recipiente destampado e em casos de plástico, cuidado ao retirar o recipiente pois o mesmo pode perder resistência em altas temperaturas.",
            StringAquecimento = "FEIJAO",
            LiberarExcederTempoMaximo = true
            }
        };
        }
    }
}