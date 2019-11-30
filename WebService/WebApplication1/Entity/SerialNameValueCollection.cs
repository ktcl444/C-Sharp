using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace WebApplication1.Entity
{
    public class SerialNameValueCollection : NameValueCollection, IXmlSerializable
    {
        public SerialNameValueCollection()
        {
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            if (reader.IsEmptyElement)
                return;

            while (reader.Read()
                && reader.NodeType != XmlNodeType.EndElement
                && reader.NodeType != XmlNodeType.None)
            {
                if (reader.NodeType != XmlNodeType.Element || reader.LocalName != "Item") continue;
                reader.MoveToAttribute("name");
                var name = reader.Value;
                reader.MoveToAttribute("value");
                var value = reader.Value;
                Add(name, value);
            }
            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            for (var index = 0; index < this.Count; index++)
            {
                writer.WriteStartElement("Item");
                writer.WriteAttributeString("name", this.Keys[index]);
                writer.WriteAttributeString("value", this[index]);
                writer.WriteEndElement();
            }
        }
    }
}