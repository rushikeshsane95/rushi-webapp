using BlazorBasic.Pages.blogs.travel;
using Client.Tests.Support;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Tests.Pages.blogs.travel;

public sealed class JotunheimTests : BunitContext
{
    [Fact]
    public void Jotunheim_RendersConvertedMarkdownContentAndBackLink()
    {
        const string markdown = """
            # Jotunheim Notes

            This is **converted** travel markdown.
            """;

        Services.AddSingleton(new HttpClient(new TestHttpMessageHandler(
            new Dictionary<string, string>
            {
                ["blogs/travel/jotunheim.md"] = markdown
            }))
        {
            BaseAddress = new Uri("http://localhost/")
        });

        var cut = Render<Jotunheim>();

        cut.WaitForAssertion(() =>
        {
            Assert.DoesNotContain("Loading blog post...", cut.Markup);
            cut.Find("h1").TextContent.MarkupMatches("Jotunheim Notes");

            var markdownParagraph = cut.FindAll("p")
                .Single(paragraph => paragraph.TextContent.Trim() == "This is converted travel markdown.");

            Assert.Contains("<strong>converted</strong>", markdownParagraph.InnerHtml);

            var backLink = cut.Find("a");
            Assert.Equal("/blogs/travel", backLink.GetAttribute("href"));
            Assert.Equal("Back to Travel Blogs", backLink.TextContent.Trim());
        });
    }
}
