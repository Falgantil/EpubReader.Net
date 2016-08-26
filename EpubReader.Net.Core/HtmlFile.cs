using System;
using System.IO;
using System.Text;
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

        public async Task<XElement> ReadContent()
        {
            var deepCopy = new XElement(this.HtmlContent);

            await this.ParseNode(deepCopy);

            return deepCopy;
        }

        private async Task ParseNode(XElement element)
        {
            foreach (var xElement in element.Elements())
            {
                await this.SetLinkStyleValue(xElement);
                await this.SetImageValue(xElement);
                await this.ParseNode(xElement);
            }
        }

        private async Task SetImageValue(XElement xElement)
        {
            if (!xElement.Name.LocalName.Equals("img", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var pathStyle = xElement.Attribute("src").Value;
            var combinePath = this.CombinePath(pathStyle);
            var content = await this.ReadContentAsByte(combinePath);
            var imgBase64 = await Task.Run(() => Convert.ToBase64String(content));
            xElement.SetAttributeValue("src", $"data:image/jpeg;base64, {imgBase64}");
        }

        private async Task SetLinkStyleValue(XElement xElement)
        {
            if (!xElement.Name.LocalName.Equals("link", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            var pathStyle = xElement.Attribute("href").Value;
            var combinePath = this.CombinePath(pathStyle);
            var content = await this.ReadContentAsString(combinePath);
            xElement.RemoveAttributes();
            xElement.SetValue(content);
            xElement.Name = "style";
        }

        private async Task<string> ReadContentAsString(string combinePath)
        {
            var content = await this.ReadContentAsByte(combinePath);
            return Encoding.UTF8.GetString(content, 0, content.Length);
        }

        private async Task<byte[]> ReadContentAsByte(string combinePath)
        {
            var input = await this.zipFile.LoadStream(combinePath);
            
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}