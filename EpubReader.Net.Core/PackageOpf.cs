using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using ICSharpCode.SharpZipLib.Zip;

namespace EpubReader.Core
{
    public class PackageOpf
    {
        private readonly ZipFile zipFile;

        private PackageOpf(ZipFile zipFile)
        {
            this.zipFile = zipFile;
        }

        public string TableOfContent { get; set; }

        public string TocHref { get; set; }

        public string RootPath { get; set; }

        internal static async Task<PackageOpf> Read(ZipFile zipFile, string packagePath)
        {
            var packageOpf = new PackageOpf(zipFile);
            await packageOpf.Initialize(packagePath);
            return packageOpf;
        }

        private async Task Initialize(string packagePath)
        {
            #region Set root path

            if (packagePath.Contains("/"))
            {
                var splitBySlash = packagePath.Split('/');
                this.RootPath = Path.Combine(splitBySlash.Take(splitBySlash.Length - 1).ToArray());
            }
            else
            {
                this.RootPath = string.Empty;
            }

            #endregion

            var document = await this.zipFile.LoadDocument(packagePath);
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

            this.MetaData = await XMetaData.Read(xPackage.ElementFirst("metadata"));

            #endregion
        }

        public XMetaData MetaData { get; set; }

        public class XMetaData
        {
            private XMetaData() { }

            public List<string> Titles { get; private set; }
            public List<string> Creators { get; private set; }
            public List<string> Languages { get; private set; }
            public string Date { get; private set; }

            public static async Task<XMetaData> Read(XElement xMetaData)
            {
                return new XMetaData
                {
                    Titles = xMetaData.GetElementValue("title"),
                    Creators = xMetaData.GetElementValue("creator"),
                    Languages = xMetaData.GetElementValue("language"),
                    Date = xMetaData.ElementFirst("date")?.Value
                };
            }
        }
    }
}
