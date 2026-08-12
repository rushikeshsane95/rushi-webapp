using BlazorBasic.Pages.blogs.technology;
using Client.Tests.Support;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Tests.Pages.blogs.technology;

public sealed class DependencyInversionPrincipleTests : BunitContext
{
    [Fact]
    public void DependencyInversionPrinciple_RendersConvertedMarkdownBackLinkAndJsInterop()
    {
        const string markdown = """
            # Dependency Inversion Principle

            High-level modules should not depend on **low-level modules**.

            ```mermaid
            graph TD
                A[Policy] --> B[Abstraction]
            ```
            """;

        Services.AddSingleton(new HttpClient(new TestHttpMessageHandler(
            new Dictionary<string, string>
            {
                ["blogs/technology/dependency-inversion-principle.md"] = markdown
            }))
        {
            BaseAddress = new Uri("http://localhost/")
        });

        var renderMermaid = JSInterop.SetupVoid("renderMermaid").SetVoidResult();
        var renderCodeHighlighting = JSInterop.SetupVoid("renderCodeHighlighting").SetVoidResult();

        var cut = Render<DependencyInversionPrinciple>();

        cut.WaitForAssertion(() =>
        {
            Assert.DoesNotContain("Loading blog post...", cut.Markup);
            cut.Find("h1").TextContent.MarkupMatches("Dependency Inversion Principle");

            var markdownParagraph = cut.FindAll("p")
                .Single(paragraph => paragraph.TextContent.Trim() == "High-level modules should not depend on low-level modules.");

            Assert.Contains("<strong>low-level modules</strong>", markdownParagraph.InnerHtml);

            var backLink = cut.Find("a");
            Assert.Equal("/blogs/technology", backLink.GetAttribute("href"));
            Assert.Equal("Back to Technology Blogs", backLink.TextContent.Trim());

            Assert.Single(renderMermaid.Invocations);
            Assert.Single(renderCodeHighlighting.Invocations);
        });
    }
}
