using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;

using ICSharpCode.SharpZipLib.Zip;

namespace EpubReader.Net.Core
{
    internal static class ZipHelper
    {
        public static async Task<XDocument> LoadDocument(this ZipFile zipFile, string fileName)
        {
            var inputStream = await LoadStream(zipFile, fileName);
            return await Task.Run(() => XDocument.Load(inputStream));
        }

        public static async Task<Stream> LoadStream(this ZipFile zipFile, string fileName)
        {
            var entry = await Task.Run(() => zipFile.GetEntry(fileName));
            var inputStream = await Task.Run(() => zipFile.GetInputStream(entry));
            return inputStream;
        }
    }
}
