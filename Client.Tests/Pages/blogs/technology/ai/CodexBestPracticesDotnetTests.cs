using BlazorBasic.Pages.blogs.technology.ai;
using Client.Tests.Support;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Tests.Pages.blogs.technology.ai;

public sealed class CodexBestPracticesDotnetTests : BunitContext
{
    [Fact]
    public void CodexBestPracticesDotnet_RendersConvertedMarkdownContentAndBackLink()
    {
        const string markdown = """
            # Codex Best Practices for .NET

            Keep changes **small** and reviewable.
            """;

        Services.AddSingleton(new HttpClient(new TestHttpMessageHandler(
            new Dictionary<string, string>
            {
                ["blogs/technology/ai/codex-best-practices-dotnet.md"] = markdown
            }))
        {
            BaseAddress = new Uri("http://localhost/")
        });

        var cut = Render<CodexBestPracticesDotnet>();

        cut.WaitForAssertion(() =>
        {
            Assert.DoesNotContain("Loading blog post...", cut.Markup);
            cut.Find("h1").TextContent.MarkupMatches("Codex Best Practices for .NET");

            var markdownParagraph = cut.FindAll("p")
                .Single(paragraph => paragraph.TextContent.Trim() == "Keep changes small and reviewable.");

            Assert.Contains("<strong>small</strong>", markdownParagraph.InnerHtml);

            var backLink = cut.Find("a");
            Assert.Equal("/blogs/technology/ai", backLink.GetAttribute("href"));
            Assert.Equal("Back to AI Blogs", backLink.TextContent.Trim());
        });
    }
}
