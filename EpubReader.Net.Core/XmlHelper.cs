using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace EpubReader.Net.Core
{
    public static class XmlHelper
    {
        public static XElement ElementFirst<T>(this T source, string localName)
            where T : XContainer
        {
            return source.ElementsFirst(localName).FirstOrDefault();
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

        public static string GetRootPath(string path)
        {
            if (path.Contains("/"))
            {
                var splitBySlash = path.Split('/');
                return string.Join("/", splitBySlash.Take(splitBySlash.Length - 1));
            }
            return string.Empty;
        }

        public static string CombinePath(params string[] paths)
        {
            return string.Join("/", paths);
        }
    }
}
