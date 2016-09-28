using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Security.Cryptography.Xml;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;

namespace GPS.Components.Cryptography.Xades
{
    public class Signer
    {
        //public XmlDocument SignDocument(X509Certificate2 cert, XmlDocument toSign)
        //{
        //    toSign.PreserveWhitespace = true;
        //    SignaturePropertiesSignedXml signer = new SignaturePropertiesSignedXml(toSign);
        //    signer.Signature.Id = "GPSOFT-Signature";

        //    signer.SigningKey = cert.PrivateKey;
        //    signer.KeyInfo = GetKeyInfo(signer, cert);
          
        //    //signer.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigExcC14NTransformUrl;
        //    signer.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigC14NTransformUrl;
            
        //    AddXAdESNodes(signer, toSign, cert);

        //    signer.ComputeSignature();            

        //    XmlElement signature = signer.GetXml();
        //    foreach (XmlNode node in signature.SelectNodes(
        //        "descendant-or-self::*[namespace-uri()='http://www.w3.org/2000/09/xmldsig#']"))
        //    {
        //        node.Prefix = "ds";
        //    }

        //    toSign.DocumentElement.AppendChild(toSign.ImportNode(signature, true));
        //    return toSign;
        //}

        //private void AddXAdESNodes(SignedXml signedXml, XmlDocument document, X509Certificate2 cert)
        //{
        //    System.Security.Cryptography.Xml.DataObject dataObject = new System.Security.Cryptography.Xml.DataObject();
        //    dataObject.Id = "Data";

        //    XmlElement result = document.CreateElement("xades", "QualifyingProperties", "http://uri.etsi.org/01903/v1.3.2#");
        //    result.SetAttribute("Target", "#" + signedXml.Signature.Id);
        //    result.SetAttribute("Id", "QualifyingProperties-1");
            
        //    XmlElement signed = document.CreateElement("xades", "SignedProperties", "http://uri.etsi.org/01903/v1.3.2#");
        //    signed.SetAttribute("Id", "SignedProperties-1");
        //    result.AppendChild(signed);

        //    XmlElement signed1 = document.CreateElement("xades", "SignedSignatureProperties", "http://uri.etsi.org/01903/v1.3.2#");
        //    signed.AppendChild(signed1);

        //    XmlElement timestamp = document.CreateElement("xades", "SigningTime", "http://uri.etsi.org/01903/v1.3.2#");
        //    timestamp.InnerText = GetDateTimeToCanonicalRepresentation();
        //    signed1.AppendChild(timestamp);

        //    //Add SigningCertificate
        //    XmlElement certificate1 = document.CreateElement("xades", "SigningCertificate", "http://uri.etsi.org/01903/v1.3.2#");
        //    XmlElement certifi2 = document.CreateElement("xades", "Cert", "http://uri.etsi.org/01903/v1.3.2#");
        //    XmlElement certifi3 = document.CreateElement("xades", "CertDigest", "http://uri.etsi.org/01903/v1.3.2#");

        //    XmlElement digestmethod = document.CreateElement("ds", "DigestMethod", SignedXml.XmlDsigNamespaceUrl);
        //    XmlElement digestValue = document.CreateElement("ds", "DigestValue", "http://uri.etsi.org/01903/v1.3.2#");
        //    //digestValue.InnerText = "2zSIHM3+EvfgdK/cF6QvQMgnDjo=";
        //    //digestValue.InnerText = "KnaO6baiX25dm1PzChHMDeA7KYQ=";
        //    digestValue.InnerText = GetDigestValue(cert);
        //    certifi3.AppendChild(digestmethod);
        //    certifi3.AppendChild(digestValue);

        //    XmlElement IssuerSerial = document.CreateElement("xades", "IssuerSerial", "http://uri.etsi.org/01903/v1.3.2#");

        //    XmlElement issuarName = document.CreateElement("ds", "x509IssuerName", "http://uri.etsi.org/01903/v1.3.2#");
        //    issuarName.InnerText = cert.Issuer; //"CN=Certum Level III CA, OU=Certum Certification Authority, O=Unizeto Technologies S.A., C=PL";
        //    XmlElement serialNumber = document.CreateElement("ds", "x509SerialNumber", "http://uri.etsi.org/01903/v1.3.2#");
        //    serialNumber.InnerText = cert.SerialNumber;
        //    IssuerSerial.AppendChild(issuarName);
        //    IssuerSerial.AppendChild(serialNumber);

        //    certifi2.AppendChild(certifi3);
        //    certificate1.AppendChild(certifi2);
        //    certificate1.AppendChild(IssuerSerial);
        //    signed1.AppendChild(certificate1);

