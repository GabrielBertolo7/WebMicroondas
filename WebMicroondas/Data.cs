using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;

namespace WebMicroondas
{
    public class Data
    {
        private string connectionString;

        public Data()
        {
            connectionString = ConfigurationManager.ConnectionStrings["Microondas"].ConnectionString;
        }

        public void SaveProgramasAdicionados(AquecimentoPreDefinido programaCustomizado)
        {
            if (programaCustomizado == null)
            {
                throw new ArgumentNullException(nameof(programaCustomizado), "Programa não pode ser nulo.");
            }

            const string query = @"INSERT INTO ProgramasAquecimento (Nome, Alimento, Potencia, Tempo, StringAquecimento, Instrucoes)
                VALUES (@Nome, @Alimento, @Potencia, @Tempo, @StringAquecimento, @Instrucoes)";

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Nome", programaCustomizado.Nome);
                        cmd.Parameters.AddWithValue("@Alimento", programaCustomizado.Alimento);
                        cmd.Parameters.AddWithValue("@Potencia", programaCustomizado.Potencia);
                        cmd.Parameters.AddWithValue("@Tempo", programaCustomizado.Tempo);
                        cmd.Parameters.AddWithValue("@StringAquecimento", programaCustomizado.StringAquecimento);
                        cmd.Parameters.AddWithValue("@Instrucoes", programaCustomizado.Instrucoes);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("Erro ao salvar programa no banco de dados: " + sqlEx.Message, sqlEx);
            }
        }

        public List<AquecimentoPreDefinido> GetProgramasAdicionados()
        {
            var programasCustomizados = new List<AquecimentoPreDefinido>();

            const string query = "SELECT * FROM ProgramasAquecimento";

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            programasCustomizados.Add(new AquecimentoPreDefinido
                            {
                                Nome = reader["Nome"].ToString(),
                                Alimento = reader["Alimento"].ToString(),
                                Potencia = reader["Potencia"] != DBNull.Value ? Convert.ToInt32(reader["Potencia"]) : 0,
                                Tempo = reader["Tempo"] != DBNull.Value ? Convert.ToInt32(reader["Tempo"]) : 0,
                                StringAquecimento = reader["StringAquecimento"].ToString(),
                                Instrucoes = reader["Instrucoes"].ToString()
                            });
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("Erro ao ler programas adicionados do banco de dados: " + sqlEx.Message, sqlEx);
            }

            return programasCustomizados;
        }
    }
}
