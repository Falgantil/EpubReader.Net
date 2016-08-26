// EpubReader.Net
// - EpubReader.Net.Core.Tests
// -- HtmlFileTests.cs
// -------------------------------------------
// Author: Bjarke Søgaard <sogaardbjarke@gmail.com>

using System;
using System.Linq;
using System.Threading.Tasks;

using Shouldly;

using Xunit;
using Xunit.Abstractions;

namespace EpubReader.Net.Core.Tests
{
    public class HtmlFileTests
    {
        private readonly ITestOutputHelper output;

        public HtmlFileTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Theory]
        [InlineData("book1")]
        [InlineData("book2")]
        [InlineData("book3")]
        [InlineData("book4")]
        [InlineData("book5")]
        public async Task VerifyHtmlExists(string fileName)
        {
            var container = await TestHelper.ReadBook(fileName);
            var package = await container.LoadPackage();
            var tocNcx = await package.LoadTableOfContent();
            var htmlFile = await tocNcx.GetHtmlFile(tocNcx.NavPoints.First());
            htmlFile.ShouldNotBeNull();
            htmlFile.HtmlContent.ShouldNotBeNull();
            var strHtml = htmlFile.HtmlContent.ToString();
            this.output.WriteLine($"Content: {strHtml.Substring(0, Math.Min(strHtml.Length, 500))}");
        }

        [Theory]
        [InlineData("book1")]
        [InlineData("book2")]
        [InlineData("book3")]
        [InlineData("book4")]
        [InlineData("book5")]
        public async Task VerifyHtmlContentCanBeReadAndParsed(string fileName)
        {
            var container = await TestHelper.ReadBook(fileName);
            var package = await container.LoadPackage();
            var tocNcx = await package.LoadTableOfContent();
            var htmlFile = await tocNcx.GetHtmlFile(tocNcx.NavPoints.First());
            var element = await htmlFile.ReadContent();
            var original = htmlFile.HtmlContent.ToString();
            var parse = element.ToString();
            this.output.WriteLine($"Length difference: {original.Length} vs {parse.Length}");
        }
    }
}