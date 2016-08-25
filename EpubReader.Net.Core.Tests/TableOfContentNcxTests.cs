// EpubReader.Net
// - EpubReader.Net.Core.Tests
// -- TableOfContentNcxTests.cs
// -------------------------------------------
// Author: Bjarke Søgaard <sogaardbjarke@gmail.com>

using System.Linq;
using System.Threading.Tasks;

using Shouldly;

using Xunit;
using Xunit.Abstractions;

namespace EpubReader.Net.Core.Tests
{
    public class TableOfContentNcxTests
    {
        private readonly ITestOutputHelper output;

        public TableOfContentNcxTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Theory]
        [InlineData("book1")]
        [InlineData("book2")]
        [InlineData("book3")]
        [InlineData("book4")]
        [InlineData("book5")]
        public async Task VerifyTocExists(string fileName)
        {
            var container = await TestHelper.ReadBook(fileName);
            var package = await container.LoadPackage();
            var tocNcx = await package.LoadTableOfContent();
            tocNcx.ShouldNotBeNull();
        }

        [Theory]
        [InlineData("book1")]
        [InlineData("book2")]
        [InlineData("book3")]
        [InlineData("book4")]
        [InlineData("book5")]
        public async Task VerifyTitleExists(string fileName)
        {
            var container = await TestHelper.ReadBook(fileName);
            var package = await container.LoadPackage();
            var tocNcx = await package.LoadTableOfContent();
            tocNcx.Title.ShouldNotBeNull();
        }

        [Theory]
        [InlineData("book1")]
        [InlineData("book2")]
        [InlineData("book3")]
        [InlineData("book4")]
        [InlineData("book5")]
        public async Task VerifyAuthorExists(string fileName)
        {
            var container = await TestHelper.ReadBook(fileName);
            var package = await container.LoadPackage();
            var tocNcx = await package.LoadTableOfContent();
            tocNcx.Title.ShouldNotBeNull();
        }

        [Theory]
        [InlineData("book1")]
        [InlineData("book2")]
        [InlineData("book3")]
        [InlineData("book4")]
        [InlineData("book5")]
        public async Task VerifyNavPointsExists(string fileName)
        {
            var container = await TestHelper.ReadBook(fileName);
            var package = await container.LoadPackage();
            var tocNcx = await package.LoadTableOfContent();
            tocNcx.NavPoints.ShouldNotBeEmpty();

            this.output.WriteLine($"Nav point count: {tocNcx.NavPoints.Count}");
        }

        [Theory]
        [InlineData("book1")]
        [InlineData("book2")]
        [InlineData("book3")]
        [InlineData("book4")]
        [InlineData("book5")]
        public async Task VerifyNavPointContainsData(string fileName)
        {
            var container = await TestHelper.ReadBook(fileName);
            var package = await container.LoadPackage();
            var tocNcx = await package.LoadTableOfContent();

            foreach (var navPoint in tocNcx.NavPoints)
            {
                navPoint.Label.ShouldNotBeNullOrEmpty();
                navPoint.PlayOrder.ShouldBeGreaterThan(0);
                navPoint.Content.ShouldNotBeNullOrEmpty();
                navPoint.ClassName.ShouldNotBeNull(); // Class name is allowed to be empty, just not null.
            }
        }

        [Theory]
        [InlineData("book1")]
        [InlineData("book2")]
        [InlineData("book3")]
        [InlineData("book4")]
        [InlineData("book5")]
        public async Task VerifyFetchingNavPointsAsOrderedListReturnsTheCorrectOrder(string fileName)
        {
            var container = await TestHelper.ReadBook(fileName);
            var package = await container.LoadPackage();
            var tocNcx = await package.LoadTableOfContent();
            var navPoints = tocNcx.GetOrderedNavPoints().ToArray();

            // Verify there are no duplicates in there.
            navPoints.Length.ShouldBe(navPoints.Distinct().Count());

            for (int i = 0; i < navPoints.Length; i++)
            {
                var navPoint = navPoints[i];
                navPoint.PlayOrder.ShouldBe(i + 1);
                navPoint.Label.ShouldNotBeNullOrEmpty();
                navPoint.Content.ShouldNotBeNullOrEmpty();
                navPoint.ClassName.ShouldNotBeNull();
            }

            this.output.WriteLine($"Contains a total of {navPoints.Length} nav points");
        }
    }
}