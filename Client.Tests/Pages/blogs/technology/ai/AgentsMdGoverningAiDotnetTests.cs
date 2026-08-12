using BlazorBasic.Pages.blogs.technology.ai;
using Client.Tests.Support;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Tests.Pages.blogs.technology.ai;

public sealed class AgentsMdGoverningAiDotnetTests : BunitContext
{
    [Fact]
    public void AgentsMdGoverningAiDotnet_RendersConvertedMarkdownContentAndBackLink()
    {
        const string markdown = """
            # AGENTS.md Governing AI in .NET

            This post explains how **AGENTS.md** guides AI coding agents.
            """;

        Services.AddSingleton(new HttpClient(new TestHttpMessageHandler(
            new Dictionary<string, string>
            {
                ["blogs/technology/ai/agents-md-governing-ai-dotnet.md"] = markdown
            }))
        {
            BaseAddress = new Uri("http://localhost/")
        });

        var cut = Render<AgentsMdGoverningAiDotnet>();

        cut.WaitForAssertion(() =>
        {
            Assert.DoesNotContain("Loading blog post...", cut.Markup);
            cut.Find("h1").TextContent.MarkupMatches("AGENTS.md Governing AI in .NET");

            var markdownParagraph = cut.FindAll("p")
                .Single(paragraph => paragraph.TextContent.Trim() == "This post explains how AGENTS.md guides AI coding agents.");

            Assert.Contains("<strong>AGENTS.md</strong>", markdownParagraph.InnerHtml);

            var backLink = cut.Find("a");
            Assert.Equal("/blogs/technology/ai", backLink.GetAttribute("href"));
            Assert.Equal("Back to AI Blogs", backLink.TextContent.Trim());
        });
    }
}
