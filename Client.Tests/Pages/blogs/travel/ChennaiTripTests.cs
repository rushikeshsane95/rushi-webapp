using BlazorBasic.Pages.blogs.travel;
using Client.Tests.Support;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Tests.Pages.blogs.travel;

public sealed class ChennaiTripTests : BunitContext
{
    [Fact]
    public void ChennaiTrip_RendersHeadingConvertedMarkdownContentAndBackLink()
    {
        const string markdown = """
            # Chennai Notes

            This is **converted** travel markdown.
            """;

        Services.AddSingleton(new HttpClient(new TestHttpMessageHandler(
            new Dictionary<string, string>
            {
                ["blogs/travel/chennai.md"] = markdown
            }))
        {
            BaseAddress = new Uri("http://localhost/")
        });

        var cut = Render<ChennaiTrip>();

        cut.WaitForAssertion(() =>
        {
            cut.Find("h2").TextContent.MarkupMatches("Travel Blog 1: Exploring the Alps");

            var markdownHeading = cut.Find("h1");
            Assert.Equal("Chennai Notes", markdownHeading.TextContent);

            var markdownParagraph = cut.FindAll("p")
                .Single(paragraph => paragraph.TextContent.Trim() == "This is converted travel markdown.");

            Assert.Contains("<strong>converted</strong>", markdownParagraph.InnerHtml);

            var backLink = cut.Find("a");
            Assert.Equal("/blogs/travel", backLink.GetAttribute("href"));
            Assert.Equal("Back to Travel Blogs", backLink.TextContent.Trim());
        });
    }
}
