<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="WebMicroondas.index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Microondas</title>
    <style type="text/css">
        #form1 {
            width: 356px;
            height: 237px;
            font-size: 46px;
        }
        body {
            height: 294px;
            width: 936px;
            margin: 0 auto;
            padding: 0;
            background-color: lightgray; 
        }
        .small-label {
            font-size: 12px;
            margin-right: 10px;
        }
        .small-textbox {
            font-size: 12px;
        }
        .programa-customizado {
            font-style: italic;
        }
    </style>
    <script type="text/javascript">
        var timer;
        var tempoSelecionado;
        var tempoRestante;

        function startTimer(initialTempoSelecionado, useRestante) {
            if (useRestante && tempoRestante) {
                tempoSelecionado = tempoRestante;
                tempoRestante = 0;
            } else {
                tempoSelecionado = parseInt(initialTempoSelecionado, 10);
            }

            function atualizarTempo() {
                var tempoLabel = document.getElementById('<%= Tempo_Label.ClientID %>');
                var hiddenField = document.getElementById('<%= TempoHiddenField.ClientID %>');
                var minutos = Math.floor(tempoSelecionado / 60);
                var segundos = tempoSelecionado % 60;
                tempoLabel.innerHTML = minutos.toString().padStart(2, '0') + '.' + segundos.toString().padStart(2, '0');
                hiddenField.value = minutos.toString().padStart(2, '0') + '.' + segundos.toString().padStart(2, '0');

                if (tempoSelecionado > 0) {
                    tempoSelecionado--;
                    timer = setTimeout(atualizarTempo, 1000);
                } else {
                    tempoLabel.innerHTML = "Aquecimento concluído";
                    clearTimeout(timer);
                }
            }

            if (timer) {
                clearTimeout(timer);
            }

            atualizarTempo();
        }

        function pauseTimer() {
            clearTimeout(timer);
            tempoRestante = tempoSelecionado;
        }

        function resumeTimer() {
            if (tempoRestante > 0) {
                startTimer(tempoRestante, true);
            }
        }
    </script>
</head>
<body>
    <h1>WebMicroondas</h1>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />
        
        <asp:Label ID="Tempo_Label" runat="server" Text="00.00" style="margin-left: 102px"></asp:Label> <br /> 
        <asp:Button ID="Limpa_Button" runat="server" Text="LIMPA" style="margin-left: 102px" Width="91px" OnClick="Limpa_Button_Click" /> <br />
        <asp:Button ID="Numero1_Button" runat="server" Text="1" Width="90px" OnClick="NumeroButton_Click"/>
        <asp:Button ID="Numero2_Button" runat="server" Text="2" Width="90px" OnClick="NumeroButton_Click"/>
        <asp:Button ID="Numero3_Button" runat="server" Text="3" Width="90px" OnClick="NumeroButton_Click"/> <br />
    
        <asp:Button ID="Numero4_Button" runat="server" Text="4" Width="90px" OnClick="NumeroButton_Click"/>
        <asp:Button ID="Numero5_Button" runat="server" Text="5" Width="90px" OnClick="NumeroButton_Click"/>
        <asp:Button ID="Numero6_Button" runat="server" Text="6" Width="90px" OnClick="NumeroButton_Click"/> <br />

        <asp:Button ID="Numero7_Button" runat="server" Text="7" Width="90px" OnClick="NumeroButton_Click"/>
        <asp:Button ID="Numero8_Button" runat="server" Text="8" Width="90px" OnClick="NumeroButton_Click"/>
        <asp:Button ID="Numero9_Button" runat="server" Text="9" Width="90px" OnClick="NumeroButton_Click"/> <br />
    
        <asp:Button ID="Numero0_Button" runat="server" Text="0" style="margin-left: 102px" Width="91px" OnClick="NumeroButton_Click"/> <br />
    
        <asp:Button ID="Ligar_Button" runat="server" Text="LIGAR" Width="90px" OnClick="Ligar_Button_Click1"/>
        <asp:Button ID="Pausar_Button" runat="server" Text="PAUSAR" Width="90px" OnClick="Pausar_Button_Click"/>
        <asp:Button ID="Cancelar_Button" runat="server" Text="CANCELAR" Width="90px" OnClick="Limpa_Button_Click"/> <br />

        <asp:Literal ID="Potencia_Literal" runat="server" Text="<span style='font-size: 14px;'>POTÊNCIA</span>" />
        <asp:TextBox ID="Potencia_TextBox" runat="server" Text="0" style="margin-left: 15px" Width="225px" Height="16px"></asp:TextBox>
        <asp:HiddenField ID="TempoHiddenField" runat="server" />

        <asp:DropDownList ID="ProgramasDropDownList" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ProgramasDropDownList_SelectedIndexChanged"></asp:DropDownList> <br />
        <asp:Label ID="Instrucoes_Label" runat="server" Text="" Style="font-size: 16px;"></asp:Label>

        <h3 style="font-size: 16px;">Cadastrar Novo Programa de Aquecimento</h3>
        <asp:Label ID="ErroCadastro_Label" runat="server" Text="Preencha todos os campos corretamente." CssClass="small-textbox"></asp:Label> <br />
        
        <label for="Nome_TextBox" class="small-label">Nome:</label>
        <asp:TextBox ID="Nome_TextBox" runat="server" CssClass="small-textbox" Width="169px"></asp:TextBox> <br />
        
        <label for="Alimento_TextBox" class="small-label">Alimento:</label>
        <asp:TextBox ID="Alimento_TextBox" runat="server" CssClass="small-textbox" Width="152px"></asp:TextBox> <br />
        
        <label for="PotenciaNovoPrograma_TextBox" class="small-label">Potência:</label>
        <asp:TextBox ID="PotenciaNovoPrograma_TextBox" runat="server" CssClass="small-textbox" Width="153px"></asp:TextBox> <br />
        
        <label for="Tempo_TextBox" class="small-label">Tempo:</label>
        <asp:TextBox ID="Tempo_TextBox" runat="server" CssClass="small-textbox" Width="162px"></asp:TextBox> <br />
        
        <label for="Caractere_TextBox" class="small-label">Caractere:</label>
        <asp:TextBox ID="Caractere_TextBox" runat="server" CssClass="small-textbox" Width="149px"></asp:TextBox> <br />
        
        <label for="Instrucoes_TextBox" class="small-label">Instruções:</label>
        <asp:TextBox ID="Instrucoes_TextBox" runat="server" CssClass="small-textbox" Width="143px"></asp:TextBox> <br />

        <asp:Button ID="Cadastrar_Button" runat="server" Text="Cadastrar Programa" OnClick="Cadastrar_Button_Click" />
    </form>
</body>
</html>
