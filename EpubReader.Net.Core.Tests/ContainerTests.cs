using System.IO;
using System.Threading.Tasks;

using Shouldly;

using Xunit;
using Xunit.Abstractions;

namespace EpubReader.Core.Tests
{
    public class ContainerTests
    {
        private readonly ITestOutputHelper output;

        public ContainerTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Theory]
        [InlineData("book1")]
        [InlineData("book2")]
        [InlineData("book3")]
        [InlineData("book4")]
        [InlineData("book5")]
        public async Task VerifyContainerExists(string fileName)
        {
            var container = await TestHelper.ReadBook(fileName);
            container.PackagePath.ShouldNotBeNullOrEmpty();
        }
    }
}