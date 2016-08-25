using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EpubReader.Core
{
    public static class XmlHelper
    {
        public static XElement ElementFirst<T>(this T source, string localName)
            where T : XContainer
        {
            return source.ElementsFirst(localName).First();
        }

        public static IEnumerable<XElement> ElementsFirst<T>(this T source, string localName)
            where T : XContainer
        {
            return source.Elements().Where(e => e.Name.LocalName == localName);
        }

        public static List<string> GetElementValue(this XContainer source, string localName)
        {
            return source.ElementsFirst(localName)?.Select(k => k.Value).ToList() ?? new List<string>();
        }
    }
}
