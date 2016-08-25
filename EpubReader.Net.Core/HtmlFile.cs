using System.Threading.Tasks;
using System.Xml.Linq;

using ICSharpCode.SharpZipLib.Zip;

namespace EpubReader.Net.Core
{
    public class HtmlFile : BaseFile
    {
        private readonly ZipFile zipFile;

        private HtmlFile(ZipFile zipFile)
        {
            this.zipFile = zipFile;
        }

        protected override async Task Initialize(string filePath)
        {
            await base.Initialize(filePath);

            var document = await this.zipFile.LoadDocument(filePath);
            this.HtmlContent = document.ElementFirst("html");
        }

        public XElement HtmlContent { get; private set; }

        internal static async Task<HtmlFile> Read(ZipFile zipFile, string content)
        {
            var htmlFile = new HtmlFile(zipFile);
            await htmlFile.Initialize(content);
            return htmlFile;
        }
    }
}