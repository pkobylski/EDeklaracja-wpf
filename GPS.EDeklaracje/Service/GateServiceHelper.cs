using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Xml;

namespace GPS.EDeklaracje.Service
{
    public class GateServiceHelper
    {
        public GateService GateService
        {
            get;
            set;
        }

        public GateServiceHelper(TypKomunikacjiMF typKomunikacjiMF)
        {
            this.GateService = new GateService();

            if (typKomunikacjiMF == TypKomunikacjiMF.Testowy)
                this.GateService.Url = "https://test-bramka.edeklaracje.gov.pl/uslugi/dokumenty/";
            else if (typKomunikacjiMF == TypKomunikacjiMF.Oficjalny)
                this.GateService.Url = "https://bramka.e-deklaracje.mf.gov.pl/uslugi/dokumenty/";
        }

        public requestUPOResponse GetUpo(string refId)
        {
            return this.GateService.requestUPO(new requestUPO() { refId = refId });
        }

        //public static byte[] GetByteArray(object deklaracja)
        //{
        //    MemoryStream memory = new MemoryStream();
        //    XmlTextWriter writer = new XmlTextWriter(memory, Encoding.UTF8);
        //    writer.Formatting = Formatting.Indented;
        //    XmlSerializer serializer = new XmlSerializer(deklaracja.GetType());
        //    serializer.Serialize(writer, deklaracja);
        //    byte[] result = memory.ToArray();
        //    writer.Close();
        //    memory.Close();

        //    return result;
        //}
    }
}
