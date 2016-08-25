using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

using ICSharpCode.SharpZipLib.Zip;

namespace EpubReader.Net.Core
{
    public class PackageOpf : BaseFile
    {
        private readonly ZipFile zipFile;

        private PackageOpf(ZipFile zipFile)
        {
            this.zipFile = zipFile;
        }

        public string TableOfContent { get; set; }

        public string TocHref { get; set; }

        internal static async Task<PackageOpf> Read(ZipFile zipFile, string packagePath)
        {
            var packageOpf = new PackageOpf(zipFile);
            await packageOpf.Initialize(packagePath);
            return packageOpf;
        }

        protected override async Task Initialize(string filepath)
        {
            await base.Initialize(filepath);
            var document = await this.zipFile.LoadDocument(filepath);
            var xPackage = document
                .ElementFirst("package");

            #region Table of Content

            var attrToc = xPackage
                .ElementFirst("spine")
                .Attribute("toc");
            this.TableOfContent = attrToc.Value;

            #endregion

            #region ToC Href

            var xTocItem = xPackage
                .ElementFirst("manifest")
                .ElementsFirst("item")
                .FirstOrDefault(k => k.Attribute("id")?.Value == this.TableOfContent);
            this.TocHref = xTocItem.Attribute("href").Value;

            #endregion

            #region Retrieve MetaData

            this.MetaData = XMetaData.Read(xPackage.ElementFirst("metadata"));

            #endregion
        }

        public XMetaData MetaData { get; set; }

        public async Task<TableOfContentNcx> LoadTableOfContent()
        {
            return await TableOfContentNcx.Read(this.zipFile, this.CombinePath(this.TocHref));
        }

        public class XMetaData
        {
            private XMetaData(XElement xMetaData)
            {
                this.Titles = xMetaData.GetElementValue("title");
                this.Creators = xMetaData.GetElementValue("creator");
                this.Languages = xMetaData.GetElementValue("language");
                this.Date = xMetaData.ElementFirst("date")?.Value;
            }

            public List<string> Titles { get; private set; }
            public List<string> Creators { get; private set; }
            public List<string> Languages { get; private set; }
            public string Date { get; private set; }

            public static XMetaData Read(XElement xMetaData)
            {
                return new XMetaData(xMetaData);
            }
        }
    }

    public abstract class BaseFile
    {
        public string RootPath { get; private set; }

        protected virtual async Task Initialize(string filePath)
        {
            this.RootPath = XmlHelper.GetRootPath(filePath);
        }

        protected string CombinePath(params string[] paths)
        {
            List<string> p = new List<string>
            {
                this.RootPath
            };
            p.AddRange(paths);
            var combinePath = XmlHelper.CombinePath(p.ToArray());
            if (combinePath.Contains("#"))
            {
                combinePath = combinePath.Substring(0, combinePath.IndexOf("#"));
            }
            return combinePath;
        }
    }
}
