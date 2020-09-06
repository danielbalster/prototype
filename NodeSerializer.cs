using Prototype.Behaviortree;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Prototype
{
    static class NodeSerializer
    {

        #region silly serializer

        static public INode load(string xml)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            try
            {
                return load(doc.LastChild.FirstChild as XmlElement);
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        static private INode load(XmlElement e)
        {
            if (e == null) return null;
            var node = NodeFactory.Create(e.Name);
            if (node == null) return null;

            node.Id = Guid.NewGuid();

            foreach (var type in node.GetType().GetProperties())
            {
                if (type.GetCustomAttribute<PropertyAttribute>() == null) continue;
                try
                {
                    var value = e.GetAttribute(type.Name);
                    var targetvalue = Convert.ChangeType(value, type.PropertyType);
                    type.SetMethod.Invoke(node, new[] { targetvalue });
                }
                catch (Exception)
                {

                }
            }

            foreach (XmlElement child in e.ChildNodes)
            {
                var c = load(child);
                if (c != null) node.Add(c);
            }
            return node;
        }

        public static string save(INode root)
        {
            var doc = new XmlDocument();
            var behavior = doc.CreateElement("Behaviortree");
            doc.AppendChild(behavior);
            try
            {
               save(behavior, root);
            }
            catch (Exception)
            {

            }
            using (var sw = new StringWriter())
            {
                using (var tw = XmlWriter.Create(sw))
                {
                    doc.WriteTo(tw);
                    tw.Flush();
                    return sw.GetStringBuilder().ToString();
                }
            }
        }

        static private void save(XmlElement parent, INode node)
        {
            var elem = parent.OwnerDocument.CreateElement(node.Name);
            parent.AppendChild(elem);
            foreach (var type in node.GetType().GetProperties())
            {
                if (type.GetCustomAttribute<PropertyAttribute>() == null) continue;
                var result = type.GetMethod.Invoke(node, null);
                elem.SetAttribute(type.Name, result.ToString());
            }
            foreach (var child in node) save(elem, child);
        }
        #endregion

    }
}
