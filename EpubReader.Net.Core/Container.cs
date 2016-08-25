using System.IO;
using System.Threading.Tasks;

using ICSharpCode.SharpZipLib.Zip;

namespace EpubReader.Net.Core
{
    public class Container
    {
        private readonly ZipFile zipFile;

        private Container(ZipFile zipFile)
        {
            this.zipFile = zipFile;
        }

        public static async Task<Container> Read(Stream stream)
        {
            var container = new Container(new ZipFile(stream));
            await container.Initialize();
            return container;
        }

        public string PackagePath { get; set; }

        private async Task Initialize()
        {
            var doc = await this.zipFile.LoadDocument("meta-inf/container.xml");

            var rootFile = doc
                .ElementFirst("container")
                .ElementFirst("rootfiles")
                .ElementFirst("rootfile");

            var attrFullPath = rootFile.Attribute("full-path");
            this.PackagePath = attrFullPath.Value;
        }

        public async Task<PackageOpf> LoadPackage()
        {
            return await PackageOpf.Read(this.zipFile, this.PackagePath);
        }
    }
}
