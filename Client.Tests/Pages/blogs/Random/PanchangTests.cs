using BlazorBasic.Pages.blogs.Random;
using Client.Tests.Support;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Tests.Pages.blogs.Random;

public sealed class PanchangTests : BunitContext
{
    [Fact]
    public void Panchang_ReplacesLoadingWithMarkdownContentAndNavigationLinks()
    {
        const string markdown = """
            # Panchang Notes

            This is **converted** markdown.
            """;

        var handler = new TestHttpMessageHandler(new Dictionary<string, string>
        {
            ["blogs/random/panchang.md"] = markdown
        });

        Services.AddSingleton(new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost/")
        });

        var cut = Render<Panchang>();

        cut.WaitForAssertion(() =>
        {
            Assert.DoesNotContain("Loading blog post...", cut.Markup);
            cut.Find("h1").TextContent.MarkupMatches("Panchang Notes");

            var markdownParagraph = cut.FindAll("p")
                .Single(paragraph => paragraph.TextContent.Trim() == "This is converted markdown.");

            Assert.Contains("<strong>converted</strong>", markdownParagraph.InnerHtml);
        });

        AssertLink(cut, "Continue to part 2", "/blogs/random/panchang2");
        AssertLink(cut, "Back to Random Blogs", "/blogs/random");
    }

    private static void AssertLink(
        IRenderedComponent<Panchang> cut,
        string expectedText,
        string expectedHref)
    {
        var link = cut.FindAll("a").Single(anchor => anchor.TextContent.Trim() == expectedText);

        Assert.Equal(expectedHref, link.GetAttribute("href"));
    }
}
