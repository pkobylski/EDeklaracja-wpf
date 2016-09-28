using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace GPS.Components.Cryptography.Xades
{
    internal sealed class XAdESSignedXml : SignedXml
    {
        private readonly List<DataObject> _dataObjects = new List<DataObject>();
        public const string XadesSignaturePropertiesNamespace = "http://uri.etsi.org/01903/v1.1.1#SignedProperties";

        public XAdESSignedXml(XmlDocument document) : base(document) { }

        public override XmlElement GetIdElement(XmlDocument doc, string id)
        {
            if (String.IsNullOrEmpty(id)) return null;

            XmlElement xmlElement = base.GetIdElement(doc, id);
            if (xmlElement != null) return xmlElement;

            //if (_dataObjects.Count == 0) return null;
            foreach (DataObject dataObject in _dataObjects)
            {
                XmlElement nodeWithSameId = findNodeWithAttributeValueIn(dataObject.Data, "Id", id);
                if (nodeWithSameId != null)
                    return nodeWithSameId;
            }
            if (KeyInfo != null)
            {
                XmlElement nodeWithSameId = findNodeWithAttributeValueIn(KeyInfo.GetXml().SelectNodes("."), "Id", id);
                if (nodeWithSameId != null)
                    return nodeWithSameId;
            }
            return null;
        }
        public new void AddObject(DataObject dataObject)
        {
            base.AddObject(dataObject);
            _dataObjects.Add(dataObject);
        }

        public XmlElement findNodeWithAttributeValueIn(XmlNodeList nodeList, string attributeName, string value)
        {
            if (nodeList.Count == 0) return null;
            foreach (XmlNode node in nodeList)
            {
                XmlElement nodeWithSameId = findNodeWithAttributeValueIn(node, attributeName, value);
                if (nodeWithSameId != null) return nodeWithSameId;
            }
            return null;
        }

        private XmlElement findNodeWithAttributeValueIn(XmlNode node, string attributeName, string value)
        {
            string attributeValueInNode = getAttributeValueInNodeOrNull(node, attributeName);
            if ((attributeValueInNode != null) && (attributeValueInNode.Equals(value))) return (XmlElement)node;
            return findNodeWithAttributeValueIn(node.ChildNodes, attributeName, value);
        }

        private string getAttributeValueInNodeOrNull(XmlNode node, string attributeName)
        {
            if (node.Attributes != null)
            {
                XmlAttribute attribute = node.Attributes[attributeName];
                if (attribute != null) return attribute.Value;
            }
            return null;
        }
    }


}
