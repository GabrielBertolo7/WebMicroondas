using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebMicroondas
{
    public partial class index : System.Web.UI.Page
    {
        private WebMicroondasServico webMicroondasServico;

        protected void Page_Load(object sender, EventArgs e)
        {
            //Garante que o código dentro do bloco será executado apenas uma vez
            if (!IsPostBack)
            {
                InicializarServico();
                InicializarViewState();
                AtualizarDropDownList();
                Instrucoes_Label.Text = "";
                ErroCadastro_Label.Visible = false;
            }
        }

        private void InicializarServico()
        {
            if (webMicroondasServico == null)
                webMicroondasServico = new WebMicroondasServico(new Data());
        }

        private void InicializarViewState()
        {
            ViewState["isIniciado"] = false;
            ViewState["tempoSelecionado"] = 0;
            ViewState["tempoRestante"] = 0;
            ViewState["isProgramasPreDefinidos"] = false;
            Tempo_Label.Text = "00.00";
            Potencia_TextBox.Text = "0";
            TempoHiddenField.Value = "00.00";
            ProgramasDropDownList.SelectedIndex = 0;
        }

        protected void NumeroButton_Click(object sender, EventArgs e)
        {
            //Não permite adicionar números quando algum programa pré-definido está selecionado.
            bool isProgramasPreDefinidos = ViewState["isProgramasPreDefinidos"] != null && (bool)ViewState["isProgramasPreDefinidos"];
            if (isProgramasPreDefinidos)
                return;

            Button clickedButton = sender as Button;
            if (clickedButton != null)
            {
                string numero = clickedButton.Text;
                if (Tempo_Label.Text[0] == '0')
                    AdicionarNumeroAoTextBoxTempo(numero);
            }
        }

        private void AdicionarNumeroAoTextBoxTempo(string numero)
        {
            string numeroAtual = Tempo_Label.Text.Replace(".", string.Empty);
            string novoNumero = numeroAtual + numero;
            novoNumero = novoNumero.Substring(novoNumero.Length - 4);
            novoNumero = novoNumero.Insert(2, ".");
            Tempo_Label.Text = novoNumero;
        }

        private string ConverterParaFormatoValidoDeHoras(string tempo)
        {
            //Adicionar o ponto decimal se o tempo não possuir
            if (!tempo.Contains('.'))
            {
                tempo = tempo.PadLeft(4, '0');
                tempo = tempo.Insert(2, ".");
            }

            string[] partes = tempo.Split('.');
            int minuto = int.Parse(partes[0]);
            int segundo = int.Parse(partes[1]);

            //Corrigir minutos e segundos para um formato válido
            minuto += segundo / 60;
            segundo %= 60;

            return $"{minuto:D2}.{segundo:D2}";
        }


        protected void Limpa_Button_Click(object sender, EventArgs e)
        {
            Tempo_Label.Text = "00.00";
            Potencia_TextBox.Text = "0";
            TempoHiddenField.Value = "00.00";

            InicializarViewState();
            Instrucoes_Label.Text = "";
            ProgramasDropDownList.SelectedIndex = 0;
        }

        protected void Ligar_Button_Click1(object sender, EventArgs e)
        {
            string tempo = Tempo_Label.Text;

            //Se o tempo estiver zerado, define o valor padrão
            if (Convert.ToInt32(tempo.Replace(".", "")) == 0)
            {
                tempo = "00.30";
                Tempo_Label.Text = "00.30";
            }

            //Se a potência estiver zerada, define o valor padrão
            if (string.IsNullOrEmpty(Potencia_TextBox.Text) || Potencia_TextBox.Text == "0")
                Potencia_TextBox.Text = "10";

            //Validar e converter o tempo
            if (Convert.ToInt32(tempo.Split('.')[0]) > 59 || Convert.ToInt32(tempo.Split('.')[1]) > 59)
                tempo = ConverterParaFormatoValidoDeHoras(tempo);

            //Validar se o tempo máximo (2 minutos) foi excedido, com exceção dos programas pré-definidos
            int tempoSelecionado = ConverterParaSegundos(tempo);
            bool isProgramasPreDefinidos = ViewState["isProgramasPreDefinidos"] != null && (bool)ViewState["isProgramasPreDefinidos"];
            if (tempoSelecionado > 120 && !isProgramasPreDefinidos)
            {
                Tempo_Label.Text = "<span style='font-size: 14px;'>Tempo máximo de 2 minutos. Por favor, insira um tempo válido.</span>";
                return;
            }
            else
                Tempo_Label.Text = tempo;

            //Atualizar o tempo restante corretamente
            if (ViewState["isIniciado"] != null && (bool)ViewState["isIniciado"])
            {
                tempoSelecionado = Convert.ToInt32(ConverterParaSegundos(TempoHiddenField.Value)) + 30;

                //Garantir que o tempo não exceda 2 minutos
                if (tempoSelecionado > 120)
                    tempoSelecionado = 120;
            }

            string potencia = Potencia_TextBox.Text.Trim();
            int potenciaValor;
            bool isNumero = int.TryParse(potencia, out potenciaValor);

            //Validar se a potência está em formato correto ou não excedeu o limite (10)
            if (!isNumero || potenciaValor < 1 || potenciaValor > 10)
            {
                //Se for um programa pré-definido ou cadastrado manualmente, ignora
                if (!isProgramasPreDefinidos)
                {
                    Potencia_TextBox.Text = "Insira um valor numérico entre 1 e 10.";
                    Tempo_Label.Text = "00.00";
                    return;
                }
            }
            else if (potenciaValor == 0)
                Potencia_TextBox.Text = "10";

            //Atualizar ViewState com o tempo atualizado
            AtualizarViewState(tempoSelecionado);

            //Passar o valor como número inteiro para JavaScript
            ScriptManager.RegisterStartupScript(this, GetType(), "CronometroJS", $"startTimer({tempoSelecionado}, false);", true);
        }

        private void AtualizarViewState(int tempoAtual)
        {
            ViewState["tempoSelecionado"] = tempoAtual;
            ViewState["tempoRestante"] = tempoAtual;
            ViewState["isIniciado"] = true;
        }

        protected void ProgramasDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Garantir que o serviço esteja inicializado
            InicializarServico();

            if (ProgramasDropDownList.SelectedIndex == 0)
                return;

            string programaSelecionado = ProgramasDropDownList.SelectedValue;

            //Seleciona também os programas que estão cadastrados no bando de dados
            var todosProgramas = webMicroondasServico.ObterTodosProgramas();
            var programa = todosProgramas.FirstOrDefault(p => p.Nome == programaSelecionado);

            if (programa != null)
            {
                Tempo_Label.Text = ConverterParaFormatoValidoDeHoras(programa.Tempo.ToString());
                Potencia_TextBox.Text = programa.Potencia.ToString();
                Instrucoes_Label.Text = programa.Instrucoes;

                ViewState["isProgramasPreDefinidos"] = true;
            }
        }

        private int ConverterParaSegundos(string tempo)
        {
            var partes = tempo.Split('.');
            int minutos = int.Parse(partes[0]);
            int segundos = int.Parse(partes[1]);
            return (minutos * 60) + segundos;
        }

        protected void Pausar_Button_Click(object sender, EventArgs e)
        {
            string tempo = TempoHiddenField.Value;
            Tempo_Label.Text = tempo;
            ViewState["tempoRestante"] = ConverterParaSegundos(tempo);
            ViewState["isIniciado"] = false;
            ScriptManager.RegisterStartupScript(this, GetType(), "pauseTimer", "pauseTimer();", true);
        }

        protected void Cadastrar_Button_Click(object sender, EventArgs e)
        {
            //Garantir que o serviço esteja inicializado
            InicializarServico();

            string nome = Nome_TextBox.Text.Trim();
            string alimento = Alimento_TextBox.Text.Trim();
            int potencia;
            int tempo;
            string caractere = Caractere_TextBox.Text.Trim();
            string instrucoes = Instrucoes_TextBox.Text.Trim();

            //Valida se os campos potência e tempo são válidos e apresenta mensagem de erro na tela
            if (!int.TryParse(PotenciaNovoPrograma_TextBox.Text.Trim(), out potencia) ||
                !int.TryParse(Tempo_TextBox.Text.Trim(), out tempo))
            {
                ErroCadastro_Label.Text = "Potência e Tempo devem ser números válidos.";
                ErroCadastro_Label.Visible = true;
                return;
            }

            //Valida se os campos foram em geral foram preenchidos corretamente e apresenta mensagem de erro na tela
            bool isValido = webMicroondasServico.ValidarCadastro(nome, alimento, potencia, tempo, caractere);
            if (!isValido)
            {
                ErroCadastro_Label.Text = "Preencha todos os campos corretamente.";
                ErroCadastro_Label.Visible = true;
                return;
            }

            //Valida se já existe programa cadastrado com o mesmo caractere e apresenta mensagem na tela
            bool caractereExistente = webMicroondasServico.CaractereExistente(caractere);
            if (caractereExistente)
            {
                ErroCadastro_Label.Text = "O caractere de aquecimento já existe.";
                ErroCadastro_Label.Visible = true;
                return;
            }

            //Cadastra um novo programa
            var novoPrograma = new AquecimentoPreDefinido
            {
                Nome = nome,
                Alimento = alimento,
                Potencia = potencia,
                Tempo = tempo,
                StringAquecimento = caractere,
                Instrucoes = instrucoes
            };

            webMicroondasServico.CadastrarPrograma(novoPrograma);
            LimparCamposCadastro();
            AtualizarDropDownList();

            ErroCadastro_Label.Text = "Programa cadastrado com sucesso!";
            ErroCadastro_Label.Visible = true;
        }

        private void LimparCamposCadastro()
        {
            Nome_TextBox.Text = string.Empty;
            Alimento_TextBox.Text = string.Empty;
            PotenciaNovoPrograma_TextBox.Text = string.Empty;
            Tempo_TextBox.Text = string.Empty;
            Caractere_TextBox.Text = string.Empty;
            Instrucoes_TextBox.Text = string.Empty;
        }

        private void AtualizarDropDownList()
        {
            var programas = webMicroondasServico.ObterTodosProgramas();
            ProgramasDropDownList.Items.Clear();
            ProgramasDropDownList.Items.Add(new ListItem("Selecione:", ""));
            if (programas != null)
            { 
                foreach (var programa in programas)
                {
                    ProgramasDropDownList.Items.Add(new ListItem(programa.Nome, programa.Nome));
                }
            }
        }
    }
}
