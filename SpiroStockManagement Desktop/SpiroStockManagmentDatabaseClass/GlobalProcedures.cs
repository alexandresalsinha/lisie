using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml;

namespace SpiroStockManagmentDatabaseClass.GlobalProcedures
{
    static class XmlSerializerExtension
    {
        public static XElement SerializeAsXElement(this XmlSerializer xs, object o)
        {
            XDocument d = new XDocument();
            using (XmlWriter w = d.CreateWriter()) xs.Serialize(w, o);
            XElement e = d.Root;
            e.Remove();
            return e;
        }
    }
}
