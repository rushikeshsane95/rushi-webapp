using BlazorBasic.Pages.blogs.technology;
using Client.Tests.Support;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Tests.Pages.blogs.technology;

public sealed class DependencyInversionPrinciplePart2Tests : BunitContext
{
    [Fact]
    public void DependencyInversionPrinciplePart2_RendersConvertedMarkdownBackLinkAndJsInterop()
    {
        const string markdown = """
            # Dependency Inversion Principle Part 2

            Details should depend on **abstractions**.
            """;

        Services.AddSingleton(new HttpClient(new TestHttpMessageHandler(
            new Dictionary<string, string>
            {
                ["blogs/technology/dependency-inversion-principle-part-2.md"] = markdown
            }))
        {
            BaseAddress = new Uri("http://localhost/")
        });

        var renderMermaid = JSInterop.SetupVoid("renderMermaid").SetVoidResult();
        var renderCodeHighlighting = JSInterop.SetupVoid("renderCodeHighlighting").SetVoidResult();

        var cut = Render<DependencyInversionPrinciplePart2>();

        cut.WaitForAssertion(() =>
        {
            Assert.DoesNotContain("Loading blog post...", cut.Markup);
            cut.Find("h1").TextContent.MarkupMatches("Dependency Inversion Principle Part 2");

            var markdownParagraph = cut.FindAll("p")
                .Single(paragraph => paragraph.TextContent.Trim() == "Details should depend on abstractions.");

            Assert.Contains("<strong>abstractions</strong>", markdownParagraph.InnerHtml);

            var backLink = cut.Find("a");
            Assert.Equal("/blogs/technology", backLink.GetAttribute("href"));
            Assert.Equal("Back to Technology Blogs", backLink.TextContent.Trim());

            Assert.Single(renderMermaid.Invocations);
            Assert.Single(renderCodeHighlighting.Invocations);
        });
    }
}