        //    Signer.CreatePropertiesNode(document, (SignaturePropertiesSignedXml)signedXml);
        //    //XmlNamespaceManager nSpace = new XmlNamespaceManager(document.NameTable);
        //    //nSpace.AddNamespace("ds", SignedXml.XmlDsigNamespaceUrl);
        //    //XmlElement signNode = (XmlElement)result.SelectSingleNode("ds:Signature", nSpace);
        //    //SamlSignedXml sx = new SamlSignedXml((XmlElement)document.DocumentElement, "SignedProperties-1");
        //    //sx.LoadXml((XmlElement)signNode);
        //    ////sx.SigningKey = cert.PrivateKey;
        //    ////sx.AddReference(new Reference("#SignedProperties-1")); // Sign this node
        //    //sx.ComputeSignature();

        //    //XmlElement signature = sx.GetXml();

        //   // dataObject.Data = result.SelectNodes(".");
        //   // //dataObject.Data = result.ChildNodes;
        //   // signedXml.AddObject(dataObject);
        //   // //dataObject.Id = "Object-1";

        //   // Reference reference = new Reference("");
        //   // //reference.Id = "SignedProperties-Reference0";
        //   // reference.Type = "http://uri.etsi.org/01903#SignedProperties";
        //   // reference.Uri = "#SignedProperties-1";
        //   // //reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
        //   //// reference.AddTransform(new XmlDsigC14NTransform());
        //   // //reference.AddTransform(new XmlDsigExcC14NTransform());
        //   //// reference.Uri = "#xades:SignedProperties";
        //   // //reference.Uri = "#" + "QualifyingProperties-1";
        //   // //reference.AddTransform(new XmlDsigExcC14NTransform());
        //   // //reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());

        //   // signedXml.AddReference(reference);            
        //}

        //private static XmlElement CreatePropertiesNode(XmlDocument document, SignaturePropertiesSignedXml signedXml)
        //{
        //    var dataObject = new DataObject();
        //    //var nodeSignatureProperties = document.CreateElement("SignatureProperties", SignedXml.XmlDsigNamespaceUrl);
        //    //nodeSignatureProperties.SetAttribute("Id", "SignedProperties-1");
        //    XmlNamespaceManager nSpace = new XmlNamespaceManager(document.NameTable);
        //    nSpace.AddNamespace("xades", SignedXml.XmlDsigNamespaceUrl);
        //    dataObject.Data = document.SelectNodes("./xades:SignedProperties", nSpace);
        //    signedXml.AddObject(dataObject);

        //    var referenceToProperties = new Reference
        //    {
        //        Uri = "#" + "SignedProperties-1",
        //        Type = "http://uri.etsi.org/01903#SignedProperties"
        //    };
        //    signedXml.AddReference(referenceToProperties);
        //    return null;
        //}

        private string GetDateTimeToCanonicalRepresentation()
        {
            var ahora = DateTime.Now.ToUniversalTime();
            return ahora.Year.ToString("0000") + "-" + ahora.Month.ToString("00") +
                   "-" + ahora.Day.ToString("00") +
                   "T" + ahora.Hour.ToString("00") + ":" +
                   ahora.Minute.ToString("00") + ":" + ahora.Second.ToString("00") +
                   "Z";
        }

        private KeyInfo GetKeyInfo(SignedXml signer, X509Certificate2 cert)
        {
            KeyInfo keyInfo = new KeyInfo();
            //keyInfo.Id = "MojeId";
            KeyInfoX509Data keyInfoClause = new KeyInfoX509Data(cert);
            keyInfoClause.AddSubjectName(cert.SubjectName.Name);
            keyInfo.AddClause(keyInfoClause);
            return keyInfo;
        }

        private string GetDigestValue(X509Certificate2 cert)
        {
            SHA1 sha = new SHA1CryptoServiceProvider();
            byte[] hash;
            hash = sha.ComputeHash(cert.RawData);

            return Convert.ToBase64String(hash);
        }

