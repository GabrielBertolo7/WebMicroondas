using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Xml.Serialization;

namespace WebMicroondas.DataXML
{
    public class DataXml
    {
        private string filePath = HttpContext.Current.Server.MapPath("~/DataXML/WebMicroondasData.xml");

        public void SalvarProgramasAdicionados(AquecimentoPreDefinido programaCustomizado)
        {
            if (programaCustomizado == null)
            {
                throw new ArgumentNullException(nameof(programaCustomizado), "Programa não pode ser nulo.");
            }

            var programas = GetProgramasAdicionados();
            programas.Add(programaCustomizado);
            SerializeToXml(programas);
        }

        public List<AquecimentoPreDefinido> GetProgramasAdicionados()
        {
            if (!File.Exists(filePath))
            {
                return new List<AquecimentoPreDefinido>();
            }

            return DeserializeFromXml();
        }

        private void SerializeToXml(List<AquecimentoPreDefinido> programas)
        {
            var serializer = new XmlSerializer(typeof(List<AquecimentoPreDefinido>));
            using (var writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, programas);
            }
        }

        private List<AquecimentoPreDefinido> DeserializeFromXml()
        {
            var serializer = new XmlSerializer(typeof(List<AquecimentoPreDefinido>));
            using (var reader = new StreamReader(filePath))
            {
                return (List<AquecimentoPreDefinido>)serializer.Deserialize(reader);
            }
        }
    }
}
