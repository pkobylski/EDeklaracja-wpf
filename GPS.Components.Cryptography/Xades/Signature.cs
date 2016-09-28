using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Security.Cryptography.Xml;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using GPS.Components.Cryptography.Xades;

namespace GPS.Components.Cryptography.Xades
{
    class Signature
    {
        private const string _xadesNamespaceUrl = "http://uri.etsi.org/01903/v1.3.2#";

        public void Validate(XmlDocument document)
        {
             XmlNamespaceManager nsMgr = new XmlNamespaceManager(document.NameTable);
             nsMgr.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");

             //Cargamos el XML en el objeto SignedXML
             SignedXml verifier = new SignedXml(document);      

             //Obtenemos el nodo de la firma y se lo proporcionamos al objeto SignedXml para poder realizar la validación
             XmlElement signatureElement = document.DocumentElement.SelectSingleNode("//ds:Signature", nsMgr) as XmlElement;
             verifier.LoadXml(signatureElement);
             
             //Realizamos la comprobación
             if (!verifier.CheckSignature())
                 throw new Exception();
        }

        public X509Certificate2 GetCertificateFromStore(StoreLocation storeLocation, 
            StoreName storeName, string thumbprint)
        {
            //X509Store store = new X509Store(storeName, storeLocation);
            //store.Open(OpenFlags.ReadOnly);
            //X509Certificate2Collection certs =
            //    store.Certificates.Find(X509FindType.FindByThumbprint,
            //    thumbprint, false);
            //store.Close();
            //return certs[0];

            X509Store x509Store = new X509Store("MY", StoreLocation.CurrentUser);
            x509Store.Open(OpenFlags.OpenExistingOnly);
            X509Certificate2Collection certificates = x509Store.Certificates;
            X509Certificate2Collection certificates2 = certificates.Find(X509FindType.FindByTimeValid, DateTime.Now, false);
            X509Certificate2Collection x509Certificate2Collection = X509Certificate2UI.SelectFromCollection(certificates2, "","", X509SelectionFlag.SingleSelection, IntPtr.Zero);
            //if (x509Certificate2Collection.Count > 0)
            //{
                return x509Certificate2Collection[0];
            //}
        }

        public XmlDocument SignDocument(X509Certificate2 cert, XmlDocument doc)
        {
            SignedXml signer = new SignedXml(doc);

            //Set the key to sign
            signer.SigningKey = cert.PrivateKey;
            signer.KeyInfo = getKeyInfo(signer, cert);

            //Set nodes idetifiers
            signer.Signature.Id = "GpSoftSignature";
            signer.KeyInfo.Id = "KeyInfo";

            //Set canonicalization method for signature
            signer.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigExcC14NTransformUrl;

            //Set the references to sign
            signer.AddReference(setCertificationReference(signer));

            //Compute signature
            signer.ComputeSignature();

            //return the Signed Xml
            doc.DocumentElement.AppendChild(doc.ImportNode(signer.GetXml(), true));
            return doc;
        }

        public XmlDocument SignDocumentXAdES(X509Certificate2 cert, XmlDocument toSign)
        {
            XAdESSignedXml signer = new XAdESSignedXml(toSign);

            //Set the key to sign
            signer.SigningKey = cert.PrivateKey;
            signer.KeyInfo = getKeyInfo(signer, cert);

            //Set nodes idetifiers
            signer.Signature.Id = "GpSoftSignature";
            signer.KeyInfo.Id = "KeyInfo";

            //Set canonicalization method for signature
            signer.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigExcC14NTransformUrl;

            //Add XAdES node
            addXAdESNodes(signer, toSign, cert);

            //Set the references to sign
            //signer.AddReference(setCertificationReference(signer));
            signer.AddReference(setXAdESReference(signer));
            signer.AddReference(setKeyInfoReference(signer));

            //Compute signature
            signer.ComputeSignature();

            //return the Signed Xml
            toSign.DocumentElement.AppendChild(toSign.ImportNode(signer.GetXml(), true));
            return toSign;
        }

        private Reference setCertificationReference(SignedXml signer)
        {
            Reference reference = new Reference(String.Empty);

            // create the XML that represents the transform
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<XPath xmlns:ds=\"http://www.w3.org/2000/09/xmldsig#\">not(ancestor-or-self::ds:Signature)</XPath>");

            XmlDsigXPathTransform xform = new XmlDsigXPathTransform();
            xform.LoadInnerXml(doc.DocumentElement.SelectNodes("."));

            reference.TransformChain = new TransformChain();
            reference.TransformChain.Add(xform);
            reference.TransformChain.Add(new XmlDsigExcC14NTransform());

            return reference;
        }

        private static Reference setXAdESReference(SignedXml signer)
        {
            Reference reference = new Reference("#XADES-Properties");
            reference.Id = "GpSoftSignature-XADES-Properties-Ref";
            reference.Type = XAdESSignedXml.XadesSignaturePropertiesNamespace;
            reference.AddTransform(new XmlDsigExcC14NTransform());
            return reference;
        }

        private static Reference setKeyInfoReference(SignedXml signedXml)
        {
            Reference reference = new Reference("#" + signedXml.KeyInfo.Id);
            reference.AddTransform(new XmlDsigExcC14NTransform());
            reference.Id = "GpSoftSignature-KeyInfo-Ref";
            return reference;
        }

