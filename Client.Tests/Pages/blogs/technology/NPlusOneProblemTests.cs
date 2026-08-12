using BlazorBasic.Pages.blogs.technology;
using Client.Tests.Support;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Tests.Pages.blogs.technology;

public sealed class NPlusOneProblemTests : BunitContext
{
    [Fact]
    public void NPlusOneProblem_RendersConvertedMarkdownContentBackLinkAndJsInterop()
    {
        const string markdown = """
            # N+1 Query Problem

            This post explains the **N+1 problem** in data access.

            ```csharp
            var orders = await db.Orders.ToListAsync();
            ```
            """;

        Services.AddSingleton(new HttpClient(new TestHttpMessageHandler(
            new Dictionary<string, string>
            {
                ["blogs/technology/n-plus-one-problem.md"] = markdown
            }))
        {
            BaseAddress = new Uri("http://localhost/")
        });

        var renderCodeHighlighting = JSInterop.SetupVoid("renderCodeHighlighting").SetVoidResult();

        var cut = Render<NPlusOneProblem>();

        cut.WaitForAssertion(() =>
        {
            Assert.DoesNotContain("Loading blog post...", cut.Markup);
            cut.Find("h1").TextContent.MarkupMatches("N+1 Query Problem");

            var markdownParagraph = cut.FindAll("p")
                .Single(paragraph => paragraph.TextContent.Trim() == "This post explains the N+1 problem in data access.");

            Assert.Contains("<strong>N+1 problem</strong>", markdownParagraph.InnerHtml);

            var codeBlock = cut.Find("pre code");
            Assert.Contains("var orders = await db.Orders.ToListAsync();", codeBlock.TextContent);

            var backLink = cut.Find("a");
            Assert.Equal("/blogs/technology", backLink.GetAttribute("href"));
            Assert.Equal("Back to Technology Blogs", backLink.TextContent.Trim());

            Assert.Single(renderCodeHighlighting.Invocations);
        });
    }
}
