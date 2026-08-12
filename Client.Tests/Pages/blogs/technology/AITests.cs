using BlazorBasic.Pages.blogs.technology;
using Client.Tests.Support;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Tests.Pages.blogs.technology;

public sealed class AITests : BunitContext
{
    [Fact]
    public void AI_RendersHeadingIntroMarkdownTitlesAndPostLinks()
    {
        Services.AddSingleton(new HttpClient(new TestHttpMessageHandler(
            new Dictionary<string, string>
            {
                ["blogs/technology/ai/agents-md-governing-ai-dotnet.md"] = """
                    # Agents.md: Governing AI in .NET

                    Notes about repository agent instructions.
                    """,
                ["blogs/technology/ai/codex-best-practices-dotnet.md"] = """
                    # Codex Best Practices for `.NET`

                    Notes about effective Codex workflows.
                    """
            }))
        {
            BaseAddress = new Uri("http://localhost/")
        });

        var cut = Render<AI>();

        cut.WaitForAssertion(() =>
        {
            cut.Find("h2").TextContent.MarkupMatches("AI Blogs");

            var intro = cut.FindAll("p")
                .Single(paragraph => paragraph.TextContent.Trim() == "Dive into AI topics and tools.");
            Assert.Equal("Dive into AI topics and tools.", intro.TextContent.Trim());

            var links = cut.FindAll("ul a");
            Assert.Equal(2, links.Count);

            AssertLink(cut, "Agents.md: Governing AI in .NET", "/blogs/technology/ai/agents-md-governing-ai-dotnet");
            AssertLink(cut, "Codex Best Practices for .NET", "/blogs/technology/ai/codex-best-practices-dotnet");
        });
    }

    private static void AssertLink(
        IRenderedComponent<AI> cut,
        string expectedText,
        string expectedHref)
    {
        var link = cut.FindAll("a").Single(anchor => anchor.TextContent.Trim() == expectedText);

        Assert.Equal(expectedHref, link.GetAttribute("href"));
    }
}