        public string signEracun(XmlDocument xml, X509Certificate2 certificate)
        {
            XmlDocument xmlDoc = xml;
            xmlDoc.PreserveWhitespace = true;            

            #region signing

            XAdESSignedXml signedXml = new XAdESSignedXml(xmlDoc);
            signedXml.Signature.Id = "SignatureId";

            #region object -> signatureProperties
            XmlElement signaturePropertiesRoot;
            XmlElement qualifyingPropertiesRoot;
            string URI = "http://uri.etsi.org/01903/v1.1.1#";
            qualifyingPropertiesRoot = xmlDoc.CreateElement("xds", "QualifyingProperties", URI);
            qualifyingPropertiesRoot.SetAttribute("Target", "#SignatureId");

            signaturePropertiesRoot = xmlDoc.CreateElement("xds", "SignedProperties", URI);
            signaturePropertiesRoot.SetAttribute("Id", "SignedPropertiesId");

            XmlElement SignedSignatureProperties = xmlDoc.CreateElement("xds", "SignedSignatureProperties", URI);

            XmlElement timestamp = xmlDoc.CreateElement("xds", "SigningTime", URI);
            timestamp.InnerText = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"); //2011-09-05T09:11:24.268Z
            SignedSignatureProperties.AppendChild(timestamp);

            XmlElement SigningCertificate = xmlDoc.CreateElement("xds", "SigningCertificate", URI);
            XmlElement Cert = xmlDoc.CreateElement("xds", "Cert", URI);
            XmlElement CertDigest = xmlDoc.CreateElement("xds", "CertDigest", URI);
            SHA1 cryptoServiceProvider = new SHA1CryptoServiceProvider();
            byte[] sha1 = cryptoServiceProvider.ComputeHash(certificate.RawData);
            XmlElement DigestMethod = xmlDoc.CreateElement("xds", "DigestMethod", URI); DigestMethod.SetAttribute("Algorithm", SignedXml.XmlDsigSHA1Url);
            XmlElement DigestValue = xmlDoc.CreateElement("xds", "DigestValue", URI); DigestValue.InnerText = Convert.ToBase64String(sha1);
            CertDigest.AppendChild(DigestMethod);
            CertDigest.AppendChild(DigestValue);
            Cert.AppendChild(CertDigest);

            XmlElement IssuerSerial = xmlDoc.CreateElement("xds", "IssuerSerial", URI);
            XmlElement X509IssuerName = xmlDoc.CreateElement("ds", "X509IssuerName", "http://www.w3.org/2000/09/xmldsig#"); X509IssuerName.InnerText = certificate.IssuerName.Name;
            XmlElement X509SerialNumber = xmlDoc.CreateElement("ds", "X509SerialNumber", "http://www.w3.org/2000/09/xmldsig#"); X509SerialNumber.InnerText = certificate.SerialNumber;
            IssuerSerial.AppendChild(X509IssuerName);
            IssuerSerial.AppendChild(X509SerialNumber);
            Cert.AppendChild(IssuerSerial);

            SigningCertificate.AppendChild(Cert);
            SignedSignatureProperties.AppendChild(SigningCertificate);

            signaturePropertiesRoot.AppendChild(SignedSignatureProperties);
            qualifyingPropertiesRoot.AppendChild(signaturePropertiesRoot);
            DataObject dataObject = new DataObject
            {
                Data = qualifyingPropertiesRoot.SelectNodes("."),
            };
            signedXml.AddObject(dataObject);
            #endregion

            // Add the key to the SignedXml document.
            signedXml.SigningKey = certificate.PrivateKey;

            KeyInfo keyInfo = new KeyInfo();
            KeyInfoX509Data keyInfoX509Data = new KeyInfoX509Data(certificate, X509IncludeOption.ExcludeRoot);
            keyInfo.AddClause(keyInfoX509Data);
            signedXml.KeyInfo = keyInfo;

            //Reference 1
            Reference reference2 = new Reference();

            reference2.Type = "http://www.gzs.si/shemas/eslog/racun/1.5#Racun";
            reference2.Uri = "#data";
            //reference2.AddTransform(new XmlDsigExcC14NTransform());
            signedXml.AddReference(reference2);

            //Reference 2
            reference2 = new Reference("#SignedPropertiesId");
            //reference2.Type = "http://uri.etsi.org/01903/v1.1.1#SignedProperties";
            //reference2.Uri = "#SignedPropertiesId";
            //Reference reference = new Reference("#XADES-Properties");
            reference2.Id = "SignatureUsuario-XADES-Properties-Ref";
            reference2.Type = XAdESSignedXml.XadesSignaturePropertiesNamespace;
            reference2.AddTransform(new XmlDsigExcC14NTransform());
            //reference2.AddTransform(new XmlDsigExcC14NTransform());
            //XmlDsigEnvelopedSignatureTransform trns = new XmlDsigEnvelopedSignatureTransform();

            //reference2.AddTransform(trns);

// XmlDsigXsltTransform t= new XmlDsigXsltTransform();
           
//reference2.AddTransform(t);          
            signedXml.AddReference(reference2);

            // Compute the signature.
            signedXml.ComputeSignature();

            // Get the XML representation of the signature and save
            // it to an XmlElement object.
            XmlElement xmlDigitalSignature = signedXml.GetXml();

            xmlDoc.DocumentElement.AppendChild(xmlDoc.ImportNode(xmlDigitalSignature, true));
            #endregion

            //check XML signature, return false if dont use transorm in seckont reference
            bool checkSign = signedXml.CheckSignature();

            return xmlDoc.OuterXml;

        }
    }
}
