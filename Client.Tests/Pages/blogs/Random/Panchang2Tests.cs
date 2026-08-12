using BlazorBasic.Pages.blogs.Random;
using Client.Tests.Support;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Tests.Pages.blogs.Random;

public sealed class Panchang2Tests : BunitContext
{
    private const string MarkdownResponse = """
        # Panchang Part 2

        This content was **converted** from markdown.
        """;

    public Panchang2Tests()
    {
        JSInterop.SetupVoid("renderMath");

        Services.AddSingleton(new HttpClient(new TestHttpMessageHandler(
            new Dictionary<string, string>
            {
                ["blogs/random/panchang2.md"] = MarkdownResponse
            }))
        {
            BaseAddress = new Uri("http://localhost/")
        });
    }

    [Fact]
    public void Panchang2_RendersConvertedMarkdownContentAndBackLink()
    {
        var cut = Render<Panchang2>();

        cut.WaitForAssertion(() =>
        {
            var markdownContent = cut.Find(".markdown-content");
            Assert.Equal("Panchang Part 2", markdownContent.QuerySelector("h1")?.TextContent);
            Assert.Equal("converted", markdownContent.QuerySelector("strong")?.TextContent);
            Assert.Contains("This content was converted from markdown.", markdownContent.TextContent);

            var backLink = cut.Find("a");
            Assert.Equal("/blogs/random", backLink.GetAttribute("href"));
            Assert.Equal("Back to Random Blogs", backLink.TextContent.Trim());
        });
    }
}
