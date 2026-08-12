using BlazorBasic.Pages.blogs.technology;
using Client.Tests.Support;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Tests.Pages.blogs.technology;

public sealed class StatefulVsStatelessApplicationsTests : BunitContext
{
    [Fact]
    public void StatefulVsStatelessApplications_RendersConvertedMarkdownBackLinkAndJsInterop()
    {
        const string markdown = """
            # Stateful vs Stateless Applications

            Stateless applications do not store **client session state** on the server.

            ```mermaid
            graph TD
                A[Request] --> B[Application]
            ```
            """;

        Services.AddSingleton(new HttpClient(new TestHttpMessageHandler(
            new Dictionary<string, string>
            {
                ["blogs/technology/stateful-vs-stateless-applications.md"] = markdown
            }))
        {
            BaseAddress = new Uri("http://localhost/")
        });

        var renderMermaid = JSInterop.SetupVoid("renderMermaid").SetVoidResult();
        var renderCodeHighlighting = JSInterop.SetupVoid("renderCodeHighlighting").SetVoidResult();

        var cut = Render<StatefulVsStatelessApplications>();

        cut.WaitForAssertion(() =>
        {
            Assert.DoesNotContain("Loading blog post...", cut.Markup);
            cut.Find("h1").TextContent.MarkupMatches("Stateful vs Stateless Applications");

            var markdownParagraph = cut.FindAll("p")
                .Single(paragraph => paragraph.TextContent.Trim() == "Stateless applications do not store client session state on the server.");

            Assert.Contains("<strong>client session state</strong>", markdownParagraph.InnerHtml);

            var backLink = cut.Find("a");
            Assert.Equal("/blogs/technology", backLink.GetAttribute("href"));
            Assert.Equal("Back to Technology Blogs", backLink.TextContent.Trim());

            Assert.Single(renderMermaid.Invocations);
            Assert.Single(renderCodeHighlighting.Invocations);
        });
    }
}
