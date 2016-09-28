using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Pkcs;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using GPS.Components.Cryptography.Xades;

namespace GPS.Components.Cryptography.EDeklaracje
{
    public class EDeklaracja
    {
        public static X509Certificate2 WybierzCertyfikat(string tytul, string opis)
        {
            X509Store x509Store = new X509Store("MY", StoreLocation.CurrentUser);
            x509Store.Open(OpenFlags.OpenExistingOnly);
            X509Certificate2Collection certificates = x509Store.Certificates;
            X509Certificate2Collection certificates2 = certificates.Find(X509FindType.FindByTimeValid, DateTime.Now, false);
            X509Certificate2Collection x509Certificate2Collection = X509Certificate2UI.SelectFromCollection(certificates2, tytul, opis, X509SelectionFlag.SingleSelection, IntPtr.Zero);
            if (x509Certificate2Collection.Count > 0)
            {
                return x509Certificate2Collection[0];
            }
            return null;
        }

        public static XmlDocument PodpiszXades(XmlDocument doc, X509Certificate2 cert)
        {
            Signer signer = new Signer();

            //XmlDocument doc1 = new XmlDocument();
            //doc.LoadXml("<?xml version=\"1.0\" encoding=\"UTF-8\"?><documento id=\"documento\"><titulo id=\"titulo\">Documento de pruebas</titulo><descripcion id=\"descripcion\">Documento destinado a realizar pruebas de firma</descripcion></documento>");

            Signature sig = new Signature();            

            XmlDocument signed = sig.SignDocumentXAdES(cert, doc);
            //sig.Validate(signed);

            //string p = signer.signEracun(doc1, cert);

            return signed;
            //return signer.SignDocument(cert, doc);
        }

        public static byte[] PodpiszPKCS7(byte[] content, CmsSigner signer)
        {
            ContentInfo contentInfo = new ContentInfo(content);
            SignedCms signedCms = new SignedCms(contentInfo);
            signedCms.ComputeSignature(signer, false);
            return signedCms.Encode();
        }

        public static void PodpiszPKCS7(string fileName, string signedFileName, CmsSigner signer)
        {
            byte[] array;
            using (FileStream fileStream = new FileStream(fileName, FileMode.Open))
            {
                array = new byte[fileStream.Length];
                fileStream.Read(array, 0, array.Length);
                fileStream.Close();
            }
            array = EDeklaracja.PodpiszPKCS7(array, signer);
            using (FileStream fileStream2 = new FileStream(signedFileName, FileMode.Create))
            {
                fileStream2.Write(array, 0, array.Length);
                fileStream2.Close();
            }
        }

        public static byte[] GetByte(object obj)
        {
            MemoryStream memory = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(memory, Encoding.UTF8);
            writer.Formatting = Formatting.Indented;
            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            serializer.Serialize(writer, obj);
            byte[] buffer = memory.ToArray();
            writer.Close();
            memory.Close();

            return buffer;
        }

        public static byte[] GetByte64(object obj)
        {
            MemoryStream memory = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(memory, Encoding.UTF8);
            writer.Formatting = Formatting.Indented;
            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            serializer.Serialize(writer, obj);
            byte[] buffer = memory.ToArray();
            writer.Close();
            memory.Close();

            string b64 = System.Convert.ToBase64String(buffer);
            byte[] result = System.Convert.FromBase64String(b64);

            return result;
        }

        public static byte[] GetByte64(object obj, ref string xml)
        {
            MemoryStream memory = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(memory, Encoding.UTF8);
            writer.Formatting = Formatting.Indented;
            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            serializer.Serialize(writer, obj);
            xml = Encoding.UTF8.GetString(memory.ToArray());
            byte[] buffer = memory.ToArray();
            writer.Close();
            memory.Close();

            string b64 = System.Convert.ToBase64String(buffer);
            byte[] result = System.Convert.FromBase64String(b64);

            return result;
        }

        public static byte[] GetByte64(byte[] byteArray)
        {                        
            string b64 = System.Convert.ToBase64String(byteArray);
            byte[] result = System.Convert.FromBase64String(b64);

            return result;
        }        

        public static bool ValidateXmlWithXsd(string xml, string xsdUri)
        {
            try
            {
                XmlReaderSettings xmlSettings = new XmlReaderSettings();
                xmlSettings.Schemas = new System.Xml.Schema.XmlSchemaSet();
                xmlSettings.Schemas.Add("http://crd.gov.pl/wzor/2011/12/19/732/", XmlReader.Create(xsdUri));
                xmlSettings.ValidationType = ValidationType.Schema;                
                XmlReader rdr = XmlReader.Create(new StringReader(xml), xmlSettings);

                // Parse the file.
                while (rdr.Read()) ;

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
