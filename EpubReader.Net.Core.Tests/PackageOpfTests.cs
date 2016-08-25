// EpubReader
// - EpubReader.Core.Tests
// -- PackageOpfTests.cs
// -------------------------------------------
// Author: Bjarke Søgaard <sogaardbjarke@gmail.com>

using System.Threading.Tasks;

using Shouldly;

using Xunit;
using Xunit.Abstractions;

namespace EpubReader.Net.Core.Tests
{
    public class PackageOpfTests
    {
        private readonly ITestOutputHelper output;

        public PackageOpfTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Theory]
        [InlineData("book1")]
        [InlineData("book2")]
        [InlineData("book3")]
        [InlineData("book4")]
        [InlineData("book5")]
        public async Task VerifyPackageExists(string fileName)
        {
            var container = await TestHelper.ReadBook(fileName);

            var package = await container.LoadPackage();
            package.ShouldNotBeNull();
        }

        [Theory]
        [InlineData("book1")]
        [InlineData("book2")]
        [InlineData("book3")]
        [InlineData("book4")]
        [InlineData("book5")]
        public async Task VerifyPackageTocExists(string fileName)
        {
            var container = await TestHelper.ReadBook(fileName);

            var package = await container.LoadPackage();
            package.TableOfContent.ShouldNotBeNullOrEmpty();

            this.output.WriteLine($"Table of Content: {package.TableOfContent}");
        }

        [Theory]
        [InlineData("book1")]
        [InlineData("book2")]
        [InlineData("book3")]
        [InlineData("book4")]
        [InlineData("book5")]
        public async Task VerifyPackageTocHrefExists(string fileName)
        {
            var container = await TestHelper.ReadBook(fileName);

            var package = await container.LoadPackage();
            package.TocHref.ShouldNotBeNullOrEmpty();

            this.output.WriteLine($"ToC Href: {package.TocHref}");
        }

        [Theory]
        [InlineData("book1")]
        [InlineData("book2")]
        [InlineData("book3")]
        [InlineData("book4")]
        [InlineData("book5")]
        public async Task VerifyPackageRootPathIsntEmpty(string fileName)
        {
            var container = await TestHelper.ReadBook(fileName);
            var package = await container.LoadPackage();

            if (container.PackagePath.Contains("/"))
            {
                package.RootPath.ShouldNotBeNullOrEmpty();

                this.output.WriteLine($"Root path: {package.RootPath}");
            }
            else
            {
                this.output.WriteLine($"Empty root path: {container.PackagePath}");
            }
        }

        [Theory]
        [InlineData("book1")]
        [InlineData("book2")]
        [InlineData("book3")]
        [InlineData("book4")]
        [InlineData("book5")]
        public async Task VerifyPackageMetaData(string fileName)
        {
            var container = await TestHelper.ReadBook(fileName);

            var package = await container.LoadPackage();
            var data = package.MetaData;
            data.ShouldNotBeNull();

            data.Titles.ShouldNotBeEmpty();
            this.output.WriteLine($"Titles are: {string.Join(", ", data.Titles)}");

            if (data.Creators.Count > 0)
            {
                this.output.WriteLine($"Creators are: {string.Join(", ", data.Creators)}");
            }
            if (data.Languages.Count > 0)
            {
                this.output.WriteLine($"Languages are: {string.Join(", ", data.Languages)}");
            }
            if (!string.IsNullOrEmpty(data.Date))
            {
                this.output.WriteLine($"Date is: {data.Date}");
            }
        }
    }
}