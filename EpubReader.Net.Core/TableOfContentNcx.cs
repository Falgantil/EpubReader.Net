// EpubReader.Net
// - EpubReader.Net.Core
// -- TableOfContentNcx.cs
// -------------------------------------------
// Author: Bjarke Søgaard <sogaardbjarke@gmail.com>

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

using ICSharpCode.SharpZipLib.Zip;

namespace EpubReader.Net.Core
{
    public class TableOfContentNcx : BaseFile
    {
        private readonly ZipFile zipFile;

        private TableOfContentNcx(ZipFile zipFile)
        {
            this.zipFile = zipFile;
        }

        public string Author { get; private set; }

        public IList<NavPoint> NavPoints { get; private set; }

        public string Title { get; private set; }

        internal static async Task<TableOfContentNcx> Read(ZipFile zipFile, string tocPath)
        {
            var tocNcx = new TableOfContentNcx(zipFile);
            await tocNcx.Initialize(tocPath);
            return tocNcx;
        }

        protected override async Task Initialize(string filePath)
        {
            await base.Initialize(filePath);
            var document = await this.zipFile.LoadDocument(filePath);
            var xRoot = document.ElementFirst("ncx");

            #region Title

            var xTitle = xRoot.ElementFirst("docTitle");
            this.Title = xTitle?.ElementFirst("text")?.Value ?? string.Empty;
            
            #endregion

            #region Author

            var xAuthor = xRoot.ElementFirst("docAuthor");
            this.Author = xAuthor?.ElementFirst("text")?.Value ?? string.Empty;

            #endregion

            #region Nav points

            var xNavMap = xRoot.ElementFirst("navMap");
            this.NavPoints = NavPoint.LoadNavPoints(xNavMap);

            #endregion
        }

        public async Task<HtmlFile> GetHtmlFile(NavPoint navPoint)
        {
            return await HtmlFile.Read(this.zipFile, this.CombinePath(navPoint.Content));
        }

        public IEnumerable<NavPoint> GetOrderedNavPoints()
        {
            var navPoints = this.GetNavPoints(this.NavPoints);
            return navPoints.OrderBy(n => n.PlayOrder);
        }

        private IEnumerable<NavPoint> GetNavPoints(IEnumerable<NavPoint> navPoint)
        {
            foreach (var n in navPoint)
            {
                yield return n;
                foreach (var point in this.GetNavPoints(n.Children))
                {
                    yield return point;
                }
            }
        }

        public class NavPoint
        {
            private NavPoint(XElement node)
            {
                this.PlayOrder = int.Parse(node.Attribute("playOrder").Value);
                this.ClassName = node.Attribute("class")?.Value ?? string.Empty;
                this.Label = node.ElementFirst("navLabel").ElementFirst("text").Value;
                this.Content = node.ElementFirst("content").Attribute("src").Value;
                this.Children = node.ElementsFirst("navPoint").Select(x => new NavPoint(x)).ToList();
            }

            public IList<NavPoint> Children { get; private set; }

            public string ClassName { get; private set; }

            public string Content { get; private set; }

            public string Label { get; private set; }

            public int PlayOrder { get; private set; }

            internal static IList<NavPoint> LoadNavPoints(XElement parent)
            {
                var xNavPoints = parent.ElementsFirst("navPoint");
                return xNavPoints.Select(xNavPoint => new NavPoint(xNavPoint)).ToList();
            }
        }
    }
}