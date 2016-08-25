using System.IO;
using System.Threading.Tasks;

namespace EpubReader.Net.Core.Tests
{
    public static class TestHelper
    {
        public static async Task<Container> ReadBook(string fileName)
        {
            var stream = new MemoryStream(File.ReadAllBytes($"Files/{fileName}.epub"));
            return await Container.Read(stream);
        }
    }
}