        private KeyInfo getKeyInfo(SignedXml signer, X509Certificate2 cert)
        {
            KeyInfo keyInfo = new KeyInfo();
            KeyInfoX509Data keyInfoClause = new KeyInfoX509Data(cert);
            keyInfoClause.AddSubjectName(cert.SubjectName.Name);
            keyInfo.AddClause(keyInfoClause);
            return keyInfo;
        }

        #region XAdES Nodes methods
        private void addXAdESNodes(XAdESSignedXml signedXml, XmlDocument document, X509Certificate2 cert)
        {
            XmlElement qualifyingPropertiesNode = addQualifyingPropertiesNode(signedXml, document);
            XmlElement signedPropertiesNode = addSignedPropertiesNode(document, qualifyingPropertiesNode);
            XmlElement signedSignatureProperties = addSignedSignaturePropertiesNode(document, signedPropertiesNode);
            addSigningTimeNode(document, signedSignatureProperties);
            addSigningCertificate(document, signedSignatureProperties, cert);
        }

        private XmlElement addQualifyingPropertiesNode(XAdESSignedXml signedXml, XmlDocument document)
        {
            DataObject dataObject = new DataObject();
            XmlElement result = document.CreateElement("xades", "QualifyingProperties", _xadesNamespaceUrl);
            result.SetAttribute("Target", signedXml.Signature.Id);
            dataObject.Data = result.SelectNodes(".");
            dataObject.Id = "XADES";
            signedXml.AddObject(dataObject);
            return result;
        }

        private XmlElement addSignedPropertiesNode(XmlDocument document, XmlElement qualifyingPropertiesNode)
        {
            XmlElement signedPropertiesNode = createNodeIn("xades", document, "SignedProperties", _xadesNamespaceUrl, qualifyingPropertiesNode);
            signedPropertiesNode.SetAttribute("Id", "XADES-Properties");
            return signedPropertiesNode;
        }

        private XmlElement addSignedSignaturePropertiesNode(XmlDocument document, XmlElement propertiesNode)
        {
            return createNodeIn("xades", document, "SignedSignatureProperties", _xadesNamespaceUrl, propertiesNode);
        }

        private void addSigningTimeNode(XmlDocument document, XmlElement signedSignaturePropertiesNode)
        {
            XmlElement node = createNodeIn("xades", document, "SigningTime", _xadesNamespaceUrl, signedSignaturePropertiesNode);
            node.InnerText = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
        }

        private void addSigningCertificate(XmlDocument document, XmlElement signedSignatureProperties, X509Certificate2 cert)
        {
            XmlElement signingCertificateNode = createNodeIn("xades", document, "SigningCertificate", _xadesNamespaceUrl, signedSignatureProperties);
            XmlElement certNode = createNodeIn("xades", document, "Cert", _xadesNamespaceUrl, signingCertificateNode);
            addCertDigestNode(document, certNode, cert);
            addIssuerSerialNode(document, certNode, cert);
        }
        private void addCertDigestNode(XmlDocument document, XmlElement certNode, X509Certificate2 cert)
        {
            XmlElement certDigestNode = createNodeIn("xades", document, "CertDigest", _xadesNamespaceUrl, certNode);
            XmlElement digestMethod = createNodeIn("ds", document, "DigestMethod", SignedXml.XmlDsigNamespaceUrl, certDigestNode);
            XmlAttribute attr = document.CreateAttribute("Algorithm");
            attr.Value = SignedXml.XmlDsigSHA1Url;
            digestMethod.Attributes.Append(attr);
            SHA1 cryptoServiceProvider = new SHA1CryptoServiceProvider();
            byte[] sha1 = cryptoServiceProvider.ComputeHash(cert.RawData);
            XmlElement digestValue = createNodeIn("ds", document, "DigestValue", SignedXml.XmlDsigNamespaceUrl, certDigestNode);
            digestValue.InnerText = Convert.ToBase64String(sha1);
        }
        private void addIssuerSerialNode(XmlDocument document, XmlElement certNode, X509Certificate2 cert)
        {
            XmlElement issuerSerialNode = createNodeIn("xades", document, "IssuerSerial", _xadesNamespaceUrl, certNode);
            XmlElement issuerName = createNodeIn("ds", document, "X509IssuerName", SignedXml.XmlDsigNamespaceUrl, issuerSerialNode);
            issuerName.InnerText = cert.IssuerName.Name;
            XmlElement serialNumber = createNodeIn("ds", document, "X509SerialNumber", SignedXml.XmlDsigNamespaceUrl, issuerSerialNode);
            serialNumber.InnerText = cert.SerialNumber;
        }
        #endregion

        #region Xml helper methods
        public XmlElement createNodeIn(XmlDocument document, string nodeName, string nameSpace, XmlElement rootNode)
        {
            XmlElement result = document.CreateElement(nodeName, nameSpace);
            rootNode.AppendChild(result);
            return result;
        }

        public XmlElement createNodeIn(string prefix, XmlDocument document, string nodeName, string nameSpace, XmlElement rootNode)
        {
            XmlElement result = document.CreateElement(prefix, nodeName, nameSpace);
            rootNode.AppendChild(result);
            return result;
        }
        #endregion

    }
}
